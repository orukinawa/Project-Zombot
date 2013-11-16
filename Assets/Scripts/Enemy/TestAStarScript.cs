using UnityEngine;
using System.Collections.Generic;

public class TestAStarScript : MonoBehaviour
{
	public GameObject mObject;
	Vector3 targetPos = Vector3.zero;
	
	public static List<SearchNode> mOpenList = new List<SearchNode>();
	public static List<SearchNode> mCloseList = new List<SearchNode>();
	public static Node startNode;
	void Update()
	{
//		if(mObject != null)
//		{
//			RaycastHit hit;
//			
//			if(Input.GetMouseButtonDown(1))
//			{
//				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//				if(Physics.Raycast(ray,out hit,Mathf.Infinity))
//				{
//					targetPos = hit.point;
//				}
//				EventMap.GetPathAi(mObject.transform.position, targetPos, EventMap.sSmallAiNodes,0,EventMap.AI_NODE_TYPE.SMALL);
//			}
//			Debug.DrawRay(mObject.transform.position,Vector3.up * 3.0f,Color.blue);
//			Debug.DrawRay(targetPos,Vector3.up * 3.0f,Color.red);
//		}
	}
	
	void OnDrawGizmos()
	{
		if((mOpenList.Count > 0 || mCloseList.Count > 0) && mObject != null) //&& targetPos != Vector3.zero)
		{
			Debug.DrawRay(startNode.mPoint,Vector3.up * 3.0f, Color.cyan);
			
			foreach(SearchNode node in mOpenList)
			{
				Gizmos.DrawIcon(node.mNode.mPoint,"LevelRelated/RedNode");
			}
			
			foreach(SearchNode node in mCloseList)
			{
				Gizmos.DrawIcon(node.mNode.mPoint,"LevelRelated/BlueNode");
			}
		}
	}
}
