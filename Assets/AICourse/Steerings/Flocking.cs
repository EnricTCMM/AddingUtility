using System.Collections.Generic;
using UnityEngine;

namespace Steerings
{
    public class Flocking : SteeringBehaviour
    {
        public override Vector3 GetLinearAcceleration()
        {
            return Flocking.GetLinearAcceleration(Context);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me)
        {
            // get all potential "mates" (the other boids) 
            //ICollection<GameObject> boids = me.groupManager.members;
            GameObject[] boids = GameObject.FindGameObjectsWithTag(me.idTag);


            Vector3 averageVelocity = Vector3.zero;
            int count = 0;

            // iterate to find average velocity
            foreach (GameObject boid in boids)
            {
                // do not take yourself into account
                if (boid == me.gameObject) continue;

                // velocity of mate required. Let's get if from its Steering context
                SteeringContext boidContext = boid.GetComponent<SteeringContext>();
               
                // disregard distant boids (what is the meaning of distant?)
                // (I've decided to define distant as "not contributing to cohesion")
                if ((boid.transform.position - me.transform.position).magnitude > me.cohesionThreshold) continue;

                // also disregard boids outside the cone of vision, if vision applies
                if (me.applyVision)
                    if (!Utils.InCone(me.gameObject, boid, me.coneOfVisionAngle)) continue;
                
                averageVelocity += boidContext.velocity;
                count++;
            }


            if (count > 0) 
                averageVelocity /= count;
            else
            {
                // if no boid is close, there's nowhere to steer for
                // (hence wander, if allowed...)
                if (me.addWanderIfZero) return Wander.GetLinearAcceleration(me);
                else return Vector3.zero; 
            }
            
            // now let's compute all the ingredients
            SURROGATE_TARGET.GetComponent<SteeringContext>().velocity = averageVelocity;
            Vector3 velocityMatching = VelocityMatching.GetLinearAcceleration(me, SURROGATE_TARGET);
            Vector3 separation = LinearRepulsion.GetLinearAcceleration(me);
            Vector3 cohesion = Cohesion.GetLinearAcceleration(me);

            separation = separation.normalized * me.maxAcceleration;
            velocityMatching = velocityMatching.normalized * me.maxAcceleration;
            // cohesion is based on seek and, when it returns an acceleration that acceleration is of max. magnitude
            // but when no mates are found, cohesion returns Vector3.zero

            // and the actual weights to use
            float vmW = me.alignmentWeight;
            float coW = me.cohesionWeight;
            float seW = me.repulsionWeight;


            // notice that if this point is reached count>0. This means that there's at least one mate boid close enough
            // hence cohesion applies and alignment applies. Only separation may not apply
            if (separation.Equals(Vector3.zero))
            { // give the separation weight to the other two... 
                vmW += seW * (vmW / (vmW + coW));
                coW += seW * (coW / (vmW + coW));
                seW = 0;
            }

            Vector3 flockingAcceleration = velocityMatching * vmW + cohesion * coW + separation * seW;

            if (me.addWanderIfZero && flockingAcceleration.Equals(Vector3.zero)) {
                return Wander.GetLinearAcceleration(me);
            }
            else return flockingAcceleration;
            
        }
    }
}
