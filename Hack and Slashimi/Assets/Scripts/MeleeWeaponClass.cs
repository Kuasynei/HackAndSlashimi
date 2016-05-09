using UnityEngine;
using System.Collections;

public class MeleeWeaponClass : MonoBehaviour {

	public bool debugMode = true;
	public float contactDamage; //Damage dealt on contact.

	protected bool contactDeadly = false; //If this is true, the weapon will deal damage on contact.
	protected float contactDeadlinessTimer = 0; //If this is greater than zero the weapon will deal damage on contact.
	protected DamageInfo myDamagePackage;

	// Update is called once per frame
	void FixedUpdate () 
	{
		//Makes this weapon
		if (contactDeadlinessTimer > 0) 
		{
			contactDeadlinessTimer -= Time.fixedDeltaTime;
		} 
	}
		
	//Set the weapon's ability to deal damage. Returns the end state.
	public bool ToggleDeadliness(bool state, DamageInfo damagePackage) 
	{ 
		myDamagePackage = damagePackage;
		contactDeadly = state;
		return contactDeadly;
	}

	public bool ToggleDeadliness(bool state) 
	{ 
		contactDeadly = state;
		return contactDeadly;
	}

	//Makes the weapon deadly for a period of time. Returns the time remaining.
	public float ToggleDeadliness(float timePeriod, DamageInfo damagePackage) 
	{
		myDamagePackage = damagePackage;
		contactDeadlinessTimer = timePeriod;
		return contactDeadlinessTimer;
	}

	public float ToggleDeadliness(float timePeriod) 
	{
		contactDeadlinessTimer = timePeriod;
		return contactDeadlinessTimer;
	}

	//This fires ticks of deadliness or "ticks of damage" over a period of time.
	public void TickContactDeadliness(float totalTime, int numberOfTicks, DamageInfo damagePackage)
	{
		myDamagePackage = damagePackage;
		StartCoroutine (execTickContactDeadliness(totalTime, numberOfTicks));
	}

	public void TickContactDeadliness(float totalTime, int numberOfTicks)
	{
		StartCoroutine (execTickContactDeadliness(totalTime, numberOfTicks));
	}

	IEnumerator execTickContactDeadliness(float execTotalTime, int execNumberOfTicks)
	{
		for (int i = 0; i < execNumberOfTicks; i++) {
			contactDeadly = true;
			yield return null;
			contactDeadly = false;
			yield return new WaitForSeconds (execTotalTime / execNumberOfTicks);
		}
	}

	void OnTriggerStay(Collider otherColl)
	{
		if ((contactDeadly || contactDeadlinessTimer > 0)) 
		{
			if (otherColl.GetComponent (typeof(EntityClass))) {
				EntityClass otherEntity = otherColl.GetComponent (typeof(EntityClass)) as EntityClass;

				if (debugMode)
				{
					Debug.Log(otherColl.name + " has taken " + contactDamage + " damage! Health is now: " + otherEntity.TakeDamage (myDamagePackage));
				}
				else
				{
					otherEntity.TakeDamage (myDamagePackage);
				}

			}
		}
	}
}
