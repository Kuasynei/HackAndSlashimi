using UnityEngine;
using System.Collections;


//An enum and package that we use for TakeDamage() to give the receiver enough information to determine whether or not its their job to get hit.
//Entities get upset when they take more damage then they signed up for.
public enum faction{goodGuys, badGuys, neutral};
public struct DamageInfo
{
	public float damage;
	public GameObject attacker;
	public faction attackerFaction;

	public DamageInfo (float INdamage, GameObject INattacker, faction INfaction)
	{
		damage = INdamage;
		attacker = INattacker;
		attackerFaction = INfaction;
	}
}

public class EntityClass : MonoBehaviour 
{
	protected float maxH = 1;
	protected float health = 1;
	protected faction myFaction = faction.neutral;

	void Start()
	{
		health = maxH;
	}

	public float TakeDamage(DamageInfo damagePackage) 
	{
		if(damagePackage.attackerFaction != myFaction || damagePackage.attackerFaction == faction.neutral)
        {
			health -= damagePackage.damage;
        }
        else
        {
            Debug.Log("DAMAGE NOT VALID");
        }

        return health;
    }

	public float Heal(float heal) 
	{
		if(health != maxH)
		{
			health += heal;
		}
		return health;
	}
}
