using UnityEngine;
using System.Collections;

public class InteractiveObjectBase : MonoBehaviour
{
	//overlaypicture
	public ParticleSystem particles;
	public bool isPermanent = false;
	
	public void Selected(bool isOn)
	{
		if(isOn)
		{
			particles.Play();
			return;
		}
		particles.Stop();
	}
	
	public virtual void ApplyEffect(GameObject obj)
	{
		
	}
	
	public void SelfDestruct()
	{
		if(isPermanent) return;
		Destroy(gameObject);
	}	
}
