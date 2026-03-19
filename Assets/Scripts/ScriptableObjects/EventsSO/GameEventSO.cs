using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Game Event", menuName = "ScriptableObjects/GameEventSO", order = 1)]
public class GameEventSO : ScriptableObject
{

    private readonly List<GameEventListener> listeners = new List<GameEventListener>();

    public void Raise()
    {
        Debug.Log($"[Demo Event] '{name}' déclenché ! ({listeners.Count} listener(s))");

        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        if (!listeners.Contains(listener))
            listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (listeners.Contains(listener))
            listeners.Remove(listener);
    }
}
