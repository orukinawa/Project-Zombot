using UnityEngine;
using System.Collections;

public class GunBase : MonoBehaviour
{
	public enum CountModType
	{
		SET,
		INCREMENT,
		DECREMENT
	}	
	
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
	
	public string defaultEffectModName;
	protected string currentEffectModName;
	protected EffectBase currentEffect;
	
	public StatTracker mStat;
	
	public virtual void Initialize()
	{
		currMagazineSize = maxMagazineSize;
		SetEffectMod(defaultEffectModName);
		
		//EffectBase mEffectBase = effectMod.GetComponent<EffectBase>();
//		currentEffect = BulletEffectManager.Instance.getEffect(currentEffectModName);
//		
//		// Sets the gun attributes by combining the gun + mods
//		bulletDamage = bulletDamageBase * (mBulletBase.damageModifierPercent/100.0f) * (currentEffect.damageModifierPercent/100.0f);
//		bulletRange = bulletRangeBase * (mBulletBase.rangeModifierPercent/100.0f) * (currentEffect.rangeModifierPercent/100.0f);
//		bulletSpeed = bulletSpeedBase * (mBulletBase.speedModifierPercent/100.0f) * (currentEffect.speedModifierPercent/100.0f);
		
//		if(effect != null)
//		{
//			GameObject temp = effect;
//			effect = null;
//			Destroy(temp);
//		}		
		//effect = Instantiate(effectMod,transform.position,transform.rotation) as GameObject;
		//effect.GetComponent<EffectBase>().Initialize(bulletDamage);
		//effect.transform.parent = this.transform;		
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
				bullet.GetComponent<BulletBase>().InitializeBullet(bulletSpeed,bulletRange,bulletDamage,currentEffect, mStat);
				
				// TODO: add logic to tell bullet if pool manager was used
				//bullet = null;
				//effect = null;
				//--currMagazineSize;
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
	
	public virtual void SetBulletsPerWave(CountModType mType, int bulletCount)
	{
	}
	
	public virtual void SetWavesPerShot(CountModType mType, int waveCount)
	{
	}
	
	public virtual bool isGunShooting()
	{
		return true;
	}
	
	public virtual bool SetEffectMod(string effectName)
	{
		if(isGunShooting()) return false;
		currentEffect = BulletEffectManager.Instance.getEffect(effectName);
		AttributeInit();
		return true;
	}
	
	public virtual bool SetBulletMod(GameObject newBulletMod)
	{
		if(isGunShooting()) return false;
		bulletMod = newBulletMod;
		AttributeInit();
		return true;
	}
	
	protected void AttributeInit()
	{
		BulletBase mBulletBase = bulletMod.GetComponent<BulletBase>();
		bulletDamage = bulletDamageBase * (mBulletBase.damageModifierPercent/100.0f) * (currentEffect.damageModifierPercent/100.0f);
		bulletRange = bulletRangeBase * (mBulletBase.rangeModifierPercent/100.0f) * (currentEffect.rangeModifierPercent/100.0f);
		bulletSpeed = bulletSpeedBase * (mBulletBase.speedModifierPercent/100.0f) * (currentEffect.speedModifierPercent/100.0f);
	}
	
//	void OnGUI()
//	{		
//		GUILayout.BeginHorizontal();
//		GUILayout.Space(20f);
//		GUILayout.BeginVertical();
//		GUILayout.Space(250f);
//		if(isReloading)
//		{
//			GUILayout.Label("Bullets: Reloading..." + (reloadRate-reloadRateTimer).ToString("0.00"));
//			return;
//		}
//		GUILayout.Label("Bullets: " + currMagazineSize + " / " + maxMagazineSize);
//		GUILayout.EndVertical();
//		GUILayout.EndHorizontal();
//	}
}
