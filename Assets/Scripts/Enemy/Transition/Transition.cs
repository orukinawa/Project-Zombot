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
	
	public virtual bool VerifyTransition(StateManager context)
	{return false;}
}
