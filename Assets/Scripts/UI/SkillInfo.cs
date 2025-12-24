using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfo : UIBase
{
    private void Awake()
    {
        EventBus.Subscribe<ShowUIEvent>(e =>
        {
            if (e.uiType == typeof(SkillInfo))
            {
                gameObject.SetActive(true);

                StartCoroutine(CoroutineUpdate());
            }
        });
    }

    IEnumerator CoroutineUpdate()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                yield return null;

                EventBus.Publish(new HideUIEvent());
                gameObject.SetActive(false);
            }

            yield return null;
        }
    }
}
