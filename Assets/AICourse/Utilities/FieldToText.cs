using TMPro;
using UnityEngine;
using System;
using System.Reflection;

public class FieldToText : MonoBehaviour
{

    private TextMeshProUGUI textMesh;
    private string originalText;
    private Component component;
    private Type type;
    private FieldInfo field;
    private PropertyInfo property; // we also cater to properties 

    private float dummy = 10.0f;

    public GameObject listenedObject;
    public string componentTypeName;
    public string fieldName;

    void Start()
    {
        object value;
        
        if (componentTypeName == null || componentTypeName.Length == 0)
            componentTypeName = "MotorManager"; // poor default. FAIL FAST would be better

        textMesh = GetComponent<TextMeshProUGUI>();
        originalText = textMesh.text;
        
        component = listenedObject.GetComponent(componentTypeName);
       
        type = component.GetType();
        field = type.GetField(fieldName);
        if (field == null)
        {
            property = type.GetProperty(fieldName);
            value = property.GetValue(component);
        }
        else value = field.GetValue(component);

        
        if (value.GetType().Equals(dummy.GetType()))
            textMesh.text = originalText + " " + ((float)value).ToString("0.00");
        else 
            textMesh.text = originalText + " " + value.ToString();
    }

    
    void Update()
    {
        object value;
        if (field!=null)
            value = field.GetValue(component);
        else value = property.GetValue(component);
        
        if (value.GetType().Equals(dummy.GetType()))
            textMesh.text = originalText + " " + ((float)value).ToString("0.00");
        else
            textMesh.text = originalText + " " + value.ToString();
    }
}
