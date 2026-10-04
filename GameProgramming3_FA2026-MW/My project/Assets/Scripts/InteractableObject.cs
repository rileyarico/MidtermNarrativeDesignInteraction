using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    //the color the object glows when the player looks at it
    public Color highlightColor = new Color(1f, 0.95f, 0.6f);
    //how strongly the highlight color blends with the original color 0 = no effect, 1 = full replace (opacity)
    [Range(0, 1f)] public float highlightStrength = 0.4f;

    public bool isHarvestable = false;
    public GameObject harvest;

    private Renderer objectRenderer; //the render comp on this obj
    private Color originalColor; //the color before any highlight was applied
    private bool isHighlighted = false;


    void Awake()
    {
        objectRenderer = GetComponent<Renderer>(); //cache the renderer so we are not calling getComp every frame
        if(objectRenderer != null )
        {
            //store the original color, so we can restore it after un-highlighting
            //we read from the material's color property
            //we use sharedmaterial to read the base color without instancing
            originalColor = objectRenderer.sharedMaterial.color;

        }
        else
        {
            Debug.Log($"Interactable object on {gameObject.name} has no renderer. highlight won't work.");
        }
    }

    public void Highlight()
    {
        if(isHighlighted || objectRenderer == null)
        {
            //Debug.Log("No object renderer & is lighted is true");
            return;
        }

        //color.lerp blends between the original color & the highlighted color
        //by the highlight strength amt
        //we use material not shared material to create a unique instance here
        //so we don't effect every object using the same material
        objectRenderer.material.color = Color.Lerp(originalColor, highlightColor, highlightStrength);
        isHighlighted = true;

    }

    //called by object grabber when the player looks away, restores original color
    public void UnHighlight()
    {
        if(!isHighlighted || objectRenderer == null)
        {
            return;
        }
        objectRenderer.material.color = originalColor;
        isHighlighted = false;
    }
}
