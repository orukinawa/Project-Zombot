using UnityEngine;
using System.Collections;

public class NodeAnchor : MonoBehaviour
{
	public enum ANCHOR_TYPE
	{
		EXTRA,//! extra anchor is just for the sake adding extra node for the pathfinding
		EXIT,//! the connectors between blocks
	}
	
	public enum ENTRANCE_SIZE
	{
		SMALL,//! 2 unit
		MEDIUM,//! 4 unit
		LARGE,//! 8 unit
	}
	
	public enum NODE_DIRECTION
	{
		NORTH = 2,
		EAST,
		SOUTH,
		WEST
	}
	
	public ANCHOR_TYPE mAnchorType;
	public ENTRANCE_SIZE mEntranceSize;
	public NODE_DIRECTION mNodeDirection;
	
	void Awake()
	{
		GetDirection();
	}
	
	//! Compare his forward direction with the global direction
	public void GetDirection()
	{
		float angle = Vector3.Angle(Vector3.forward, transform.forward);
		//Debug.Log("Cross: " + Vector3.Cross(Vector3.forward,transform.forward));
		Vector3 cross = Vector3.Cross(Vector3.forward, transform.forward);
		if(cross.y < 0.0f)
		{
			angle = 360 - angle;
			
		}
		angle = Mathf.Round(angle);
		if(angle >= 0.0f && angle < 1.0f || angle <= 360.0f && angle >= 359.0f)
		{
			mNodeDirection = NODE_DIRECTION.NORTH;
		}
		else if(angle >= 90.0f && angle < 91.0f)
		{
			mNodeDirection = NODE_DIRECTION.EAST;
		}
		else if(angle >= 180.0f && angle < 181.0f)
		{
			mNodeDirection = NODE_DIRECTION.SOUTH;
		}
		else if(angle >= 270.0f && angle < 271.0f)
		{
			mNodeDirection = NODE_DIRECTION.WEST;
		}
		else
		{
			Debug.LogError("Some Node don not have direction! " + angle);
		}
	}
	
	public static float GetAngle(int direction)
	{
		if(direction == (int)NODE_DIRECTION.NORTH)
		{
			return 0.0f;
		}
		else if(direction == (int)NODE_DIRECTION.SOUTH)
		{
			return 180.0f;
		}
		else if(direction == (int)NODE_DIRECTION.EAST)
		{
			return 90.0f;
		}
		else if(direction == (int)NODE_DIRECTION.WEST)
		{
			return 270.0f;
		}
		
		return 0.0f;
	}
	
	//! returns Vector in world space
	public static Vector3 GetDirectionInVector(int direction)
	{
		if(direction == (int)NODE_DIRECTION.NORTH)
		{
			return Vector3.forward;
		}
		else if(direction == (int)NODE_DIRECTION.SOUTH)
		{
			return Vector3.back;
		}
		else if(direction == (int)NODE_DIRECTION.EAST)
		{
			return Vector3.right;
		}
		else if(direction == (int)NODE_DIRECTION.WEST)
		{
			return Vector3.left;
		}
		return Vector3.zero;
	}
	
	public static int GetReverseDirection(int direction)
	{
		if(direction == (int)NODE_DIRECTION.NORTH)
		{
			direction = (int)NODE_DIRECTION.SOUTH;
		}
		else if(direction == (int)NODE_DIRECTION.SOUTH)
		{
			direction = (int)NODE_DIRECTION.NORTH;
		}
		else if(direction == (int)NODE_DIRECTION.EAST)
		{
			direction = (int)NODE_DIRECTION.WEST;
		}
		else if(direction == (int)NODE_DIRECTION.WEST)
		{
			direction = (int)NODE_DIRECTION.EAST;
		}
		
		return direction;
	}
	
	void DrawDirection()
	{
		GetDirection();
	}
	
//	void OnDrawGizmos()
//	{
//		//DrawDirection();
//		if(mNodeDirection == NODE_DIRECTION.NORTH)
//		{
//			Gizmos.DrawIcon(transform.position,"Directional/Up");
//		}
//		if(mNodeDirection == NODE_DIRECTION.EAST)
//		{
//			Gizmos.DrawIcon(transform.position,"Directional/Right");
//		}
//		if(mNodeDirection == NODE_DIRECTION.SOUTH)
//		{
//			Gizmos.DrawIcon(transform.position,"Directional/Down");
//		}
//		if(mNodeDirection == NODE_DIRECTION.WEST)
//		{
//			Gizmos.DrawIcon(transform.position,"Directional/Left");
//		}
//	}
}
