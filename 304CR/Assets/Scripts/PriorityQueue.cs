using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<P,V> {

    private SortedDictionary<P, LinkedList<V>> list = new SortedDictionary<P, LinkedList<V>>();
    public int count=0;
    //add new item to queue
    public void Enqueue(V value, P priority)
    {
        LinkedList<V> newItem;
        //check if priority level exists in current queue
        if(!list.TryGetValue(priority, out newItem))
        {
            //add priority level to queue
            newItem = new LinkedList<V>();
            list.Add(priority, newItem);
        }
        //add item to list 
        newItem.AddLast(value);
        count=list.Count;
    }
    
    //remove first element in queue with the higest priority
    public V Dequeue()
    {
        //get current enumerator
        SortedDictionary<P, LinkedList<V>>.KeyCollection.Enumerator currentEnum = list.Keys.GetEnumerator();
        //move to next enumerator 
        currentEnum.MoveNext();
        P key = currentEnum.Current;
        LinkedList<V> v = list[key];
        V res = v.First.Value;
        v.RemoveFirst();
        if(v.Count == 0)
        {
            list.Remove(key);
        }
        count = list.Count;
        return res;
    }

    public bool isEmpty()
    {
        if (list.Count ==0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override string ToString()
    {
        string res = "";
        foreach(P key in list.Keys)
        {
            foreach(V val in list[key])
            {
                res += val + ", ";
            }
        }
        return res;
    }
}