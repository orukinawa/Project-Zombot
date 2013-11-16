using UnityEngine;
using System.Collections;

public class StatusInvertControl : StatusBase
{
	public override void updateStatus ()
	{
		if(ticksLeft < 1)
		{
			gameObject.GetComponent<StatsBase>().RevertControl();
			UnsubscribeFromTickEvent();
			return;
		}
		--ticksLeft;
	}
	
	public void InitiateInvert(int tick)
	{
		ticksLeft = tick;
		gameObject.GetComponent<StatsBase>().InvertControl();
		SubscribeToTickEvent();
	}
}
