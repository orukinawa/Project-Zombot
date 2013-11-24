using UnityEngine;
using System.Collections;

abstract public class BehaviourBase : MonoBehaviour
{
	public float mInfluenceWeight = 1.0f;
	
	/// <summary>
	/// Initialize when entering the new state
	/// </summary>
	/// <param name='enemyBase'>
	/// Enemy base.
	/// </param>
	public virtual void Init(EnemyBase enemyBase)
	{
		
	}
	
	/// <summary>
	/// Deinitialize when change new state
	/// </summary>
	/// <param name='enemyBase'>
	/// Enemy base.
	/// </param>
	public virtual void DeInit(EnemyBase enemyBase)
	{
		
	}
	
	//! all the necessary thing to do after death(eg. releasing player from grip)
	public virtual void Death(EnemyBase enemyBase)
	{
		
	}
	
	public virtual GameObject GetNearestToTarget(Vector3 targetPos, float radius, LayerMask targetLayer)
	{
		Collider[] colliders = Physics.OverlapSphere(targetPos,radius,targetLayer);
		float nearestDist = Mathf.Infinity;
		GameObject nearestObj  = null;
		foreach(Collider col in colliders)
		{
			//! get the nearest to the trap
			float sqrDist = (targetPos - col.transform.position).sqrMagnitude;
			if(sqrDist <= nearestDist)
			{
				nearestDist = sqrDist;
				nearestObj = col.gameObject;
			}
		}
		return nearestObj;
	}
	
	public virtual void SearchForNewTarget(EnemyBase enemyBase, float radius, LayerMask targetLayer)
	{
		Collider[] colliders = Physics.OverlapSphere(enemyBase.transform.position,radius,targetLayer);
		float nearestDist = Mathf.Infinity;
		foreach(Collider col in colliders)
		{
			//! get the nearest to the trap
			float sqrDist = (enemyBase.transform.position - col.transform.position).sqrMagnitude;
			if(sqrDist <= nearestDist)
			{
				nearestDist = sqrDist;
				enemyBase.mTargetPlayer = col.gameObject;
			}
		}
	}
	
	//! deals with movement only
	public virtual Vector3 UpdateBehaviour(EnemyBase enemyBase)
	{
		return Vector3.zero;
	}
	
	//! function to forcefully execute a transition
	public virtual void ExecuteTransition(EnemyBase enemyBase)
	{
		BehaviourManager manager = enemyBase.GetBehaviourManager();
		ActionExecute transition = GetComponent<ActionExecute>();
		//! if there is no Action Execute transition it will not change state
		if(!transition)
		{
			return;
		}
		
		if(transition.mTargetBehaviour == this)
		{
			ActionExecuteData data = (ActionExecuteData)manager.mCustomData[transition];
			data.mFlag = true;
		}
	}
}
