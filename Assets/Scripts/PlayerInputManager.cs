using UnityEngine;
using System.Collections;

public class PlayerInputManager : MonoBehaviour
{
	MovementController moveController;
	InventoryManager invManager;
	InteractionManager interManager;
	StatsCharacter statChar;
	
	void Awake()
	{
		invManager = GetComponent<InventoryManager>();
		moveController = GetComponent<MovementController>();
		interManager = GetComponent<InteractionManager>();
		statChar = GetComponent<StatsCharacter>();
	}
	
	void Update()
	{
		if(Input.GetButton("Shoot"))
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
