using UnityEngine;
using System.Collections.Generic;

//public class NodeLink
//{
//	public int neighbourIndex;
//	public float cost;
//	
//	public NodeLink(int neighbourIndex, float cost)
//	{
//		this.neighbourIndex = neighbourIndex;
//		this.cost = cost;
//	}
//}
//
//public class Node
//{
//	public Vector3 mPoint = Vector3.zero;
//	List<NodeLink> mNodeLinkNeighbour = new List<NodeLink>();
//}

public class LevelGenerator1BKP : MonoBehaviour
{
	//! number of block needed to reach the goal
	public int mNumBlockToGoal;
	public GameObject[] mCorridorList = new GameObject[0];
	public GameObject[] mDeadEndList = new GameObject[0];
	public static List<GameObject> mSpawnList = new List<GameObject>();
	//! the starting block to spawn
	public GameObject mInitialBlock;
	//! the goal block to spawn
	//public GameObject mFinalBlock;
	//! keeps track of the previous block that was spawned
	GameObject mPreviousBlock;
	
	//! Pool Manager for levelBlocks
	PoolManager mBlockPoolManager;
	PoolManager mDeadEndPoolManager;
	
	//! for debugging purpose!
	//public GameObject mTestBlock;
	//public List<GameObject> mGameObjs = new List<GameObject>();
	// Use this for initialization
	void Start ()
	{
		GameObject obj = (GameObject)Instantiate(mInitialBlock, Vector3.zero, Quaternion.identity);
		mSpawnList.Add(obj);
		mPreviousBlock = obj;
		//! create an internal pool Manager
		mBlockPoolManager = new PoolManager(mCorridorList, "Pool_Corridor");
		mDeadEndPoolManager = new PoolManager(mDeadEndList, "Pool_DeadEnd");
		GenerateLevel();
	}
	
	List<GameObject> GetAllCorridorBlocks()
	{
		List<GameObject> gameObjList = new List<GameObject>();
		for(int i = 0; i < mCorridorList.Length; ++i)
		{
			if(mCorridorList[i].GetComponent<LevelBlock>().mBlockType == LevelBlock.BLOCK_TYPE.CORRIDOR)
			{
				gameObjList.Add(mCorridorList[i]);
			}
		}
		return gameObjList;
	}
	
	List<GameObject> GetAllDeadEndBlocks()
	{
		List<GameObject> gameObjList = new List<GameObject>();
		for(int i = 0; i < mDeadEndList.Length; ++i)
		{
			if(mDeadEndList[i].GetComponent<LevelBlock>().mBlockType == LevelBlock.BLOCK_TYPE.DEAD_END)
			{
				gameObjList.Add(mDeadEndList[i]);
			}
		}
		return gameObjList;
	}
	
	//! Ruleset on how the level should be generate
	void GenerateLevel()
	{
		Debug.Log("Generate level...");
		for(int i = 0; i < mNumBlockToGoal; ++i)
		{
			//Debug.Log("numBlock generate: " + i);
			//! Gets all the possible exits of the previous block
			List<GameObject> todoSpawnList = mPreviousBlock.GetComponent<LevelBlock>().ExitNodeAnchorGameObjects();
			while(todoSpawnList.Count > 0)
			{
				//Debug.Log("todo: " + todoSpawnList.Count);
				//! select exit anchor
				GameObject lastAnchor = todoSpawnList[Random.Range(0,todoSpawnList.Count)];
				todoSpawnList.Remove(lastAnchor);
				List<GameObject> corridors = GetAllCorridorBlocks();
				List<GameObject> deadEnds = GetAllDeadEndBlocks();
				bool sucessSpawn = false;
				
				//Debug.Log("Corridors: " + corridors.Count);
				//Debug.Log("deadEnds: " + deadEnds.Count);
				
				while((corridors.Count > 0 && deadEnds.Count > 0) || !sucessSpawn)
				{
					GameObject nextBlock = null;
					if(corridors.Count > 0)
					{
						//Debug.Log("Selected Corridor for block: " + i);
						int rand = Random.Range(0,corridors.Count);
						//nextBlock = (GameObject)Instantiate(corridors[rand]);
						nextBlock = mBlockPoolManager.GetPoolObject(rand);
						corridors.Remove(corridors[rand]);
					}
					else if (deadEnds.Count > 0)
					{
						//Debug.Log("Selected DeadEnd for block: " + i);
						int rand = Random.Range(0,deadEnds.Count);
						//nextBlock = (GameObject)Instantiate(deadEnds[rand]);
						nextBlock = mDeadEndPoolManager.GetPoolObject(rand);
						deadEnds.Remove(deadEnds[rand]);
					}
					else
					{
						//! do nothing
						break;
					}
					
					List<GameObject> nodeAnchors = nextBlock.GetComponent<LevelBlock>().ExitNodeAnchorGameObjects();
					
//					Debug.Log("nodeAnchors count : " + nodeAnchors.Count);
					
					//! check whether the selected prefab will collide with others
					if(IsAbleToSpawn(lastAnchor, nextBlock, nodeAnchors))
					{	
						//! the prefab is successfully instantiated
						sucessSpawn = true;
					}
					else
					{
						//! remove nextBlock obj from the scene if fail to spawn
						if(nextBlock.GetComponent<LevelBlock>().mBlockType == LevelBlock.BLOCK_TYPE.CORRIDOR)
						{
							mBlockPoolManager.ReturnPoolObject(nextBlock);
						}
						else if(nextBlock.GetComponent<LevelBlock>().mBlockType == LevelBlock.BLOCK_TYPE.DEAD_END)
						{
							mDeadEndPoolManager.ReturnPoolObject(nextBlock);
						}
					}
				}
			}
		}
	}
	
	bool IsAbleToSpawn(GameObject lastAnchor, GameObject nextBlock, List<GameObject> nodeAnchors)
	{
		int nodeCount = 0;
		while(nodeAnchors.Count > 0)
		{
			nodeCount++;
			GameObject nodeAnchor = nodeAnchors[Random.Range(0, nodeAnchors.Count)];
			nodeAnchors.Remove(nodeAnchor);
			
			float targetRotation = lastAnchor.transform.rotation.eulerAngles.y + 180.0f;
			//Debug.Log(targetRotation);
			float resultantRotation = targetRotation - nodeAnchor.transform.rotation.eulerAngles.y;
			
			nextBlock.transform.Rotate(new Vector3(0.0f, resultantRotation, 0.0f));
			nextBlock.transform.Translate(lastAnchor.transform.position - nodeAnchor.transform.position, Space.World);
			
			/*Vector3 targetDir = lastAnchor.transform.position - nodeAnchor.transform.position;
			targetDir.Normalize();
			Debug.Log("targetDir : " + targetDir);
			//! get the parent to child angle
			float angleParentToNode = AngleDir(nextBlock.transform.right, nodeAnchor.transform.right, nextBlock.transform.up);
			Debug.Log("angleParentToNode : " + angleParentToNode);
			//! get the node to target angle
			float angleNodeToTarget = AngleDir(nodeAnchor.transform.right, targetDir, nodeAnchor.transform.up);
			Debug.Log("angleNodeToTarget : " + angleNodeToTarget);
			//! the actual angle for the parent to rotate
			float finalAngle = angleParentToNode + angleNodeToTarget;
			Debug.Log("finalAngle : " + finalAngle);
			Vector3 childToParentVector = nextBlock.transform.position - nodeAnchor.transform.position;
			//! rotate the next block to the designated anchor
			nextBlock.transform.rotation = Quaternion.Euler(new Vector3(0.0f, finalAngle, 0.0f));
			//! translate the next block to the designated anchor
			nextBlock.transform.position = lastAnchor.transform.position + childToParentVector;*/
			
			if(!IsColliderOthers(nextBlock, mSpawnList))
			{
				//GameObject obj = (GameObject)Instantiate(nextBlock);
				mSpawnList.Add(nextBlock);
				
				//!test only, should refer to the spawnList
				mPreviousBlock = nextBlock;
				//Debug.Log("nodeCount : " + nodeCount);
				return true;
			}
		}
		return false;
	}
	
	bool IsColliderOthers(GameObject nextBlock, List<GameObject> spawnBlockList)
	{
		for(int i = 0; i < spawnBlockList.Count; i++)
		{	if(spawnBlockList[i] == mPreviousBlock)
			{
				continue;
			}
			MeshCollider meshCollider = spawnBlockList[i].GetComponent<LevelBlock>().GetMeshCollider();
			if(nextBlock.GetComponent<LevelBlock>().GetMeshCollider().bounds.Intersects(meshCollider.bounds))
			{
				return true;
			}
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
	
	//! create a new block
	void CreateNewBlock()
	{
		
	}
}
