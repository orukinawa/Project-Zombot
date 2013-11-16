using UnityEngine;
using System.Collections;

public class GunBase : MonoBehaviour
{
	//Attributes
	public GameObject bulletMod;
	public GameObject effectMod;
	
	public float fireRate = 0.0f;
	public float reloadRate = 0.0f;
	public int maxMagazineSize = 0;
	
	public int bulletDamageBase;
	public float bulletRangeBase;
	public float bulletSpeedBase;
	
	public bool usePoolManager = false;
	
	public Transform bulletSpawnNode;
		
	//Logic
	protected GameObject bullet = null;
	protected GameObject effect = null;	
	
	protected float fireRateTimer = 0.0f;
	protected float reloadRateTimer = 0.0f;
	protected int currMagazineSize = 0;	
	
	protected int bulletDamage;
	protected float bulletRange;
	protected float bulletSpeed;
	
	protected bool canShoot = true;
	protected bool isReloading = false;
	
	public virtual void Initialize()
	{
		currMagazineSize = maxMagazineSize;
		
		//testing purposes
		bulletDamage = bulletDamageBase;
		bulletRange = bulletRangeBase;
		bulletSpeed = bulletSpeedBase;
	}
			
	public virtual void Shoot()
	{
		if(canShoot)
		{
			if(usePoolManager)
			{
				bullet = PoolManager.pools["Bullet Pool"].Spawn(bulletMod,bulletSpawnNode.position,transform.rotation) as GameObject;
			}
			else
			{
				bullet = Instantiate(bulletMod,transform.position,transform.rotation) as GameObject;
			}
						
			if(bullet != null)
			{
				Debug.Log(bullet.name + " fired!");
				effect = PoolManager.pools["Effect Pool"].Spawn(effectMod) as GameObject;
				effect.transform.parent = bullet.transform;
				effect.transform.localPosition = Vector3.zero;
				effect.transform.localRotation = Quaternion.identity;
				bullet.GetComponent<BulletBase>().InitializeBullet(bulletDamage,bulletSpeed,bulletRange,effect);
				
				// TODO: add logic to tell bullet if pool manager was used
				bullet = null;
				effect = null;
				--currMagazineSize;
				fireRateTimer = 0.0f;
				canShoot = false;
			}
			else
			{
				Debug.LogError("Bullet is null!");
			}
		}
	}
	
	public virtual void Reload()
	{
		if(currMagazineSize != maxMagazineSize && !isReloading)
		{
			currMagazineSize = 0;
		}
	}
	
	public virtual void Reset()
	{
		Debug.Log("Reset called");
		if(isReloading)
		{
			Debug.Log("reload timer reset");
			reloadRateTimer = 0.0f;
		}
	}
	
	public virtual void GunUpdate()
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
	}
}
