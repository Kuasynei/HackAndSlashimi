﻿using UnityEngine;
using System.Collections;

public class Footsoldier : EnemyClass {

	[Header("Main")]
	public bool debugMode = true;
	[SerializeField] GameObject healthOrb;
    [SerializeField] WeaponClass enemyWeapon;

	[Header("Movement")]
	[SerializeField] float runSpeed = 4;
	[SerializeField] float marchSpeed = 2;
	[SerializeField] float acceleration = 10;
	[SerializeField] float turnSpeed = 1;

	[Header("Combat Stats")]
	[SerializeField] float maxHealth;
	[SerializeField] float basicAttackDamage = 10;
	[SerializeField] int rngDropPercent = 30;
	[SerializeField] faction setFaction = global::faction.neutral;

    //THIS WILL BE CHANGED TO AXE
	private Footsoldier_BasicAxe enemySword;
	private GameObject player;
	private GameObject playerColossus;
	private Rigidbody rB;

    private float distToPlayer;
    private float distToColossus;
    private float weaponLock = 0;
	private float commandTick; //Allows certain orders to run for cyclically.

	enum MovementMode {Run, March};
	MovementMode myMovementMode;

	protected override void Awake()
	{
		maxH = maxHealth;

		base.Awake ();

		//Store the character controller and the enemy sword/weapon
		enemySword = enemyWeapon as Footsoldier_BasicAxe;

		myFaction = setFaction;
		rB = GetComponent <Rigidbody> ();
	}

    // Use this for initialization
	protected override void Start () 
    {
		base.Start ();

		player = GameManager.GetPlayer ();
		playerColossus = GameManager.GetPColossus();
		commandTick = Random.value; //Staggers commandTicks for all AI so they don't look like robits.
	}
	
	// Update is called once per frame
	protected override void Update ()
    {
		base.Update ();

        if (commandTick <= 0) commandTick = 1f;
		commandTick -= Time.deltaTime;

        if (transform.position.z >= 1)
            rB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;


        //Check distance from the enemy to the player
        distToPlayer = Vector3.Distance(player.transform.position, transform.position);

        //Check distance from the enemy to the player colossus
        distToColossus = Vector3.Distance(playerColossus.transform.position, transform.position);

        // ------------------------------------------------------------ AI BEHAVIOUR START ------------------------------------------------------------ //
        

        //Move back to the forward layer
        if(transform.position.z > 0.5f || transform.position.z < -0.5f)
        {
            Debug.Log("Is Not In Main Lane");

            RaycastHit oHit;
            Debug.DrawRay(transform.position, -transform.right, Color.red);

            if (!Physics.Raycast(transform.position, -transform.right , out oHit, 100.0f))
            {
                if (debugMode) Debug.Log("FUCK");

                //rB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                //rB.AddForce(new Vector3(0, 0, -150));
            }
        }
        //Move to the back layer.
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 5.0f))
            {
                if (hit.collider.tag == "Enemy" && hit.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude <= 0.5f)
                {

                    Vector3 angleToMoveDir = (-transform.right + transform.forward) / 2;

					if (debugMode) Debug.Log("Detected Enemy Infront " + hit.collider.gameObject.name);

                    if (!Physics.Raycast(transform.position, angleToMoveDir, out hit, 7.0f))
                    {
                        rB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                        rB.AddForce(new Vector3(0, 0, 150));

						if (debugMode) Debug.Log("Moving To The Left " + this.name);
                    }
                }
            }
        }

        //This entire area is basically a behavior tree. This prioritizes running towards the colossus then towards the player else it will just
        //run to the left
        //@note: the basic else functionality will have to change because enemies will not only spawn on the right of the map
        if (distToColossus < 15)
        {
			XBrake ();

            if (distToColossus > 7)
            {
                //Move toward the colossus
                Vector3 headingDir = playerColossus.transform.position - transform.position;
				float leftOrRightValue = AngleDir(-Vector3.forward, headingDir, transform.up); //I replaced transform.forward with -vector3.forward
                if (leftOrRightValue != 0)
                {
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(leftOrRightValue * Vector3.right), Time.deltaTime * turnSpeed);
                }

				//charController.SimpleMove(new Vector3(leftOrRightValue * runSpeed, 0, 0));
				XMove(new Vector3 (leftOrRightValue, 0, 0), MovementMode.Run);
            }
			else if ( distToColossus < 7 && commandTick <= 0.5f)
            {    
				//Rotate...
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (new Vector3 (playerColossus.transform.position.x, 0, playerColossus.transform.position.z) 
					- new Vector3 (transform.position.x, 0, transform.position.z)), Time.deltaTime * turnSpeed);
				
				if (weaponLock <= 0)
				{
					//...and smack the colossus
					OmniAttackInfo orcDamagePackage = new OmniAttackInfo(this.gameObject, myFaction, basicAttackDamage, 0, Vector3.zero);
					enemySword.BasicAttack (0.4f, 0.1f, 1, orcDamagePackage);
					weaponLock = 1.0f;

					//Stop and look at the colossus
					//charController.SimpleMove (Vector3.zero);
					XBrake ();
				}
            }
			else if (distToColossus > 6.65)
			{
				//Move toward the colossus
				Vector3 headingDir = playerColossus.transform.position - transform.position;
				float leftOrRightValue = AngleDir(-Vector3.forward, headingDir, transform.up); //I replaced transform.forward with -vector3.forward
				if (leftOrRightValue != 0)
				{
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(leftOrRightValue * Vector3.right), Time.deltaTime * turnSpeed);
				}

				//charController.SimpleMove(new Vector3(leftOrRightValue * runSpeed, 0, 0));
				XMove(new Vector3 (leftOrRightValue, 0, 0), MovementMode.Run);
			}
        }
        else if(distToPlayer < 10)
        {
            if (distToPlayer > 1.5f)
            {
                Vector3 headingDir = player.transform.position - transform.position;
				float leftOrRightValue = AngleDir(-Vector3.forward, headingDir, transform.up);
                if(leftOrRightValue != 0)
                {
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(leftOrRightValue * Vector3.right), Time.deltaTime * turnSpeed);
                }

				//charController.SimpleMove(new Vector3(leftOrRightValue * runSpeed, 0, 0));
				XMove(new Vector3 (leftOrRightValue, 0, 0), MovementMode.Run);
            }
            else if (distToPlayer < 1.5f)
            {
				//charController.SimpleMove(Vector3.zero); 
				XBrake ();
                if(weaponLock <= 0)
                {
					OmniAttackInfo orcDamagePackage = new OmniAttackInfo(this.gameObject, myFaction, basicAttackDamage, 0, Vector3.zero);
					enemySword.BasicAttack (0.4f, 0.1f, 1, orcDamagePackage);
                    weaponLock = 1.2f;
                }
            }
        }
        else
        {
            Vector3 defaultTarget;
            if (!playerColossus.GetComponent<Colossus>().getIsDead())
            {
                defaultTarget = playerColossus.transform.position;
            }
            else
            {
                defaultTarget = player.transform.position;
            }

            Vector3 headingDir = defaultTarget - transform.position;
            headingDir.z = 0;
			float leftOrRightValue = AngleDir(-Vector3.forward, headingDir, transform.up);
            if (leftOrRightValue != 0)
            {
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(leftOrRightValue * Vector3.right), Time.deltaTime * turnSpeed);
            }
			XMove(new Vector3 (leftOrRightValue, 0, 0), MovementMode.March);

        }

        // ------------------------------------------------------------ AI BEHAVIOUR END ------------------------------------------------------------ //

        //Ticks down the weapon lock allowing the enemy to attack again
        weaponLock -= Time.deltaTime;

        //Checks the health value that is on the EntityClass and destroys the object
        if(health <= 0)
        {
			int rng = Random.Range(0, 100);
			if(rng <= rngDropPercent)
			{
				Instantiate(healthOrb, transform.position, Quaternion.identity);
			}

            gameObject.SetActive(false);
            EnemySpawner.enemiesSpawned--;
        }
	}

    //Solves the problem of "Is the object left or right of me"
    //This was obtained from a thread on a forum because I need to git gud.
    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return -1f;
        }
        else if (dir < 0f)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    public void setPlayer(GameObject p)
    {
        player = p;
    }

    public void setPlayerColossus(GameObject pC)
    {
        playerColossus = pC;
    }

    public void setResetHealth()
    {
        health = 20;
    }

	void XMove (Vector3 moveDir, MovementMode moveMode)
	{
		if (moveMode == MovementMode.Run)
		{
			if (moveDir.x < 0 && rB.velocity.x > -runSpeed)
			{
				rB.AddForce (new Vector3 (moveDir.x, 0, 0) * Time.deltaTime * 100 * acceleration);
			}
			else if (moveDir.x > 0 && rB.velocity.x < runSpeed)
			{
				rB.AddForce (new Vector3 (moveDir.x, 0, 0) * Time.deltaTime * 100 * acceleration);
			}
		}
		else if (moveMode == MovementMode.March)
		{
			if (moveDir.x < 0 && rB.velocity.x > -marchSpeed)
			{
				rB.AddForce (new Vector3 (moveDir.x, 0, 0) * Time.deltaTime * 100 * acceleration);
			}
			else if (moveDir.x > 0 && rB.velocity.x < marchSpeed)
			{
				rB.AddForce (new Vector3 (moveDir.x, 0, 0) * Time.deltaTime * 100 * acceleration);
			}
		}
	}

	void XBrake ()
	{
		rB.AddForce (new Vector3(-rB.velocity.x, 0, 0) * Time.deltaTime * 100 * runSpeed);
	}

    //Exact copy of the XMove function, just going to be used for the Z move so that
    //The AI can change its layer to move around other enemies. 
    void ZMove(Vector3 moveDir, MovementMode moveMode)
    {
        if (moveMode == MovementMode.Run)
        {
            if (moveDir.z < 0 && rB.velocity.z > -runSpeed)
            {
                rB.AddForce(new Vector3(moveDir.z, 0, 0) * Time.deltaTime * 100 * acceleration);
            }
            else if (moveDir.z > 0 && rB.velocity.z < runSpeed)
            {
                rB.AddForce(new Vector3(moveDir.z, 0, 0) * Time.deltaTime * 100 * acceleration);
            }
        }
        else if (moveMode == MovementMode.March)
        {
            if (moveDir.z < 0 && rB.velocity.z > -marchSpeed)
            {
                rB.AddForce(new Vector3(moveDir.z, 0, 0) * Time.deltaTime * 100 * acceleration);
            }
            else if (moveDir.z > 0 && rB.velocity.z < marchSpeed)
            {
                rB.AddForce(new Vector3(moveDir.z, 0, 0) * Time.deltaTime * 100 * acceleration);
            }
        }
    }

    void ZBrake()
    {
        rB.AddForce(new Vector3(-rB.velocity.z, 0, 0) * Time.deltaTime * 100 * runSpeed);
    }
}
