using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerClass : EntityClass {
	[SerializeField] bool disableInput = false;
	[SerializeField] bool debugMode = true;
	[SerializeField] float maxHealth = 100;
	[SerializeField] float acceleration = 10;
	[SerializeField] float maxHorzSpeed = 10;
	[SerializeField] float jumpDetectionLine = 1;

	float maxJumps = 2; //Maximum number of jumps, two for double jump, 0 and you can't jump at all.
	float hAxis;
	float vAxis;

	Rigidbody rB;
	List<ContactPoint[]> groundContacts = new List<ContactPoint[]> ();

	// Use this for initialization
	void Awake () {
		health = maxHealth;
		rB = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Player Input
		hAxis = Input.GetAxis ("Horizontal");
		vAxis = Input.GetAxis ("Vertical");

		if (debugMode) {
			Debug.DrawRay (transform.position - Vector3.up * jumpDetectionLine, transform.right * 0.5f, Color.yellow);
			Debug.DrawRay (transform.position - Vector3.up * jumpDetectionLine, -transform.right * 0.5f, Color.yellow);
		}
	}

	void FixedUpdate(){

		if (rB.velocity.x < maxHorzSpeed && rB.velocity.x > -maxHorzSpeed) {
			rB.AddForce (Vector2.right * hAxis * acceleration * Time.fixedDeltaTime * 1000);
		}
	}

	void OnCollisionStay(Collision collInfo){
		
		for (int i = 0; i < collInfo.contacts.Length; i++) {
			if (collInfo.contacts [i].point.y < transform.position.y - jumpDetectionLine) {
				//groundContacts.Add (collInfo.contacts [i].point);
			}
		}

		//for (int i = 0; i < groundContacts.Count; i++) {
			//contactPoint = groundContacts [i];
			//Workin ere
		//}

	}
}
