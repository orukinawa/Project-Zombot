using UnityEngine;
using System.Collections.Generic;

public class Pool : MonoBehaviour
{
	[System.Serializable]
	public class SubPool
	{
		public GameObject prefab;
		public int preloadAmount;
		public bool cloneLimit = false;
		public int cloneLimitAmount;
		//[HideInInspector]
		public List<GameObject> prefabList = new List<GameObject>();
		public int instantiateCalls = 0;
		public int destroyCalls = 0;
		public bool EnableDebug = false;
	}
	
	public string poolName ="";
	public List<SubPool> subPoolList = new List<SubPool>();
	
	void Start()
	{
		poolName = this.name;
		
		GameObject tempGameObject;
		for(int i=0; i<subPoolList.Count; ++i)
		{
//			if(subPoolList[i].prefab.GetComponent<PoolableObject>() == null)
//			{
//				Debug.LogWarning(subPoolList[i].prefab.name + " is not a poolable object. SubPool removed from List");
//				subPoolList.RemoveAt(i);
//				--i;
//				continue;
//			}
			if(subPoolList[i].cloneLimit)
			{
				if(subPoolList[i].cloneLimitAmount < subPoolList[i].preloadAmount)
				{
					Debug.LogWarning("Warning: CloneLimitAmount is less than PreloadAmount.\n PreloadAmount will be set to: " + subPoolList[i].cloneLimitAmount);
					subPoolList[i].preloadAmount = subPoolList[i].cloneLimitAmount;
				}
			}
			for(int j = 0; j < subPoolList[i].preloadAmount; ++j)
			{
				tempGameObject = Instantiate(subPoolList[i].prefab, transform.position, transform.rotation) as GameObject;
				++subPoolList[i].instantiateCalls;
				tempGameObject.SetActive(false);
				tempGameObject.transform.parent = transform;
				tempGameObject.name += j.ToString("000");
				if(!tempGameObject.GetComponent<PoolableObject>())
				{
					tempGameObject.AddComponent<PoolableObject>();
				}
				tempGameObject.GetComponent<PoolableObject>().objectPrefab = subPoolList[i].prefab;
				subPoolList[i].prefabList.Add(tempGameObject);
			}
		}
	}
	
	public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		foreach(SubPool mSubPool in subPoolList)
		{
			if(mSubPool.prefab == prefab)
			{
				if(mSubPool.prefabList.Count == 0)
				{
					// Add new clone to pool first
					GameObject tempGameObject;
					tempGameObject = Instantiate(mSubPool.prefab, transform.position, transform.rotation) as GameObject;
					++mSubPool.instantiateCalls;
					tempGameObject.SetActive(false);
					tempGameObject.transform.parent = transform;
					++mSubPool.preloadAmount; // Use the preloadAmount to add the number suffix for the new clone
					tempGameObject.name += (mSubPool.preloadAmount - 1).ToString("000");
					if(!tempGameObject.GetComponent<PoolableObject>())
					{
						tempGameObject.AddComponent<PoolableObject>();
					}
					tempGameObject.GetComponent<PoolableObject>().objectPrefab = mSubPool.prefab;
					mSubPool.prefabList.Add(tempGameObject);					
					if(mSubPool.EnableDebug) Debug.Log(this.name + " is empty! Adding " + tempGameObject.name + " to pool");
				}
				
				//Spawn Object
				int lastIndex = mSubPool.prefabList.Count-1;
				GameObject tempObject = mSubPool.prefabList[lastIndex];
				if(tempObject == null)
				{
					// HACK: Sometimes the last index of the list gets set to null. In that case, Instantiate new clone and set it to that index
					mSubPool.prefabList.RemoveAt(lastIndex);					
					tempObject = Instantiate(mSubPool.prefab, transform.position, transform.rotation) as GameObject;
					++mSubPool.instantiateCalls;
					tempObject.SetActive(false);
					tempObject.transform.parent = transform;
					++mSubPool.preloadAmount; // Use the preloadAmount to add the number suffix for the new clone
					tempObject.name += (mSubPool.preloadAmount - 1).ToString("000");
					if(!tempObject.GetComponent<PoolableObject>())
					{
						tempObject.AddComponent<PoolableObject>();
					}
					tempObject.GetComponent<PoolableObject>().objectPrefab = mSubPool.prefab;
					mSubPool.prefabList.Add(tempObject);					
					Debug.LogWarning("Last index returned null. " + tempObject.name + " instantiated to fill in the gap");					
				}
				tempObject.transform.parent = null;
				tempObject.transform.position = position;
				tempObject.transform.rotation = rotation;
				tempObject.SetActive(true);
				mSubPool.prefabList.RemoveAt(lastIndex);
				if(mSubPool.EnableDebug) Debug.Log(tempObject.name + " left the Pool!");
				return tempObject;
			}
		}
		Debug.LogError("Prefab not found in " + this.name + " !");
		return null;
	}
	
	public GameObject Spawn(GameObject prefab)
	{
		return Spawn(prefab, Vector3.zero, Quaternion.identity);
	}
	
	public List<GameObject> SpawnArray(GameObject prefab, int count)
	{
		foreach(SubPool mSubPool in subPoolList)
		{
			if(mSubPool.prefab == prefab)
			{
				while(mSubPool.prefabList.Count < count)
				{
					// Add new clones to pool until the required count is reached
					GameObject tempGameObject;
					tempGameObject = Instantiate(mSubPool.prefab, transform.position, transform.rotation) as GameObject;
					++mSubPool.instantiateCalls;
					tempGameObject.SetActive(false);
					tempGameObject.transform.parent = transform;
					++mSubPool.preloadAmount; // Use the preloadAmount to add the number suffix for the new clone
					tempGameObject.name += (mSubPool.preloadAmount - 1).ToString("000");
					if(!tempGameObject.GetComponent<PoolableObject>())
					{
						tempGameObject.AddComponent<PoolableObject>();
					}
					tempGameObject.GetComponent<PoolableObject>().objectPrefab = mSubPool.prefab;
					mSubPool.prefabList.Add(tempGameObject);					
					if(mSubPool.EnableDebug) Debug.Log(this.name + " Not enough clones in pool! Adding " + tempGameObject.name + " to pool");
				}
				
				//Spawn Object
				int lastIndex = mSubPool.prefabList.Count-1;
				GameObject tempObject = mSubPool.prefabList[lastIndex];
				if(tempObject == null)
				{
					// HACK: Sometimes the last index of the list gets set to null. In that case, Instantiate new clone and set it to that index
					mSubPool.prefabList.RemoveAt(lastIndex);					
					tempObject = Instantiate(mSubPool.prefab, transform.position, transform.rotation) as GameObject;
					++mSubPool.instantiateCalls;
					tempObject.SetActive(false);
					tempObject.transform.parent = transform;
					++mSubPool.preloadAmount; // Use the preloadAmount to add the number suffix for the new clone
					tempObject.name += (mSubPool.preloadAmount - 1).ToString("000");
					if(!tempObject.GetComponent<PoolableObject>())
					{
						tempObject.AddComponent<PoolableObject>();
					}
					tempObject.GetComponent<PoolableObject>().objectPrefab = mSubPool.prefab;
					mSubPool.prefabList.Add(tempObject);					
					Debug.LogWarning("Last index returned null. " + tempObject.name + " instantiated to fill in the gap");					
				}
				
				List<GameObject> spawnList = new List<GameObject>();
				
				for(int i=0; i<count; ++i)
				{
					spawnList.Add(mSubPool.prefabList[lastIndex]);
					mSubPool.prefabList[lastIndex].transform.parent = null;
					mSubPool.prefabList[lastIndex].SetActive(true);
					mSubPool.prefabList.RemoveAt(lastIndex);
					if(mSubPool.EnableDebug) Debug.Log(tempObject.name + " left the Pool in a List!");
					--lastIndex;
				}
				return spawnList;
			}
		}
		Debug.LogError("Prefab not found in " + this.name + " !");
		return null;
	}
	
	public void DeSpawn(GameObject prefab)
	{
		PoolableObject tempPoolableObject = prefab.GetComponent<PoolableObject>();
		if(tempPoolableObject == null)
		{
			Debug.LogWarning(prefab.name + " is not a PoolableObject and will be Destroyed instead");
			Destroy(prefab);
			return;
		}
		
		foreach(SubPool mSubPool in subPoolList)
		{			
			if(mSubPool.prefab == tempPoolableObject.objectPrefab)
			{
				if(mSubPool.cloneLimit)
				{
					if(mSubPool.prefabList.Count >=  mSubPool.cloneLimitAmount)
					{
						if(mSubPool.EnableDebug) Debug.Log("CloneLimit reached! " + prefab.name + " will be destroyed");
						Destroy(prefab);
						++mSubPool.destroyCalls;
						return;
					}
				}
				prefab.transform.parent = transform;
				//prefab.transform.localPosition = Vector3.zero;
				prefab.transform.localRotation = Quaternion.identity;
				mSubPool.prefabList.Add(prefab);
				prefab.SetActive(false);
				if(mSubPool.EnableDebug) Debug.Log(prefab.name + " returned to Pool!");
				return;
			}
		}
		Debug.LogWarning("No Pool containing " + prefab.name + " was found! " + prefab.name + " will be Destroyed");
	}
}
