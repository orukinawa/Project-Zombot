using UnityEngine;
using System.Collections;

public class StatsCharacter : StatsBase
{
	MovementController mMovementController;
	public float maxEnergy;
	float currentEnergy;
	public GameObject hitEffectPrefab;
	public GameObject healEffectPrefab;
	GamePadInput mGamePadInput;
	
	void Start ()
	{
		initializeStats();
		mGamePadInput = GetComponent<GamePadInput>();
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
	
	public override void ApplyDamage (float damage, GameObject player = null)
	{
		base.ApplyDamage (damage, player);
		mGamePadInput.VibrateOnce();
		//GameObject tempObject;
		//if(damage < 0) tempObject = PoolManager.pools["Visual Pool"].Spawn(hitEffectPrefab,transform.position,transform.rotation);
		//else tempObject = PoolManager.pools["Visual Pool"].Spawn(healEffectPrefab,transform.position,transform.rotation);
		//tempObject.transform.parent = transform;
	}
	
	//added for submission
//	void OnGUI()
//	{
//		GUILayout.BeginHorizontal();
//		GUILayout.Space(20f);
//		GUILayout.BeginVertical();
//		GUILayout.Space(200f);
//		GUILayout.Label("HP: " + currentHealth + " / " + maxHealth);
//		GUILayout.Space(200f);
//		GUILayout.Label("R     - Reload");
//		GUILayout.Label("E     - PickupItem/Interact");
//		GUILayout.Label("Y     - Change Gun Menu");
//		GUILayout.Label("Space - Dash");
//		GUILayout.EndVertical();
//		GUILayout.EndHorizontal();		
//	}
	
	public override void SelfDestruct ()
	{
		Immobilize();
	}
}
