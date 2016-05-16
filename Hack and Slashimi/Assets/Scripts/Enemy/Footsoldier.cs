using UnityEngine;
using System.Collections;

public class Footsoldier : EnemyClass {

	[Header("Main")]
	public bool debugMode = true;
	[SerializeField] GameObject healthOrb;
    [SerializeField] MeleeWeaponClass enemyWeapon;

	[Header("Movement")]
	[SerializeField] float runSpeed = 4;
	[SerializeField] float marchSpeed = 2;
	[SerializeField] float acceleration = 10;
	[SerializeField] float turnSpeed = 1;

	[Header("Combat Stats")]
	[SerializeField] float maxHealth;
	[SerializeField] float basicAttackDamage = 10;
	[SerializeField] int rngDropPercent = 30;
	[SerializeField] faction setFaction = faction.badGuys;

    //THIS WILL BE CHANGED TO AXE
    private Sword enemySword;
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
		enemySword = enemyWeapon as Sword;

		myFaction = setFaction;
		player = GameManager.GetPlayer ();
		playerColossus = GameManager.GetPColossus();
		rB = GetComponent <Rigidbody> ();
	}

    // Use this for initialization
	protected override void Start () 
    {
		base.Start ();
		commandTick = Random.value; //Staggers commandTicks for all AI so they don't look like robits.
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (commandTick <= 0) commandTick = 1f;
		commandTick -= Time.deltaTime; 

        //Check distance from the enemy to the player
        distToPlayer = Vector3.Distance(player.transform.position, transform.position);

        //Check distance from the enemy to the player colossus
        distToColossus = Vector3.Distance(playerColossus.transform.position, transform.position);

        // ------------------------------------------------------------ AI BEHAVIOUR START ------------------------------------------------------------ //
		

        //Move them back to the middle lane if they are in front of the player on the Z-axis
        //@note: They seem to freak out if they are there.

        //Move to the layer below if you cant move forward
        if(transform.position.z > 0.5f || transform.position.z < -0.5f)
        {
            RaycastHit hit;
            Vector3 angleToMoveDir = (transform.right + transform.forward) / 2;

            if (!Physics.Raycast(transform.position, angleToMoveDir, out hit, 5.0f))
            {
                ZMove(angleToMoveDir, MovementMode.March);

                if (debugMode) Debug.Log("Moving back to the right");
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.right, out hit, 5.0f))
            {
                if (hit.collider.tag == "Enemy")
                {
                    //&& hit.collider.gameObject.GetComponent<Rigidbody>().velocity.x <= 0.5f

                    Vector3 angleToMoveDir = (-transform.right + transform.forward) / 2;

					if (debugMode) Debug.Log("Detected Enemy Infront");

                    if (!Physics.Raycast(transform.position, angleToMoveDir, out hit, 5.0f))
                    {
                        ZMove(angleToMoveDir, MovementMode.March);

						if (debugMode) Debug.Log("Moving To The Left");
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
					DamageInfo orcDamagePackage = new DamageInfo(basicAttackDamage, this.gameObject, myFaction);
					enemySword.BasicAttack (0.4f, 1, orcDamagePackage);
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
					DamageInfo orcDamagePackage = new DamageInfo(basicAttackDamage, this.gameObject, myFaction);
					enemySword.BasicAttack (0.4f, 1, orcDamagePackage);
                    weaponLock = 1.0f;
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
