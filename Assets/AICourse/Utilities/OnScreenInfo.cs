using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class OnScreenInfo : MonoBehaviour
{
    public int maxLogEntries = 25;
    
    private GameObject visualContent; 
    private TextMeshProUGUI displayText;
    private ScrollRect scrollRect;
    private Queue<string> logQueue = new Queue<string>();
    
    void Awake()
    {
        // 1. Find the visual panel by its exact name in the hierarchy
        Transform panelTransform = transform.Find("OnScreenInfoPanel");
        
        if (panelTransform != null)
        {
            visualContent = panelTransform.gameObject;
        }
        else
        {
            // Log an error if it's not found
            Debug.LogError("[OnScreenInfo] ERROR: No child object named 'OnScreenInfoPanel' was found. Did you rename it in the hierarchy?", this);
        }

        // 2. Find the TextMeshProUGUI component
        // The 'true' tells Unity to search even if the object is inactive/hidden
        displayText = GetComponentInChildren<TextMeshProUGUI>(true);
        
        // 3. Find the ScrollRect component within the children
        scrollRect = GetComponentInChildren<ScrollRect>(true);
        if (scrollRect == null)
        {
            Debug.LogWarning("[OnScreenInfo] WARNING: No 'ScrollRect' component found. Auto-scrolling will be disabled.", this);
        }
        
        if (displayText == null)
        {
            // Log an error if no text component is found
            Debug.LogError("[OnScreenInfo] ERROR: No 'TextMeshProUGUI' component found in the children of this object.", this);
        }

        // If the panel was found, show it by default on startup
        if (visualContent != null)
        {
            visualContent.SetActive(true);
        }
    }

    void Update()
    {
        // Toggle visibility with the F3 key
        if (Input.GetKeyDown(KeyCode.F3))
        {
            if (visualContent != null)
            {
                visualContent.SetActive(!visualContent.activeSelf);
            }
        }
    }
    
    /// <summary>
    /// Generic method to update or append text to the screen.
    /// </summary>
    /// <param name="textData">The formatted string to display</param>
    /// <param name="append">If true, adds the text to the end instead of replacing it (default is false)</param>
    public void InjectInfo(string textData, bool append = false)
    {
        // Prevent errors if initial setup failed
        if (visualContent == null) return;

        if (displayText != null)
        {
            if (append)
            {
                // 1. Add the new text to the queue
                logQueue.Enqueue(textData);
            
                // 2. If we exceed the limit, remove the oldest entry
                while (logQueue.Count > maxLogEntries)
                {
                    logQueue.Dequeue();
                }
                
                // 3. Rebuild the text using StringBuilder (Much faster than string concatenation)
                StringBuilder sb = new StringBuilder();
                foreach (string log in logQueue)
                {
                    sb.Append(log);
                }
                
                // show the text
                displayText.text = sb.ToString();
            }
            else
            {
                // If not appending, clear the queue and start fresh
                logQueue.Clear();
                logQueue.Enqueue(textData);
                displayText.text = textData;
            }
            
            // Trigger the auto-scroll behavior if we have a ScrollRect
            if (scrollRect != null)
            {
                StartCoroutine(ScrollToBottom());
            }
        }
    }

    /// <summary>
    /// Optional helper method to quickly clear the text if needed.
    /// </summary>
    public void ClearInfo()
    {
        if (displayText != null)
        {
            displayText.text = "";
        }
    }
    
    // <summary>
    /// Coroutine to wait for the UI layout to rebuild before scrolling down.
    /// This prevents the scroll from stopping before the newly added text is rendered.
    /// </summary>
    private IEnumerator ScrollToBottom()
    {
        // Wait for Unity to calculate the new height of the text content
        yield return new WaitForEndOfFrame();

        // Set the vertical scrollbar to the absolute bottom (0.0 is bottom, 1.0 is top)
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
