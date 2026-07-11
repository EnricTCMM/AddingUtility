using UnityEngine;
using TMPro;

public class ConstantLabelController : MonoBehaviour
{
    private TextMeshPro textComponent;
    private SpriteRenderer parentSprite;
    private Vector3 initialScale;
    private Vector3 centerOffset;

    [Header("Label Configuration")]
    [Tooltip("Enter the text that will always be displayed on this GameObject")]
    [SerializeField] private string textConstant = "Label";

    // This method runs AUTOMATICALLY inside Unity every time 
    // you change a variable from the Inspector (no need to be in Play mode)
    private void OnValidate()
    {
        if (textComponent == null) textComponent = GetComponent<TextMeshPro>();
        
        if (textComponent != null)
        {
            textComponent.text = textConstant;
        }
    }

    void Awake()
    {
        textComponent = GetComponent<TextMeshPro>();
        
        // Ensure the text is applied when the game starts
        if (textComponent != null)
        {
            textComponent.text = textConstant;
        }
        else
        {
            Debug.LogError("No TextMeshPro component found on this GameObject!", this);
        }

        // Find the SpriteRenderer in the parent
        parentSprite = transform.parent != null ? transform.parent.GetComponent<SpriteRenderer>() : null;
        
        // Capture the initial scale set in the Editor
        initialScale = transform.localScale;
        
        if (parentSprite != null)
        {
            // Calculate the initial difference between the label's position in the Editor
            // and the REAL geometric center of the sprite (ignoring the pivot).
            centerOffset = transform.position - parentSprite.bounds.center;
        }
        else
        {
            Debug.LogWarning("The label needs the parent to have a SpriteRenderer to calculate the center.", this);
        }
    }

    void LateUpdate()
    {
        // 1. MAINTAIN POSITION AT GEOMETRIC CENTER
        if (parentSprite != null)
        {
            // Get the exact visual center of this frame
            Vector3 currentCenter = parentSprite.bounds.center;

            // Check which direction the character is facing (positive or negative scale)
            float signX = Mathf.Sign(transform.parent.localScale.x);

            // Apply the offset (inverting X in case the character flips)
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