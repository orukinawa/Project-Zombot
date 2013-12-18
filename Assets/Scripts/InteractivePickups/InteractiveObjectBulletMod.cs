using UnityEngine;
using System.Collections;

public class InteractiveObjectBulletMod : InteractiveObjectBase
{
	public GameObject bulletMod;
	
	public override void ApplyEffect (GameObject obj)
	{
		GunBase tempGun = obj.GetComponent<InventoryManager>().GunScript;
		if(tempGun.isGunShooting()) return;
		tempGun.SetBulletMod(bulletMod);
		SelfDestruct();
	}
}
