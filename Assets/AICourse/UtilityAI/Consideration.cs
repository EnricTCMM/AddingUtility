using System;
using System.Text;
using UnityEngine;

namespace Utility
{
    public class Consideration : IConsideration
    {
        public string Name { get; set; }
        
        public GameObject gameObject;
        public DynamicBlackboard blackboard;
        
        // functional delegate for response curves (a float -> float function)
        private Func<float, float> responseCurve;
        
        // functional delegate for synthetic properties (a () -> float function)
        private Func<float> syntheticProperty;

        private string propertyKey; 
        
        private float propertyMin;
        private float propertyMax;
        
        // private bool explicitRangeProvided = false; // range or fallback to [0, 100]

        private bool normalize; // if true, the property value is normalized to [0, 1]
                                // before applying the response curve. Normalization uses propertyMin and propertyMax.
                                // Normalization is required almost always.
                                // An exception is [0,24] time properties submitted to custom curves.
        
        private float lastScore; // for debugging purposes
        private float lastValue; // id

        // Beware: at construction time the consideration is not yet contextualized.
        // This is the "normal" constructor. A Consideration out of a "blackboarded" property. 
        public Consideration(string key, Func<float, float> responseCurve, string name = null, bool normalize = true) 
        {
            this.propertyKey = key;
            this.responseCurve = responseCurve;
            // this.explicitRangeProvided = false;
            this.normalize = normalize;
            Name = name == null ? "Consideration("+key+")" : name + "("+key+")";
        }

        /* 
        // this is an extra constructor for non-ranged blackboarded properties
        public Consideration(string key, Func<float, float> responseCurve, float min, float max, string name = null)
        {
            this.propertyKey = key;
            this.responseCurve = responseCurve;
            this.explicitRangeProvided = true;
            propertyMin = min;
            propertyMax = max;
            Name = name == null ? "Consideration("+key+")" : name + "("+key+")";
        }
        */
        
        //  this constructor is specific for synthetic properties.
        public Consideration(Func<float> syntheticProperty,  
                             Func<float, float> responseCurve, 
                             float min, float max,
                             string name = null)
        {
            this.syntheticProperty = syntheticProperty;
            this.responseCurve = responseCurve;
            // this.explicitRangeProvided = true;
            propertyMin = min;
            propertyMax = max;
            Name = name == null ? "Synth. Consideration" : name;
        }
        
        public void Contextualize(GameObject go)
        {
            gameObject = go;
            blackboard = gameObject.GetComponent<DynamicBlackboard>();
            
            // contextualization time is when getting min and max makes sense
            //if (!explicitRangeProvided)
            //{
                bool hasRange = blackboard.TryGetRange(propertyKey, out propertyMin, out propertyMax);
                // remember that TryGetRange defaults to [0, 100] if property is not ranged
                if(!hasRange) Debug.LogWarning("In "+Name+" " + propertyKey + " is not ranged. Using [0, 100] as default range"); 
            //}
            if (propertyMin==propertyMax)
                Debug.LogError("In "+Name+" " + propertyKey + " has zero range. This is not allowed");
        }

        public float GetScore()
        {
            float propertyValue;
            propertyValue = syntheticProperty!=null ? 
                            syntheticProperty() : blackboard.Get<float>(propertyKey);
            
            // values in [min, max] must be mapped to [0, 1] if not otherwise specified
            if (normalize)
            {
                propertyValue = (propertyValue - propertyMin) / (propertyMax - propertyMin);
                propertyValue = Mathf.Clamp01(propertyValue);
            }
            lastValue = propertyValue;
            
            lastScore =  Mathf.Clamp01(responseCurve(propertyValue));
            return lastScore;
        }

        public void AppendDebugInfo(StringBuilder info, int depth)
        {
            //info.Append('\t', depth).AppendLine(Name + " --> " + lastScore);
            info.Append('\t', depth).AppendLine($"{Name}[={lastValue:F4}] --> {lastScore:F4}");
        }
    }
}