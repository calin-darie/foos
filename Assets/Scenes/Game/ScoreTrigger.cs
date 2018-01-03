using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTrigger : MonoBehaviour {

    void OnTriggerEnter(Collider c)
    {
        OnScored();
    }

    public event Action Scored;

    protected virtual void OnScored()
    {
        var handler = Scored;
        if (handler != null) handler();
    }
}
