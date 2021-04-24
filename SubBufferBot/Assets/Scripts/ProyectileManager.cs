using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileManager : MonoBehaviour
{
    Proyectile[] proyectiles;
    private void Awake()
    {
        proyectiles = GetComponentsInChildren<Proyectile>(true);
    }
    public void ReleaseProyectile(int chargeLvl, Vector3 direction)
    {
        for (int i = 0; i < proyectiles.Length; i++)
        {
            if (!proyectiles[i].gameObject.activeInHierarchy)
            {
                proyectiles[i].gameObject.SetActive(true);
                proyectiles[i].Fire(chargeLvl, direction);
                break;
            }
        }
    }

}
