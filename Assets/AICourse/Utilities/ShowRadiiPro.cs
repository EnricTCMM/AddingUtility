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
    }


    // Update is called once per frame
    void Update()
    {
        float inner = (float)innerField.GetValue(component);
        float outer = (float)outerField.GetValue(component);

        DebugExtension.DebugCircle(transform.position, up, Color.blue, inner);
        DebugExtension.DebugCircle(transform.position, up, Color.red, outer);
    }
}
