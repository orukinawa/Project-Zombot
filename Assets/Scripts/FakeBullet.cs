using UnityEngine;
using System.Collections;

public class FakeBullet : MonoBehaviour
{
	float lifeTime = 0.0f;
	float currTime = 0.0f;
	float speed = 0.0f;
	Transform trans;
	
	public void Initialize(float velocity, float distance)
	{
		trans = transform;
		speed = velocity;
		currTime = 0.0f;
		lifeTime = distance/speed;
	}
	
	void Update()
	{
		trans.position += trans.forward * speed * Time.deltaTime;
		currTime += Time.deltaTime;
		if(currTime > lifeTime)
		{
			PoolManager.pools["Bullet Pool"].DeSpawn(gameObject);
		}
	}
}
