using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<DialogueSection> dialogueSections;

    // Start is called before the first frame update
    void Start()
    {
        EventBus.Subscribe<EndSceneTransitionEvent>(RealStart);
        EventBus.Publish(new EndSceneLoadEvent());
    }

    private void RealStart(EndSceneTransitionEvent _)
    {
        EventBus.Publish(new BeginDialogueEvent
        {
            sections = dialogueSections
        });
    }
}
