﻿using UnityEngine;

public class ChargedAttack : MonoBehaviour
{
    [HideInInspector] public bool charged = false;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (charged)
        {
            if (collision.tag == "Player") Debug.Log("player damaged");

            Destroy(gameObject);
        }
    }
}