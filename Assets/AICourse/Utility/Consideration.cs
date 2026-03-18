using UnityEngine;

namespace Utility
{
    public abstract class Consideration : IConsideration
    {
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