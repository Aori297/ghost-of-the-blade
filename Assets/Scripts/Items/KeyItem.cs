using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : MonoBehaviour, ICollectable
{
    [SerializeField] private int id;
    public void Collect()
    {
        PlayerInventory.Instance.AddKeyItem(this);
        PlayerController.Instance.OnAbilityUnlock(id);
        Destroy(gameObject);
    }

}
