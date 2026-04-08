using UnityEngine;
using TMPro;

public class StateLabelController : MonoBehaviour
{
    private TextMeshPro text;
    private IStateNameProvider stateNameProvider;
    private SpriteRenderer parentSprite; // We need to reference the parent's sprite

    // private Vector3 initialLocalPosition;
    private Vector3 initialScale;
    
    // We will store the distance between the label and the geometric center
    private Vector3 centerOffset;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();

        // Automatically searches for the parent's script
        stateNameProvider = GetComponentInParent<IStateNameProvider>();

        // Search for the SpriteRenderer in the parent
        parentSprite = transform.parent.GetComponent<SpriteRenderer>();
        
        // Capture the initial position and scale set in the Editor
        // initialLocalPosition = transform.localPosition;
        initialScale = transform.localScale;
        
        if (parentSprite != null)
        {
            // Calculate the initial difference between where you placed the label in the Editor
            // and the REAL geometric center of the sprite (ignoring the pivot).
            centerOffset = transform.position - parentSprite.bounds.center;
        }
        else
        {
            Debug.LogWarning("The label needs the parent to have a SpriteRenderer to calculate the center.");
        }
    }

    void OnEnable()
    {
        // Subscribe to the event
        if (stateNameProvider != null)
        {
            // register method UpdateText to be called when the event is raised
            stateNameProvider.OnStateNameChanged += UpdateText;
        }
    }

    void OnDisable()
    {
        // Unsubscribe (Very important to avoid errors if the object is destroyed)
        if (stateNameProvider != null)
        {
            stateNameProvider.OnStateNameChanged -= UpdateText;
        }
    }

    private void UpdateText(string newState)
    {
        if (text != null)
        {
            text.text = newState;
        }
    }

    void LateUpdate()
    {
        // 1. MAINTAIN POSITION AT GEOMETRIC CENTER
        if (parentSprite != null)
        {
            // Get the exact visual center of this frame
            Vector3 currentCenter = parentSprite.bounds.center;

            // Check which direction the character is facing
            float signX = Mathf.Sign(transform.parent.localScale.x);

            // Apply the offset (inverting X in case you placed the label
            // slightly to one side and the character does a "flip")
            Vector3 finalOffset = new Vector3(centerOffset.x * signX, centerOffset.y, centerOffset.z);

            // Fix the position in World Space. This overrides the parent/child behavior!
            transform.position = currentCenter + finalOffset;
        }

        // 2. CANCEL ROTATION
        transform.rotation = Quaternion.identity;

        // 3. COUNTERACT NEGATIVE SCALE
        if (transform.parent != null)
        {
            float parentScaleX = transform.parent.localScale.x;
            transform.localScale = new Vector3(
                initialScale.x * Mathf.Sign(parentScaleX),
                initialScale.y,
                initialScale.z
            );
        }
    }
}