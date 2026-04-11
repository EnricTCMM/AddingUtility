using UnityEngine;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro
using UnityEngine.Events; // Required to assign actions to buttons

public class ButtonBarController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The button prefab previously created.")]
    public GameObject buttonPrefab; 

    //The Transform of the Panel where the Layout Group is.
    private Transform buttonContainer; 

    void Awake()
    {
        // Take the transform of the panel (should contain the Layout Group) and use it as the container
        buttonContainer = this.transform; 
    }
    
    /// <summary>
    /// Removes all current buttons from the bar.
    /// </summary>
    public void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Creates a new button and assigns it a text and an action.
    /// </summary>
    /// <param name="buttonText">The text that the button will display.</param>
    /// <param name="onClickAction">The method that will be executed on click.</param>
    public void AddButton(string buttonText, UnityAction onClickAction)
    {
        // Instantiate the button inside the container
        GameObject newBtn = Instantiate(buttonPrefab, buttonContainer);

        // Change the text
        newBtn.GetComponentInChildren<TextMeshProUGUI>().text = buttonText;

        // Assign the action that was passed as a parameter
        Button btnComponent = newBtn.GetComponent<Button>();
        btnComponent.onClick.RemoveAllListeners(); // For safety, we clear
        btnComponent.onClick.AddListener(onClickAction);
    }
}