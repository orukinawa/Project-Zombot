using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunUpgradable : GunBase
{
	//! Number of bullets per wave
	int bulletCount;
	public int defaultBulletCount;
	
	//! Angle of the spread
	public float spread;
	
	//! List to store the bullets in the wave before shooting
	protected List<GameObject> spawnList = new List<GameObject>();
	
	//! Cached Quaternion for spread calculations
	Quaternion rot;
	
	//! Number of waves to fire
	int wavesToFire;
	public int defaultWavesToFire;
	
	//! Time interval between waves
	public float waveBurstInterval;
	
	//! Timer for calculation
	private float waveBurstIntervalTimer;
	
	//! Counter to know how many waves are left to shoot
	private int wavesFired;
	
	//! Flag to check if the gun is shooting
	private bool isShooting = false;
	
	void Start()
	{
		Initialize();
	}
	
	void Update()
	{
		GunUpdate();
	}
	
	public override void Initialize ()
	{
		base.Initialize ();
		bulletCount = defaultBulletCount;
		wavesToFire = defaultWavesToFire;
		waveBurstIntervalTimer = waveBurstInterval*2;
		wavesFired = 0;
		isShooting = false;
	}
	
	public override void GunUpdate ()
	{
		if(!isReloading)
		{
			if(!canShoot)
			{
				if(fireRateTimer < fireRate)
				{
					fireRateTimer += Time.deltaTime;
				}
				else
				{
					canShoot = true;
				}
			}
			
			if(currMagazineSize <= 0)
			{
				isReloading = true;
				canShoot = false;
			}
		}
		else
		{
			reloadRateTimer += Time.deltaTime;
			if(reloadRateTimer > reloadRate)
			{
				fireRateTimer = 0.0f;
				currMagazineSize = maxMagazineSize;
				reloadRateTimer = 0.0f;
				isReloading = false;
				canShoot = true;
			}
		}
		
		if(isShooting)
		{
			if(wavesFired < wavesToFire)
			{
				if(waveBurstIntervalTimer > waveBurstInterval)
				{					
					spawnList = PoolManager.pools["Bullet Pool"].SpawnArray(bulletMod, bulletCount);
					Quaternion tempRot = transform.rotation;
					
					for(int i=0; i < bulletCount; ++i)
					{
						if(bulletCount == 1) rot = tempRot;
						else rot = tempRot * Quaternion.Euler(0,(-spread/2) + ((spread/(bulletCount-1)*1.0f)*i),0);
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
							//Debug.Log(bullet.name + " fired!");
							bullet.GetComponent<BulletBase>().InitializeBullet(bulletSpeed,bulletRange,bulletDamage,currentEffect,mStat);
							bullet = null;
							// TODO: add logic to tell bullet if pool manager was used							
						}
						else
						{
							Debug.LogError("Bullet is null!");
						}
					}
					++wavesFired;
					waveBurstIntervalTimer = 0.0f;
				}
				else
				{
					waveBurstIntervalTimer += Time.deltaTime;
				}
			}
			else
			{
				wavesFired = 0;
				waveBurstIntervalTimer = waveBurstInterval*2;
				--currMagazineSize;
				fireRateTimer = 0.0f;
				canShoot = false;
				isShooting = false;				
				bullet = null;
				spawnList.Clear();
			}
		}
	}
	
	public override void Shoot ()
	{
		if(canShoot)
		{
			isShooting = true;
		}
	}
	
	public override void SetBulletsPerWave (CountModType mType, int newBulletCount)
	{
		if(mType == GunBase.CountModType.SET) bulletCount = newBulletCount;
		else if(mType == GunBase.CountModType.INCREMENT) bulletCount += newBulletCount;
		else
		{
			bulletCount -= newBulletCount;
			if(bulletCount < 1) bulletCount = 1;
		}
	}
	
	public override void SetWavesPerShot (CountModType mType, int newWaveCount)
	{
		if(mType == GunBase.CountModType.SET) wavesToFire = newWaveCount;
		else if(mType == GunBase.CountModType.INCREMENT) wavesToFire += newWaveCount;
		else
		{
			wavesToFire -= newWaveCount;
			if(wavesToFire < 1) wavesToFire = 1;
		}
	}
	
	public override bool isGunShooting()
	{
		return isShooting;
	}
}
