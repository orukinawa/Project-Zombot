using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
	public float detectionRadius;
	float sqrDetectionRadius;
	Transform mTransform;
	Transform target;
	int layerInt;
	InteractiveObjectBase intObj;
	public LayerMask targetLayer;
	
	void Start()
	{
		mTransform = transform;
		target = null;
		layerInt = targetLayer;
	 	sqrDetectionRadius = detectionRadius*detectionRadius;
	}
	
	void Update()
	{
		if(target == null)
		{
			target = FindTarget();
		}
		else
		{
			if((target.position - mTransform.position).sqrMagnitude > (sqrDetectionRadius))
			{
				intObj.Selected(false);
				intObj = null;
				target = null;
				return;
			}
			//make interactive object display 'press E' overlay
		}
	}
	
	public virtual Transform FindTarget()
	{
		Transform closestTarget = null;
		float distance;
		Collider[] colliders = Physics.OverlapSphere(mTransform.position, detectionRadius, layerInt);
		if(colliders.Length > 0)
		{
			float closestDistance = Mathf.Infinity;
			foreach(Collider col in colliders)
			{
				distance = (mTransform.position - col.transform.position).sqrMagnitude;
				if(distance < closestDistance)
				{
					closestDistance = distance;
					closestTarget = col.transform;
				}
			}
			intObj = closestTarget.GetComponent<InteractiveObjectBase>();
			intObj.Selected(true);
		}
		return closestTarget;
	}
	
	public void Interact()
	{
		if(intObj != null)
		{
			intObj.ApplyEffect(gameObject);
		}
	}
	
}
