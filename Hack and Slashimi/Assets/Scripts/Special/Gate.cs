using UnityEngine;
using System.Collections;

public class Gate : EntityClass 
{
	[SerializeField] float maxHealth = 100000;

	void Awake () 
	{
		health = maxHealth;
	}

	void Update()
	{
		//Debug.Log(gameObject.name + "'s Health: " + health);

		if(health <= 0)
		{
			Destroy(gameObject);
		}
	}
}
