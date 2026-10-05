using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxInteract : MonoBehaviour
{
    //Done!: check if player is in range of boxes to enable interaction
    //Done!: display interaction text

    //TODO: destroy boxes once you "get" them
    //TODO: call dialogue runner to initiate a node for interaction 
    //TODO: load the NPC_Interact class to activate next phase of "quest"

    public bool canInteract;
    public TextMeshProUGUI interactText;
    public NPC_Interact npcI;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Player entered box trigger");
            setText("Press E");
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            setText("");
            canInteract= false;
        }
    }

    public void setText(string text)
    {
        interactText.text = text;
    }


    public void Update()
    {
        if(canInteract)
        {
            if(Keyboard.current.eKey.wasPressedThisFrame)
            {
                npcI.npcData.currentPhase = NPC_Data.dialoguePhase.questComplete;
                canInteract = false;
            }
        }
    }
}
