using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class ProjectileClass : MonoBehaviour {

	bool collided = false;
	float contactDamage;
	Rigidbody rB;
	Vector3 originalPosition;
	Quaternion originalRotation;

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

		/*
		if (collided == true)
		{
			//We do this because isKinematic doesn't like unique positions. It resets transform.
			transform.position = originalPosition;
			transform.rotation = originalRotation;
		}*/
	}

	void OnCollisionEnter(Collision collInfo)
	{
		if (collInfo.transform.CompareTag("Colossus"))
		{

			originalPosition = transform.position;
			originalRotation = transform.rotation;

			rB.isKinematic = true;
			rB.detectCollisions = false;

			collided = true;

			transform.SetParent (collInfo.transform);
		}
	}
}
