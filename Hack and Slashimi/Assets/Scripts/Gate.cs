using UnityEngine;
using System.Collections;

public class Gate : EntityClass 
{
	[SerializeField] float maxHealth = 100000;

	void Awake () 
	{
		health = maxHealth;
	}
}
