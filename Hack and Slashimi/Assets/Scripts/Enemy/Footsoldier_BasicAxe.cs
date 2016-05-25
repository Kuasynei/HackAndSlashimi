using UnityEngine;
using System.Collections;

public class Footsoldier_BasicAxe : WeaponClass {

	[SerializeField] Color debugDeadlinessColor;
	MeshRenderer mR;

	protected override void Awake() 
	{
		base.Awake ();
		mR = GetComponentInChildren<MeshRenderer> ();
	}

	//This attack a certain amount of damage numerous times over its attack duration, changing color when active.
	public void BasicAttack(float delay, float attackDuration, int ticksOfDamage, OmniAttackInfo damagePackage) 
	{
		StartCoroutine (execBasicAttack(delay, attackDuration, ticksOfDamage, damagePackage));
	}

	//Handle how the actual attack plays out step-by-step here.
	IEnumerator execBasicAttack(float delay, float attackDuration, int ticks, OmniAttackInfo damagePackage) 
	{
		yield return new WaitForSeconds (delay);

		transform.Rotate (new Vector3(75, 0, 0), Space.Self); //Delete once we get animations.

		Color originalColor = mR.material.color;
		mR.material.color = debugDeadlinessColor;
		TickWeaponizedContact (attackDuration, ticks, damagePackage);
		yield return new WaitForSeconds (attackDuration);

		transform.Rotate (new Vector3(-75, 0, 0), Space.Self); //Delete once we get animations.

		mR.material.color = originalColor;
	}
}
