using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class DynamicBlackboard : MonoBehaviour /*, Utility.IUtilityTarget */
{
    private Dictionary<string, object> map = new Dictionary<string, object>();
    private Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
    private Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
    
    // March 2026 fields and properties may have ranges. We need to keep track of them for Utility normalization.
    private Dictionary<string, (float min, float max)> propertyRanges = new Dictionary<string, (float, float)>();
    
    private bool initialised = false;
    
    private void Initialize()
    {
        RangeAttribute rangeAttr;
        
        // Reflection-based discovery of public fields (and 2026 their ranges)
        FieldInfo[] allFields = this.GetType().GetFields();
        foreach (FieldInfo field in allFields)
        {
            string name = field.Name.ToUpper();
            fields.Add(name, field);
            
            // let's know the min/max range of this field
            rangeAttr = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
                propertyRanges.Add(name, (rangeAttr.min, rangeAttr.max));
        }

        // Reflection-based discovery of public properties (and 2026 their ranges)
        PropertyInfo[] allProperties = this.GetType().GetProperties();
        foreach (PropertyInfo property in allProperties)
        {
            string name = property.Name.ToUpper();
            properties.Add(name, property);
            
            rangeAttr = property.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null) 
                propertyRanges.Add(name, (rangeAttr.min, rangeAttr.max));
        }

        initialised = true;
    }


    public T Get<T> (string key)
    {
        object value;

        if (!initialised) Initialize();

        if (typeof(T).Equals(typeof(float)))
        {
            // if key parses to a float then return the float itself
            float theFloat; 
            if (float.TryParse(key, NumberStyles.Float, CultureInfo.InvariantCulture, out theFloat))
            {
                value = theFloat;
                return (T)value;
            }
            else if (key.ToUpper().EndsWith('F'))
            {
                if (float.TryParse(key.Substring(0,key.Length-1), NumberStyles.Float, CultureInfo.InvariantCulture, out theFloat))
                {
                    value = theFloat;
                    return (T)value;
                }
            }
        }
        else if (typeof(T).Equals(typeof(int)))
        {
            // if key parses to an int then return the int itself
            int theInt;
            if (int.TryParse(key, out theInt))
            {
                value = theInt;
                return (T)value;
            }
        }
        else if (typeof(T).Equals(typeof(bool)))
        {
            // if key parses to a bool then return the bool itself
            bool theBool;
            if (bool.TryParse(key, out theBool))
            {
                value = theBool;
                return (T)value;
            }
        }
        else if (typeof(T).Equals(typeof(string)))
        {
            // if key is unknown then return it verbatim 
            if (key == null) Debug.LogWarning("null key");
            string upperName = key.ToUpper();
            if (!fields.ContainsKey(upperName) && !map.ContainsKey(upperName))
            {
                value = key;
                return (T)value;
            }
        }

        // no other type is "verbatimable" 
        return InnerGet<T>(key);
    }


    private T InnerGet<T> (string name)
    {
        object value = null;

        name = name.ToUpper();
        if (fields.ContainsKey(name)) // name refers to a field 
            value = fields[name].GetValue(this);
        else if (properties.ContainsKey(name))
            value = properties[name].GetValue(this);
        else if (map.ContainsKey(name))
            value = map[name];
        else
            Debug.LogWarning("Unknown key in blackboard: "+name);

        return (T)value;
    }

    public void Put(string name, object value)
    {
        if (!initialised) Initialize();

        name = name.ToUpper();

        if (fields.ContainsKey(name)) // name refers to a field 
            fields[name].SetValue(this, value);
        else if (properties.ContainsKey(name))
        {
            // the property exists. Can we change it?
            if (properties[name].CanWrite)
                properties[name].SetValue(this, value);
            else
                Debug.LogWarning("property "+name+" cannot be set");
        }
        else
            map[name] = value;  // adds or updates...
        
    }

    public void PutIfNotPresent(string name, object value)
    {
        if (!Exists(name))
            Put(name, value);
    }

    public bool Exists (string key)
    {
        if (!initialised) Initialize();
        key=key.ToUpper();
        return map.ContainsKey(key) || fields.ContainsKey(key);
    }

    public bool TryGetRange(string key, out float min, out float max)
    {
        (float min, float max) range; 
        
        if (!initialised) Initialize();
        key=key.ToUpper();
        if (propertyRanges.TryGetValue(key, out range))
        {
            min = range.min;
            max = range.max;
            return true;
        }
        else
        {
            min = 0;
            max = 100;
            return false;
        }
        
    }
    

    // -- required by IUtilityTarget interface
    public object targetObject => this.gameObject;

    //--------------------------------------- 

    public void Dump ()
    {
        if (!initialised) Initialize();
        Debug.Log("---Dumping fields");
        foreach (string s in fields.Keys)
        {
            Debug.Log(s);
        }
    }

    // ----------- autokey generation
    private static string prefix = "__autoKey_";
    private static int seq = 0;



    public string NewKey()
    {
        string result = prefix + seq;
        seq++;
        return result;  
    }
}
