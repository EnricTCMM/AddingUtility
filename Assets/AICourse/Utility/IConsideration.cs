using UnityEngine;

namespace Utility
{
    public interface IConsideration
    {
        string Name { get; } // all considerations should have a name at least for debugging purposes
        public void Contextualize(GameObject go);
        public float GetScore();
    }
}

