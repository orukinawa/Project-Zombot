using UnityEngine;
using System.Collections.Generic;

public class DrawCommandoHealthBar : MonoBehaviour
{
	StatsCharacter statCh;
	public int radiusSize;
	
	Node bigNodeDebug;
	Node smallNodeDebug;
	Vector3 pos;
	
	//! debug path
	List<Vector3> mPath = new List<Vector3>();
	
	void Start()
	{
		statCh = GetComponent<StatsCharacter>();
	}
	
	void Update()
	{
//		if(EventMap.mPathNodes.Length > 0)
//		{
			bigNodeDebug = EventMap.GetBigNodeAI(transform.position,EventMap.sBigAiNodes,radiusSize);	
			Debug.DrawRay(bigNodeDebug.mPoint, Vector3.up * 4.0f, Color.cyan);
			smallNodeDebug = EventMap.GetSmallNodeAI(transform.position,EventMap.sSmallAiNodes,radiusSize);	
			Debug.DrawRay(smallNodeDebug.mPoint, Vector3.up * 4.0f, Color.red);
//		}
//		
//		if(Input.GetMouseButtonDown(1))
//		{
//		 	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hit;
//			if(Physics.Raycast(ray,out hit,Mathf.Infinity))
//			{
//				pos = hit.point;
//			}
//			testNode = EventMap.GetNodeAI(pos,EventMap.mPathNodes,radiusSize);
//			mPath = EventMap.GetPathAi(transform.position,pos,EventMap.mPathNodes,radiusSize);
//		}
////		Debug.DrawRay(pos, Vector3.up * 4.0f, Color.red);
////		
//		Debug.DrawRay(testNode.mPoint, Vector3.up * 4.0f, Color.red);
	}
	
//	void OnDrawGizmos()
//	{
//		Gizmos.color = Color.red;
//		for(int i = 0; i < mPath.Count - 1; i++)	
//		{
//			Gizmos.DrawLine(mPath[i],mPath[i + 1]);
//		}
//	}
	
	void OnGUI()
	{
		float currentHealth = statCh.currentHealth;
		float maxHealth = statCh.maxHealth;
		GUI.Button(new Rect(0,0,5 * currentHealth,50),"CurrentHealth: " + currentHealth);
	}
}
