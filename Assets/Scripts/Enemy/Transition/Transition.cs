using UnityEngine;
using System.Collections;

abstract public class Transition : MonoBehaviour
{
	public enum MODE
	{
		PUSH,
		POP,
		REPLACE
	}
	
	public State mTargetState;
	public MODE mTransitionMode;
	public int mPriority;
	
	public virtual void Init(StateManager stateManager)
	{
		
	}
	
	//! a deinit function for behaviours when a state transition switch occur
	public virtual void DeInit(EnemyBase enemyBase)
	{
		BehaviourBase[] behaviourBase = GetComponents<BehaviourBase>();
		foreach(BehaviourBase behaviour in behaviourBase)
		{
			behaviour.DeInit(enemyBase);
		}
	}
	
	public virtual bool VerifyTransition(StateManager context)
	{return false;}
}
