using UnityEngine;
using System.Collections;

public class SkillBase : MonoBehaviour
{
	public float energyCost;
	StatsCharacter mStatsCharacter;
	
	public void Initialize()
	{
		mStatsCharacter = GetComponent<StatsCharacter>();
	}
	
	public virtual void UseSkill()
	{
		
	}
}
