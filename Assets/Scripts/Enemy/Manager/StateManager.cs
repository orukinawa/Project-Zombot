using UnityEngine;
using System.Collections.Generic;

abstract public class StateManager : MonoBehaviour
{
	public Stack<State> mStateStack = new Stack<State>();
	public State mCurrentState;
	public State mStartState;
	
	public Dictionary<object, object> mCustomData = new Dictionary<object, object>();
	
	void Start()
	{
		//! Initial a start state
		RequestState(Transition.MODE.PUSH, mStartState);
	}
	
	public void RequestState(Transition.MODE mode, State targetState)
	{
		//checks whether the stack is null
		State activeState = (mStateStack.Count > 0) ? mStateStack.Peek() : null;
		
		if(activeState)
		{
			if(mode == Transition.MODE.POP)
			{	
				//! pop the stack and get from the top of the stack
				mStateStack.Pop();
				activeState = mStateStack.Peek();
				
				if(activeState)
				{
					activeState.StateEnter(this);
				}
				
				return;
			}
			else if(mode == Transition.MODE.REPLACE)
			{
				mStateStack.Pop();
			}
		}
		//! push the state into the stack
		targetState.StateEnter(this);
		mStateStack.Push(targetState);
//		foreach(KeyValuePair<object,object> pair in mCustomData)
//		{
//			Debug.Log("Key: " + pair.Key + " Value: " + pair.Value);
//		}
	}
	
}
