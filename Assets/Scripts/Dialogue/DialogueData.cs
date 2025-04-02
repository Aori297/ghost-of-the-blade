using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Conversation")]
public class DialogueData : ScriptableObject
{
    [Serializable]
    public class DialogueBranch
    {
        [TextArea(3, 10)]
        public string[] dialogueLines;
        public DialogueChoice[] choices;
        public bool choiceCheck;
        public bool triggerBattle;
        public Sprite npcImage;
        public string npcName;
    }

    [Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public int nextBranchIndex;
        public bool hasOutcome;
        public string outcomeKey;
    }

    public DialogueBranch[] dialogueBranches;
    public int startingBranchIndex = 0;
}