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
		Init();
	}
	
	protected virtual void Init()
	{
		//Debug.Log("Init state");
		//! Initial a start state
		RequestState(Transition.MODE.PUSH, mStartState);
	}
	
	/// <summary>
	/// Clears all the reference state
	/// </summary>
	public void ResetStateMachine()
	{
		//Debug.Log("reset state machine!");
		mStateStack.Clear();
		mCurrentState = null;
	}
	
	/// <summary>
	/// Request the state with 3 different transition mode of choice
	/// </summary>
	/// <param name='mode'>
	/// Transition mode
	/// </param>
	/// <param name='targetState'>
	/// Target state.
	/// </param>
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
	}
}
