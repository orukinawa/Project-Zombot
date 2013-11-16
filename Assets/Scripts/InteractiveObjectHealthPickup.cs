using UnityEngine;
using System.Collections;

public class InteractiveObjectHealthPickup : InteractiveObjectBase
{
	public float healAmount;
	
	public override void ApplyEffect (GameObject obj)
	{
		StatsBase stat = obj.GetComponent<StatsBase>();
		if(stat == null)
		{
			Debug.LogWarning("StatBase not found!");
			return;
		}
		if(stat.isFullHealth())
		{
			Debug.Log("Full Health!");
			return;
		}
		stat.ApplyDamage(healAmount);
		SelfDestruct();
	}
}
