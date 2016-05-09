using UnityEngine;
using System.Collections;

public class EnemyColossus : EntityClass 
{
	[SerializeField] float maxHealth = 10000;

	void Awake () 
	{
		maxH = maxHealth;
	}

	void Update()
	{
		Debug.Log(gameObject.name + "'s Health: " + health);

		if(health <= 0)
		{
			Destroy(gameObject);
		}
	}
}
