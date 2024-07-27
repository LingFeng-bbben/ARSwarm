using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwkUnityMainThreadDispatcher : MonoBehaviour
{
    private static DwkUnityMainThreadDispatcher instance;
    private readonly Queue<System.Action> actions = new Queue<System.Action>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static DwkUnityMainThreadDispatcher Instance()
    {
        if (!instance)
        {
            throw new System.Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
        }
        return instance;
    }

    public void Enqueue(System.Action action)
    {
        lock (actions)
        {
            actions.Enqueue(action);
        }
    }

    public void Update()
    {
        while (actions.Count > 0)
        {
            actions.Dequeue().Invoke();
        }
    }
}