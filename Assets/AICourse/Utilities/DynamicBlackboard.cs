using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class DynamicBlackboard : MonoBehaviour
{
    private Dictionary<string, object> map = new Dictionary<string, object>();
    private Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
    private bool initialised = false;
    
    private void Initialize()
    {
       
        // Reflection-based discovery of public fields
        FieldInfo[] allFields = this.GetType().GetFields();
        foreach (FieldInfo field in allFields)
        {
            string name = field.Name.ToUpper();
            fields.Add(name, field);
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
        else if (map.ContainsKey(name))
            value = map[name];
        else
            Debug.LogWarning("Unknown key in blackboard: "+name);

        return (T)value;
    }

    public void Put (string name, object value)
    {
        if (!initialised) Initialize();
        name = name.ToUpper();
        if (fields.ContainsKey(name)) // name refers to a field 
            fields[name].SetValue(this, value);
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
}
