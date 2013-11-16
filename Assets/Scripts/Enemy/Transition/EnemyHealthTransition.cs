using UnityEngine;
using System.Collections;

public class EnemyHealthTransition : Transition
{
	public enum STATUS
	{
		ABOVE,
		BELOW,
	}
	
	public STATUS mStatus;
	public float mHealthPercentage;
	
	public override bool VerifyTransition(StateManager context)
	{
		float maxHealth = (float)context.gameObject.GetComponent<StatsEnemy>().maxHealth;
		float currentHealth =(float)context.gameObject.GetComponent<StatsEnemy>().currentHealth;
		
		if(mStatus == STATUS.ABOVE)
		{
			if(currentHealth > (maxHealth * mHealthPercentage) / 100.0f)
			{
				return true;
			}
		}
		else if(mStatus == STATUS.BELOW)
		{
			if(currentHealth < (maxHealth * mHealthPercentage) / 100.0f)
			{
				return true;
			}
		}
		
		return false;
	}
}
