using UnityEngine;
using System.Collections.Generic;

public class Commando : MonoBehaviour 
{
	public GameObject[] equippedWeapons;
	int currWeaponIndex;
	public Transform meshTransform;
	public Vector3 weaponOffset;
	GunBase gunScript;
	
	public string inputAxis;
	
	void Start()
	{
		InitializeGuns();
	}
	
	void Update()
	{
		//! Actions inside can only be done with a weapon equipped.
		if(equippedWeapons[currWeaponIndex] != null)
		{
			if(Input.GetAxis(inputAxis) > 0)
			{
				gunScript.Shoot();
			}
			if(Input.GetKeyDown(KeyCode.G))
			{
				DropWeapon();
			}
		}		
		if(Input.GetKeyDown(KeyCode.R))
		{
			gunScript.Reload();
		}
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			CycleWeaponDown();
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			CycleWeaponUp();
		}
	}	
	
	//! Spawns a weapon and sets it to "NOT ACTIVE".
	void SpawnWeapon(int index)
	{
		if(equippedWeapons[index] != null)
		{
			Debug.Log("Spawned Weapon: " + equippedWeapons[index].name);
			string tempName = equippedWeapons[index].name;
			equippedWeapons[index] = Instantiate(equippedWeapons[index], transform.position, transform.rotation) as GameObject;
			equippedWeapons[index].transform.parent = meshTransform;
			equippedWeapons[index].transform.localPosition = weaponOffset;
			equippedWeapons[index].name = tempName;
			equippedWeapons[index].GetComponentInChildren<CapsuleCollider>().enabled = false;
			equippedWeapons[index].SetActive(false);
			equippedWeapons[index].GetComponent<PickupBase>().isOnGround = false;
			//implement to disable pickupweapon
		}		
	}
	
	//! Sets target weapon to "ACTIVE".
	void EquipWeapon(int index)
	{
		currWeaponIndex = index;
		if(equippedWeapons[index] != null)
		{
			Debug.Log("Equip Weapon: " + equippedWeapons[index].name);
			equippedWeapons[currWeaponIndex].SetActive(true);
			gunScript = equippedWeapons[currWeaponIndex].GetComponent<GunBase>();
			gunScript.Reset();
		}		
	}
	
	//! Unequips current weapon.
	void UnEquipWeapon()
	{
		if(equippedWeapons[currWeaponIndex] != null)
		{
			equippedWeapons[currWeaponIndex].SetActive(false);
			gunScript = null;
		}	
	}
	
	//! Cycles until a weapon is found.
	void EquipNextValidWeapon()
	{
		for(int i=currWeaponIndex; i < equippedWeapons.Length; ++i)
		{
			if(equippedWeapons[i] != null)
			{
				EquipWeapon(i);
				return;
			}
		}
		for(int i=0; i < currWeaponIndex; ++i)
		{
			if(equippedWeapons[i] != null)
			{
				EquipWeapon(i);
				return;
			}
		}		
	}
	
	//! Switch to selected weapon
	void SwitchWeapon(int index)
	{
		Debug.Log("Switching Weapon!");
		UnEquipWeapon();
		EquipWeapon(index);
	}
	
	//! Switches with next weapon.
	void CycleWeaponUp()
	{
		if(currWeaponIndex != equippedWeapons.Length - 1)
		{
			SwitchWeapon(currWeaponIndex+1);
		}
		else
		{
			SwitchWeapon(0);
		}
	}
	
	//! Switches with previous weapon.
	void CycleWeaponDown()
	{
		if(currWeaponIndex != 0)
		{
			SwitchWeapon(currWeaponIndex-1);
		}
		else
		{
			SwitchWeapon(equippedWeapons.Length - 1);
		}
	}
	
	//! Drops Weapon on the ground.
	void DropWeapon()
	{		
		if(equippedWeapons[currWeaponIndex] != null)
		{
			Debug.Log("Dropped Weapon: " + equippedWeapons[currWeaponIndex].name);
			equippedWeapons[currWeaponIndex].GetComponentInChildren<CapsuleCollider>().enabled = true;
			equippedWeapons[currWeaponIndex].GetComponent<GunBase>().enabled = false;
			equippedWeapons[currWeaponIndex].GetComponent<PickupBase>().isOnGround = true;
			//implement to enable pickupweapon
			gunScript = null;
		}
		equippedWeapons[currWeaponIndex].transform.parent = null;
		equippedWeapons[currWeaponIndex].transform.position += new Vector3(0.0f,-0.5f,0.0f);
		equippedWeapons[currWeaponIndex] = null;
		EquipNextValidWeapon();
	}
	
	public void PickupWeapon(GameObject weapon)
	{
		// Is a weapon currently equipped?
		if(equippedWeapons[currWeaponIndex] != null) // YES
		{
			// Is there a free weapon slot?
			int freeWeaponSlot =  -1;		
			for(int i=0; i < equippedWeapons.Length; ++i)
			{
				if(equippedWeapons[i] == null)
				{
					freeWeaponSlot = i;
					break;
				}
			}
			if(freeWeaponSlot >= 0) // YES
			{
				equippedWeapons[freeWeaponSlot] = weapon;
				
				equippedWeapons[freeWeaponSlot].transform.parent = meshTransform;
				equippedWeapons[freeWeaponSlot].transform.localPosition = weaponOffset;
				equippedWeapons[freeWeaponSlot].transform.localRotation = Quaternion.identity;
				equippedWeapons[freeWeaponSlot].GetComponentInChildren<CapsuleCollider>().enabled = false;
				equippedWeapons[freeWeaponSlot].SetActive(false);
				equippedWeapons[freeWeaponSlot].GetComponent<PickupBase>().isOnGround = false;
				equippedWeapons[freeWeaponSlot].GetComponent<GunBase>().enabled = true;				
				equippedWeapons[freeWeaponSlot].GetComponent<GunBase>().Reset();
			}
			else // NO
			{
				if(equippedWeapons[currWeaponIndex] != null)
				{
					Debug.Log("Dropped Weapon: " + equippedWeapons[currWeaponIndex].name);
					equippedWeapons[currWeaponIndex].GetComponentInChildren<CapsuleCollider>().enabled = true;
					equippedWeapons[currWeaponIndex].GetComponent<GunBase>().enabled = false;
					equippedWeapons[currWeaponIndex].GetComponent<PickupBase>().isOnGround = true;
					//implement to enable pickupweapon
					gunScript = null;
				}
				equippedWeapons[currWeaponIndex].transform.parent = null;
				equippedWeapons[currWeaponIndex].transform.position += new Vector3(0.0f,-0.5f,0.0f);
				equippedWeapons[currWeaponIndex] = null;
				
				equippedWeapons[currWeaponIndex] = weapon;
				
				equippedWeapons[currWeaponIndex].transform.parent = meshTransform;
				equippedWeapons[currWeaponIndex].transform.localPosition = weaponOffset;
				equippedWeapons[currWeaponIndex].transform.localRotation = Quaternion.identity;
				equippedWeapons[currWeaponIndex].GetComponentInChildren<CapsuleCollider>().enabled = false;
				equippedWeapons[currWeaponIndex].SetActive(false);
				equippedWeapons[currWeaponIndex].GetComponent<PickupBase>().isOnGround = false;
				equippedWeapons[currWeaponIndex].GetComponent<GunBase>().enabled = true;				
				equippedWeapons[currWeaponIndex].GetComponent<GunBase>().Reset();
				
				EquipWeapon(currWeaponIndex);
			}
			
		}
		else // NO
		{
			equippedWeapons[currWeaponIndex] = weapon;
			
			// TO DO : Make function for this
			equippedWeapons[currWeaponIndex].transform.parent = meshTransform;
			equippedWeapons[currWeaponIndex].transform.localPosition = weaponOffset;
			equippedWeapons[currWeaponIndex].transform.localRotation = Quaternion.identity;
			equippedWeapons[currWeaponIndex].GetComponentInChildren<CapsuleCollider>().enabled = false;
			equippedWeapons[currWeaponIndex].SetActive(false);
			equippedWeapons[currWeaponIndex].GetComponent<PickupBase>().isOnGround = false;
			equippedWeapons[currWeaponIndex].GetComponent<GunBase>().enabled = true;				
			equippedWeapons[currWeaponIndex].GetComponent<GunBase>().Reset();
			
			EquipWeapon(currWeaponIndex);
		}
	}
	
	public void InitializeGuns()
	{		
		for (int i = 0; i < equippedWeapons.Length; ++i)
		{
			SpawnWeapon(i);
		}
		EquipNextValidWeapon();
	}
}
