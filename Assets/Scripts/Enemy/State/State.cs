using UnityEngine;
using System.Collections;

abstract public class State : MonoBehaviour
{
	//! list of transition that can change the state
	public Transition[] mTransitionsList;
	
	//! For debugging purpose
	public string mStateName;
	
	protected void Init()
	{
		mTransitionsList = GetComponents<Transition>();
		SortTransition(mTransitionsList);
	}
	
	public void SortTransition(Transition[] transitionList)
	{
		//! bubble sort
		bool isSortedFlag = false;
		while(!isSortedFlag)
		{
			//! determine is the swap taken place
			int flag = 0;
			
			for(int i = 0; i < transitionList.Length - 1; i++)
			{
				if(transitionList[i].mPriority > transitionList[i + 1].mPriority)
				{
					//! swap
					Transition temp = transitionList[i];
					transitionList[i] = transitionList[i + 1];
					transitionList[i + 1] = temp;
					flag = 1;
				}
			}
			
			if(flag == 0)
			{
				isSortedFlag = true;
			}
		}
	}
	
	//! this function updates per frame
	public virtual void VerifyTransition(StateManager stateManager)
	{
		//Debug.Log("StateName: " + mStateName + " transition count: " + mTransitionsList.Length);
		foreach(Transition transition in mTransitionsList)
		{
			if(transition.VerifyTransition(stateManager))
			{
				//! Do request state here
				stateManager.RequestState(transition.mTransitionMode, transition.mTargetState);
				return;
			}
		}
	}
	
	public string GetStateName()
	{
		return gameObject.name;
	}
	
//	public virtual void TransitionInit(StateManager stateManager)
//	{
//		foreach(Transition transition in mTransitionsList)
//		{
//			transition.Init(stateManager);
//		}
//	}
	
	public virtual void StateUpdate(StateManager stateManager)
	{}
	
	public virtual void StateEnter(StateManager stateManager)
	{}
	
}
