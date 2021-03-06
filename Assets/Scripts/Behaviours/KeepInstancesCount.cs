﻿using System;
using UnityEngine;

/// <summary>
/// This MonoBehaviour uses a static variable to keep count of
/// instances of itself and sends a message when zero is reached.
/// The message passes this class which can then be used to inspect
/// e.g. the gameobject's tag.
/// </summary>

public class KeepInstancesCount : MonoBehaviour {

    //
    // Static fields
    //
    public static int Count { get; private set; }

    //
    //  Events
    //
    public static Action<KeepInstancesCount> OnLastDestroyed;

    //
    // Increment count on Awake. 
    //
    private void Awake()
    {
        Count += 1;
    }

    //
    // Decrement count on Destroy. Fire event if zero. 
    //
    private void OnDestroy()
    {
        Count -= 1;
        if (Count == 0)
        {
            if (OnLastDestroyed != null) { OnLastDestroyed(this); }
        }
    }
}
