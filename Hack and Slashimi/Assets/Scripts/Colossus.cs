using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Colossus : EntityClass 
{
	public float movementSpeed;
	[SerializeField] float maxHealth = 100;
	[SerializeField] float cleft = 1000;
	bool gateToDestroy;
	Rigidbody rB;
	float timeSinceLastHit = 0.0f;
	Gate gateScript;

	void Awake () 
	{
		health = maxHealth;
		rB = GetComponent<Rigidbody> ();
	}

	void Update()
	{
		timeSinceLastHit += Time.deltaTime;
		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.right, out hit, 1.5f))
		{
			if(hit.collider.tag == "Gate")
			{

				gateToDestroy = true;
				rB.velocity = new Vector3(0,0,0);
				if(timeSinceLastHit > 5)
				{
					gateScript = (Gate)hit.collider.GetComponent("Gate");
					gateScript.takeDamage(cleft);
					timeSinceLastHit = 0;
				}

				Debug.DrawLine(transform.position, hit.point);
			}
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
