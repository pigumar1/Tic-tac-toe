using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    public void PublishShowUIEvent()
    {
        EventBus.Publish(new ShowUIEvent
        {
            uiType = GetType(),
        });
    }
}
