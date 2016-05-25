using UnityEngine;
using System.Collections;

//Used AttackInfo structs to determine what takes damage and what ignores damage.
public enum faction{goodGuys, badGuys, neutral};

//A package that we use for TakeDamage() and TakeCrowdControl() to give the receiver enough information to determine whether or not its their job to get hit.
public struct OmniAttackInfo
{
	public GameObject attacker;
	public faction attackerFaction;
	public float damage; 
	public float stunDuration;
	public Vector3 launchVector;

	public OmniAttackInfo (GameObject INattacker, faction INfaction, float INdamage, float INstunDuration, Vector3 INlaunchVector)
	{
		attacker = INattacker;
		attackerFaction = INfaction;
		damage = INdamage;
		stunDuration = INstunDuration;
		launchVector = INlaunchVector;
	}
};

public class EntityClass : MonoBehaviour 
{
	protected float maxH = 1;
	protected float health = 1;
	protected float stun = 0;
	protected faction myFaction = global::faction.neutral;

	protected virtual void Awake()
	{
		health = maxH;
	}

	protected virtual void Start() { }

	protected virtual void Update()
	{
		if (stun > 0)
			stun -= Time.deltaTime;
	}

	public virtual float TakeDamage(OmniAttackInfo omniPackage) 
	{
		if(omniPackage.attackerFaction != myFaction || omniPackage.attackerFaction == global::faction.neutral)
        {
			health -= omniPackage.damage;
        }

        return health;
    }

	public virtual void TakeCrowdControl(OmniAttackInfo omniPackage)
	{
		if (omniPackage.attackerFaction != myFaction || omniPackage.attackerFaction == global::faction.neutral)
		{
			Launch (omniPackage.launchVector);
			stun = omniPackage.stunDuration;
		}
	}

	public virtual float Heal(float heal) 
	{
		if(health != maxH)
		{
			health += heal;

            Debug.Log("Heal Run");
		}
		return health;
	}

	public virtual Vector3 Launch(Vector3 INlaunchVector)
	{
		if (GetComponent<Rigidbody> ())
		{
			Rigidbody rB = GetComponent<Rigidbody> ();
			rB.velocity = INlaunchVector;
			return INlaunchVector;
		}

		return Vector3.zero;
	}


	public faction GetFaction()
	{
		return myFaction;
	}
}
