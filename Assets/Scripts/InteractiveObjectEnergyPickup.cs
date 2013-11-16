using UnityEngine;
using System.Collections;

public class InteractiveObjectEnergyPickup : InteractiveObjectBase
{
	public float energyAmount;
	
	public override void ApplyEffect (GameObject obj)
	{
		StatsCharacter statChar = obj.GetComponent<StatsCharacter>();
		if(statChar == null)
		{
			Debug.LogWarning("StatCharacter not found!");
			return;
		}
		if(statChar.isFullEnergy())
		{
			Debug.Log("Full Energy!");
			return;
		}
		statChar.ApplyEnergy(energyAmount);
		SelfDestruct();
	}
}
