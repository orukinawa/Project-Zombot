using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour
{
	MovementController moveController;
	InventoryManager invManager;
	InteractionManager interManager;
	StatsCharacter statChar;
	GamePadInput mGamePadInput;
	public Animator mAnimator;
	
	void Awake()
	{
		invManager = GetComponent<InventoryManager>();
		moveController = GetComponent<MovementController>();
		interManager = GetComponent<InteractionManager>();
		statChar = GetComponent<StatsCharacter>();
		mGamePadInput = GetComponent<GamePadInput>();
	}
	
	void Update()
	{
		bool tempShootFlag = mGamePadInput.GetAxis(GamePadInput.AxisType.RIGHT_TRIGGER) > 0.5f;
		mAnimator.SetBool("isShooting",tempShootFlag);
		if(tempShootFlag)
		{
			invManager.ShootWeapon();
		}
		if(Input.GetButtonDown("Reload"))
		{
			invManager.ReloadWeapon();
		}		
		if(Input.GetButtonDown("Use Item"))
		{
			//Code to use item here
		}		
		if(Input.GetButtonDown("Interact"))
		{
			interManager.Interact();
		}		
		if(Input.GetButtonDown("Dash"))
		{
			moveController.Dash();
		}
		
		//DEBUG
		if(Input.GetKeyDown(KeyCode.O) && Input.GetKeyDown(KeyCode.P))
		{
			statChar.ApplyDamage(-10.0f);
			statChar.ApplyEnergy(-10.0f);
		}		
	}
	
	//move
	
	//Pause
}
