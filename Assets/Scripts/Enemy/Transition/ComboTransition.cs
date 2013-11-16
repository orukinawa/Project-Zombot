using UnityEngine;
using System.Collections;

public class ComboTransition : Transition
{
	int mTotalTransition;
	
	//! list of the transition that needs to check
	Transition[] mTransitionList;
	
	//! the gameobject(empty) in the scene/parent etc reference
	//! always put it where the parent of the obj is the state gameobject eg. IDLE
	public GameObject mTransitionListObj;
	
	public override void Init (StateManager stateManager)
	{
		mTransitionList = mTransitionListObj.GetComponents<Transition>();
		mTotalTransition = mTransitionList.Length;
	}
	
	public override bool VerifyTransition (StateManager context)
	{
		//! number of transition that return true at current frame
		int successTransition = 0;
		if(mTotalTransition <= 0)return false;
		
		foreach(Transition transition in mTransitionList)
		{
			bool flag = transition.VerifyTransition(context);
			if(flag)
			{
				successTransition += 1;
			}
		}
		
		if(successTransition == mTotalTransition)
		{
			return true;
		}
		return false;
	}
}
