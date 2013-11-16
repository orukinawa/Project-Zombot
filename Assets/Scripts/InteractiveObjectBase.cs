using UnityEngine;
using System.Collections;

public class InteractiveObjectBase : MonoBehaviour
{
	//overlaypicture
	public ParticleSystem particles;
	
	public void Selected(bool isOn)
	{
		if(isOn)
		{
			particles.renderer.enabled = true;
			return;
		}
		particles.renderer.enabled = false;
	}
	
	public virtual void ApplyEffect(GameObject obj)
	{
		
	}
	
	public void SelfDestruct()
	{
		Destroy(gameObject);
	}	
}
