using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;
    private static readonly Queue<System.Action> Actions = new Queue<System.Action>();

    private void Awake()
    {
        _instance = this;
    }

    public static void Enqueue(System.Action action)
    {
        lock (Actions)
        {
            Actions.Enqueue(action);
        }
    }

    private void Update()
    {
        while (Actions.Count > 0)
        {
            System.Action action;
            lock (Actions)
            {
                action = Actions.Dequeue();
            }
            action?.Invoke();
        }
    }
}
