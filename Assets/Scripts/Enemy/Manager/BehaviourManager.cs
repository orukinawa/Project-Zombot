using UnityEngine;
using System.Collections;

public class BehaviourManager : StateManager
{
	void Update()
	{
		//Debug.Log("StateManager is calling");
		State activeState = (mStateStack.Count > 0) ? mStateStack.Peek() : null;
		mCurrentState = activeState;
		if(!activeState)
		{
			Debug.LogError("object with null state found!");
			return;
		}
		activeState.StateUpdate(this);
	}
	
	void OnDrawGizmos()
	{
		State activeState = (mStateStack.Count > 0) ? mStateStack.Peek() : null;
		
		if(!activeState)
		{
			//Debug.LogError("object with null state found!");
			return;
		}
		Vector3 offset = new Vector3(0.0f,2.0f,0.0f);
		Gizmos.DrawIcon(transform.position + offset, "AiRelated/"+ activeState.GetStateName());	
	}
}
