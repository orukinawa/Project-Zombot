using UnityEngine;
using System.Collections;

public class CursorManager : MonoBehaviour
{
	public static Vector3 PlaneRayIntersection (Plane plane, Ray ray)
	{
		float dist = 0.0f;
		plane.Raycast (ray, out dist);
		return ray.GetPoint (dist);
	}
	
	public static Vector3 ScreenPointToWorldPointOnPlane (Vector3 screenPoint, Plane plane , Camera camera)
	{
		// Set up a ray corresponding to the screen position
		Ray ray = camera.ScreenPointToRay (screenPoint);
		
		// Find out where the ray intersects with the plane
		return PlaneRayIntersection (plane, ray);
	}
}
