using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Collider))]
public class MeleeWeaponClass : MonoBehaviour {

	public bool debugMode = true;
	protected Collider myColl;

	protected bool contactDeadly = false; //If this is true, the weapon will deal damage on contact.
	protected DamageInfo myDamagePackage;

	protected bool contactLaunchy = false; //If this is true, the weapon will launch entities that come into contact.
	protected Vector3 myLaunchVector = Vector3.zero;

	protected virtual void Awake()
	{
		myColl = GetComponent<Collider> ();
	}

	protected virtual void FixedUpdate () 
	{
		//Turn the collider on and off when needed to increase performance.
		if ((contactDeadly || contactLaunchy) && !myColl.enabled)
		{
			myColl.enabled = true;
		}
		else if (!contactDeadly && !contactLaunchy && myColl.enabled)
		{
			myColl.enabled = false;
		}
	}
		
	//Set the weapon's ability to deal damage. Returns the end state.
	public bool ToggleDeadliness(bool state, DamageInfo damagePackage) 
	{ 
		myDamagePackage = damagePackage;
		contactDeadly = state;
		return contactDeadly;
	}

	//DOES NOT CLEAN THE WEAPON, BEWARE PHANTOM DATA
	public bool ToggleDeadliness(bool state) 
	{ 
		contactDeadly = state;
		return contactDeadly;
	}

	//Set the weapon's ability to launch enemies, returns the end state.
	public bool ToggleLaunchiness (bool state, Vector3 launchVector)
	{
		contactLaunchy = state;
		myLaunchVector = launchVector;
		return contactLaunchy;
	}

	//DOES NOT CLEAN THE WEAPON, BEWARE PHANTOM DATA
	public bool ToggleLaunchiness (bool state)
	{
		contactLaunchy = state;
		return contactLaunchy;
	}

	//This fires ticks of deadliness or "ticks of damage" over a period of time. Splits the damage requested over the number of ticks requested.
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
		if (otherColl.GetComponent (typeof(EntityClass))) {
			EntityClass otherEntity = otherColl.GetComponent (typeof(EntityClass)) as EntityClass;

			if (contactDeadly)
			{
				float entityHealthRemaining = otherEntity.TakeDamage (myDamagePackage);

				if (debugMode)
				{
					Debug.Log (otherColl.name + " has taken " + myDamagePackage.damage + " damage from " + myDamagePackage.attacker + ". Health is now: " + entityHealthRemaining);
				}
			}

			if (contactLaunchy)
			{
				Vector3 launchVectorSent = otherEntity.Launch (myLaunchVector);

				if (debugMode)
				{
					Debug.Log (otherColl.name + " has been launched " + launchVectorSent + "! By: " + myDamagePackage.attacker);
				}
			}
		}
	}
}
