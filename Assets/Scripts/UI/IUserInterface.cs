using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserInterface
{
    public void PublishShowUIEvent()
    {
        EventBus.Publish(new ShowUIEvent
        {
            uiType = GetType(),
        });
    }
}
