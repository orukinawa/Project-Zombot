using UnityEngine;
using System.Collections;

public class InteractiveObjectEffectMod : InteractiveObjectBase
{
	public string effectName;
	
	public override void ApplyEffect (GameObject obj)
	{
		GunBase tempGun = obj.GetComponent<InventoryManager>().GunScript;
		if(tempGun.isGunShooting()) return;
		tempGun.SetEffectMod(effectName);
		SelfDestruct();
	}
}
