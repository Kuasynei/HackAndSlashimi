using UnityEngine;
using System.Collections;

public class Sword : MeleeWeaponClass 
{
	[SerializeField] Color debugDeadlinessColor;
	MeshRenderer mR;

	protected override void Awake() 
	{
		base.Awake ();
		mR = GetComponentInChildren<MeshRenderer> ();
	}

	//This attack ticks three times over 0.6 seconds, changing color when lethal.
	public void BasicAttack(float attackDuration, int ticksOfDamage, DamageInfo damagePackage) 
	{
		myDamagePackage = damagePackage;
		contactDamage = damagePackage.damage;
		StartCoroutine (execBasicAttack(0, attackDuration, ticksOfDamage));
	}

	//This variant includes a delay.
	public void BasicAttack(float delay, float attackDuration, int ticksOfDamage, DamageInfo damagePackage) 
	{
		myDamagePackage = damagePackage;
		contactDamage = damagePackage.damage;
		StartCoroutine (execBasicAttack(delay, attackDuration, ticksOfDamage));
	}

	IEnumerator execBasicAttack(float delay, float execAttackDuration, int execTicksOfDamage) 
	{
		yield return new WaitForSeconds (delay);

		transform.Rotate (new Vector3(75, 0, 0), Space.Self); //Delete once we get animations.

		Color originalColor = mR.material.color;
		mR.material.color = debugDeadlinessColor;
		TickContactDeadliness (execAttackDuration, execTicksOfDamage);
		yield return new WaitForSeconds (execAttackDuration);

		transform.Rotate (new Vector3(-75, 0, 0), Space.Self); //Delete once we get animations.

		mR.material.color = originalColor;
	}
}
