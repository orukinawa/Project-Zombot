using UnityEngine;
using System.Collections;

// For slow and haste
public class StatusSpeedModifier : StatusBase
{
	public override void updateStatus ()
	{
		if(ticksLeft < 1)
		{
			gameObject.GetComponent<StatsBase>().RestoreMoveSpeed();
			UnsubscribeFromTickEvent();
			return;
		}
		--ticksLeft;
	}
	
	public void InitiateSlow(float multiplier, int tick)
	{
		ticksLeft = tick;
		gameObject.GetComponent<StatsBase>().ApplySlow(multiplier);
		SubscribeToTickEvent();
	}
}
