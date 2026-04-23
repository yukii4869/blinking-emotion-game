using System;
using UnityEngine;

public abstract class BaseCondition : MonoBehaviour
{
    public event Action OnCompleted;
    public event Action OnFailed;

    protected bool isActive = false;

    public virtual void ActivateCondition()
    {
        isActive = true;
    }

    protected void Complete()
    {
        isActive = false;
        OnCompleted?.Invoke();
        this.enabled = false;
    }

    protected void Fail()
    {
        isActive = false;
        OnFailed?.Invoke();
    }
}

