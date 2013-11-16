using UnityEngine;
using System.Collections;

public class Tester : MonoBehaviour
{
	public MovementController mController;
	public StatusImmobilize mStatusImmobilize;
	public StatusSpeedModifier mStatusSpeedModifier;
	public StatsCharacter mStatsCharacter;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.B))
		{
			mStatusSpeedModifier.InitiateSlow(0.5f,10);
		}
		if(Input.GetKeyDown(KeyCode.H))
		{
			mStatsCharacter.Freeze();
		}
		if(Input.GetKeyDown(KeyCode.J))
		{
			mStatsCharacter.RemoveFreeze();
		}
		if(Input.GetKeyDown(KeyCode.K))
		{
			mStatsCharacter.Immobilize();
		}
		if(Input.GetKeyDown(KeyCode.L))
		{
			mStatsCharacter.RemoveImmobilize();
		}
	}
}
