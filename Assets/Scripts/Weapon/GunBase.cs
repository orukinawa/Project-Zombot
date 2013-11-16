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
	
	public float bulletDamageBase;
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
	
	protected float bulletDamage;
	protected float bulletRange;
	protected float bulletSpeed;
	
	protected bool canShoot = true;
	protected bool isReloading = false;
	
	public virtual void Initialize()
	{
		currMagazineSize = maxMagazineSize;
		
		BulletBase mBulletBase = bulletMod.GetComponent<BulletBase>();
		EffectBase mEffectBase = effectMod.GetComponent<EffectBase>();
		
		// Sets the gun attributes by combining the gun + mods
		bulletDamage = bulletDamageBase * (mBulletBase.damageModifierPercent/100.0f) * (mEffectBase.damageModifierPercent/100.0f);
		bulletRange = bulletRangeBase * (mBulletBase.rangeModifierPercent/100.0f) * (mEffectBase.rangeModifierPercent/100.0f);
		bulletSpeed = bulletSpeedBase * (mBulletBase.speedModifierPercent/100.0f) * (mEffectBase.speedModifierPercent/100.0f);
		
		if(effect != null)
		{
			GameObject temp = effect;
			effect = null;
			Destroy(temp);
		}		
		effect = Instantiate(effectMod,transform.position,transform.rotation) as GameObject;
		effect.GetComponent<EffectBase>().Initialize(bulletDamage);
		effect.transform.parent = this.transform;		
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
				//Debug.Log(bullet.name + " fired!");
//				effect = PoolManager.pools["Effect Pool"].Spawn(effectMod) as GameObject;
//				effect.transform.parent = bullet.transform;
//				effect.transform.localPosition = Vector3.zero;
//				effect.transform.localRotation = Quaternion.identity;
				bullet.GetComponent<BulletBase>().InitializeBullet(bulletSpeed,bulletRange,effect);
				
				// TODO: add logic to tell bullet if pool manager was used
				//bullet = null;
				//effect = null;
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
		//Debug.Log("Reset called");
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
	
	void OnGUI()
	{		
		GUILayout.BeginHorizontal();
		GUILayout.Space(20f);
		GUILayout.BeginVertical();
		GUILayout.Space(250f);
		if(isReloading)
		{
			GUILayout.Label("Bullets: Reloading..." + (reloadRate-reloadRateTimer).ToString("0.00"));
			return;
		}
		GUILayout.Label("Bullets: " + currMagazineSize + " / " + maxMagazineSize);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}
