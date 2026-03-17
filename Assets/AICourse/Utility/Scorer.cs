using UnityEngine;
using System.Collections.Generic;

namespace Utility 
{
    [CreateAssetMenu(fileName = "Scorer", menuName = "Scriptable Objects/Scorer")]
    // segurament en el futur la voldré fer abstracta 
    public class Scorer : ScriptableObject
    {
        public List<Consideration> considerations = new List<Consideration>();
        public float score = 25;

        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        public float Evaluate()
        {
            return 0.99f;
        }
    }
}


