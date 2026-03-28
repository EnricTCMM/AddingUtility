using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class Scorer : IConsideration
    {
        public string Name { get; set; }
        public GameObject gameObject;
        public DynamicBlackboard blackboard;
        
        private List<IConsideration> considerations = new List<IConsideration>();

        public AggregationPolicy policy = AggregationPolicy.ADJUSTED_MULTIPLY;
        
        public void Contextualize(GameObject go)
        {
            gameObject = go;
            blackboard = gameObject.GetComponent<DynamicBlackboard>();
            // contextualize your considerations...
            foreach (IConsideration consideration in considerations)
            { 
                consideration.Contextualize(gameObject);
            }
        }

        public Scorer(string name, AggregationPolicy policy, params IConsideration[] considerations)
        {
            // for practical purposes, second parameter is optional (just give nothing and the
            // parameter will be a zero-length array
            Name = name;
            this.policy = policy;
            AddConsiderations(considerations);
        }
        
        public void AddConsideration(IConsideration c)
        {
            considerations.Add(c);
        }

        public void AddConsiderations(params IConsideration[] considerations)
        {
            foreach (IConsideration consideration in considerations)
            {
                AddConsideration(consideration);
            }
        }

        public float GetScore()
        {
            if (considerations.Count == 0) 
            {
                throw new System.InvalidOperationException("Cannot get score from scorer " + Name + " because it has no considerations");
            }
            
            switch (policy)
            {
                case AggregationPolicy.MULTIPLY:
                    return AggregateMultiply();
                case AggregationPolicy.ADJUSTED_MULTIPLY:
                    return AggregateAdjustedMultiply();
                case AggregationPolicy.AVERAGE:
                    return AggregateAverage();
                default:
                    return 0;
            }
        }

        private float AggregateMultiply()
        {
            float product = 1;
            foreach (IConsideration consideration in considerations)
            {
                product *= consideration.GetScore();
                if (product ==0) return 0f;  // immediate veto
            }
            return product;
        }

        private float AggregateAdjustedMultiply()
        {
            float product = AggregateMultiply();
            float modFactor = 1f - (1f / considerations.Count);
            float makeupValue = (1f - product) * modFactor;
            
            return product + (makeupValue * product);
        }

        private float AggregateAverage()
        {
            float sum = 0;
            foreach (IConsideration consideration in considerations)
            {
                sum += consideration.GetScore();
            }
            return sum / considerations.Count;
        }

    }
}