using UnityEngine;
using System.Collections;

public class AiMovement : EntityClass {

	[SerializeField] GameObject healthOrb;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerColossus;
    [SerializeField] WeaponClass enemyWeapon;
	[SerializeField] float runSpeed = 4;
	[SerializeField] float marchSpeed = 2;
	[SerializeField] float turnSpeed = 1;
	[SerializeField] int rngDropPercent = 30;

    private CharacterController charController;
    //THIS WILL BE CHANGED TO AXE
    private Sword enemySword;
    

    private float distToPlayer;
    private float distToColossus;
    private float weaponLock = 0;
	private float commandTick; //Allows certain orders to run for cyclically.
	
    // Use this for initialization
	void Start () 
    {
        //Store the character controller and the enemy sword/weapon
        charController = GetComponent<CharacterController>();
        enemySword = (Sword)enemyWeapon;

		commandTick = Random.value; //Staggers commandTicks for all AI so they don't look like robits.
	}
	
	// Update is called once per frame
	void Update ()
    {
		//
		if (commandTick <= 0) commandTick = 1f;
		commandTick -= Time.deltaTime; 

        //Check distance from the enemy to the player
        distToPlayer = Vector3.Distance(player.transform.position, transform.position);

        //Check distance from the enemy to the player colossus
        distToColossus = Vector3.Distance(playerColossus.transform.position, transform.position);

        // ------------------------------------------------------------ AI BEHAVIOUR START ------------------------------------------------------------ //

        //This entire area is basically a behavior tree. This prioritizes running towards the colossus then towards the player else it will just
        //run to the left
        //@note: the basic else functionality will have to change because enemies will not only spawn on the right of the map
        if(distToColossus < 15)
        {
            if(distToColossus > 7)
            {
                //Move toward the colossus
                Vector3 headingDir = playerColossus.transform.position - transform.position;
				float leftOrRightValue = AngleDir(-Vector3.forward, headingDir, transform.up); //I replaced transform.forward with -vector3.forward
                if (leftOrRightValue != 0)
                {
					transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(leftOrRightValue * Vector3.right), Time.deltaTime * turnSpeed);
                }

				charController.SimpleMove(new Vector3(leftOrRightValue * runSpeed, 0, 0));
            }
			else if ( distToColossus < 7 && commandTick <= 0.5f)
            {    
				//Rotate...
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation (new Vector3 (playerColossus.transform.position.x, 0, playerColossus.transform.position.z) 
					- new Vector3 (transform.position.x, 0, transform.position.z)), Time.deltaTime * turnSpeed);
				
				if (weaponLock <= 0)
				{
					//...and smack the colossus
					enemySword.BasicAttack (0.4f, 1);
					weaponLock = 1.0f;

					//Stop and look at the colossus
					charController.SimpleMove (Vector3.zero);
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

				charController.SimpleMove(new Vector3(leftOrRightValue * runSpeed, 0, 0));
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

				charController.SimpleMove(new Vector3(leftOrRightValue * runSpeed, 0, 0));
            }
            else if (distToPlayer < 1.5f)
            {
                charController.SimpleMove(Vector3.zero);
                if(weaponLock <= 0)
                {
                    enemySword.BasicAttack(0.4f, 1);
                    weaponLock = 1.0f;
                }
            }
        }
        else
        {
            Vector3 headingDir = playerColossus.transform.position - transform.position;
            headingDir.z = 0;
			float leftOrRightValue = AngleDir(-Vector3.forward, headingDir, transform.up);
            if (leftOrRightValue != 0)
            {
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(leftOrRightValue * Vector3.right), Time.deltaTime * turnSpeed);
            }
			charController.SimpleMove(new Vector3(leftOrRightValue * marchSpeed, 0, 0));

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
        health = 10;
    }
}
