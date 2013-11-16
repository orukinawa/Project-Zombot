using UnityEngine;
using System.Collections;

public class EffectPooler : MonoBehaviour
{
	ParticleSystem mParticleSystem;
	float duration;
	float timer = 0.0f;
	
	void Awake()
	{
		mParticleSystem = particleSystem;
		duration = mParticleSystem.duration;
	}
	
	void Update()
	{
		timer += Time.deltaTime;
		if(timer > duration)
		{
			timer = 0.0f;
			mParticleSystem.Clear();
			PoolManager.pools["HitEffect Pool"].DeSpawn(gameObject);
		}		
	}
}
