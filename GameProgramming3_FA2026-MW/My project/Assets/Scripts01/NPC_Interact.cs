using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn;
using Yarn.Unity;

public enum iType { npc, item }

public class NPC_Interact : MonoBehaviour
{
    [Tooltip("Set the type of interaction to load the correct variables.")]
    public iType type;

    //we changed our variables to be grouped within structs for visual clarity
    //and to be able to call groupings easier in editor script.
    [System.Serializable]
    public struct TextData
    { 
        public TextMeshProUGUI interactText;
        public string interaction;
    }
    [Header("Text Display Variables")]
    public TextData textData;
    public bool canInteract;
    [System.Serializable]
    public struct CamData
    { 
        public NPC_Cam_Controller camController;
        public GameObject npcCamPrefab;
    }
    [Header("Camera Data for Dialogue Interaction")]
    public CamData camData;
    [System.Serializable]
    public struct DialogData
    {
        public DialogueRunner dialogue;
        public NPC_Data npcData;
    }
    public DialogData dialogData;
    public bool questComplete = false;


    public void Start()
    {
        //this behavior is a bit 'static' compared to previous system design techniques
        //but we are making this more 'hard-coded' in an attempt to standardize our NPC_Cam_Rig system
        //camController = transform.GetComponentInChildren<NPC_Cam_Controller>();
    }

    //-----EDITOR FUNCTIONS START-----

    public void SetTextVars()
    {
        textData.interactText = GameObject.FindGameObjectWithTag("pText").GetComponent<TextMeshProUGUI>();
    }

    public void LoadNpcCamPrefab()
    {
        Instantiate(camData.npcCamPrefab, transform);
    }

    public void SetNpcCam()
    {
        camData.camController = transform.GetComponentInChildren<NPC_Cam_Controller>();
    }

    public void SetDialogue()
    {
        dialogData.dialogue = GameObject.FindAnyObjectByType<DialogueRunner>();
        textData.interaction = "Press E to Talk";
    }

    //-----EDITOR FUNCTIONS COMPLETE-----


    private void Awake()
    {
        if(GameController.instance.currentCoins >= dialogData.npcData.amountNeeded)
        {
            questComplete = true;
            dialogData.npcData.currentPhase = NPC_Data.dialoguePhase.questComplete;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            setText(textData.interaction);
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
        textData.interactText.text = txt;
    }

    public void Update()
    {
        if (canInteract)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame) //new syntax, no longer getKeyDown
            {
                Debug.Log("You talked to the NPC");
                setText("");
                camData.camController.movePlayer();
                dialogData.dialogue.StartDialogue(dialogData.npcData.startingNode); //be mindful of the case of yarnSpinner node
                canInteract = false;
            }
        }
    }
    //this is the set of the current phase of our YS dialogue, pulled from our npcData variable
    [YarnCommand("dialoguePhaseSet")]
    public void setYarnPhase()
    {
        InMemoryVariableStorage vData = GameObject.FindAnyObjectByType<InMemoryVariableStorage>();
        vData.SetValue("$dialoguePhase", dialogData.npcData.currentPhase.ToString());
    }

    //this is the 'get' of the current phase MADE in our yarnSpinner dialogue based on what options we have selected
    [YarnCommand("dialoguePhaseGetter")]
    public void getYarnPhase(string phase)
    {
        Debug.Log("GetYarnPhase called. Setting phase to " + phase);
        if (phase == "questTaken")
        {
            dialogData.npcData.currentPhase = NPC_Data.dialoguePhase.questTaken;
            GameController.instance.farmerState = "questTaken";
            Debug.Log("farmerState is = " + GameController.instance.farmerState);
        }
        if (phase == "questCompleteReturn")
        {
            dialogData.npcData.currentPhase = NPC_Data.dialoguePhase.questCompleteReturn;
            GameController.instance.farmerState = "questCompleteReturn";
            Debug.Log("farmerState is = " + GameController.instance.farmerState);
        }
        if(phase == "repeat")
        {
            dialogData.npcData.currentPhase = NPC_Data.dialoguePhase.repeat;
            GameController.instance.farmerState = "repeat";
            Debug.Log("farmerState is = " + GameController.instance.farmerState);
        }
    }

    public void OnApplicationQuit()
    {
        dialogData.npcData.resetData();
    }
}
