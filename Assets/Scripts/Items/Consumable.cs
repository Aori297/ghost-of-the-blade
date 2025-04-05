using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour, ICollectable
{
    public void Collect()
    {
        PlayerHealthStamina.Instance.Heal(40);
        Destroy(gameObject);
    }

}
