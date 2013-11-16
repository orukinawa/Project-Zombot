using UnityEngine;
using System.Collections;

public class SomeTestTransition : Transition
{
	
	public override bool VerifyTransition (StateManager context)
	{
		return false;
	}
	
}
