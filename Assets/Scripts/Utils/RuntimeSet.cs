using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class RuntimeSet<T> : ScriptableObject
{
    [NonSerialized]
    private List<T> items = new List<T>();

    public void Initialize()
    {
        items.Clear();
    }

    public T this[int index] => items[index];
    public bool Contains(T item) => items.Contains(item);
    public int Count => items.Count;

    public void Add(T item)
    {
        if (!items.Contains(item))
        {
            items.Add(item);
        } 
    }

    public void Remove(T item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        } else
        {
            Debug.LogWarning("Tried to remove an item that isn't in the set");
        }
    }
}
