using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance
    {
        get; private set;
    }

    [SerializeField] private Queue<KeyItem> keyItemQueue = new Queue<KeyItem>();

    public int keyItemCount;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    public void AddKeyItem(KeyItem item)
    {
        keyItemQueue.Enqueue(item);
        Debug.Log("Added key Total: " + keyItemQueue.Count);
        keyItemCount += 1;
    }

    public void UseNextKeyItem()
    {
        if (keyItemQueue.Count == 0)
        {
            Debug.Log("No Key left!");
            return;
        }

        keyItemCount -= 1;
        KeyItem nextItem = keyItemQueue.Dequeue();
        nextItem.Collect(); 
        Debug.Log("Used. Remaining: " + keyItemQueue.Count);
    }

    public int GetKeyItemCount()
    {
        return keyItemQueue.Count;
    }

}
