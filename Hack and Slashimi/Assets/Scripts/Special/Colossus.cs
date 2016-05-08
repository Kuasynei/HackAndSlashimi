using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Colossus : EntityClass 
{
	[SerializeField] bool debugMode = true;
	[SerializeField] float movementSpeed;
	[SerializeField] float maxHealth = 100;
	[SerializeField] float damage = 1000;
	[SerializeField] float gateDetectionLength = 5;
	bool gateToDestroy;
	Rigidbody rB;
	float timeSinceLastHit = 0.0f;
	Gate gateScript;

	void Awake () 
	{
		health = maxHealth;
		rB = GetComponent<Rigidbody> ();
	}

	void FixedUpdate()
	{
		timeSinceLastHit += Time.fixedDeltaTime;
		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.right, out hit, gateDetectionLength))
		{
			if(hit.collider.CompareTag("Gate"))
			{
				gateToDestroy = true;
				rB.velocity = new Vector3(0,0,0);
				if(timeSinceLastHit > 5)
				{
					gateScript = (Gate)hit.collider.GetComponent("Gate");
					float gateHealth = gateScript.TakeDamage(damage);
					timeSinceLastHit = 0;
					Debug.DrawLine (transform.position, hit.point, Color.red, .3f);

					if (debugMode) 
					{
						Debug.Log ("Colossus dealt " + damage + " to a Gate!\n" + "The gate has " + gateHealth + " health remaining.");
					}
				}
				Debug.DrawLine(transform.position, hit.point, Color.green);
			}
		}
		else
		{
			gateToDestroy = false;
		}

		if(!gateToDestroy)
		{
			rB.velocity = new Vector3(movementSpeed / 10, rB.velocity.y, rB.velocity.z);
		}
	}
}
