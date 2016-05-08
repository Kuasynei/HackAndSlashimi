using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyPool : MonoBehaviour
{

    public GameObject enemy;
    public int pooledAmount = 5;
    public bool willGrow = true;

    public List<GameObject> pEnemies;

    // Use this for initialization
    void Start()
    {

        pEnemies = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject temp = (GameObject)Instantiate(enemy);
            temp.SetActive(false);
            pEnemies.Add(enemy);
        }
    }

    public GameObject GetFromPool()
    {
        for (int i = 0; i < pEnemies.Count; i++)
        {
            if (pEnemies[i] == null)
            {
                GameObject temp = (GameObject)Instantiate(enemy);
                temp.SetActive(false);
                pEnemies[i] = temp;
                return pEnemies[i];
            }
            if (!pEnemies[i].activeInHierarchy)
            {
                return pEnemies[i];
            }
        }

        if (willGrow)
        {
            GameObject temp = (GameObject)Instantiate(enemy);
            pEnemies.Add(temp);
            return temp;
        }

        return null;
    }
}