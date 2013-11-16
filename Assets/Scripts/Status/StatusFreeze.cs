using UnityEngine;
using System.Collections;

public class StatusFreeze : StatusBase
{
	public override void updateStatus ()
	{
		if(ticksLeft < 1)
		{
			gameObject.GetComponent<StatsBase>().RemoveFreeze();
			UnsubscribeFromTickEvent();
			return;
		}
		--ticksLeft;
	}
	
	public void InitiateFreeze(int tick)
	{
		ticksLeft = tick;
		gameObject.GetComponent<StatsBase>().Freeze();
		SubscribeToTickEvent();
	}
}
