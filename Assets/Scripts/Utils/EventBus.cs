using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> _events = new();

    // 订阅
    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);

        if (_events.TryGetValue(type, out var existing))
            _events[type] = Delegate.Combine(existing, handler);
        else
            _events[type] = handler;
    }

    // 取消订阅
    public static void Unsubscribe<T>(Action<T> handler)
    {
        var type = typeof(T);

        if (!_events.TryGetValue(type, out var existing))
            return;

        var current = Delegate.Remove(existing, handler);

        if (current == null)
            _events.Remove(type);
        else
            _events[type] = current;
    }

    // 发布事件
    public static void Publish<T>(T evt)
    {
        if (_events.TryGetValue(typeof(T), out var del))
            ((Action<T>)del)?.Invoke(evt);
    }

    // 可选：清空（切场景时）
    public static void Clear()
    {
        _events.Clear();
    }
}
