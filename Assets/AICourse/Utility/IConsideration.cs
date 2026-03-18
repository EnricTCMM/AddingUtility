using UnityEngine;

namespace Utility
{
    public interface IConsideration
    {
        public void Contextualize(GameObject go);
        public float GetScore();
    }
}

