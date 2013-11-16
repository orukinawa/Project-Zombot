using UnityEngine;
using System.Collections;

public class StatsCharacter : StatsBase
{
	MovementController mMovementController;
	public float maxEnergy;
	float currentEnergy;
	
	void Start ()
	{
		initializeStats();
	}
	
	public override void initializeStats ()
	{
		base.initializeStats ();
		mMovementController = GetComponent<MovementController>();
		currentEnergy = maxEnergy;
	}
	
	public override void ApplySlow (float multiplier)
	{
		mMovementController.moveSpeedMultiplier = multiplier;
	}
	
	public override void RestoreMoveSpeed ()
	{
		mMovementController.moveSpeedMultiplier = 1.0f;
	}
	
	public override void Freeze ()
	{
		mMovementController.SetMotor(MovementController.MOTORTYPE.FROZEN);
	}
	
	public override void RemoveFreeze ()
	{
		if(mMovementController.currMotortype == MovementController.MOTORTYPE.FROZEN)
			mMovementController.SetMotor(MovementController.MOTORTYPE.NORMAL);
	}
	
	public override void Immobilize()
	{
		if(mMovementController.currMotortype == MovementController.MOTORTYPE.NORMAL)		
			mMovementController.SetMotor(MovementController.MOTORTYPE.IMMOBILE);
	}
	
	public override void RemoveImmobilize()
	{
		if(mMovementController.currMotortype == MovementController.MOTORTYPE.IMMOBILE)		
			mMovementController.SetMotor(MovementController.MOTORTYPE.NORMAL);
	}
	
	public override void InvertControl()
	{
		mMovementController.invertMultiplier = -1.0f;
	}
	
	public override void RevertControl()
	{
		mMovementController.invertMultiplier = 1.0f;
	}
	
	public void ApplyEnergy(float ener)
	{
		currentEnergy += ener;
		if(currentEnergy <= 0.0f)
		{
			currentEnergy = 0.0f;
		}
		if(currentEnergy > maxEnergy)
		{
			currentEnergy = maxEnergy;
		}
	}
	
	public bool isFullEnergy()
	{
		if(currentEnergy == maxEnergy)
		{
			return true;
		}
		return false;
	}
	
	//added for submission
	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical();
		GUILayout.Space(200f);
		GUILayout.Label("HP: " + currentHealth + " / " + maxHealth);
		GUILayout.Space(200f);
		GUILayout.Label("R     - Reload");
		GUILayout.Label("E     - PickupItem/Interact");
		GUILayout.Label("Y     - Change Gun Menu");
		GUILayout.Label("Space - Dash");
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
	}
	
	public override void SelfDestruct ()
	{
		Immobilize();
	}
}
