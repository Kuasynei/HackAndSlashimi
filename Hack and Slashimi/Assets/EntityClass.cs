using UnityEngine;
using System.Collections;

public class Entity
{
	protected float health = 1;

	protected float takeDamage(float damage)
	{
		health -= damage;
		return health;
	}

	protected float heal(float heal)
	{
		heal += heal;
		return health;
	}
}
