using UnityEngine;
using System.Collections;

public class PickupStats : PickupBase
{
	public int healthRestored;
	StatsBase statsBase1;

	void Start ()
	{
	
	}
	
	void Update ()
	{
	
	}
	
	public override void PickupEffect(StatsBase statsBase)
	{
		Debug.Log("Called inside PickupStats");
		statsBase1 = statsBase;
		statsBase1.ApplyDamage(healthRestored);
		Destroy(gameObject);
	}
}
