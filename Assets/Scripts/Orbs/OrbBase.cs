using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]

public class OrbBase : MonoBehaviour
{
	public float detectionRadius;
	public float moveSpeed;
	public float rotationSpeed;
	public LayerMask targetlayer;
	protected Transform target = null;
	protected Transform mTransform;
	
	public virtual void _Update()
	{
		if(target == null)
		{
			target = FindTarget();
		}
		else
		{
			//mTransform.rotation = Quaternion.Slerp(mTransform.rotation, Quaternion.LookRotation(target.position - mTransform.position), Time.deltaTime * rotationSpeed);
			mTransform.LookAt(target.position);
			mTransform.position += mTransform.forward * moveSpeed;			
		}
	}
	
	public void _OnTriggerEnter(Collider col)
	{
		if(col.transform == target)
		{
			OrbEffect(col.gameObject.GetComponent<StatsCharacter>());
			target = null;
			SelfDestruct();
		}
	}
	
	public virtual Transform FindTarget()
	{
		Transform closestTarget = null;
		float distance;
		Collider[] colliders = Physics.OverlapSphere(mTransform.position, detectionRadius, targetlayer);
		if(colliders.Length > 0)
		{
			float closestDistance = Mathf.Infinity;
			foreach(Collider col in colliders)
			{
				Debug.Log(col);
				distance = (mTransform.position - col.transform.position).sqrMagnitude;
				if(distance < closestDistance)
				{
					closestDistance = distance;
					closestTarget = col.transform;
				}
			}
		}
		return closestTarget;
	}
	
	public void InitializeOrb()
	{
		mTransform = transform;
		target = null;
	}
	
	public virtual void OrbEffect (StatsCharacter statsChar)
	{
	}
	
	public void SelfDestruct()
	{
		Destroy(gameObject);
	}
}
