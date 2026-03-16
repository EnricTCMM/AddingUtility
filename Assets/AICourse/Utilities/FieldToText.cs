
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Steerings;
using System;
using System.Reflection;

public class FieldToText : MonoBehaviour
{

    private TextMeshProUGUI textMesh;
    private string originalText;
    private Component component;
    private Type type;
    private FieldInfo field;

    private float dummy = 10.0f;

    public GameObject listenedObject;
    public string componentTypeName;
    public string fieldName;

    void Start()
    {
        if (componentTypeName == null || componentTypeName.Length == 0)
            componentTypeName = "Steerings.SteeringContext";

        textMesh = GetComponent<TextMeshProUGUI>();
        originalText = textMesh.text;
        
        component = listenedObject.GetComponent(componentTypeName);
       
        type = component.GetType();
        field = type.GetField(fieldName);
        object value = field.GetValue(component);

        
        if (value.GetType().Equals(dummy.GetType()))
            textMesh.text = originalText + " " + ((float)value).ToString("0.00");
        else 
            textMesh.text = originalText + " " + value.ToString();
    }

    
    void Update()
    {
        object value = field.GetValue(component);
        if (value.GetType().Equals(dummy.GetType()))
            textMesh.text = originalText + " " + ((float)value).ToString("0.00");
        else
            textMesh.text = originalText + " " + value.ToString();
    }
}
