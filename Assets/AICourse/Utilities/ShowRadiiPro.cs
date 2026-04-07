using System;
using System.Reflection;
using UnityEngine;

public class ShowRadiiPro : MonoBehaviour
{

    public string componentTypeName;
    public string innerFieldName;
    public string outerFieldName;

    public GameObject listenedObject;
    private Component component;
    private Type type;
    private FieldInfo innerField, outerField;
    private Vector3 up = new Vector3(0, 0, 1);

    public void Start()
    {
        // if not "listening" to someone else, listen to yourself
        if (listenedObject==null) listenedObject = gameObject;

        component = listenedObject.GetComponent(componentTypeName);
        type = component.GetType();
        innerField = type.GetField(innerFieldName);
        outerField = type.GetField(outerFieldName);
        
        if (component == null) Debug.LogWarning("No component named " + componentTypeName + " in " + gameObject.name);
        else
        {
            if (innerField == null) Debug.LogWarning("No field named " + innerFieldName + " in " + componentTypeName);
            if (outerField == null) Debug.LogWarning("No field named " + outerFieldName + " in " + componentTypeName);
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        if (innerField != null)
        {
            float inner = (float)innerField.GetValue(component);
            DebugExtension.DebugCircle(transform.position, up, Color.blue, inner);
        }

        if (outerField != null)
        {
            float outer = (float)outerField.GetValue(component);
            DebugExtension.DebugCircle(transform.position, up, Color.red, outer);
        }
        
    }
}
