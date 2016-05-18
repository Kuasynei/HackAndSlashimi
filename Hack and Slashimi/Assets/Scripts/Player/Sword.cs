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

	//This attack a certain amount of damage numerous times over its attack duration, changing color when active.
	public void BasicAttack(float attackDuration, int ticksOfDamage, DamageInfo damagePackage) 
	{
		myDamagePackage = damagePackage;
		StartCoroutine (execBasicAttack(0, attackDuration, ticksOfDamage));
	}

	//This variant includes a delay.
	public void BasicAttack(float delay, float attackDuration, int ticksOfDamage, DamageInfo damagePackage) 
	{
		myDamagePackage = damagePackage;
		StartCoroutine (execBasicAttack(delay, attackDuration, ticksOfDamage));
	}

	IEnumerator execBasicAttack(float delay, float attackDuration, int ticksOfDamage) 
	{
		yield return new WaitForSeconds (delay);

		transform.Rotate (new Vector3(75, 0, 0), Space.Self); //Delete once we get animations.

		Color originalColor = mR.material.color;
		mR.material.color = debugDeadlinessColor;
		TickContactDeadliness (attackDuration, ticksOfDamage);
		yield return new WaitForSeconds (attackDuration);

		transform.Rotate (new Vector3(-75, 0, 0), Space.Self); //Delete once we get animations.

		mR.material.color = originalColor;
	}

	//An attack that launches nearby enemies into the air and deals damage.
	public void LaunchingAttack(float delay, Vector3 launchVector, DamageInfo damagePackage)
	{
		myDamagePackage = damagePackage;
		myLaunchVector = launchVector;
		StartCoroutine (execLaunchingAttack(delay));
	}

	IEnumerator execLaunchingAttack(float delay)
	{
		yield return new WaitForSeconds (delay);

		Color originalColor = mR.material.color;
		mR.material.color = debugDeadlinessColor;
		ToggleDeadliness (true);
		ToggleLaunchiness (true);

		yield return null;

		mR.material.color = originalColor;
		ToggleDeadliness (false);
		ToggleLaunchiness (false);
	}
}
