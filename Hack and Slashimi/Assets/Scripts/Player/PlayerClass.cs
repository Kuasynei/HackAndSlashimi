using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class PlayerClass : EntityClass 
{

	[Header("Main")]
	[SerializeField] bool debugMode = true;
	[SerializeField] bool disableGameInput = false;
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
	[SerializeField] float jumpMercy; //To account for bumps in the ground, if you lose contact with the ground you will still count as onGround for a short time (unless you jumped).

	[Header("Combat")]
	[SerializeField] float maxHealth = 100;
	[SerializeField] faction setFaction;
	[SerializeField] MeleeWeaponClass myBlade;

	////Private Vars
	//Player Inputs
	float hAxis;
	float vAxis;
	//float fire1Axis;
	bool fire1Down;
	bool fire2Down;

	//Horizontal Movement
	bool hMoveEnabled = true;

	//Jumping
	float jumpGuideline = 0.2f; //This is so that you don't waste your double jumps. 
	float jumpCooldown = 0; //You can only jump once per half second.
	float jumpsAvailable;
	float eGFactor; //ENHANCED GRAVITY FACTOR TM
	float jumpMercyTimer = 0;
	bool onGround;
	bool jumpEnabled = true;
	List<ContactPoint> groundContacts = new List<ContactPoint> ();

	//Combat
	float weaponLock = 0; //Prevents the player from launching multiple attack commands (or other commands) before the attack finishes.
	bool respawning = false;

	//Rotation
	int facing = 1; //-1 Facing Left / 1 Facing Right
	Vector3 lookOrb = new Vector3(0,0,0); //Orbits the player clockwise when they turn to make all rotations clockwise.

	//Other
	Collider coll;
	Rigidbody rB;

	protected override void Awake () 
	{
		maxH = maxHealth;

		base.Awake ();

		rB = GetComponent<Rigidbody> ();
		jumpsAvailable = maxJumps;
		eGFactor = enhancedGravityFactorTM;
		coll = GetComponent<Collider> ();
		myFaction = setFaction; 

		GameManager.SetPlayer (this.gameObject);
	}

	void Update () 
	{
        //The death check is updated with checking if the colossus is alive or not. If it is dead then the game is over.
		if (health <= 0 && !respawning) 
		{
			if(GameManager.GetPColossus().GetComponent<Colossus>().getIsDead())
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

		////Player Input
		if (!disableGameInput) {
			hAxis = Input.GetAxis ("Horizontal");
			vAxis = Input.GetAxis ("Vertical");
			//fire1Axis = Input.GetAxis ("Fire1");
			fire1Down = Input.GetMouseButtonDown (0);
			fire2Down = Input.GetMouseButtonDown (1);
		} else {
			hAxis = 0;
			vAxis = 0;
			//fire1Axis = 0;
			fire1Down = false;
			fire2Down = false;
		}

		if (debugMode) {
			//Any collisions the player makes below this line will count as "contact with ground".
			if (onGround || jumpMercyTimer > 0) {
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, Vector3.right * 0.5f, Color.green);
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, -Vector3.right * 0.5f, Color.green);
			} /*else if (!onGround) {
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, Vector3.right * 0.5f, Color.yellow);
				Debug.DrawRay (transform.position + Vector3.up * jumpDetectionLine, -Vector3.right * 0.5f, Color.yellow);
			}*/

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
			
		////Attack Commands
		weaponLock -= Time.deltaTime;

		if (myBlade.GetType() == typeof(Sword)) // Weapon Type: Sword
		{
			if (fire1Down) //Left Mouse Button
			{ 
				//Basic Attack, can only be used on the ground.
				if ((onGround || jumpMercyTimer > 0)  && weaponLock <= 0)
				{
					float basicAttackDamage = 5;
					int ticksOfDamage = 2;
					float dashSpeed = 15;

					//Brake for a short time.
					StartCoroutine (ManualBrake(0, 0.1f, 600));

					//Dash in the direction you are facing over the span of 0.4 seconds. Gravity applies.
					StartCoroutine (Dash (0.1f, 0.1f, Vector3.right * facing * dashSpeed, false)); 

					StartCoroutine (ManualBrake(0.2f, 0.1f, 600));

					DamageInfo basicAttackPackage = new DamageInfo (basicAttackDamage, this.gameObject, myFaction);
					(myBlade as Sword).BasicAttack (0.1f, 0.1f, ticksOfDamage, basicAttackPackage);

					weaponLock = 0.2f;
				}
			}
			else if (fire2Down) //Right Mouse Button
			{
				//Launching Attack, can only be used on the ground.
				if ((onGround || jumpMercyTimer > 0)  && weaponLock <= 0)
				{
					float launchingAttackDamage = 10;
					float dashSpeed = 15;
					Vector3 launchVector;

					launchVector = Vector3.up * 7;

					//Brake for a short time.
					StartCoroutine (ManualBrake(0, 0.3f, 300));

					//Dash in the direction you are facing over the span of 0.4 seconds. Gravity applies.
					StartCoroutine (Dash (0.3f, 0.4f, Vector3.right * facing * dashSpeed, false)); 

					StartCoroutine (ManualBrake(0.4f, 0.5f, 1000));

					DamageInfo launchingAttackPackage = new DamageInfo (launchingAttackDamage, this.gameObject, myFaction);
					(myBlade as Sword).LaunchingAttack(0.4f, launchVector, launchingAttackPackage);

					weaponLock = 0.6f;
				}
			}
		}

		////UI
		uI_HP.text = Mathf.Round(health).ToString();
	}

	void FixedUpdate()
	{
		//// Horizontal Movement Settings
		if (hMoveEnabled)
		{ 
			Vector2 addHorizontalForce = Vector2.zero;

			if (hAxis != 0 && onGround) //...when on the ground
			{
				addHorizontalForce = Vector2.right * hAxis * acceleration * Time.fixedDeltaTime * 100;
			} 
			else if (hAxis != 0 && !onGround) //...when in the air
			{
				addHorizontalForce = Vector2.right * hAxis * acceleration * Time.fixedDeltaTime * 100 / airAccelDampener;
			} 
			else if (onGround) ///...when there is no horizontal input AND on the ground. (Allows flying through the air majestically and avoids ground sliding.)
			{
				addHorizontalForce = Vector2.right * -rB.velocity.x * Time.fixedDeltaTime * 1000 / slidingFactor;
			}

			//Horizontal Speed Limit, designed to be less restrictive on air manuevers.
			//This only limits horizontal base speed, speed gained from jumping off of angled surfaces is not included.
			if (rB.velocity.x > maxHorzSpeed)
			{
				addHorizontalForce = new Vector2(Mathf.Clamp(addHorizontalForce.x, -Mathf.Infinity, 0), addHorizontalForce.y);
			}
			else if (rB.velocity.x < -maxHorzSpeed)
			{
				addHorizontalForce = new Vector2(Mathf.Clamp(addHorizontalForce.x, 0, Mathf.Infinity), addHorizontalForce.y);
			}

			rB.AddForce (addHorizontalForce);
		}

		////Jumping
		if (jumpCooldown > 0) jumpCooldown -= Time.fixedDeltaTime; //Decreasing the cooldown on the player's jump.
		if (jumpMercyTimer > 0) jumpMercyTimer -= Time.fixedDeltaTime; 

		//Aerial jumps (every jump after the first one)
		if (jumpEnabled && !fire1Down) //Attacking overrides jump commands.
		{
			if (!onGround && jumpCooldown <= 0 && jumpsAvailable > 0 && vAxis > 0)
			{
				rB.velocity = new Vector3 (rB.velocity.x, 0, rB.velocity.z); //Resetting the velocity so that the jump feels impactful no matter what.
				rB.AddForce (Vector3.up * jumpPower * 100);
				jumpCooldown = jumpGuideline;
				jumpsAvailable -= 1;
				jumpMercyTimer = 0;
			}
		}

		////Setting player facing direction.
		if (weaponLock <= 0)
		{
			if (hAxis > 0)
			{
				facing = 1;
			}
			else if (hAxis < 0)
			{
				facing = -1;
			}
		}

		//Orb lerps around the player to determine where they should be looking. THEN HAVE THE PLAYER LOOK AT IT.
		if (facing == 1)
		{
			lookOrb = Vector3.Lerp (new Vector3 (lookOrb.x, 0, 0), new Vector3 (1, 0, 0), rotationSpeed * Time.fixedDeltaTime);
			lookOrb += new Vector3 (0, 0, 1f - Mathf.Abs (lookOrb.x));
		}
		else if (facing == -1)
		{
			lookOrb = Vector3.Lerp (new Vector3 (lookOrb.x, 0, 0), new Vector3 (-1, 0, 0), rotationSpeed * Time.fixedDeltaTime);
			lookOrb += new Vector3 (0, 0, -1f + Mathf.Abs (lookOrb.x));
		}
		//LOOK AT THE LOOK ORB DAMN IT.
		transform.LookAt (lookOrb + transform.position);
		

		////groundContacts determine status of ground contact.
		if (groundContacts.Count > 0) 
		{
			onGround = true;
			jumpMercyTimer = jumpMercy;
			jumpsAvailable = maxJumps - 1;
		} 
		else 
		{
			onGround = false;
		}

		groundContacts.Clear(); //Emptying out ground contacts so they don't stack between frames.
		rB.AddForce (Vector3.down * eGFactor); //Enhanced Gravity FactorTM for your platforming enjoyment.
	}

	void OnCollisionStay(Collision collInfo) {
		////Label all collisions made under the "jumpDetectionLine" as ground contacts.
		for (int i = 0; i < collInfo.contacts.Length; i++) {
			if (collInfo.contacts [i].point.y < transform.position.y + jumpDetectionLine) {
				groundContacts.Add (collInfo.contacts [i]);
			}
		}

		////Ground contact enables jumping. First jump code is done here.
		if (jumpEnabled && !fire1Down) //Attacking overrides jump commands.
		{
			if ((onGround || jumpMercyTimer > 0) && vAxis > 0 && jumpCooldown <= 0)
			{
				Vector3 jumpVector = Vector3.zero;

				for (int i = 0; i < groundContacts.Count; i++)
				{
					ContactPoint contactPoint = groundContacts [i];
					jumpVector = (jumpVector + contactPoint.normal).normalized;
				}

				//Bouncy House Factor. The steeper an angle is, the stronger the jump will be to compensate.
				float bHF = Mathf.Abs (jumpVector.x) * bouncyHouseFactor + 1;

				rB.AddForce (jumpVector * bHF * jumpPower * 100);
				jumpCooldown = jumpGuideline;
				jumpsAvailable -= 1;
				jumpMercyTimer = 0;
			}
		}
	}

	//Other movement options are disabled when dashing. Does not account for dampening and friction.
	IEnumerator Dash(float delay, float duration, Vector3 dashVector, bool defyGravity)
	{
		yield return new WaitForSeconds (delay);

		hMoveEnabled = false;
		jumpEnabled = false;
		if (defyGravity) rB.useGravity = false;

		rB.velocity += dashVector;

		yield return new WaitForSeconds (duration);

		hMoveEnabled = true;
		jumpEnabled = true;
		if (defyGravity) rB.useGravity = true;
	}

	//Brake strength is capped at 1000, anymore and you'd be going backwards. You must be on the ground to brake.
	IEnumerator ManualBrake(float delay, float duration, float strength)
	{
		if (onGround || jumpMercyTimer > 0)
		{
			yield return new WaitForSeconds (delay);

			hMoveEnabled = false;
			jumpEnabled = false;

			strength = Mathf.Clamp (strength, 0, 1000);

			for (float t = 0; t < duration;)
			{
				t += Time.fixedDeltaTime;	
				rB.AddForce (Vector2.right * -rB.velocity.x * Time.fixedDeltaTime * strength);
				yield return new WaitForFixedUpdate ();
			}

			hMoveEnabled = true;
			jumpEnabled = true;
		}

		//yield return null;
	}

	IEnumerator Die() {
		//Disabling and restarting some variables.
		float timeToRecharge = 5;
		float deathLerpSpeed = 0.05f;
		disableGameInput = true;
		rB.useGravity = false;
		rB.velocity = new Vector3 (0, 0, 0);
		jumpsAvailable = maxJumps - 1;
		eGFactor = 0;
		coll.enabled = false;

		yield return new WaitForSeconds (1f);

		//Returning to spawn point.
		while (Vector3.Distance (transform.position, new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0)) > 2f) {
			transform.position = Vector3.Lerp (transform.position, spawnPoint.position, deathLerpSpeed);
			yield return new WaitForFixedUpdate ();
		}

		//Recharging health.
		for (; health < maxHealth;) {
			transform.position = Vector3.Lerp (transform.position, new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0), deathLerpSpeed);
			health += (Time.fixedDeltaTime * 10000 / timeToRecharge) / maxHealth;

			if (health > maxHealth) {
				health = maxHealth;
			}
			yield return new WaitForFixedUpdate ();
		}


		Vector3.Lerp (transform.position, new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0), deathLerpSpeed);
		yield return new WaitForSeconds (0.5f);

		//Reenabling some features.
		disableGameInput = false;
		rB.useGravity = true;
		eGFactor = enhancedGravityFactorTM;
		coll.enabled = true;

		respawning = false;
	}
}
