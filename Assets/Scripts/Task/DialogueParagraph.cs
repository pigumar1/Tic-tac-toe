using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Paragraph")]
public class DialogueParagraph : ScriptableObject
{
    public int id = -1;

    public List<DialogueNode> dialogueNodes;
    public DialogueParagraph next;
    public int[] stateMod;
    public bool skippable = true;
}
