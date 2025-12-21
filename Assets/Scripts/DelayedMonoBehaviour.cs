using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DelayedMonoBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        EventBus.Subscribe<EndSceneTransitionEvent>(DelayedStart);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<EndSceneTransitionEvent>(DelayedStart);
    }

    protected abstract void DelayedStart(EndSceneTransitionEvent _);
}
