using UnityEngine;
using System.Collections;

//! helper class to get angle
public static class GetAngleHelper
{
	//! get angle between 0 to -180(anticlockwise) / 180(clockwise)
	public static float GetAngle(Vector3 targetDir, Vector3 facing, Vector3 localAxis)
	{
		float targetAngle = Vector3.Angle(facing,targetDir);
		Vector3 perp = Vector3.Cross(facing, targetDir);
		float dir = Vector3.Dot (perp, localAxis);
		if(dir < 0.0f)
		{
			targetAngle = targetAngle * -1;
		}
		
		return targetAngle;
	}
}
