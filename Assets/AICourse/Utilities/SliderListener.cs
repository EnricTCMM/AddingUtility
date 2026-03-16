using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Steerings;
using System;
using System.Reflection;

public class SliderListener : MonoBehaviour
{
    private TextMeshProUGUI minText;
    private TextMeshProUGUI maxText;
    private TextMeshProUGUI currVal;
    private Slider slider;

    private object[] pars = new object[1];

    public GameObject listenerObject;
    public string componentType;
    public string fieldName;
  
    void Start()
    {
        float value;

        slider = this.GetComponent<Slider>();
        minText = transform.Find("MinText").GetComponent<TextMeshProUGUI>();
        maxText = transform.Find("MaxText").GetComponent<TextMeshProUGUI>();
        currVal = transform.Find("CurrentValText").GetComponent<TextMeshProUGUI>();

        minText.text = slider.minValue.ToString();
        maxText.text = slider.maxValue.ToString();

        // By default take SteeringContext as the component's type (retro compatibility)
        if (componentType == null || componentType.Length == 0)
            componentType = "SteeringContext";

        Component component = listenerObject.GetComponent(componentType);

        Type type = component.GetType();
        FieldInfo field = type.GetField(fieldName);

        // in some case the field may be null because its non-existent. Even so,
        // there could be a setter with an "equivalent" name (SetFieldName,,,) 
        if (field != null)
        {
            // if the field exists, initialize slider with its value
            value = (float)field.GetValue(component);
            currVal.text = value.ToString("0.00");
            slider.value = value;
        }
        else
        {
           
            // if the field does not exit, use the slider's inital value
            value = slider.value;
            currVal.text = value.ToString("0.00");
        }

        // let's check if there's a setter for the field. If there's a setter the listener
        // attached to the slider will give priority to  this setter
        string setterName = "Set" + ("" + fieldName[0]).ToUpper() + fieldName.Substring(1);
        MethodInfo method = type.GetMethod(setterName);
        // the listener will decide...


        slider.onValueChanged.AddListener((x)=> { 
            currVal.text = slider.value.ToString("0.00");
            if (method!=null)
            {
                // use the setter if possible
                pars[0] = x;
                method.Invoke(component, pars);
            }
            else if (field!=null) // if no setter available change the field directly, if it is non-null
                field.SetValue(component, x);
        });
    }
}
