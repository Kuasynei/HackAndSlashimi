using UnityEngine;
using System.Collections;

public class EntityClass : MonoBehaviour {
	protected float health = 1;

	public float TakeDamage(float damage) {
		health -= damage;
		return health;
	}

	protected float Heal(float heal) {
		heal += heal;
		return health;
	}
}
