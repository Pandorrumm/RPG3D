using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float damage; // сколько наносит урона

    public float GetDamage //получаем урон
    {
        get
        {
            return damage;
        }
    }
}
