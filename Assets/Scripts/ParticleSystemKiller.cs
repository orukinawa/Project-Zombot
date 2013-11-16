using UnityEngine;
using System.Collections;

public class ParticleSystemKiller : MonoBehaviour
{
	ParticleSystem mParticleSystem;
	
	void Awake()
	{
		mParticleSystem = particleSystem;
	}
	
	void OnDisable()
	{
		mParticleSystem.Clear();
	}
}
