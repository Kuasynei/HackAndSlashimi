using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Colossus : EntityClass 
{
	public float movementSpeed;
	[SerializeField] float maxHealth = 100;
	bool gateToDestroy;
	Rigidbody rB;

	void Awake () 
	{
		health = maxHealth;
		rB = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.right, out hit, 1.5f))
		{
			gateToDestroy = true;
			rB.velocity = new Vector3(0,0,0);
			Debug.DrawLine(transform.position, hit.point);
		}
		else
		{
			gateToDestroy = false;
		}
	}

	void FixedUpdate () 
	{
		if(!gateToDestroy)
		{
			rB.velocity = new Vector3(movementSpeed / 10,0,0);
		}
	}
}
