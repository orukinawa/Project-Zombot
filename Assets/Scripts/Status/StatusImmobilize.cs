using UnityEngine;
using System.Collections;

public class StatusImmobilize : StatusBase
{
	public override void updateStatus ()
	{
		if(ticksLeft < 1)
		{
			gameObject.GetComponent<StatsBase>().RemoveImmobilize();
			UnsubscribeFromTickEvent();
			return;
		}
		--ticksLeft;
	}
	
	public void InitiateImmobilize(int tick)
	{		
		ticksLeft = tick;
		gameObject.GetComponent<StatsBase>().Immobilize();
		SubscribeToTickEvent();		
	}
}
