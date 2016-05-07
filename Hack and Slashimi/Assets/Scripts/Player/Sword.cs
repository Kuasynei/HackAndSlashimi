using UnityEngine;
using System.Collections;

public class Sword : WeaponClass {

	[SerializeField] Color debugDeadlinessColor;
	MeshRenderer mR;

	void Awake() {
		mR = GetComponentInChildren<MeshRenderer> ();
	}

	//This attack ticks three times over 0.6 seconds, changing color when lethal.
	public void BasicAttack(float attackDuration, int ticksOfDamage) {
		StartCoroutine (execBasicAttack(attackDuration, ticksOfDamage));
	}

	IEnumerator execBasicAttack(float execAttackDuration, int execTicksOfDamage) {
		Color originalColor = mR.material.color;
		mR.material.SetColor ("_Color", debugDeadlinessColor);
		TickContactDeadliness (execAttackDuration, execTicksOfDamage);
		yield return new WaitForSeconds (execAttackDuration);

		mR.material.color = originalColor;
	}
}
