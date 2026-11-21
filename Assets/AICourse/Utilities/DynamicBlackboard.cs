using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class DynamicBlackboard : MonoBehaviour
{
    private Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
    // fields contains all the public fields present in this class.

    private Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
    // properties contains all the public properties present in this class.

    private Dictionary<string, object> dynamicKeys = new Dictionary<string, object>();
    // dynamicKeys contains all the keys that are not fields or properties (hence dynamically created)

    // March 2026 fields and properties may have ranges. We need to keep track of them for Utility normalization.
    private Dictionary<string, (float min, float max)> propertyRanges = new Dictionary<string, (float, float)>();

    private bool initialised = false;

    private static string NormalizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(
                "Blackboard key cannot be null, empty or whitespace.",
                nameof(key));
        }

        return key.ToUpperInvariant();
    }

    private void Initialize()
    {
        RangeAttribute rangeAttr;
        HashSet<string> usedNames = new HashSet<string>();

        // Reflection-based discovery of public fields (and 2026 their ranges)
        FieldInfo[] allFields = this.GetType().GetFields();
        foreach (FieldInfo field in allFields)
        {
            string name = NormalizeKey(field.Name);

            if (!usedNames.Add(name))
            {
                throw new Exception(
                    $"Duplicate blackboard key '{name}' detected in type '{GetType().Name}'.");
            }

            fields.Add(name, field);

            // let's know the min/max range of this field
            rangeAttr = field.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                propertyRanges[name] = (rangeAttr.min, rangeAttr.max);
            }
        }

        // Reflection-based discovery of public properties (and 2026 their ranges)
        PropertyInfo[] allProperties = this.GetType().GetProperties();
        foreach (PropertyInfo property in allProperties)
        {
            string name = NormalizeKey(property.Name);

            if (!usedNames.Add(name))
            {
                throw new Exception(
                    $"Duplicate blackboard key '{name}' detected in type '{GetType().Name}'.");
            }

            properties.Add(name, property);

            rangeAttr = property.GetCustomAttribute<RangeAttribute>();
            if (rangeAttr != null)
            {
                propertyRanges[name] = (rangeAttr.min, rangeAttr.max);
            }
        }

        initialised = true;
    }

    public T Get<T>(string key)
    {
        object value;

        if (!initialised) Initialize();

        // first of all, code considers the possibility of verbatim keys
        if (typeof(T).Equals(typeof(float)))
        {
            // if key parses to a float then return the float itself
            float theFloat;

            if (float.TryParse(key, NumberStyles.Float, CultureInfo.InvariantCulture, out theFloat))
            {
                value = theFloat;
                return (T)value;
            }
            else if (!string.IsNullOrEmpty(key) && key.ToUpperInvariant().EndsWith('F'))
            {
                if (float.TryParse(
                    key.Substring(0, key.Length - 1),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out theFloat))
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
            string upperName = NormalizeKey(key);

            if (!fields.ContainsKey(upperName)
                && !dynamicKeys.ContainsKey(upperName)
                && !properties.ContainsKey(upperName))
            {
                value = key;
                return (T)value;
            }
        }

        // no other type is "verbatimable"
        return InnerGet<T>(key);
    }

    private T InnerGet<T>(string name)
    {
        object value = null;

        name = NormalizeKey(name);

        if (fields.ContainsKey(name)) // name refers to a field
            value = fields[name].GetValue(this);
        else if (properties.ContainsKey(name))
            value = properties[name].GetValue(this);
        else if (dynamicKeys.ContainsKey(name))
            value = dynamicKeys[name];
        else
        {
            // no soft landing here. This is serious...
            throw new System.Exception("Unknown key in blackboard: " + name);
        }

        // now, what if T is float and value is int?
        // (T)value causes an Exception.
        // so let's take this into account and save the situation
        if (value.GetType() == typeof(int) && typeof(T) == typeof(float))
        {
            float val = (int)value;
            value = val;
        }

        return (T)value;
    }

    public void Put(string name, object value)
    {
        if (!initialised) Initialize();

        name = NormalizeKey(name);

        if (fields.ContainsKey(name)) // name refers to a field
            fields[name].SetValue(this, value);
        else if (properties.ContainsKey(name))
        {
            // the property exists. Can we change it?
            if (properties[name].CanWrite)
                properties[name].SetValue(this, value);
            else
                Debug.LogWarning("property " + name + " exists but cannot be set");
        }
        else
            dynamicKeys[name] = value;  // adds or updates...
    }

    public void PutIfNotPresent(string name, object value)
    {
        if (!Exists(name))
            Put(name, value);
    }

    public bool Exists(string key)
    {
        if (!initialised) Initialize();

        key = NormalizeKey(key);

        return dynamicKeys.ContainsKey(key)
            || fields.ContainsKey(key)
            || properties.ContainsKey(key);
    }

    // added April 2026 for the benefit of ACTION_UpdateKey
    public Type GetCurrentType(string key)
    {
        object value = null;

        key = NormalizeKey(key);

        if (fields.ContainsKey(key)) // name refers to a field
            value = fields[key].GetValue(this);
        else if (properties.ContainsKey(key))
            value = properties[key].GetValue(this);
        else if (dynamicKeys.ContainsKey(key))
            value = dynamicKeys[key];

        if (value == null) return null;
        else return value.GetType();
    }

    public bool TryGetRange(string key, out float min, out float max)
    {
        (float min, float max) range;

        if (!initialised) Initialize();

        key = NormalizeKey(key);

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

    //---------------------------------------

    public void Dump()
    {
        if (!initialised) Initialize();

        Debug.Log("---Dumping fields");
        foreach (string s in fields.Keys)
        {
            Debug.Log(s);
        }

        Debug.Log("---Dumping properties");
        foreach (string s in properties.Keys)
        {
            Debug.Log(s);
        }

        Debug.Log("---Dumping dynamic keys");
        foreach (string s in dynamicKeys.Keys)
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