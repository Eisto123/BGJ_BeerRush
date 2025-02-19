using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventHandler
{
    public static event Action<int> GetPointEvent;
    public static void CallGetPointEvent(int point)
    {
        GetPointEvent?.Invoke(point);
    }

    public static event Action GameOverEvent;
    public static void CallGameOverEvent()
    {
        GameOverEvent?.Invoke();
    }
}
