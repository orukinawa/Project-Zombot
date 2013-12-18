using UnityEngine;
using System.Collections;

public class BehaviourState : State
{
	//! List of behaviour in one state
	public BehaviourBase[] mBehaviourList;
	
	void Awake()
	{
		Init();
		mBehaviourList = GetComponents<BehaviourBase>();
		mStateName = gameObject.name;
		//Debug.Log("StateName: " + mStateName + " transition count: " + mTransitionsList.Length);
	}
	
	//! performs once it enters a new state
	public override void StateEnter (StateManager stateManager)
	{
		//! init the custom data require for the transition
		foreach(Transition transition in mTransitionsList)
		{
			transition.Init(stateManager);
		}
		//! Update the controller behaviour 
		EnemyBase enemyBase = stateManager.gameObject.GetComponent<EnemyBase>();
		enemyBase.RefreshBehaviour(this);
	}
	//! Updates the state every new frame
	public override void StateUpdate (StateManager stateManager)
	{
		//! Do verify here
		VerifyTransition(stateManager);
	}
	
	public override void StateExit(StateManager stateManager)
	{
		EnemyBase enemyBase = stateManager.GetComponent<EnemyBase>();
		BehaviourBase[] behaviourBase = GetComponents<BehaviourBase>();
		foreach(BehaviourBase behaviour in behaviourBase)
		{
			behaviour.DeInit(enemyBase);
		}
	}
	
	public override void VerifyTransition (StateManager stateManager)
	{
		//Debug.Log("StateName: " + mStateName + " transition count: " + mTransitionsList.Length);
		foreach(Transition transition in mTransitionsList)
		{
			if(transition.VerifyTransition(stateManager))
			{
				//! deinit for all the behaviour
				StateExit(stateManager);
				//! Do request state here
				stateManager.RequestState(transition.mTransitionMode, transition.mTargetState);
				return;
			}
		}
	}
}
