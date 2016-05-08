using UnityEngine;
using System.Collections;

public class EntityClass : MonoBehaviour 
{
	protected float maxH = 1;
	protected float health = 1;

	void Start()
	{
		health = maxH;
	}

	public float TakeDamage(float damage) 
	{
		health -= damage;
		return health;
	}

	public float Heal(float heal) 
	{
		if(health != maxH)
		{
			health += heal;
		}
		return health;
	}
}
