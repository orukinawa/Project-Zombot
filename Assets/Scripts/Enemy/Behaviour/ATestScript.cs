using UnityEngine;
using System.Collections;

public class ATestScript : MonoBehaviour {
	
	GameObject gameObj;
	public GameObject mPlayerObj;
	
	public GameObject mPrefab;
	
	public enum AggroType
	{
		NONE,
		AGGRO,
	}
	
	public AggroType CuurAggroType;
	SpawnManager mSpawnManager;
	
	
	void Start()
	{
		mSpawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameObj == null)
		{
			if(CuurAggroType == AggroType.NONE)
			{
				gameObj = mSpawnManager.SpawnEnemy(mPrefab,transform.position,Quaternion.identity);
			}
			else
			{
				gameObj = mSpawnManager.SpawnEnemy(mPrefab, transform.position, Quaternion.identity, mPlayerObj, "PURSUE");	
			}
			
		}
		else
		{
			if(!gameObj.activeSelf)
			{
				if(CuurAggroType == AggroType.NONE)
				{
					gameObj = mSpawnManager.SpawnEnemy(mPrefab,transform.position,Quaternion.identity);
				}
				else
				{
					gameObj = mSpawnManager.SpawnEnemy(mPrefab, transform.position, Quaternion.identity, mPlayerObj, "PURSUE");	
				}
				//Debug.LogError("HAHAH");
			}
		}
	}
}
