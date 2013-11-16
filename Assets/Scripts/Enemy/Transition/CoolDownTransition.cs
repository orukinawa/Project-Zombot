using UnityEngine;
using System.Collections;

class CoolDownTransitionData
{
	public float mTimer;
}

public class CoolDownTransition : Transition {
	
	public float mTimerDuration;
	
	public override void Init (StateManager stateManager)
	{
		if(!stateManager.mCustomData.ContainsKey(this))
		{
			CoolDownTransitionData data = new CoolDownTransitionData();
			data.mTimer = 0.0f;
			stateManager.mCustomData[this] = data;
		}
	}
	
	public override bool VerifyTransition (StateManager context)
	{
		CoolDownTransitionData data = (CoolDownTransitionData)context.mCustomData[this];
		
		data.mTimer += Time.deltaTime;
		
		if(data.mTimer >= mTimerDuration)
		{
			data.mTimer = 0;
			return true;
		}
		return false;
	}
}
