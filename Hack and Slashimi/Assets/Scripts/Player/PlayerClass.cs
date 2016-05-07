using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PlayerClass : EntityClass {

	[SerializeField] bool debugMode = true;
	[SerializeField] bool disableInput = false;
	[SerializeField] Transform spawnPoint;
	[SerializeField] Text uI_HP;
	[SerializeField] float maxHealth = 20;
	[SerializeField] float rotationSpeed = 1;

	bool respawning = false;
	int facing = 1; //-1 Facing Left / 1 Facing Right
	Vector3 lookOrb = new Vector3(0,0,0); //Orbits the player clockwise when they turn to make all rotations clockwise.
	Collider coll;

	//Horizontal Movement
	[SerializeField] float acceleration = 5;
	[SerializeField] float maxHorzSpeed = 10;
	[SerializeField] float slidingFactor = 3;

	//Jumping
	[SerializeField] float jumpPower = 2;
	[SerializeField] float jumpDetectionLine = 0.6f; //If a surface is this far under the player's origin
	[SerializeField] float airAccelDampener = 2;
	[SerializeField] float maxJumps = 2; //Maximum number of jumps, two for double jump, 0 and you can't jump at all.
	[SerializeField] float enhancedGravityFactorTM = 2;
	[SerializeField] float bouncyHouseFactor = 2; //Increases jump power against weird angles that aren't straight up, to make them feel better to jump against.
	float jumpGuideline = 0.2f; //This is so that you don't waste your double jumps. 
	float jumpCooldown = 0; //You can only jump once per half second.
	float jumpsAvailable;
	float eGFactor; //ENHANCED GRAVITY FACTOR TM
	bool onGround;

	//Player Inputs
	float hAxis;
	float vAxis;
	float fire1Axis;

	//Weapons
	[SerializeField] WeaponClass myBlade;
	float weaponLock = 0; //Prevents the player from launching multiple attack commands before the previous finishes.

	Rigidbody rB;

	List<ContactPoint> groundContacts = new List<ContactPoint> ();

	// Use this for initialization
	void Awake () {
		health = maxHealth;
		rB = GetComponent<Rigidbody> ();
		jumpsAvailable = maxJumps;
		eGFactor = enhancedGravityFactorTM;
		coll = GetComponent<Collider> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (health <= 0 && !respawning) {
			health = 0;
			respawning = true;
			StartCoroutine ("Die");
		}

		//Player Input
		if (!disableInput) {
			hAxis = Input.GetAxis ("Horizontal");
			vAxis = Input.GetAxis ("Vertical");
			fire1Axis = Input.GetAxis ("Fire1");
		} else {
			hAxis = 0;
			vAxis = 0;
			fire1Axis = 0;
		}

		if (debugMode) {
			//Any collisions the player makes below this line will count as "contact with ground".
			if (onGround) {
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, Vector3.right * 0.5f, Color.green);
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, -Vector3.right * 0.5f, Color.green);
			} else if (!onGround) {
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, Vector3.right * 0.5f, Color.yellow);
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, -Vector3.right * 0.5f, Color.yellow);
			}

			//Drawing collision angles.
			for (int i = 0; i < groundContacts.Count; i++) {
				ContactPoint contactPoint = groundContacts [i];
				Debug.DrawRay (contactPoint.point, contactPoint.normal, Color.green);
			}

			//Drawing the look orb (LOOK AT IT)
			Debug.DrawRay(lookOrb + transform.position, Vector3.up * 0.5f, Color.blue, Time.deltaTime, true);

			//HURT YOURSELF BUTTON
			if (Input.GetKeyDown (KeyCode.H)) {
				TakeDamage (100);
				Debug.Log ("LIFE did 100 damage to " + name + "! " + health + " health remaining.");
			}
		}
			
		//Attack Commands
		weaponLock -= Time.deltaTime;
		if (fire1Axis != 0 && weaponLock <= 0) {
			if (myBlade.GetType () == typeof(Sword)){
				(myBlade as Sword).BasicAttack (0.4f, 2);
				weaponLock = 0.5f;
			}
		}

		//UI
		uI_HP.text = health.ToString();
	}

	void FixedUpdate(){
		//Horizontal Movement Settings
		if (hAxis != 0 && onGround) {
			//...when on ground
			rB.AddForce (Vector2.right * hAxis * acceleration * Time.fixedDeltaTime * 100);
		} else if (hAxis != 0 && !onGround) {
			//...when in the air
			rB.AddForce (Vector2.right * hAxis * acceleration * Time.fixedDeltaTime * 100 / airAccelDampener);
		} else if (onGround){
			///...when there is no horizontal input AND on the ground. (Allows flying through the air majestically and avoids ground sliding.)
			rB.AddForce (Vector2.right * -rB.velocity.x * Time.fixedDeltaTime * 1000 / slidingFactor);
		}

		//Horizontal Speed Limit
		rB.velocity = new Vector3 (Mathf.Clamp(rB.velocity.x, -maxHorzSpeed, maxHorzSpeed), rB.velocity.y, rB.velocity.z);

		if (jumpCooldown > 0) jumpCooldown -= Time.fixedDeltaTime; //Decreasing the cooldown on the player's jump.

		//Aerial jumps (every jump after the first one)
		if (!onGround && jumpCooldown <= 0 && jumpsAvailable > 0 && vAxis > 0) {
			rB.velocity = new Vector3 (rB.velocity.x, 0, rB.velocity.z); //Resetting the velocity so that the jump feels impactful no matter what.
			rB.AddForce (Vector3.up * jumpPower * 100);
			jumpCooldown = jumpGuideline;
			jumpsAvailable -= 1;
		}

		//Setting player facing direction.
		if (hAxis > 0) {
			facing = 1;
		} else if (hAxis < 0){
			facing = -1;
		}

		//Orb lerps around the player to determine where they should be looking. THEN HAVE THE PLAYER LOOK AT IT.
		if (facing == 1) {
			lookOrb = Vector3.Lerp (new Vector3 (lookOrb.x, 0, 0), new Vector3 (1, 0, 0), rotationSpeed * Time.fixedDeltaTime);
			lookOrb += new Vector3 (0, 0, 1f - Mathf.Abs(lookOrb.x));
		} else if (facing == -1) {
			lookOrb = Vector3.Lerp (new Vector3 (lookOrb.x, 0, 0), new Vector3(-1, 0, 0), rotationSpeed * Time.fixedDeltaTime);
			lookOrb += new Vector3 (0, 0, -1f + Mathf.Abs(lookOrb.x));
		}
		//LOOK AT THE LOOK ORB DAMN IT.
		transform.LookAt (lookOrb + transform.position);

		//groundContacts determine status of ground contact.
		if (groundContacts.Count > 0) {
			onGround = true;
			jumpsAvailable = maxJumps - 1;
		} else {
			onGround = false;
		}

		groundContacts.Clear(); //Emptying out ground contacts so they don't stack between frames.
		rB.AddForce (Vector3.down * eGFactor); //Enhanced Gravity FactorTM for your platforming enjoyment.
	}

	void OnCollisionStay(Collision collInfo){

		//Label all collisions made under the "jumpDetectionLine" as ground contacts.
		for (int i = 0; i < collInfo.contacts.Length; i++) {
			if (collInfo.contacts [i].point.y < transform.position.y + jumpDetectionLine) {
				groundContacts.Add (collInfo.contacts [i]);
			}
		}

		//Ground contact enables jumping. First jump code is done here.
		if (onGround && vAxis > 0 && jumpCooldown <= 0) {
			Vector3 jumpVector = Vector3.zero;

			for (int i = 0; i < groundContacts.Count; i++) {
				ContactPoint contactPoint = groundContacts [i];
				jumpVector = (jumpVector + contactPoint.normal).normalized;
			}

			//Bouncy House Factor. The steeper an angle is, the stronger the jump will be to compensate.
			float bHF = Mathf.Abs(jumpVector.x) * bouncyHouseFactor + 1;

			rB.AddForce (jumpVector * bHF * jumpPower * 100);
			jumpCooldown = jumpGuideline;
			jumpsAvailable -= 1;
		}
	}

	IEnumerator Die() {
		//Disabling and restarting some variables.
		float timeToRecharge = 3;
		disableInput = true;
		rB.useGravity = false;
		rB.velocity = new Vector3 (0, 0, 0);
		jumpsAvailable = maxJumps - 1;
		eGFactor = 0;
		coll.enabled = false;

		yield return new WaitForSeconds (1f);

		//Returning to spawn point.
		while (Vector3.Distance (transform.position, spawnPoint.position) > 0.1f) {
			transform.position = Vector3.Lerp (transform.position, spawnPoint.position, 0.05f);
			yield return new WaitForFixedUpdate ();
		}

		//Recharging health.
		yield return new WaitForSeconds (0.5f);
		for (; health < maxHealth; health++) {
			yield return new WaitForSeconds (timeToRecharge / maxHealth);
		}

		yield return new WaitForSeconds (0.5f);

		//Reenabling some features.
		disableInput = false;
		rB.useGravity = true;
		eGFactor = enhancedGravityFactorTM;
		coll.enabled = true;

		respawning = false;
	}
}
