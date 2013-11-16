using UnityEngine;
using System.Collections.Generic;

//! This static class holds the list of all Pools in the Scene
public class PoolManager : MonoBehaviour
{
	public static Dictionary<string,Pool> pools = new Dictionary<string, Pool>();
	
	void Awake()
	{
		//! Looks for all pools in the scene and add them to the list
		Pool[] tempPool = FindObjectsOfType(typeof(Pool)) as Pool[];
		foreach (Pool pool in tempPool)
		{
			bool poolExists = false;
			foreach(KeyValuePair<string, Pool> entry in pools)
			{
			    if(entry.Key == pool.poolName)
				{
					poolExists = true;
					Debug.Log(pool.poolName + " already exists in PoolDictionary");
					break;
				}
			}
			
			if(!poolExists)
			{
				pools.Add(pool.poolName, pool);
				Debug.Log("Added " + pool.poolName + " to PoolDictionary");
			}
		}
	}
	
	//! Adds a new pool to the poolList 
	void AddPool()
	{		
	}
	
	//! Removes pool of name " " from poolList
	void RemovePool()
	{
	}
}
