using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGamePrompt : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button confirmButton;

    string playerName => inputField.text;

    public void Apply()
    {
        EventBus.Publish(new CreateSaveDataEvent
        {
            playerName = playerName
        });

        EventBus.Publish(new BeginSceneLoadEvent
        {
            sceneName = "Tutorial1"
        });
    }

    public void UpdateWithName()
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            confirmButton.interactable = false;
            return;
        }

        int length = playerName.Trim().Length;

        confirmButton.interactable = length >= 1 && length <= 12;
    }
}
