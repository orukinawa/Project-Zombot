using UnityEngine;
using System.Collections;

public class InventoryManager : MonoBehaviour
{
	// WEAPON
	//public Transform weaponPosition; // where the character holds the weapon
	public Transform meshTransform; // reference to mesh transform	
	public GameObject defaultWeapon;
	GameObject equippedWeapon;
	GunBase gunScript;
	public GunBase GunScript{ get{ return gunScript;} }
	public Vector3 weaponOffset;
	public bool isFrozen = false;
	MovementController mController;
	
	// ITEM
//	public GameObject defaultItem;
//	GameObject equippedItem;	
	
	void Start()
	{
		InitializeInventory();
	}
	
	public void InitializeInventory()
	{
		// WEAPON
		if(equippedWeapon != null)
		{
			Destroy(equippedWeapon);
		}
		equippedWeapon = SpawnWeapon(defaultWeapon);
		mController = GetComponent<MovementController>();
	}
	
	public void ShootWeapon()
	{
		if(mController.currMotortype == MovementController.MOTORTYPE.FROZEN)
		{
			return;
		}
		if(gunScript != null)
		{
			gunScript.Shoot();
		}
	}
	
	public void ReloadWeapon()
	{
		if(mController.currMotortype == MovementController.MOTORTYPE.FROZEN)
		{
			return;
		}
		if(gunScript != null)
		{
			gunScript.Reload();
		}
	}
	
	GameObject SpawnWeapon(GameObject weapon)
	{
		if(weapon == null)
		{
			Debug.LogWarning("Default weapon is null");
			return null;
		}
		Debug.Log("Spawned Weapon: " + weapon.name);
		GameObject tempGameObject = Instantiate(weapon, transform.position, transform.rotation) as GameObject;
		tempGameObject.transform.parent = meshTransform;
		tempGameObject.transform.localPosition = weaponOffset;
		tempGameObject.name = weapon.name;
		gunScript = tempGameObject.GetComponent<GunBase>();
		return tempGameObject;
	}
	
	public void ReplaceWeapon(GameObject weapon)
	{
		if(equippedWeapon != null)
		{
			GameObject tempObject = equippedWeapon;
			equippedWeapon = null;
			Destroy(tempObject);
		}
		equippedWeapon = SpawnWeapon(weapon);
	}
}
