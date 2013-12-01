using UnityEngine;
using System.Collections;

public class EntitySearch {
	
	/// <summary>
	/// Gets the nearest target based on unity overlap sphere based on the given target layer. The target/targets needs
	/// a collider and layer set respectively
	/// </summary>
	/// <returns>
	/// The target.
	/// </returns>
	/// <param name='self'>
	/// GameObject instance
	/// </param>
	/// <param name='radius'>
	/// radius of the checking
	/// </param>
	/// <param name='targetLayer'>
	/// Target layermask
	/// </param>
	public static GameObject NearestTarget(GameObject self, float radius, LayerMask targetLayer)
	{
		Collider[] colliders = Physics.OverlapSphere(self.transform.position,radius,targetLayer);
		if(colliders.Length == 0){
			return null;
		}
		
		float nearestDist = Mathf.Infinity;
		GameObject gameObj = null;
		foreach(Collider col in colliders)
		{
			float sqrDist = (self.transform.position - col.transform.position).sqrMagnitude;
			if(sqrDist <= nearestDist)
			{
				nearestDist = sqrDist;
				gameObj = col.gameObject;
			}
		}
		return gameObj;
	}
	
	public static GameObject NearestTarget(Vector3 pos, float radius, LayerMask targetLayer)
	{
		Collider[] colliders = Physics.OverlapSphere(pos,radius,targetLayer);
		if(colliders.Length == 0){
			return null;
		}
		
		float nearestDist = Mathf.Infinity;
		GameObject gameObj = null;
		foreach(Collider col in colliders)
		{
			float sqrDist = (pos - col.transform.position).sqrMagnitude;
			if(sqrDist <= nearestDist)
			{
				nearestDist = sqrDist;
				gameObj = col.gameObject;
			}
		}
		return gameObj;
	}
	
	/// <summary>
	/// Searches for a new target gameobject other than the old target
	/// </summary>
	/// <returns>
	/// The target gameobject
	/// </returns>
	/// <param name='self'>
	/// Self.
	/// </param>
	/// <param name='oldTarget'>
	/// Old target.
	/// </param>
	/// <param name='radius'>
	/// radius of the overlap sphere
	/// </param>
	/// <param name='targetLayer'>
	/// Target layermask.
	/// </param>
	public static GameObject SearchForNewTarget(GameObject self, GameObject oldTarget, float radius, LayerMask targetLayer)
	{
		Collider[] colliders = Physics.OverlapSphere(self.transform.position,radius,targetLayer);
		if(colliders.Length == 0){
			return null;
		}
		
		float nearestDist = Mathf.Infinity;
		GameObject gameObj = null;
		foreach(Collider col in colliders)
		{
			if(oldTarget == col.gameObject)
			{
				continue;	
			}
			float sqrDist = (self.transform.position - col.transform.position).sqrMagnitude;
			if(sqrDist <= nearestDist)
			{
				nearestDist = sqrDist;
				gameObj = col.gameObject;
			}
		}
		return gameObj;
	}
	
	public static GameObject SearchForNewTarget(Vector3 pos, GameObject oldTarget, float radius, LayerMask targetLayer)
	{
		Collider[] colliders = Physics.OverlapSphere(pos,radius,targetLayer);
		if(colliders.Length == 0){
			return null;
		}
		
		float nearestDist = Mathf.Infinity;
		GameObject gameObj = null;
		foreach(Collider col in colliders)
		{
			if(oldTarget == col.gameObject)
			{
				continue;	
			}
			float sqrDist = (pos - col.transform.position).sqrMagnitude;
			if(sqrDist <= nearestDist)
			{
				nearestDist = sqrDist;
				gameObj = col.gameObject;
			}
		}
		return gameObj;
	}
}
