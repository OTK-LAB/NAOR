using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownController : MonoBehaviour
{
    private Queue<GameObject> itemQueue;
    private float nextDequeueTime;
    private float cooldownTime;
    
    private void Start()
    {
        itemQueue = new Queue<GameObject>();
    }

    public void SetCooldown(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;
    }

    //bir GameObject alıp Queue'ya yerleştir
    public void EnqueueItem(GameObject item)
    {
        if(item != null){
            //Eğer Queue'ya giren ilk item buysa nextQueueTime'ı senkronize et
            if (itemQueue.Count == 0)
            {
                nextDequeueTime = Time.time + cooldownTime;
            }
            itemQueue.Enqueue(item);
        }
        Debug.Log("Item Queued!\nItems in Queue: " + itemQueue.Count );
    }

    public GameObject DequeueLastItem()
    {
        GameObject item = itemQueue.Dequeue();
        nextDequeueTime = Time.time + cooldownTime;
        Debug.Log("Item Dequeued!\nItems in Queue: " + itemQueue.Count );
        return item;
    }

    public Queue<GameObject> GetQueue()
    {
        return itemQueue;
    }

    public float GetDequeueTime()
    {
        return nextDequeueTime;
    }
}