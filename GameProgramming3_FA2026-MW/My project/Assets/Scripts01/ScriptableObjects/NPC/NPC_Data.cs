using UnityEngine;

[CreateAssetMenu(fileName = "NPC_Data", menuName = "NPC_Data")]
public class NPC_Data : ScriptableObject
{
    public string npcName;
    public string startingNode;
    public enum dialoguePhase { start, repeat, questTaken, questComplete, questCompleteReturn };
    public dialoguePhase currentPhase;

    public void resetData()
    {
        currentPhase = dialoguePhase.start;
    }

}
