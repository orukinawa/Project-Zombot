using UnityEngine;
using System.Collections;

public class StatsBase : MonoBehaviour
{
	public float maxHealth;
	//public float defaultMoveSpeed;
	
	public float currentHealth; // should be protected, set to public because of Glenn's commando healthbar
	//public float currentMoveSpeed;	// should be protected, set to public for testing
	
	public virtual void initializeStats()
	{
		currentHealth = maxHealth;
		//currentMoveSpeed = defaultMoveSpeed;
	}
	
	public virtual void ApplyDamage(float damage, GameObject player = null)
	{
		currentHealth += damage;
		if(currentHealth <= 0)
		{
			currentHealth = 0.0f;
			SelfDestruct();
		}
		if(currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
	}
	
	public virtual void SelfDestruct()
	{
		Destroy (gameObject);
	}	
	
	public bool isFullHealth()
	{
		if(currentHealth == maxHealth)
		{
			return true;
		}
		return false;
	}
	
	public virtual void ApplySlow(float multiplier)
	{		
	}
	
	public virtual void RestoreMoveSpeed()
	{	 	
	}	
	
	public virtual void Freeze()
	{		
	}
	
	public virtual void RemoveFreeze()
	{		
	}
	
	public virtual void Immobilize()
	{		
	}
	
	public virtual void RemoveImmobilize()
	{		
	}
	
	public virtual void InvertControl()
	{
	}
	
	public virtual void RevertControl()
	{
	}	
}
