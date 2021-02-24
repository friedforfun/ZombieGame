using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{

    // EventManager inspiration:
    // https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa
    
    private Dictionary<string, UnityEvent> eventDictionary;

    // Lazy EventManager
    private static readonly Lazy<EventManager> singleton = new Lazy<EventManager>(() => Init(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static EventManager eventManager { get { return singleton.Value; } }


    private static EventManager Init()
    {
        EventManager eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
        if (!eventManager)
        {
            Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
        }
        else if (eventManager.eventDictionary == null)
        {
            eventManager.eventDictionary = new Dictionary<string, UnityEvent>();
        }
        return eventManager;
    }

    /// <summary>
    /// Add listener to event with eventName
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StartListening(string eventName, UnityAction listener)
    {
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventManager.eventDictionary.Add(eventName, thisEvent);
        }
    }

    /// <summary>
    /// Remove listener from event with this eventName
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StopListening(string eventName, UnityAction listener)
    {
        if (singleton == null) return;
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    /// <summary>
    /// Trigger Event by eventName string
    /// </summary>
    /// <param name="eventName"></param>
    public static void TriggerEvent(string eventName)
    {
        UnityEvent thisEvent = null;
        if (eventManager.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
        }
    }

}
