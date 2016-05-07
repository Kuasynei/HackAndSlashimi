using UnityEngine;
using System.Collections;

public class EntityClass : MonoBehaviour {
	protected float health = 1;

	public float takeDamage(float damage) {
		health -= damage;
		return health;
	}

	protected float heal(float heal) {
		heal += heal;
		return health;
	}
}
