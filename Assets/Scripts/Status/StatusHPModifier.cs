using UnityEngine;
using System.Collections;

//This is used for Damage and Heal Over Time
public class StatusHPModifier : StatusBase
{
	int damage = 0;
	
	public override void updateStatus ()
	{
		if(ticksLeft < 1)
		{
			UnsubscribeFromTickEvent();
			return;
		}
		gameObject.GetComponent<StatsBase>().ApplyDamage(-damage);
		--ticksLeft;
	}
	
	public void initiateDmg(int setDamage, int tick)
	{
		ticksLeft = tick;
		damage = setDamage;
		SubscribeToTickEvent();
	}
}
