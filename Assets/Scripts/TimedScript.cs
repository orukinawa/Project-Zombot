using UnityEngine;
using System.Collections;

public class TimedScript : MonoBehaviour
{
	public float timeAlive;
	float aliveTimer;
	public string poolName;
	
	public void Initialize()
	{
		aliveTimer = 0.0f;
	}
	
	void Update()
	{
		aliveTimer += Time.deltaTime;
		if(aliveTimer > timeAlive)
		{
			aliveTimer = 0.0f;
			PoolManager.pools[poolName].DeSpawn(gameObject);
		}
	}
}
