using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PlayerClass : EntityClass {

	[Header("Main")]
	[SerializeField] bool debugMode = true;
	[SerializeField] bool disableInput = false;
	[SerializeField] Transform spawnPoint;
	[SerializeField] Text uI_HP;

	[Header("Horizontal Movement")]
	[SerializeField] float acceleration = 5;
	[SerializeField] float maxHorzSpeed = 10;
	[SerializeField] float slidingFactor = 3;

	[Header("Rotation")]
	[SerializeField] float rotationSpeed = 1;

	[Header("Aerial Stats")]
	[SerializeField] float jumpPower = 2;
	[SerializeField] float jumpDetectionLine = 0.6f; //If a surface is this far under the player's origin
	[SerializeField] float airAccelDampener = 2;
	[SerializeField] float maxJumps = 2; //Maximum number of jumps, two for double jump, 0 and you can't jump at all.
	[SerializeField] float enhancedGravityFactorTM = 2;
	[SerializeField] float bouncyHouseFactor = 2; //Increases jump power against weird angles that aren't straight up, to make them feel better to jump against.

	[Header("Combat")]
	[SerializeField] float maxHealth = 100;
	[SerializeField] float basicAttackDamage = 10;
	[SerializeField] faction setFaction;
	[SerializeField] MeleeWeaponClass myBlade;

	////Private Vars
	//Player Inputs
	float hAxis;
	float vAxis;
	float fire1Axis;

	//Jumping
	float jumpGuideline = 0.2f; //This is so that you don't waste your double jumps. 
	float jumpCooldown = 0; //You can only jump once per half second.
	float jumpsAvailable;
	float eGFactor; //ENHANCED GRAVITY FACTOR TM
	bool onGround;

	//Combat
	bool respawning = false;
	float weaponLock = 0; //Prevents the player from launching multiple attack commands before the previous finishes.

	//Rotation
	int facing = 1; //-1 Facing Left / 1 Facing Right
	Vector3 lookOrb = new Vector3(0,0,0); //Orbits the player clockwise when they turn to make all rotations clockwise.

	//Other
	Collider coll;
	Rigidbody rB;
	List<ContactPoint> groundContacts = new List<ContactPoint> ();

    //Storing the Colossus for use later
    [SerializeField] GameObject Colossus;

	// Use this for initialization
	void Awake () {
		maxH = maxHealth;
		rB = GetComponent<Rigidbody> ();
		jumpsAvailable = maxJumps;
		eGFactor = enhancedGravityFactorTM;
		coll = GetComponent<Collider> ();
		myFaction = setFaction; 
	}
	
	// Update is called once per frame
	void Update () {
        //The death check is updated with checking if the colossus is alive or not. If it is dead then the game is over.
		if (health <= 0 && !respawning) {
            if(Colossus.GetComponent<Colossus>().getIsDead())
            {
                //Game Over
            }
            else
            {
                health = 0;
                respawning = true;
                StartCoroutine("Die");
            }
			
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
				DamageInfo suicidePackage = new DamageInfo (health, this.gameObject, faction.neutral);
				TakeDamage (suicidePackage);
				Debug.Log ("LIFE did " + health + " damage to " + name + "! " + health + " health remaining.");
			}
			if (Input.GetKeyDown (KeyCode.J)) {
				DamageInfo suicidePackage = new DamageInfo (50, this.gameObject, faction.neutral);
				TakeDamage (suicidePackage);
				Debug.Log ("LIFE did " + 50 + " damage to " + name + "! " + health + " health remaining.");
			}
		}
			
		//Attack Commands
		weaponLock -= Time.deltaTime;
		if (fire1Axis != 0 && weaponLock <= 0) {
			if (myBlade.GetType () == typeof(Sword)){
				DamageInfo basicAttackPackage = new DamageInfo (basicAttackDamage, this.gameObject, faction.goodGuys);
				(myBlade as Sword).BasicAttack (0.4f, 2, basicAttackPackage);
				weaponLock = 0.5f;
			}
		}

		//UI
		uI_HP.text = Mathf.Round(health).ToString();
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

	void OnCollisionStay(Collision collInfo) {

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
		float timeToRecharge = 5;
		float deathLerpSpeed = 0.05f;
		disableInput = true;
		rB.useGravity = false;
		rB.velocity = new Vector3 (0, 0, 0);
		jumpsAvailable = maxJumps - 1;
		eGFactor = 0;
		coll.enabled = false;

		yield return new WaitForSeconds (1f);

		//Returning to spawn point.
		while (Vector3.Distance (transform.position, spawnPoint.position) > 2f) {
			transform.position = Vector3.Lerp (transform.position, spawnPoint.position, deathLerpSpeed);
			yield return new WaitForFixedUpdate ();
		}

		//Recharging health.
		for (; health < maxHealth;) {
			transform.position = Vector3.Lerp (transform.position, spawnPoint.position, deathLerpSpeed);
			health += (Time.fixedDeltaTime * 10000 / timeToRecharge) / maxHealth;

			if (health > maxHealth) {
				health = maxHealth;
			}
			yield return new WaitForFixedUpdate ();
		}


		Vector3.Lerp (transform.position, spawnPoint.position, deathLerpSpeed);
		yield return new WaitForSeconds (0.5f);

		//Reenabling some features.
		disableInput = false;
		rB.useGravity = true;
		eGFactor = enhancedGravityFactorTM;
		coll.enabled = true;

		respawning = false;
	}
}
