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
		data.mSpawnManagerRef = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
	}
	
	public override void Death (EnemyBase enemyBase)
	{
		BreederBehaviourData data = (BreederBehaviourData)enemyBase.mCustomData[this];
		Instantiate(mBreedEffect,enemyBase.transform.position,Quaternion.identity);
		for(int i = 0; i < mMaxSpawn; i++)
		{
			data.mSpawnManagerRef.SpawnEnemy(mSpawnPrefab,enemyBase.transform.position,enemyBase.transform.rotation);
		}
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		return Vector3.zero;
	}
}
