using UnityEngine;
using System.Collections.Generic;

public class NodeLink
{
	public int neighbourRow;
	public int neighbourCol;
	public float cost;
	
	public NodeLink(int row, int col, float cost)
	{
		this.neighbourRow = row;
		this.neighbourCol = col;
		this.cost = cost;
	}
}

public class Node
{
	public Vector3 mPoint = Vector3.zero;
	public List<NodeLink> mNodeLinkNeighbour = new List<NodeLink>();
	//! the size of which unit may pass through this node
	public int mClearanceSize = 0;
}

[System.Serializable]
public class CorridorManager
{
	public GameObject[] mCorridorBlocks;
}

public class LevelGenerator : MonoBehaviour
{
	//! blocks prefab dictionary
	public CorridorManager[] mCorridorList = new CorridorManager[0];
	
	public List<GameObject> mSpawnList = new List<GameObject>();
	//! the starting block to spawn
	public GameObject mInitialBlock;
	//! keeps track of the previous block that was spawned
	GameObject mPreviousBlock;
	
	//! Just for backup purposes
	public GameObject mBackupTile;
	
	//! Pool Manager for levelBlocks
//	PoolManager mBlockPoolManager;
//	PoolManager mDeadEndPoolManager;
	
	//! for debugging purpose!
	//public GameObject mTestBlock;
	//public List<GameObject> mGameObjs = new List<GameObject>();
	// Use this for initialization
	void Awake ()
	{
		//! create an internal pool Manager
		//mBlockPoolManager = new PoolManager(mCorridorList, "Pool_Corridor");
		//mDeadEndPoolManager = new PoolManager(mDeadEndList, "Pool_DeadEnd");
	}
	
	//! return a list of corridor blocks based on the number of steps
	List<GameObject> GetAllCorridorBlocks(int index)
	{
		List<GameObject> gameObjList = new List<GameObject>();
		GameObject[]tempCorridor = mCorridorList[index].mCorridorBlocks;
		for(int i = 0; i < tempCorridor.Length; ++i)
		{
			gameObjList.Add(tempCorridor[i]);
		}
		return gameObjList;
	}
	
	
	public void CreateNewBlock(Vector3 position)
	{
		Instantiate(mBackupTile,position,Quaternion.identity);
	}
	
	//! Ruleset on how the level should be generate
	public void CreateNewBlock(List<BlockNode> blockNodes, int minRow, int minCol, int steps)
	{
//		Debug.Log("Creating path at step: " + steps);
		int index = steps - 1;
		GameObject nextBlock;
		List<GameObject> spawnCheckList = GetAllCorridorBlocks(index);
		//bool successSpawn = false;
		int counter = 0;
		int nodeAnchorsListCounter = 0;
		
		Debug.Log("spawnCheckList count: " + spawnCheckList.Count);
		
		while(spawnCheckList.Count > 0)
		{
			//! get a random block
			int rand = Random.Range(0,spawnCheckList.Count);
//			Debug.Log("index: " + index);
//			Debug.Log("rand: " + rand);
			
			nextBlock = (GameObject)Instantiate(spawnCheckList[rand]);
			//! scale the block that was spawned
			EventMap.ScaleLevelBlock(nextBlock);
			
			spawnCheckList.Remove(spawnCheckList[rand]);
			
			Debug.Log(nextBlock.name);
			
			counter++;
			if(counter > 10000)
			{
				Debug.LogError("Infinite loop at spawnCheckList");
				return;
			}
			
			//! get the node anchors
			List<GameObject> nodeAnchors = nextBlock.GetComponent<LevelBlock>().ExitNodeAnchorGameObjects();
			
			while(nodeAnchors.Count > 0)
			{
				Debug.Log("Num nodeAnchors: " + nodeAnchors.Count);
				GameObject nodeAnchor = nodeAnchors[0];
				nodeAnchors.RemoveAt(0);
				
				//! if the block can spawn without colliding with other blocks
				if(IsAbleToSpawn(blockNodes[0],nextBlock,nodeAnchor))
				{
					Debug.Log("Able to spawn without colliding!");
					//! get the col and row of the spanwed block
					BlockNode[,] nextBlockNodes = nextBlock.GetComponent<LevelBlock>().ReCalculate();
					int blockRow = nextBlock.GetComponent<LevelBlock>().mRow;
					int blockCol = nextBlock.GetComponent<LevelBlock>().mCol;
					
					Debug.Log("minRow: " + minRow + " minCol: " + minCol);
					Debug.Log("blockRow: " + blockRow + " blockCol: " + blockCol);
					
					//! if the spawn block has minimum row and col for the path
					if(blockCol >= minCol && blockRow >= minRow)
					{
						Debug.Log("Begin matching");
						
						List<Vector3> pathSteps = new List<Vector3>();
						for(int i = 0; i < steps - 1; i++)
						{	
							Vector3 resultant = blockNodes[i + 1].mPoint - blockNodes[i].mPoint;
							pathSteps.Add(resultant.normalized);
						}
						
						GameObject obj = nextBlock;
						
						//! if the block matches with the path
						if(IsMatchWithBlock(nextBlockNodes,blockNodes,pathSteps,blockCol,blockRow,obj))
						{
							Debug.Log("MATCHED!!!!");
							mPreviousBlock = nextBlock;
							mSpawnList.Add(nextBlock);
							return;
						}
					}
					
					Debug.Log("Matching failed!");
				}
				
				nodeAnchorsListCounter++;
				if(nodeAnchorsListCounter > 10000)
				{
					Debug.LogError("Infinite loop at nodeAnchorsListCounter");
					return;
				}
			}
			
			//! this is block is no good to spawn
			nextBlock.SetActive(false);
			Destroy(nextBlock);
		}
		
		//Debug.LogError("There is no block suitable!!!!");
	}
	
	bool IsMatchWithBlock(BlockNode[,] nextBlockNodes, List<BlockNode> blockNodes,List<Vector3> pathSteps,int maxCol, int maxRow, GameObject objTarget)						
	{
		int currCol = 0;
		int currRow = 0;
		bool foundStartPoint = false;
		
		//Debug.Log("Name: " + objTarget.name);
		//! get the index of the anchorNode
		for(int i = 0; i < maxCol; i++)
		{
			for(int j = 0; j < maxRow; j++)
			{
				if(nextBlockNodes[i,j].id == blockNodes[0].id)
				{
//					Debug.Log((NodeAnchor.NODE_DIRECTION)blockNodes[0].id);
					currRow = j;
					currCol = i;
					foundStartPoint = true;
					break;
				}
			}
			if(foundStartPoint)
			{
				break;
			}
		}
		
		Debug.Log("pathSteps count: " + pathSteps.Count);
		
		//! match the nodes based on the id and number of steps
		for(int i = 0; i < pathSteps.Count; i++)
		{
			if(pathSteps[i].x > 0.0f)
			{
				if(currRow + 1 < maxRow)
				{
					currRow += 1;
					Debug.Log("row + 1");
					
				}
				else
				{
					Debug.Log("row out of bounds +ve");
					return false;
				}
			}
			else if(pathSteps[i].x < 0.0f)
			{
				if(currRow - 1 >= 0)
				{
					currRow -= 1;
					Debug.Log("row - 1");
				}
				else
				{
					Debug.Log("row out of bounds -ve");
					return false;
				}
			}
			else if(pathSteps[i].z < 0.0f)
			{
				if(currCol - 1 >= 0)
				{
					Debug.Log("col - 1");
					currCol -= 1;
				}
				else
				{
					Debug.Log("col out of bounds -ve");
					return false;
				}
			}
			else if(pathSteps[i].z > 0.0f)
			{
				if(currCol + 1 < maxCol)
				{
					currCol += 1;
					Debug.Log("col - 1");
				}
				else
				{
					Debug.Log("col out of bounds +ve");
					return false;
				}
			}
			
			if(blockNodes[i + 1].id == nextBlockNodes[currCol,currRow].id)
			{
				Debug.Log("Match" + blockNodes[i + 1].id + " = " + nextBlockNodes[currCol,currRow].id);
				continue;
			}
			else
			{
				Debug.Log("Not match!: " + blockNodes[i + 1].id + " = " + nextBlockNodes[currCol,currRow].id);
				return false;
			}
		}
		
		//Debug.Log("Spawn Successful!!");
//		for(int i = 0; i < pathSteps.Count; i++)
//		{
//			Debug.Log("Success pathStep: " + pathSteps[i]);
//			
//		}
//		Debug.Log("currCol : " + currCol);
//		Debug.Log("currRow : " + currRow);
		
		return true;
	}
	
	bool IsAbleToSpawn(BlockNode lastAnchor, GameObject nextBlock, GameObject nodeAnchor)
	{
		float targetAngle = NodeAnchor.GetAngle(lastAnchor.id);
//		Debug.Log("targetAngle: " + targetAngle);
		float resultantRotation = targetAngle - nodeAnchor.transform.rotation.eulerAngles.y;
//		Debug.Log("resultantRotation: " + resultantRotation);
		
		nextBlock.transform.Rotate(new Vector3(0.0f, resultantRotation, 0.0f));
		nextBlock.transform.Translate(lastAnchor.mPoint - nodeAnchor.transform.position, Space.World);
		
		if(!IsColliderOthers(nextBlock, mSpawnList))
		{
			//GameObject obj = (GameObject)Instantiate(nextBlock);
			//!test only, should refer to the spawnList
			//mPreviousBlock = nextBlock;
			//Debug.Log("nodeCount : " + nodeCount);
			return true;
		}
		
		return false;
	}
	
	bool IsColliderOthers(GameObject nextBlock, List<GameObject> spawnBlockList)
	{
		for(int i = 0; i < spawnBlockList.Count; i++)
		{	
			if(spawnBlockList[i] == mPreviousBlock)
			{
				continue;
			}
			//MeshCollider meshCollider = spawnBlockList[i].GetComponent<LevelBlock>().GetMeshCollider();
			Collider otherCollider = spawnBlockList[i].collider;
			Collider nextBlockCollider = nextBlock.collider;
			//! the tolerance of intersection between 2 blocks
			float tolerance = 0.5f;
			
			if(nextBlockCollider.bounds.min.x + tolerance <= otherCollider.bounds.max.x - tolerance && nextBlockCollider.bounds.max.x - tolerance>= otherCollider.bounds.min.x &&
				nextBlockCollider.bounds.min.z + tolerance <= otherCollider.bounds.max.z - tolerance && nextBlockCollider.bounds.max.z - tolerance >= otherCollider.bounds.min.z + tolerance)
			{
				Debug.Log(nextBlock.name + " colliding with " + otherCollider.gameObject.name);
				return true;
			}
			
//			if(nextBlock.collider.bounds.Intersects(otherCollider.bounds))
//			{
//				Debug.Log(nextBlock.name + " colliding with " + otherCollider.gameObject.name);
//				return true;
//			}
		}
		return false;
	}
	
	//! used for to get angle from 0 - 180 or 0 -180 as Vector3.angle can't
	public static float AngleDir(Vector3 facingVector, Vector3 targetDir, Vector3 up)
	{
		float angle = Vector3.Angle(facingVector, targetDir); //! only returns 0 - 180
		Vector3 perp = Vector3.Cross(facingVector, targetDir);
		float dir = Vector3.Dot(perp, up);
		float counter = 0.0f;
		if (dir < 0f)
		{
			counter = -1f;
		}
		if(counter == -1)//! if the angle is to the left direction 180 to 359 onwards
		{
			return -angle;
		}
		
		return angle;
	}
}
