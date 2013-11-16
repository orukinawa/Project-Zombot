using UnityEngine;
using System.Collections;

public class SkillFreeze : SkillBase
{
	public float freezeRadius;
	public int freezeTickDuration;
	public LayerMask targetLayer;
	int layerInt;
	Plane ground;
	
	void Start()
	{
		Initialize();
		ground = new Plane(Vector3.up, 0.0f);
		layerInt = 1 << targetLayer;
	}
	
	public override void UseSkill ()
	{
		Vector3 targetPos = CursorManager.ScreenPointToWorldPointOnPlane (Input.mousePosition, ground, Camera.main);
		Collider[] targets = Physics.OverlapSphere(targetPos,freezeRadius, targetLayer);
		foreach(Collider col in targets)
		{
			if(col.GetComponent<StatusSpeedModifier>() != null)
			{
				col.GetComponent<StatusSpeedModifier>().InitiateSlow(0.0f,freezeTickDuration);
			}
		}
	}
}
