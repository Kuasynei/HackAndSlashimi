using UnityEngine;
using System.Collections;

public class EntityClass : MonoBehaviour {
	protected float health = 1;

	public float TakeDamage(float damage) {
		health -= damage;
		return health;
	}

	public float Heal(float heal) {
		health += heal;
		return health;
	}
}
