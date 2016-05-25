using UnityEngine;
using System.Collections;

public class WeaponClass : MonoBehaviour {

	[SerializeField] bool debugMode = true;
	[SerializeField] Collider[] hitBoxes;

	OmniAttackInfo myOmniPackage;
	bool contactWeaponized = false; //If this is true the weapon will attempt to inflict any effects its Omnipackage carries (damage, cc, etc.)

	protected virtual void Awake() {}
	protected virtual void Start() {}

	//Causes this weapon to apply its damage and affects to enemy targets on contact.
	protected void WeaponizeContact(OmniAttackInfo omniPackage) //
	{
		myOmniPackage = omniPackage;
		contactWeaponized = true;
	}

	//Causes this weapon to pacify and inflict no effects on contact.
	protected void PacifyContact()
	{
		contactWeaponized = false;
	}

	//Weaponizes this weapon for one frame
	protected void OneShotWeaponize(OmniAttackInfo omniPackage) 
	{
		StartCoroutine (execOneShotWeaponize (omniPackage));
	}

	IEnumerator execOneShotWeaponize(OmniAttackInfo omniPackage)
	{
		WeaponizeContact (omniPackage);
		yield return null;
		PacifyContact ();
	}

	//Causes this weapon to weaponize contact, then pacify contact, multiple times for one frame each over a period time.
	protected void TickWeaponizedContact(float totalTime, int numberOfTicks, OmniAttackInfo omniPackage)
	{
		myOmniPackage = omniPackage;
		StartCoroutine (execTickContactDeadliness(totalTime, numberOfTicks));
	}

	IEnumerator execTickContactDeadliness(float execTotalTime, int execNumberOfTicks)
	{
		for (int i = 0; i < execNumberOfTicks; i++) {
			WeaponizeContact (myOmniPackage);
			yield return null;
			PacifyContact ();
			yield return new WaitForSeconds (execTotalTime / execNumberOfTicks);
		}
	}

	void OnTriggerStay(Collider otherColl)
	{
		Debug.Log (otherColl.gameObject.name);
		if (otherColl.GetComponent (typeof(EntityClass))) 
		{
			EntityClass otherEntity = otherColl.GetComponent (typeof(EntityClass)) as EntityClass;
			if (contactWeaponized)
			{
				if (myOmniPackage.damage > 0)
				{
					float entityHealthRemaining = otherEntity.TakeDamage (myOmniPackage);

					if (debugMode)
					{
						Debug.Log (otherColl.name + " has taken " + myOmniPackage.damage + " damage from " + myOmniPackage.attacker + ". Health is now: " + entityHealthRemaining);
					}
				}

				if (myOmniPackage.stunDuration > 0)
				{

				}

				if (myOmniPackage.launchVector != Vector3.zero)
				{
					Vector3 launchVectorSent = otherEntity.Launch (myOmniPackage.launchVector);

					if (debugMode)
					{
						Debug.Log (otherColl.name + " has been launched " + launchVectorSent + "! By: " + myOmniPackage.attacker);
					}
				}
			}
		}
	}
}
