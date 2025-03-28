using System;
using System.Collections;
using UnityEngine;

public class ServiceCoroutine : Singleton<ServiceCoroutine> 
{
    public Coroutine RunCoroutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    public void DelayAction(float seconds, Action action)
    {
        StartCoroutine(DelayedAction(seconds, action));
    }

    private IEnumerator DelayedAction(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }
}
