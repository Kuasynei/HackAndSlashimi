using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class BallistaBolt : MonoBehaviour {

	[SerializeField] float impaleTime;
	Rigidbody rB;
	DamageInfo myDamagePackage;

	void Awake() 
	{
		rB = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (rB.velocity != Vector3.zero)
		{
			transform.rotation = Quaternion.LookRotation (rB.velocity);
		}
	}

	void OnTriggerEnter(Collider otherColl)
	{
		//Impale ALL the things.
		StartCoroutine (Impale(otherColl));

		EntityClass otherEntity = otherColl.GetComponent<EntityClass> ();
		if (otherEntity)
		{
			otherEntity.TakeDamage (myDamagePackage);
		}
	}

	IEnumerator Impale(Collider otherColl)
	{
		yield return new WaitForSeconds (impaleTime);
		rB.isKinematic = true;
		rB.detectCollisions = false;

		transform.SetParent (otherColl.transform);
	}

	public void giveDamagePackage(DamageInfo damagePackage)
	{
		myDamagePackage = damagePackage;
	}
}
