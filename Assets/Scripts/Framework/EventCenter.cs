using System;
using System.Collections.Generic;

/// <summary>
/// 全局事件中心
/// </summary>
public class EventCenter
{
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    // 无参数事件
    public static void AddListener(string eventType, Action handler)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }
        eventTable[eventType] = (Action)eventTable[eventType] + handler;
    }

    public static void Broadcast(string eventType)
    {
        if (eventTable.TryGetValue(eventType, out Delegate d))
        {
            Action handler = d as Action;
            handler?.Invoke();
        }
    }

    public static void RemoveListener(string eventType, Action handler)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = (Action)eventTable[eventType] - handler;
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 单参数事件
    public static void AddListener<T>(string eventType, Action<T> handler)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }
        eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
    }

    public static void Broadcast<T>(string eventType, T arg)
    {
        if (eventTable.TryGetValue(eventType, out Delegate d))
        {
            Action<T> handler = d as Action<T>;
            handler?.Invoke(arg);
        }
    }

    public static void RemoveListener<T>(string eventType, Action<T> handler)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }

    // 双参数事件
    public static void AddListener<T1, T2>(string eventType, Action<T1, T2> handler)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }
        eventTable[eventType] = (Action<T1, T2>)eventTable[eventType] + handler;
    }

    public static void Broadcast<T1, T2>(string eventType, T1 arg1, T2 arg2)
    {
        if (eventTable.TryGetValue(eventType, out Delegate d))
        {
            Action<T1, T2> handler = d as Action<T1, T2>;
            handler?.Invoke(arg1, arg2);
        }
    }

    public static void RemoveListener<T1, T2>(string eventType, Action<T1, T2> handler)
    {
        if (eventTable.ContainsKey(eventType))
        {
            eventTable[eventType] = (Action<T1, T2>)eventTable[eventType] - handler;
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }
    }
}
