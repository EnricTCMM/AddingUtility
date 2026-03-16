
using UnityEngine;

namespace BTs
{
    public class BehaviourTreeExecutor : MonoBehaviour
    {
        public BehaviourTree behaviourTree;
        public string status = Status.LIMBO.ToString();

        public void Start()
        {
            
            if (behaviourTree == null) return;

            if (GetComponent<DynamicBlackboard>() == null)
            {
                Debug.LogError("Executing a BT without a dynamic blackboard in "+gameObject.name+" \nIf a blackboard is not needed, just add an empty one");
            }

            // el canvio per una còpia. Per quina rao? Per evitar comparticions indesitjades...
            behaviourTree = (BehaviourTree)ScriptableObject.CreateInstance(behaviourTree.GetType().Name);
            // Aquesta creació sembla impedir la inicialització directa dels atributs/paràmetres


            // Set the values of the InputParameter<> in the behaviour tree
            // ManageParameters(); // FOSSIL CODE. All management done through blackboard 

            // Efectively construct tree and propagate the gameobject down the hierarchy.
            behaviourTree.Contextualize(gameObject);

            status = behaviourTree.GetStatus().ToString();
        }

        public void Update()
        {
            if (behaviourTree == null) return;

            if (!behaviourTree.IsTerminated())
            {
                    behaviourTree.Tick();
            }
            
            status = behaviourTree.GetStatus().ToString();
        }
    }

     
}
