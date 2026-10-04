using Unity.Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class NPC_Cam_Controller : MonoBehaviour
{
    //Done! switch from TPC camera to NPC_cCam (modifying 'priority' attribute)
    //Done! create methods for switching the NPC_cCam transform.position
    //Done! get switching transform position methods to talk to yarnSpinner

    //reposition the TPC 'into frame'(so that we don't look weird)

    public Transform npc_loc;
    public Transform pc_loc;
    public Transform pc_move_loc;
    public CinemachineCamera npc_cCam;
    public int priorityValue;

    //need additional transform variable for where we want the player character to move to

    [YarnCommand("cam_move")] //giving an attribute that yarnSpinner can accsess. This string is the method name to call in YS.
    public void cam_Move(string focus)
    {
        Debug.Log("cam_moved called");
        npc_cCam.Priority = priorityValue;
        if(focus == "npc")
        {
            npc_cCam.gameObject.transform.transform.position = npc_loc.position;
        }
        else if (focus == "pc")
        {
            npc_cCam.gameObject.transform.transform.position = pc_loc.position;
        }
        else if(focus == "return")
        {
            npc_cCam.Priority = 0;
        }
    }



    public void movePlayer()
    {
        GameController.instance.moveForDialogue(pc_move_loc);
    }
}
