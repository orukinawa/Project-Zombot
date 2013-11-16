using UnityEngine;
using System.Collections;

public class BulletRaycast : BulletBase
{
	public GameObject fakeBullet;
	public float offsetPos = -5.0f;
	
	void Update()
	{
		RaycastHit hit;
		float distance = bulletRange;
		if(Physics.Raycast(transform.position + (transform.forward * offsetPos), transform.forward, out hit, bulletRange, ~(1 << LayerMask.NameToLayer("Player"))))
		{
			effectPrefab.GetComponent<EffectBase>().ApplyEffect(hit.collider,gameObject, hit.point);
			distance = Vector3.Distance(transform.position, hit.point);
			//Debug.Log(hit.collider.name);
		}
		GameObject bullet = PoolManager.pools["Bullet Pool"].Spawn(fakeBullet, transform.position, transform.rotation);
		//Debug.Log("Spawned!");
		bullet.GetComponent<FakeBullet>().Initialize(bulletSpeed, distance);
		SelfDestruct();
	}
	
	public override void InitializeBullet (float speed, float range, GameObject effect)
	{
		base.InitializeBullet (speed, range, effect);
	}
	
	public override void SelfDestruct ()
	{
		PoolManager.pools["Bullet Pool"].DeSpawn(gameObject);
	}
	
	
}