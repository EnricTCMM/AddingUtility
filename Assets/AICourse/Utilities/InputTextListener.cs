using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Steerings;
using System;
using System.Reflection;

public class InputTextListener : MonoBehaviour
{
    private TMP_InputField inputField;
    private object[] pars = new object[1];

    public GameObject listenerObject;
    public string componentType;
    public string fieldName;

    void Start()
    {
        float value;

        inputField = this.GetComponent<TMP_InputField>();

        Component component = listenerObject.GetComponent(componentType);

        Type type = component.GetType();
        FieldInfo field = type.GetField(fieldName);

        if (field!=null)
        {
            // if field exists, initialize this text with its value
            value = (float)field.GetValue(component);
            inputField.text = value.ToString("0.00");
        }
        else
        {
            // field does not exist. Do nothing, keep the value in it
        }

        

        // let's check if there's a setter for the field. 
        string setterName = "Set" + ("" + fieldName[0]).ToUpper() + fieldName.Substring(1);
        MethodInfo method = type.GetMethod(setterName);
        // the listener will decide...

        inputField.onEndEdit.AddListener( (x) => {
            float newVal = Convert.ToSingle(x);
            if (method!=null)
            {
                // use available setter
                pars[0] = newVal;
                method.Invoke(component, pars);
            }
            if (field!=null)
                field.SetValue(component, newVal);
        });
    }

   
}
