using UnityEngine;
using System.Collections;

class ActionExecuteData
{
	public bool mFlag;
}

public class ActionExecute : Transition
{
	public BehaviourBase mTargetBehaviour;
	
	public override void Init (StateManager stateManager)
	{
		ActionExecuteData data;
		
		if(!stateManager.mCustomData.ContainsKey(this))
		{
			data = new ActionExecuteData();
			data.mFlag = false;
			stateManager.mCustomData[this] = data;
		}
		else
		{
			data = (ActionExecuteData)stateManager.mCustomData[this];
			data.mFlag = false;
		}
	}
	
	public override bool VerifyTransition (StateManager context)
	{
		ActionExecuteData data = (ActionExecuteData)context.mCustomData[this];
		
		if(data.mFlag)
		{
			return true;
		}
		
		return false;
	}
}
