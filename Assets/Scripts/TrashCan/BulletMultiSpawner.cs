//using UnityEngine;
//using System.Collections;
//
//public class BulletMultiSpawner : BulletBase
//{
//	public GameObject spawnBulletPrefab;
//	private GameObject spawnBullet;
//	
//	public int bulletCount;
//	public float spread;
//	
//	Quaternion rot;	
//	
//	void OnEnable()
//	{
//		Debug.Log(name + " called OnEnable!");
//	}
//	
//	void Update ()
//	{
//		// Moved this chunk of code to Update() because OnEnable() gets called even if the object is not active
//		for(int i=0; i < bulletCount; ++i)
//		{
//			rot = transform.rotation * Quaternion.Euler(0,(-spread/2) + ((spread/(bulletCount-1)*1.0f)*i),0);
//			//spawnBullet = Instantiate(spawnBulletPrefab,transform.position,rot) as GameObject;
//			spawnBullet = PoolManager.pools["Pool A"].Spawn(spawnBulletPrefab,transform.position,rot) as GameObject;
//			spawnBullet.GetComponent<BulletBase>().InitializeBullet(damage,speed,range);
//		}		
//		SelfDestruct();	
//	}
//	
//	void SelfDestruct()
//	{
//		Debug.Log(gameObject.name + " selfdestructed!");	
//		PoolManager.pools["Pool A"].DeSpawn(gameObject);
//	}
//}
