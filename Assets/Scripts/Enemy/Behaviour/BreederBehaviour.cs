using UnityEngine;
using System.Collections;

class BreederBehaviourData
{
	public SpawnManager mSpawnManagerRef;
}

public class BreederBehaviour : BehaviourBase
{
	public GameObject mSpawnPrefab;
	public GameObject mBreedEffect;
	public int mMaxSpawn;
	
	public override void Init (EnemyBase enemyBase)
	{
		BreederBehaviourData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new BreederBehaviourData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (BreederBehaviourData)enemyBase.mCustomData[this];
		}
		
		GameObject spawnManager = GameObject.FindGameObjectWithTag("SpawnManager");
		
		if(spawnManager != null)
		{
			data.mSpawnManagerRef = spawnManager.GetComponent<SpawnManager>();
		}
	}
	
	public override void Death (EnemyBase enemyBase)
	{	
		BreederBehaviourData data = (BreederBehaviourData)enemyBase.mCustomData[this];
		// no spawn manager found
		if(data.mSpawnManagerRef == null)
		{
			Debug.LogWarning("No spawn manager found for breeder behaviour, will not perform the behaviour properly!");
			return;	
		}
		
		Instantiate(mBreedEffect,enemyBase.transform.position,Quaternion.identity);
		for(int i = 0; i < mMaxSpawn; i++)
		{
			data.mSpawnManagerRef.SpawnEnemy(mSpawnPrefab,enemyBase.transform.position,enemyBase.transform.rotation,
				enemyBase.mTargetPlayer, "PURSUE");
		}
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		return Vector3.zero;
	}
}
