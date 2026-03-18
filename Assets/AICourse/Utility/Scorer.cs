using UnityEngine;

namespace Utility
{
    public abstract class Scorer : IConsideration
    {
        // internally a Scorer keeps a list of considerations
        
        public GameObject gameObject;
        public DynamicBlackboard blackboard;
        public void Contextualize(GameObject go)
        {
            gameObject = go;
            blackboard = gameObject.GetComponent<DynamicBlackboard>();
        }

        public abstract float GetScore();

    }
}