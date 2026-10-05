using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn;
using Yarn.Unity;

public class NPC_Interact : MonoBehaviour
{
    //Done! check if player is in the trigger radius
    //Done! activate interaction text
    //Done! check if player is in the trigger when they press the interaction key
    //Done! activate Yarn Spinner -- our Dialogue runner
    //Done! Checking player controller for movement
    //Done! player movement is locked

    //semi finished... 'limit' (and eventually control) camera movement

    public TextMeshProUGUI interactText;
    public string interaction;
    public bool canInteract;
    public DialogueRunner dialogue;
    public NPC_Cam_Controller camController;
    public NPC_Data npcData;

    public void Start()
    {
        //this behavior is a bit 'static' compared to previous system design techniques
        //but we are making this more 'hard-coded' in an attempt to standardize our NPC_Cam_Rig system
        camController = transform.GetComponentInChildren<NPC_Cam_Controller>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setText(interaction);
            canInteract = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setText("");
            canInteract = false;
        }
    }

    //setter method for interact text
    public void setText(string txt)
    {
        interactText.text = txt;
    }

    public void Update()
    {
        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) //new syntax, no longer getKeyDown
            {
                Debug.Log("You talked to the NPC");
                setText("");
                camController.movePlayer();
                dialogue.StartDialogue(npcData.startingNode); //be mindful of the case of yarnSpinner node
                canInteract = false;
            }
        }
    }
    //this is the set of the current phase of our YS dialogue, pulled from our npcData variable
    [YarnCommand("dialoguePhaseSet")]
    public void setYarnPhase()
    {
        InMemoryVariableStorage vData = GameObject.FindAnyObjectByType<InMemoryVariableStorage>();
        vData.SetValue("$dialoguePhase", npcData.currentPhase.ToString());
    }

    //this is the 'get' of the current phase MADE in our yarnSpinner dialogue based on what options we have selected
    [YarnCommand("dialoguePhaseGetter")]
    public void getYarnPhase(string phase)
    {
        if (phase == "questTaken")
        {
            npcData.currentPhase = NPC_Data.dialoguePhase.questTaken;
        }
        if (phase == "questCompleteReturn")
        {
            npcData.currentPhase = NPC_Data.dialoguePhase.questCompleteReturn;
        }
    }

    public void OnApplicationQuit()
    {
        npcData.resetData();
    }
}
