﻿using UnityEngine;
using System.Collections;

public class Gate : EntityClass 
{
	[SerializeField] float maxHealth = 100000;

	protected override void Awake () 
	{

		maxH = maxHealth;

		base.Awake ();
	}

	protected override void Update()
	{
		base.Update ();

		//Debug.Log(gameObject.name + "'s Health: " + health);

		if(health <= 0)
		{
			Destroy(gameObject);
		}
	}
}
