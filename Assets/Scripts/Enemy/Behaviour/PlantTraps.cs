using UnityEngine;
using System.Collections;

class PlantTrapsData
{
	public float mPlantingTimer;
	public int mNumTrapSpawned;
}

public class PlantTraps : BehaviourBase
{
	public int mMaxTrap;
	public GameObject mTrapPrefab;
	public float mPlantingDuration;
	
	public override void Init (EnemyBase enemyBase)
	{
		PlantTrapsData data;
		if(!enemyBase.mCustomData.ContainsKey(this))
		{
			data = new PlantTrapsData();
			enemyBase.mCustomData[this] = data;
		}
		else
		{
			data = (PlantTrapsData)enemyBase.mCustomData[this];
		}
		data.mPlantingTimer = 0.0f;
	}
	
	public override void Death (EnemyBase enemyBase)
	{
		PlantTrapsData data = (PlantTrapsData)enemyBase.mCustomData[this];
		data.mNumTrapSpawned = 0;
	}
	
	public override Vector3 UpdateBehaviour (EnemyBase enemyBase)
	{
		PlantTrapsData data = (PlantTrapsData)enemyBase.mCustomData[this];
		
		data.mPlantingTimer += Time.deltaTime;
		//! do planting animation here
		
		if(data.mPlantingTimer > mPlantingDuration)
		{
			//! do planting success animation here
			if(data.mNumTrapSpawned < mMaxTrap)
			{
				GameObject trapObj = (GameObject) Instantiate(mTrapPrefab, enemyBase.transform.position, Quaternion.identity);
				trapObj.GetComponent<TrapBase>().GetTrapOwner(enemyBase,this);
				data.mNumTrapSpawned += 1;
			}
			//! change state
			ExecuteTransition(enemyBase);
		}
		
		return Vector3.zero;
	}
}
