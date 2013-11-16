using UnityEngine;
using System.Collections;

public class TempProjectile : MonoBehaviour {
	
	int mBulletDmg;
	float mSpeed;
	Vector3 mDirection;
	float mLifeTimer = 0.0f;
	
	// Use this for initialization
	
	public void InitProjectile(int damage, float speed, Vector3 dir, float duration)
	{
		mBulletDmg = damage;
		mSpeed = speed;
		mDirection = dir;
		mLifeTimer = duration;
	}
	
	// Update is called once per frame
	void Update ()
	{
		mLifeTimer -= Time.deltaTime;
		if(mLifeTimer <= 0.0f)
		{
			Destroy(gameObject);
			return;
		}
		
		transform.position += (mDirection.normalized * mSpeed) * Time.deltaTime;
	}
	
	void OnTriggerEnter(Collider other)
	{
		//Debug.Log(other.gameObject);
		if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			other.GetComponentInChildren<StatsCharacter>().currentHealth -= mBulletDmg;
			Destroy(gameObject);
		}
		if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			Destroy(gameObject);
		}
	}
}
