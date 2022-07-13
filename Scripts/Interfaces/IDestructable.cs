using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDestructable
{
    void Destroy(Action OnInteractionComplete);
}
