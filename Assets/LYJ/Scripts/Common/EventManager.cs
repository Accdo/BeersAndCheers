using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("EventManager");
                instance = obj.AddComponent<EventManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private Dictionary<string, Action<object>> eventDic = new();

    public void AddListener(string eventName, Action<object> newListener)
    {
        if (eventDic.TryGetValue(eventName, out Action<object> currentListeners))
        {
            currentListeners += newListener;
            eventDic[eventName] = currentListeners;
        }
        else
        {
            eventDic.Add(eventName, newListener);
        }        
    }

    public void RemoveListener(string eventName, Action<object> targetListener)
    {
        if (eventDic.TryGetValue(eventName, out Action<object> currentListeners))
        {
            currentListeners -= targetListener;
            eventDic[eventName] = currentListeners;
        }
        else
        {
            Debug.Log($"Can not Remove {targetListener?.Method?.Name} From {eventName}");
        }
    }

    public void TriggerEvent(string eventName, object data = null)
    {
        if (eventDic.TryGetValue(eventName, out Action<object> currentListeners))
        {
            currentListeners?.Invoke(data);
        }
    }
}
