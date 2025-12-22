using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Database")]
public class DialogueDatabase : ScriptableObject
{
    [SerializeField] List<DialogueParagraph> paragraphs;

    void OnEnable()
    {
        for (int id = 0; id < paragraphs.Count; ++id)
        {
            paragraphs[id].id = id;
        }
    }

    public DialogueParagraph this[int id]
    {
        get => paragraphs[id];
    }
}