using UnityEngine;
using System.Collections.Generic;

public class GunShotgun : GunBase
{
	public int bulletCount;
	public float spread;
	protected List<GameObject> spawnList = new List<GameObject>();
	
	Quaternion rot;
	
	void Start()
	{
		base.Initialize();
	}
	
	void Update()
	{
		base.GunUpdate();
	}
	
	public override void Shoot()
	{
		if(canShoot)
		{
			spawnList = PoolManager.pools["Bullet Pool"].SpawnArray(bulletMod, bulletCount);
			Quaternion tempRot = transform.rotation;
			for(int i=0; i < bulletCount; ++i)
			{
				//TODO: try rot = base then increment each time in loop
				rot = tempRot * Quaternion.Euler(0,(-spread/2) + ((spread/(bulletCount-1)*1.0f)*i),0);
				if(usePoolManager)
				{
					bullet = spawnList[i];
					bullet.transform.position = bulletSpawnNode.position;
					bullet.transform.rotation = rot;
				}
				else
				{
					bullet = Instantiate(bulletMod,transform.position,transform.rotation) as GameObject;
				}
							
				if(bullet != null)
				{
					// TODO: split expected damage evenly through bullets (divide by count)
					bullet.GetComponent<BulletBase>().InitializeBullet(bulletSpeed,bulletRange,bulletDamage,currentEffect,mStat);
				}
				else
				{
					Debug.LogError("Bullet is null!");
				}
			}
			bullet = null;
			spawnList.Clear();
			--currMagazineSize;
			fireRateTimer = 0.0f;
			canShoot = false;
		}
	}
}
