using UnityEngine;
using System.Collections;

public class InteractiveObjectGunMod : InteractiveObjectBase
{
	public enum modType
	{
		SHOTGUN,
		BURST
	}
	
	public modType mModType;
	public GunBase.CountModType countType;
	public int count;
	
	public override void ApplyEffect (GameObject obj)
	{
		GunBase tempGun = obj.GetComponent<InventoryManager>().GunScript;
		if(tempGun.isGunShooting()) return;
		if(mModType == modType.SHOTGUN) tempGun.SetBulletsPerWave(countType, count);
		else tempGun.SetWavesPerShot(countType, count);
		SelfDestruct();
	}
}
