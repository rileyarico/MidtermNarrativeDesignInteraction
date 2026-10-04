using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectGrabber : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("How far away the player can grab objects from.")]
    public float grabRange = 4f;
    [Tooltip("How fast the held object moves to the hold point. Higher = snappier.")]
    public float holdSmoothing = 15f;

    //The point in front of the camera, where the object is held.
    public Transform holdPoint;

    //how much force is applied when throwing
    public float throwForce = 15f;

    private Rigidbody heldObject;
    private bool isHolding = false;

    private InteractableObject currentHighlight;

    void FixedUpdate()
    {
        //fixed update runs on an interval schedule
        //we move the held object here, so it stays smooth & physics is accurate
        if(isHolding && heldObject != null)
        {
            MoveHeldObject();
        }
    }

    void Update()
    {
        //run the detection raycast every frame to update the highlight 
        //this is different from grab raycast, it just checks what the player is looking at
        //and highlights/unhighlights accordingly.
        UpdateHighlight();
        if(heldObject == null )
        {
            isHolding = false;
        }
    }

    void TryGrab()
    {
        //We don't necessarily need to check if we are holding an object before picking one up,
        //since the object we are 
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        //drawing the ray for debugging
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.magenta, 0.5f);

        if (Physics.Raycast(ray, out hit, grabRange))
        {
            //check if the hit object has the interactable marker script
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();
            if(interactable != null)
            {
                if(interactable.isHarvestable == true)
                {
                    GameObject thisParent = interactable.transform.parent.gameObject;
                    GameObject harvest = Instantiate(interactable.harvest, this.gameObject.transform.position, Quaternion.identity);

                    Debug.Log("Instantiated harvest prefab");
                    isHolding = true;
                    Destroy(thisParent);
                    Destroy(interactable);
                    return;
                }
                //get rigidbody so we can move it with physics
                heldObject = hit.collider.GetComponent<Rigidbody>();
                if(heldObject != null)
                {
                    //disable gravity so it floats in front of us when held.
                    heldObject.useGravity = false;

                    //freeze rotation so it doesn't spin around while carried
                    heldObject.freezeRotation = true;

                    //zero out any existing velocity so it doesn't fly away
                    heldObject.linearVelocity = Vector3.zero;
                    heldObject.angularVelocity = Vector3.zero;

                    //unhighlight when grabbed, object is now in hand
                    interactable.UnHighlight();
                    currentHighlight = null;

                    isHolding = true;
                    Debug.Log("Grabbed " + heldObject.name);
                }
            }
        }
    }

    //called every fixed update while holding an object
    //smoothly moves the rigidbody toward the hold point
    void MoveHeldObject()
    {
        Vector3 targetPosition = holdPoint.position;
        Vector3 currentPosition = heldObject.position;

        //smoothly interpolate toward the whole point
        //move position respects physics collisions, objects won't clip through walls/obstacles

        Vector3 newPosition = Vector3.Lerp(currentPosition, targetPosition, holdSmoothing * Time.fixedDeltaTime); //fixed update runs @ scheduled intervals
        heldObject.MovePosition(newPosition);
    }

    //Drop releases the object & restores normal physics behaviors
    void DropObject()
    {
        if (heldObject == null) { return; }

        //re enable gravity & rotation
        heldObject.useGravity = true;
        heldObject.freezeRotation = false;

        heldObject = null; //clearing it
        isHolding = false;
        Debug.Log("Dropped object.");

    }

    //releases the object & launches it forward using AddForce
    void ThrowObject()
    {
        if (heldObject == null) { return; }

        //reenable physics & rotation
        heldObject.useGravity = true;
        heldObject.freezeRotation = false;

        //apply force in the direction the camera is facing
        //forcemode.impulse applies force INSTANTLY like a punch
        //as opposed to forcemode.force whic applies force gradually over time
        heldObject.AddForce(transform.forward * throwForce, ForceMode.Impulse);

        heldObject = null;
        isHolding = false;
        Debug.Log("Threw object.");
    }

    public void OnGrabPerformed(InputAction.CallbackContext context)
    {
        if (isHolding) DropObject();
        else TryGrab();
    }

    public void OnThrowPerformed(InputAction.CallbackContext context)
    {
        if (isHolding) ThrowObject();

    }

    void UpdateHighlight()
    {
        //don't change highlight while holding an object.
        if(isHolding) return;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        //checks that if we aren't hitting something, we unhighlight 
        bool hitResult = Physics.Raycast(transform.position, transform.forward, out hit);
        //Debug.Log(hitResult);
        if (hitResult)
        {

        }
        else
        {
            if (currentHighlight != null)
            {
                //Debug.Log("No hit detected");
                currentHighlight.UnHighlight();
                currentHighlight = null;
                return;
            }
        }

        if (Physics.Raycast(ray, out hit, grabRange))
        {
            InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                //if we are looking at a different object, unhighlight the old one
                if(currentHighlight != null && currentHighlight != interactable)
                {
                    currentHighlight.UnHighlight();
                    //Debug.Log("Un-Highlighted.");
                }
                //highlight the new object.
                interactable.Highlight();
                currentHighlight = interactable;
                return;
            }

            //raycast hits nothing interactable. Clear highlight
            if (currentHighlight != null)
            {
                currentHighlight.UnHighlight();
                currentHighlight = null;
            }
        }
        if (currentHighlight != null)
        {
            float dist = Vector3.Distance(currentHighlight.gameObject.transform.position, this.gameObject.transform.position);
            if (dist > grabRange)
            {
                currentHighlight.UnHighlight();
                currentHighlight = null;
            }
        }
    }

}
