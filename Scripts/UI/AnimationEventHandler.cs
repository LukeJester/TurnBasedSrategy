using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationEventHandler : MonoBehaviour
{
    public static event EventHandler<OnAnyThrow> OnAnyThrowActionAnimation;

    public class OnAnyThrow : EventArgs
    {
        public Unit unit;
    }

    private Unit unit;

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }

    void OnThrowAnimation()
    {
        OnAnyThrowActionAnimation?.Invoke(this, new OnAnyThrow{ unit = unit });
    }
}
