using UnityEngine;
using System.Collections.Generic;

public class PoolManager
{
	//! Memory pool
	List<GameObject> mPoolGameObject = new List<GameObject>();
	//! Game Object as a dictionary
	GameObject[] mGameObjectList;
	//! Maximum creation of the type
	int mMaxTypePoolCount = 5;
	//! Maximum of the object that the pool will handle
	int mTotalMaxPoolCount = 50;
	//! name of poolManager
	string mName;
	
	public PoolManager(GameObject[] gameObjectList, string name)
	{
		mGameObjectList = gameObjectList;
		mName = name;
		for(int i = 0; i < gameObjectList.Length; i++)
		{
			int poolCount = mMaxTypePoolCount;
			while(poolCount > 0)
			{
				GameObject obj = (GameObject)GameObject.Instantiate(gameObjectList[i]);
				obj.GetComponent<LevelBlock>().SetPoolId(i);
				obj.name = obj.name + i;
				obj.SetActive(false);
				mPoolGameObject.Add(obj);
				poolCount -= 1;
			}
		}
		
		//Debug.LogError(mPoolGameObject.Count);
	}
	
	//! Gets the requested object from the pool
	public GameObject GetPoolObject(int poolId)
	{
		//! search for any non active game object with the specific poolID
		foreach(GameObject gameObj in mPoolGameObject)
		{
			if(gameObj.GetComponent<LevelBlock>().GetPoolId() == poolId && gameObj.activeInHierarchy == false)	
			{
				Debug.Log(mName + " Pool is used");
				gameObj.SetActive(true);
				return gameObj;
			}
		}
		
		Debug.Log(mName + " Creation is used");
		//! create new obj if not found
		GameObject obj = (GameObject)GameObject.Instantiate(mGameObjectList[poolId]);
		obj.GetComponent<LevelBlock>().SetPoolId(poolId);
		obj.name = obj.name + poolId;
		
		
		return obj;
	}
	
	//! Returns object back to the pool and set it inactive
	public void ReturnPoolObject(GameObject gameObject)
	{
		if(mPoolGameObject.Count <= mTotalMaxPoolCount)
		{
			Debug.Log(mName + " pool size: " + mPoolGameObject.Count);
			gameObject.transform.rotation = Quaternion.identity;
			gameObject.transform.position = Vector3.zero;
			gameObject.SetActive(false);
			if(!mPoolGameObject.Contains(gameObject))
			{
				mPoolGameObject.Add(gameObject);
			}
		}
		else
		{
			Debug.Log(mName + " Destroy used");
			GameObject.Destroy(gameObject);
		}
	}
}
