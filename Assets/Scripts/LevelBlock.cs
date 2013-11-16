using UnityEngine;
using System.Collections.Generic;

public class BlockNode
{
	//! the position of the BlockNode
	public Vector3 mPoint = Vector3.zero;
	//! the id of the node
	public int id = 0;
}

public class LevelBlock : MonoBehaviour
{
	public enum BLOCK_TYPE
	{
		CORRIDOR,
		DEAD_END,
	}
	
	public NodeAnchor[] mNodeAnchors;
	public BLOCK_TYPE mBlockType;
	MeshCollider mMeshCollider;
	//! only to be used for poolManager to refer
	int mPoolId;
	
	//! Debug purpose only
	//public List<Vector3> mPos = new List<Vector3>();
	//public List<GameObject> mGameObjects = new List<GameObject>();
	BlockNode[,] mBlockNodes = new BlockNode[0,0];
	public int mRow = 0;
	public int mCol = 0;
	
	// Use this for initialization
	public void Awake ()
	{
		Init();
		//mGameObjects = ExitNodeAnchorGameObjects();
		//mPos = GetAllExitAnchorPos();
	}
	
	//! Gets the mesh collider
	public MeshCollider GetMeshCollider()
	{
		return mMeshCollider;
	}
	
	public void SetPoolId(int num)
	{
		mPoolId = num;
	}
	
	public int GetPoolId()
	{
		return mPoolId;
	}
	
	public void Init()
	{
		//Debug.Log("Inited....");
		mNodeAnchors = GetComponentsInChildren<NodeAnchor>();
		mMeshCollider = GetComponentInChildren<MeshCollider>();
	}
	
	//! Returns a recalculate array of nodes based on request
	//! call GetDirection from node anchor as well
	public BlockNode[,] ReCalculate()
	{
		BlockNode[,] blockNodes = new BlockNode[0,0];
		List<GameObject> nodeAnchors = new List<GameObject>();
		
		float halfCellLength = EventMap.sTileSize * 0.5f;
		int cellLength = EventMap.sTileSize;
		
		Bounds bounds = collider.bounds;
		
		//! get the row and col of the block based on the box collider size
		int row = (int)Mathf.Round((bounds.max.x - bounds.min.x) / cellLength);
		int col = (int)Mathf.Round((bounds.max.z - bounds.min.z) / cellLength);
		
		blockNodes = new BlockNode[col,row];
		//! get all the position of the node anchors
		nodeAnchors = ExitNodeAnchorGameObjects();
		
		for(int i = 0; i < col; i++)
		{
			for(int j = 0; j < row; j++)
			{
				BlockNode blockNode = new BlockNode();
				blockNode.mPoint = new Vector3(
					bounds.min.x + halfCellLength + cellLength * j,
					0.0f,
					bounds.min.z + halfCellLength + cellLength * i);
				
				blockNode.id = 1;
				blockNodes[i,j] = blockNode;
			}
		}
//		Debug.Log("max: " + bounds.max.x);
//		Debug.Log("min: " + bounds.min.x);
//		float rowValue = (bounds.max.x - bounds.min.x) / cellLength;
//		Debug.Log("rowValue: " + rowValue.ToString("F8"));
		
		mRow = row;
		mCol = col;
		
//		Debug.Log("row: " + row);
//		Debug.Log("col: " + col);
		
		
		//! label the position of anchor node
		for(int i = 0; i < nodeAnchors.Count; i++)
		{
			int nodeRow = (int)((nodeAnchors[i].transform.position.x - bounds.min.x) / cellLength);
			int nodeCol = (int)((nodeAnchors[i].transform.position.z - bounds.min.z) / cellLength);
			nodeAnchors[i].GetComponent<NodeAnchor>().GetDirection();
			blockNodes[nodeCol,nodeRow].id = (int)nodeAnchors[i].GetComponent<NodeAnchor>().mNodeDirection;
		}
		
//		Debug.Log("row: " + row);
//		Debug.Log("col: " + col);
		
		return blockNodes;
	}
	
	public int GetNumExitAnchor()
	{
		int num = 0;
		for(int i = 0; i < mNodeAnchors.Length; i++)
		{
			if(mNodeAnchors[i].mAnchorType == NodeAnchor.ANCHOR_TYPE.EXIT)
			{
				num += 1;
			}
		}
		return num;
	}
	
	public List<NodeAnchor> GetAllNodeAnchor()
	{
		List<NodeAnchor> nodeAnchors = new List<NodeAnchor>();
		for(int i = 0; i < mNodeAnchors.Length; i++)
		{
			if(mNodeAnchors[i].mAnchorType == NodeAnchor.ANCHOR_TYPE.EXIT)
			{
				nodeAnchors.Add(mNodeAnchors[i]);
			}
		}
		return nodeAnchors;
	}
	
	//! Get all exit node anchor objects
	public List<GameObject> ExitNodeAnchorGameObjects()
	{
		//Debug.Log("ExitNodeAnchorGameObjects called");
		List<GameObject> gameObjects = new List<GameObject>();
		for(int i = 0; i < mNodeAnchors.Length; i++)
		{
			if(mNodeAnchors[i].mAnchorType == NodeAnchor.ANCHOR_TYPE.EXIT)
			{
				gameObjects.Add(mNodeAnchors[i].gameObject);
			}
		}
		return gameObjects;
	}
	
	//! Gets all positions anchor of exit type 
	public List<Vector3> GetAllExitAnchorPos()
	{
		List<Vector3> posList = new List<Vector3>();
		for(int i = 0; i < mNodeAnchors.Length; i++)
		{
			if(mNodeAnchors[i].mAnchorType == NodeAnchor.ANCHOR_TYPE.EXIT)
			{
				posList.Add(mNodeAnchors[i].gameObject.transform.position);
			}
		}
		return posList;
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			Debug.Log("HEY");
			mBlockNodes = ReCalculate();
		}
//		Debug.Log("Forward: " + transform.forward);
	}
	
	void OnDrawGizmos()
	{
//		//! Draw the AnchorType
		if(mNodeAnchors.Length > 0)
		{
			foreach(NodeAnchor nodeAnchor in mNodeAnchors)
			{
				if(nodeAnchor.mAnchorType == NodeAnchor.ANCHOR_TYPE.EXIT)
				{
					Gizmos.DrawIcon(nodeAnchor.transform.position,"Anchor_Exit");
				}
				else
				{
					Gizmos.DrawIcon(nodeAnchor.transform.position,"Anchor_Extra");
				}
			}
		}
		
		foreach(BlockNode blockNode in mBlockNodes)
		{
			if(blockNode.id > 1)
			{
				Gizmos.DrawIcon(blockNode.mPoint,"RedNode");
			}
			else if(blockNode.id == 1)
			{
				Gizmos.DrawIcon(blockNode.mPoint,"NodeIconBig");
			}
		}
		
//		
//		if(mMeshCollider)
//		{
//			Gizmos.DrawIcon(mMeshCollider.bounds.min, "Collider_min");
//			Gizmos.DrawIcon(mMeshCollider.bounds.max, "Collider_max");
//		}
//			float row = (collider.bounds.max.x - collider.bounds.min.x) / EventMap.mTileSize;
//			float col = (collider.bounds.max.z - collider.bounds.min.z) / EventMap.mTileSize;
//			Debug.Log("row: " + row);
//			Debug.Log("col: " + col);
//			float someNum = transform.position.z - collider.bounds.min.z;
//			Debug.Log("bound: " + someNum);
		
//			Gizmos.DrawIcon(transform.collider.bounds.min, "Collider_min");
//			Gizmos.DrawIcon(transform.collider.bounds.max, "Collider_max");
	}
}
