using UnityEngine;
using System.Collections.Generic;

public class PathLink
{
	public Vector3 mStart;
	public Vector3 mGoal;
	public NodeAnchor.NODE_DIRECTION mInitialDir;
	public NodeAnchor.NODE_DIRECTION mGoalDir;
	public List<BlockNode> mBlockNodes = new List<BlockNode>();
	//! remember to remove this!!
	public List<BlockNode> mDebugNodes = new List<BlockNode>();
}

public class SearchNode
{
	public Node mNode;
	public SearchNode mParentNode;
    public float mHeuristic; 
	public float mRelativeCost;
	
	public SearchNode()
	{
		mHeuristic = 0.0f;
		mRelativeCost = 0.0f;
	}
}

[System.Serializable]
public class PrefabMap
{
	public GameObject[] mPrefabList = new GameObject[0];
}

public class EventMap : MonoBehaviour
{
	public enum AI_NODE_TYPE
	{
		SMALL,
		BIG,
	}
	
	//! size of the map in num grids
	public int mCol;
	public int mRow;
	//! num segments of the ai node
	int mAiCol;
	int mAiRow;
	
	//! static variable to cache the row and col needed for static function
	public static int sSmallAiCol;
	public static int sSmallAiRow;
	public static int sBigAiCol;
	public static int sBigAiRow;
	
	//! The default tile size(units) of a scale (1, 1, 1) on the parent of the level block in the prefab
	public const int mDefaultTileSize = 1;
	
	//! measure in units eg. 4x4 per tile
	public static int sTileSize = 10;
	
	public static int AiCellLength = 2;
	
	//! all entrance that are not link will be replace with this prefab
	public GameObject mDeadEndPrefab;
	
	//! all connectors between blocks will be connected via this gameobject
	public GameObject mConnectingPillar;
	
	//! the default block when create map button in inspector is pressed
	public GameObject mDefaultBlock;
	
	//! allow the block to be assign via inspector
	public PrefabMap[] mPrefabMap = new PrefabMap[0];
	//! cache the initial block spawned
	public static GameObject[,] sInitBlockSpawned = new GameObject[0,0];
	
	//! The nodes of the map
	Node[,] mNodes = new Node[0,0];
	
	//! The nodes for the AI enemy
	public static Node[,] sSmallAiNodes = new Node[0,0];
	public static Node[,] sBigAiNodes = new Node[0,0];
	
	List<PathLink> mPathLinks = new List<PathLink>();
	
	//! the boundary to spawn on each grid
	public static int mMaxBoundary;
	
	//! for the debugGUI
	bool mDrawLevelNodes;
	bool mDrawPathFindingLink;
	bool mDrawBigAiNodes;
	bool mDrawSmallAiNodes;
	
	//! Level Generator
	LevelGenerator mLevelGenerator;
	
	void Awake()
	{
		mLevelGenerator = GetComponentInChildren<LevelGenerator>();
		CalculateMaxBoundary();
		GenerateMap();
		SpawnInitialBlocks();
		LinkPaths();
		GeneratePath();
		GenerateBigPathNodes();
		GenerateSmallPathNodes();	
		CacheGeneratedBlocks();
	}
	
	//! for event manager to get all the generated event blocks based on row and col
	void CacheGeneratedBlocks()
	{
		foreach(GameObject blocks in mLevelGenerator.mSpawnList)
		{
			//Debug.Log("boundary: " + mMaxBoundary);
			int cellWidth = sTileSize * mMaxBoundary;
			int cellHeight = sTileSize * mMaxBoundary; 
			
			int row = (int)(blocks.transform.position.x / cellWidth);
			int col = (int)(-1 * blocks.transform.position.z / cellHeight);
			
			EventManager.sInitialBlockMap[col,row].mGeneratedBlock.Add(blocks);
		}
	}
	
	//! scale the parent based on the tile size and the default tile size
	public static void ScaleLevelBlock(GameObject levelBlock)
	{
		float parentScale = (float)sTileSize / (float)mDefaultTileSize;
		//Debug.Log("parentScale: " + parentScale);
		levelBlock.transform.localScale = new Vector3(parentScale, parentScale * 0.8f, parentScale);
	}
	
	//! creates the array size require to spawn the initial blocks
	//! this function is access via inspector
	public void CreateMap()
	{
		//! init the "multidimensional array"
		mPrefabMap = new PrefabMap[mCol];
		//Debug.Log("Length of the blockmap: " + mPrefabMap.Length);
		for(int i = 0; i < mPrefabMap.Length; i++)
		{
			PrefabMap prefabMap = new PrefabMap();
			prefabMap.mPrefabList = new GameObject[mRow];
			mPrefabMap[i] = prefabMap;
		}
		
		//! assign all space with the default block
		for(int i = 0; i < mPrefabMap.Length; i++)
		{
			for(int j = 0; j < mPrefabMap[i].mPrefabList.Length; j++)
			{
				mPrefabMap[i].mPrefabList[j] = mDefaultBlock;
			}
		}
	}
	
	//! calculate the biggest boundary using the biggest block in the prefabMap
	public void CalculateMaxBoundary()
	{
		GameObject objTarget;
		float currArea = 0.0f;
		//! instantiate phase..
		sInitBlockSpawned = new GameObject[mCol,mRow];
		//! for the eventManager(hardcode)
		EventManager.sInitialBlockMap = new InitialBlock[mCol,mRow];
		
		for(int i = 0; i < mPrefabMap.Length; i++)
		{
			for(int j = 0; j < mPrefabMap[i].mPrefabList.Length; j++)
			{
				GameObject prefabObj = mPrefabMap[i].mPrefabList[j];
				if(prefabObj == null) continue;
				objTarget = (GameObject)Instantiate(prefabObj);
				
				//! scale the level block 
				ScaleLevelBlock(objTarget);
				
				float width = objTarget.collider.bounds.size.x / sTileSize;
				float height = objTarget.collider.bounds.size.z / sTileSize;
				float objArea = width * height;
				mLevelGenerator.mSpawnList.Add(objTarget);
				sInitBlockSpawned[i,j] = objTarget;
				
				if(objArea > currArea)
				{
					currArea = objArea;
					//! for now!
					mMaxBoundary = (int)(width + height) + 2;
				}
			}
		}
	}
	
	//! as name suggested generates the nodes for level block placement later
	void GenerateMap()
	{
		int column = mMaxBoundary * mCol;
		int row = mMaxBoundary * mRow;
		
		mNodes = new Node[column,row];
		//j = row, i = col (row and col is twisted):(
		
		int count = 0;
		
		for(int i = 0; i < column; i++)
		{
			for(int j = 0; j < row; j++)
			{
				Node node = new Node();
				node.mPoint = new Vector3(j * sTileSize, 0.0f, -i * sTileSize);
				mNodes[i,j] = node;
				count++;
			}
		}
		
		float moveCost = 10.0f;
		
		for(int i = 0; i < column; i++)
		{
			for(int j = 0; j < row; j++)
			{
				Node currNode = mNodes[i,j];
				//! link right
				if(j + 1 < row)
				{
					Node rightNode = mNodes[i,j + 1];
					NodeLink neighbourLink = new NodeLink(j + 1, i, moveCost);
					NodeLink currLink = new NodeLink(j, i,moveCost);
					currNode.mNodeLinkNeighbour.Add(neighbourLink);
					rightNode.mNodeLinkNeighbour.Add(currLink);
				}
				//! link down
				if(i + 1 < column)
				{
					Node bottomNode = mNodes[i + 1,j];
					NodeLink neighbourLink = new NodeLink(j, i + 1, moveCost);
					NodeLink currLink = new NodeLink(j, i,moveCost);
					currNode.mNodeLinkNeighbour.Add(neighbourLink);
					bottomNode.mNodeLinkNeighbour.Add(currLink);
				}
			}
		}
	}
	
	void SpawnInitialBlocks()
	{
		GameObject obj;
		int counterCount = 0;
		for(int i = 1; i <= mCol; i++)
		{
			for(int j = 0; j < mRow; j++)
			{
				//! if it is null
				//if(j + (i - 1) * mRow >= mLevelGenerator.mSpawnList.Count)continue;
				if(sInitBlockSpawned[i - 1, j] == null)continue;
				
				//obj = mLevelGenerator.mSpawnList[j + (i - 1) * mRow];
				obj = sInitBlockSpawned[i - 1, j];
				
				//! get the node which is btm left of each boundary
				int col = i * mMaxBoundary - 1;
				int row = j * mMaxBoundary;
				//! get the randomize col and row
				int randRow = Random.Range(j * mMaxBoundary, j * mMaxBoundary + mMaxBoundary);
				int randCol = Random.Range((i - 1) * mMaxBoundary, i * mMaxBoundary);
				//int id = (i - 1) + j * mCol;
//				Debug.Log("block ID: " + id);
//				Debug.Log("randCol: " + randCol);
//				Debug.Log("randRow: " + randRow);
//				Debug.Log("maxBoundary: " + mMaxBoundary);
				
				//! position the block
				obj.transform.position = mNodes[randCol, randRow].mPoint;
				
				//! check if the block will intersect the boundary and if yes relocate again
				while(!IsInTheSpawnBoundary(col,row,obj))
				{
					randRow = Random.Range(j * mMaxBoundary, j * mMaxBoundary + mMaxBoundary);
					randCol = Random.Range((i - 1) * mMaxBoundary, i * mMaxBoundary);
					obj.transform.position = mNodes[randCol, randRow].mPoint;
					counterCount++;
					
					if(counterCount > 1000)	
					{
						Debug.LogError("Infinite Loop occur");
						return;
					}
				}
				
				obj.collider.enabled = false;
			}
		}
	}
	
	void CreateDeadEnd(GameObject nodeAnchor)
	{
		//! spawn dead end here
		GameObject deadEnd = (GameObject)Instantiate(mDeadEndPrefab,Vector3.zero,Quaternion.identity);
		//! scale
		ScaleLevelBlock(deadEnd);
		//! rotate
		Vector3 targetDir = -nodeAnchor.transform.forward;
		float rotateAngle = GetAngleHelper.GetAngle(targetDir,mDeadEndPrefab.transform.forward,Vector3.up);
		Quaternion rot = Quaternion.Euler(new Vector3(0.0f,rotateAngle,0.0f));
		deadEnd.transform.rotation = rot;
		//! translate
		deadEnd.transform.position = nodeAnchor.transform.position + (nodeAnchor.transform.forward * (sTileSize * 0.5f));
	}
	
	void CreatePillarConnectors(int nodeDirectionId, Vector3 position)
	{
		GameObject pillar = (GameObject)Instantiate(mConnectingPillar,Vector3.zero,Quaternion.identity);
		ScaleLevelBlock(pillar);
		//! world space
		float angle = NodeAnchor.GetAngle(nodeDirectionId);
		Debug.Log("Angle: " + angle);
		Quaternion rot = Quaternion.Euler(new Vector3(0.0f,angle,0.0f));
		pillar.transform.rotation = rot;
		pillar.transform.position = position + (NodeAnchor.GetDirectionInVector(nodeDirectionId) * (sTileSize * 0.5f));
	}
	
	void LinkPaths()
	{
		GameObject currentBlock;
		List<GameObject> anchorList = new List<GameObject>();
		List<GameObject> anchorCheckList = new List<GameObject>();
		List<NodeAnchor> neighbourAnchors = new List<NodeAnchor>();
		
		for(int i = 0; i < mCol; i++)
		{
			for(int j = 0; j < mRow; j++)
			{
				currentBlock = sInitBlockSpawned[i,j];
				if(currentBlock == null)continue;
				//! Get all the node anchors
				anchorList = currentBlock.GetComponent<LevelBlock>().ExitNodeAnchorGameObjects();
				
				foreach(GameObject nodeAnchor in anchorList)
				{
					bool successLink = false;
					
					if(anchorCheckList.Contains(nodeAnchor))
					{
						continue;
					}
					
					NodeAnchor.NODE_DIRECTION direction = nodeAnchor.GetComponent<NodeAnchor>().mNodeDirection;
					//! get the neighbour block based on the direction
					GameObject neighbourBlock = GetNeighbourBlock(direction, j, i);
					//! if there is no block located based on the current anchor direction
					if(!neighbourBlock)
					{
						//! create dead end if his next neighbour is null
						CreateDeadEnd(nodeAnchor);
						continue;
					}
					
					//! Get all neighbour block anchors
					neighbourAnchors = neighbourBlock.GetComponent<LevelBlock>().GetAllNodeAnchor();
					
					//! check if there is an oppose anchor with the currentBlock
					foreach(NodeAnchor neighbourAnchor in neighbourAnchors)
					{
						//based on the enum abs(currDir - neighbourDir) = 2
						int result = Mathf.Abs((int)direction - (int)neighbourAnchor.mNodeDirection);
						if(result == 2)
						{
							//! map the event manager map
							EventManager.LinkInitialBlock(currentBlock,i,j,direction);
							
							//! create a path link
							PathLink pathLink = new PathLink();
							pathLink.mStart = nodeAnchor.transform.position + nodeAnchor.transform.forward * sTileSize;
							pathLink.mGoal = neighbourAnchor.transform.position + neighbourAnchor.transform.forward * sTileSize;
							
							pathLink.mInitialDir = direction;
							pathLink.mGoalDir = neighbourAnchor.GetComponent<NodeAnchor>().mNodeDirection;
							mPathLinks.Add(pathLink);
							
							//! add to the checkList
							anchorCheckList.Add(nodeAnchor);
							anchorCheckList.Add(neighbourAnchor.gameObject);
							successLink = true;
							break;
						}
					}
					
					if(!successLink)
					{
						CreateDeadEnd(nodeAnchor);
					}
				}
			}
		}
		
		//! Create path
		for(int i = 0; i < mPathLinks.Count; i++)
		{
			mPathLinks[i].mBlockNodes = GetPath(mPathLinks[i].mStart, mPathLinks[i].mGoal,mNodes);
//			for(int j = 0; j < mPathLinks[i].mBlockNodes.Count;j++)
//			{
//				mLevelGenerator.CreateNewBlock(mPathLinks[i].mBlockNodes[j].mPoint);
//			}
			//! REMEMBER TO REMOVE THIS!
			mPathLinks[i].mDebugNodes = GetPath(mPathLinks[i].mStart, mPathLinks[i].mGoal,mNodes);
		}
	}
	
	void GeneratePath()
	{
		foreach(PathLink pathLink in mPathLinks)
		{
			int startDir = (int)pathLink.mInitialDir;
			int finalGoalDir = (int)pathLink.mGoalDir;
			List<BlockNode> blockNodes = pathLink.mBlockNodes;
			
			int loopCounter = 0;
			
			Debug.Log("Total Count: " + blockNodes.Count);
			
			while(blockNodes.Count > 0)
			{
				int steps;
				//! prevent the blocknode list to have 1 node
				if(blockNodes.Count <= 2)
				{
					steps = blockNodes.Count;
				}
				else if(blockNodes.Count == 3)
				{
					steps = 3;
				}
				else if(blockNodes.Count == 4)
				{
					//steps = 4;
					steps = 2;
				}
//				else if(blockNodes.Count == 5)
//				{
//					//! get steps 2 - 3
//					steps = Random.Range(2,4);
//				}
				else
				{
					//! get steps 2 - 4
					//steps = Random.Range(2,5);
					steps = Random.Range(2,4);
				}
				
				
				//! mark the first node to have a startDir id
				blockNodes[0].id = NodeAnchor.GetReverseDirection(startDir);//reverse
				CreatePillarConnectors(blockNodes[0].id,blockNodes[0].mPoint);
				
				int currGoalDir = 0;
				int minRow = 1;
				int minCol = 1;
					
				//! get the minimum of row and col(size of block) needed to spawn
				for(int i = 0; i < steps - 1; i++)
				{
					Vector3 resultant = blockNodes[i + 1].mPoint - blockNodes[i].mPoint;
					float resultantX = Mathf.Abs(resultant.x);
					float resultantZ = Mathf.Abs(resultant.z);
					if(resultantX > resultantZ) minRow += 1;
					if(resultantZ > resultantX) minCol += 1;
					
					Debug.Log("resultant.x: " + resultantX + " resultant.z: " + resultantZ);
				}
				
				//! Get the goal dir if the current goal is not the last node(finalGoalNode)
				if(steps < blockNodes.Count - 1)
				{
					Vector3 resultant = blockNodes[steps].mPoint - blockNodes[steps - 1].mPoint;
					Vector3.Normalize(resultant);
					if(resultant.x > 0.0f)currGoalDir = (int)NodeAnchor.NODE_DIRECTION.EAST;
					if(resultant.x < 0.0f)currGoalDir = (int)NodeAnchor.NODE_DIRECTION.WEST;
					if(resultant.z > 0.0f)currGoalDir = (int)NodeAnchor.NODE_DIRECTION.NORTH;
					if(resultant.z < 0.0f)currGoalDir = (int)NodeAnchor.NODE_DIRECTION.SOUTH;
					
					blockNodes[steps - 1].id = currGoalDir;
				}
				else
				{
					blockNodes[steps - 1].id = NodeAnchor.GetReverseDirection(finalGoalDir);//reverse
					CreatePillarConnectors(blockNodes[steps - 1].id,blockNodes[steps - 1].mPoint);
				}
				
				Debug.Log("Steps: " + steps);
				
				//do level generation here
				mLevelGenerator.CreateNewBlock(blockNodes,minRow,minCol,steps);
				
				//! remove the blocknode what has been generated
				for(int i = 0; i < steps; i++)
				{
//					Debug.Log("The ID: " + (NodeAnchor.NODE_DIRECTION)blockNodes[0].id);
//					Debug.Log("position" + blockNodes[0].mPoint);
				
					blockNodes.RemoveAt(0);
				}
				//Debug.Log("================================");
				startDir = currGoalDir;
				
				loopCounter++;
				if(loopCounter > 100)
				{
					Debug.LogError("Infinite loop in GeneratePath()");
					return;
				}
				
				//! to test by generating 1 block only (FOR DEBUG PURPOSES!)
				//return;
			}
		}
		
		//! disable all the spawn block measurement
		foreach(GameObject obj in mLevelGenerator.mSpawnList)
		{
			obj.collider.enabled = false;
		}
	}
	
	void GenerateBigPathNodes()
	{
		int column = mMaxBoundary * mCol;
		int row = mMaxBoundary * mRow;
		
		sBigAiNodes = new Node[column,row];
		//j = row, i = col (row and col is twisted):(
		
		sBigAiCol = column;
		sBigAiRow = row;
		
		for(int i = 0; i < column; i++)
		{
			for(int j = 0; j < row; j++)
			{
				Vector3 point = new Vector3(j * sTileSize, 0.0f, -i * sTileSize);
				if(Physics.CheckSphere(point,2.0f))
				{
					if(!Physics.CheckSphere(point,0.5f,1 << LayerMask.NameToLayer("Environment")))
					{
						Node node = new Node();
						node.mPoint = point;
						sBigAiNodes[i,j] = node;
					}
				}
			}
		}
		
		float horizontalMoveCost = sTileSize;
		float diagonalMoveCost = Mathf.Sqrt((sTileSize * sTileSize) + (sTileSize * sTileSize));
		Vector3 offSetY = Vector3.up;
		//! link the paths
		for(int i = 0; i < column; i++)
		{
			for(int j = 0; j < row; j++)
			{
				Node currNode = sBigAiNodes[i,j];
				if(currNode == null)continue;
				
				//! link right
				if(j + 1 < row && sBigAiNodes[i,j + 1] != null)
				{
					if(!TestRayHit(currNode.mPoint + offSetY,sBigAiNodes[i,j + 1].mPoint + offSetY))
					{
						Node rightNode = sBigAiNodes[i,j + 1];
						NodeLink neighbourLink = new NodeLink(j + 1,i,horizontalMoveCost);
						NodeLink currLink = new NodeLink(j,i,horizontalMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						rightNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
				
				//! link bottom right
				if(j + 1 < row && i + 1 < column && sBigAiNodes[i + 1,j + 1] != null)
				{
					if(!TestRayHit(currNode.mPoint + offSetY,sBigAiNodes[i + 1,j + 1].mPoint + offSetY))
					{
						Node btmRightNode = sBigAiNodes[i + 1,j + 1];
						NodeLink neighbourLink = new NodeLink(j + 1,i + 1,diagonalMoveCost);
						NodeLink currLink = new NodeLink(j,i,diagonalMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						btmRightNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
				
				//! link bottom
				if(i + 1 < column && sBigAiNodes[i + 1,j] != null)
				{
					if(!TestRayHit(currNode.mPoint + offSetY,sBigAiNodes[i + 1,j].mPoint + offSetY))
					{
						Node btmNode = sBigAiNodes[i + 1,j];
						NodeLink neighbourLink = new NodeLink(j,i + 1,horizontalMoveCost);
						NodeLink currLink = new NodeLink(j,i,horizontalMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						btmNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
			
				//! link bottom left
				if(j - 1 >= 0 && i + 1 < column && sBigAiNodes[i + 1,j - 1] != null)
				{
					if(!TestRayHit(currNode.mPoint + offSetY,sBigAiNodes[i + 1,j - 1].mPoint + offSetY))
					{
						Node btmLeftNode = sBigAiNodes[i + 1,j - 1];
						NodeLink neighbourLink = new NodeLink(j - 1,i + 1,diagonalMoveCost);
						NodeLink currLink = new NodeLink(j,i,diagonalMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						btmLeftNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
			}
		}
		
	}
	
	void GenerateSmallPathNodes()
	{
		int col = mCol * mMaxBoundary;
		int row = mRow * mMaxBoundary;
		float bigCellHalfLength = sTileSize * 0.5f;
		//! get the top left point of the map
		Node node = mNodes[0,0];
		Vector3 topLeft = new Vector3(node.mPoint.x, 0, node.mPoint.z);
		//! get the btm left point of the map
		node = mNodes[col - 1, 0];
		Vector3 btmLeft = new Vector3(node.mPoint.x, 0, node.mPoint.z);
		//! get the top right point of the map
		node = mNodes[0, row - 1];
		Vector3 topRight = new Vector3(node.mPoint.x, 0, node.mPoint.z);
		
		float cellWidth = AiCellLength;//Mathf.Round(topRight.x - topLeft.x)/ mAiRow;
		float cellHeight = AiCellLength;//Mathf.Round(topLeft.z - btmLeft.z) / mAiCol;
		float halfCellWidth = cellWidth * 0.5f;
		float halfCellHeight = cellHeight * 0.5f;
		mAiCol = (int)Mathf.Round(topLeft.z - btmLeft.z);
		mAiRow = (int)Mathf.Round(topRight.x - topLeft.x);
		
		//! cache the max col and max row into a static variable for static function use
		sSmallAiCol = mAiCol;
		sSmallAiRow = mAiRow;
		
		sSmallAiNodes = new Node[mAiCol,mAiRow];
		
		for(int i = 0; i < mAiCol; i++)
		{
			for(int j = 0; j < mAiRow; j++)
			{
				Vector3 point = new Vector3(topLeft.x + halfCellWidth + cellWidth * j, 1.0f, topLeft.z - halfCellHeight - cellHeight * i);
				//! if there is a wall
				if(Physics.CheckSphere(point, 0.1f, 1 << LayerMask.NameToLayer("Environment")))
				{
					//0.5f * cellWidth
					//Debug.Log("Hit the environment: " + point);
					continue;
				}
				
				RaycastHit hit;
				//! if there is no floor
				if(!Physics.SphereCast(point,0.3f,Vector3.down,out hit,Mathf.Infinity))
				{
					continue;
				}
				
				Node newNode = new Node();
				newNode.mPoint = new Vector3(topLeft.x + halfCellWidth + cellWidth * j, 1.0f, topLeft.z - halfCellHeight - cellHeight * i);
				sSmallAiNodes[i,j] = newNode;
			} 
		}
		
		float straightMoveCost = AiCellLength;
		float diagonalMoveCost = Mathf.Sqrt((AiCellLength * AiCellLength) + (AiCellLength * AiCellLength));
		//int numTime = 0;
		//! linking all the ai node
		for(int i = 0; i < mAiCol; i++)
		{
			for(int j = 0; j < mAiRow; j++)
			{
				Node currNode = sSmallAiNodes[i,j];
				if(currNode == null)continue;
				
				//! link right 
				if(j + 1 < mAiRow && sSmallAiNodes[i, j + 1] != null)
				{
					if(!TestRayHit(currNode.mPoint, sSmallAiNodes[i, j + 1].mPoint))
					{
						//numTime += 1;
						Node rightNode = sSmallAiNodes[i,j + 1];
						NodeLink neighbourLink = new NodeLink(j + 1, i, straightMoveCost);
						NodeLink currLink = new NodeLink(j, i, straightMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						rightNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
				
				//! link bottom right
				if(i + 1 < mAiCol && j + 1 < mAiRow && sSmallAiNodes[i + 1, j + 1] != null)
				{
					if(!TestRayHit(currNode.mPoint, sSmallAiNodes[i + 1, j + 1].mPoint))
					{
						Node btmRightNode = sSmallAiNodes[i + 1,j + 1];
						NodeLink neighbourLink = new NodeLink(j + 1, i + 1, diagonalMoveCost);
						NodeLink currLink = new NodeLink(j, i, diagonalMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						btmRightNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
				
				//! link bottom left
				if(i + 1 < mAiCol && j - 1 >= 0 && sSmallAiNodes[i + 1, j - 1] != null)
				{
					if(!TestRayHit(currNode.mPoint, sSmallAiNodes[i + 1, j - 1].mPoint))
					{
						Node btmLeftNode = sSmallAiNodes[i + 1,j - 1];
						NodeLink neighbourLink = new NodeLink(j - 1, i + 1, diagonalMoveCost);
						NodeLink currLink = new NodeLink(j, i, diagonalMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						btmLeftNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
				
				//! link bottom
				if(i + 1 < mAiCol && sSmallAiNodes[i + 1, j] != null)
				{
					if(!TestRayHit(currNode.mPoint, sSmallAiNodes[i + 1, j].mPoint))
					{
						Node btmNode = sSmallAiNodes[i + 1,j];
						NodeLink neighbourLink = new NodeLink(j, i + 1, straightMoveCost);
						NodeLink currLink = new NodeLink(j, i, straightMoveCost);
						currNode.mNodeLinkNeighbour.Add(neighbourLink);
						btmNode.mNodeLinkNeighbour.Add(currLink);
					}
				}
			}
		}
		
		//! Assign path clearance
		for(int i = 0; i < mAiCol; i++)
		{
			for(int j = 0; j < mAiRow; j++)
			{
				Node targetNode = sSmallAiNodes[i,j];
				if(targetNode == null)
				{
					continue;
				}
				//! flag if the checking encounter an obstacle
				bool hitObstacle = false;
				int currClearance = 1;
				int neighbourIndex = 1;
				//! infinite loop checking
				int counter = 0;
				
				while(!hitObstacle)
				{
					//Debug.Log("col: " + i + " row: " + j);
					int colIndex = i + neighbourIndex;
					int rowIndex = j + neighbourIndex;
					Node startingNode = null;
					//! the starting node is the most btmright of the current neighbourIndex checking
					if(colIndex < mAiCol && rowIndex < mAiRow)
					{
						startingNode = sSmallAiNodes[colIndex, rowIndex];
					}
					if(startingNode == null)
					{
						targetNode.mClearanceSize = currClearance * AiCellLength;
						break;
					}
					
					//! check with neighbour by going up & left of the tile grid based on the starting node
					for(int k = 1; k <= neighbourIndex; k++)
					{
						if(rowIndex - k >= 0 && colIndex - k >= 0)
						{
							Node upNeighbourNode = sSmallAiNodes[colIndex - k, rowIndex];
							Node leftNeighbourNode = sSmallAiNodes[colIndex, rowIndex - k];
							if(upNeighbourNode == null || leftNeighbourNode == null)
							{
								//! found an obstacle
								targetNode.mClearanceSize = currClearance * AiCellLength;
								hitObstacle = true;
								break;
							}	
						}
					}
					
					//! if there is no obstacle in sight, expand search and clearance size
					if(!hitObstacle)
					{
						currClearance += 1;
						neighbourIndex += 1;
					}
					
					if(counter > 1000000)
					{
						Debug.LogError("Infinite Loop!");
						return;
					}
				}
			}
		}
		
//		//! check all node that doesn't have any neighbour remove it
//		for(int i = 0; i < mAiCol; i++)
//		{
//			for(int j = 0; j < mAiRow; j++)
//			{
//				Node currNode = sSmallAiNodes[i,j];
//				if(currNode == null)continue;
//				//Debug.Log(currNode.mClearanceSize);
//				if(currNode.mClearanceSize <= 0)
//				{
//					currNode = null;
//				}
//			}
//		}
	}
	
	//! check if the spawn is in the boundary
	bool IsInTheSpawnBoundary(int col, int row, GameObject gameObject)
	{
		//! obj pos is offset to the btm left instead of the center
		//! boundary data
		Vector3 offset = new Vector3 (sTileSize * 0.5f, 0.0f, sTileSize * 0.5f);
		Vector3 boundaryPos = mNodes[col,row].mPoint + offset;
		float boundaryLength = (mMaxBoundary - 2) * sTileSize;
		
		//! obj data
		Vector3 objPos = gameObject.transform.position - gameObject.collider.bounds.size * 0.5f;
		float objWidth = gameObject.collider.bounds.size.x;
		float objHeight = gameObject.collider.bounds.size.z;
		
//		Debug.Log("Pos: " + objPos);
//		Debug.Log("objHeight" + objHeight);
//		Debug.Log("objWidth" + objWidth);
		//! if inside the boundary
		if(objPos.x >= boundaryPos.x && objPos.x + objWidth <= boundaryPos.x + boundaryLength &&
			objPos.z >= boundaryPos.z && objPos.z + objHeight <= boundaryPos.z + boundaryLength)
		{
			//Debug.Log("Success");
			return true;
		}
		else
		{
			//Debug.Log("fail");
			return false;
		}
	}
	
	bool TestRayHit(Vector3 start, Vector3 end)
	{
		RaycastHit result;
		Vector3 dir = end - start;
		float dist = dir.magnitude;
		int targetLayer = 1 << LayerMask.NameToLayer("Environment");
		dir.Normalize();
		
		if(Physics.SphereCast(start, 0.1f, dir, out result,dist,targetLayer))
		{
			return true;
		}
		if(Physics.SphereCast(end, 0.1f, -dir, out result,dist,targetLayer))
		{
			return true;
		}
		
		return false;	
	}
	
	//! get the node based on the position
	public Node GetNode(Vector3 pos, Node[,] nodeArray)
	{
		int row = (int) Mathf.Round(pos.x) / sTileSize;
		int col = (int)Mathf.Round((-1 * pos.z)) / sTileSize;
		
		return nodeArray[col,row];
	}
	
	//! please use this for AI purpose only
	public static Node GetSmallNodeAI(Vector3 pos, Node[,] nodeArray, int size)
	{
		//! for every node of Ai cell is one unit apart
		int row = (int)Mathf.Round(pos.x / AiCellLength);
		int col = (int)Mathf.Round((-1 * pos.z) / AiCellLength);
//		Debug.Log("row: " + row);
//		Debug.Log("col: " + col);
		
		Node foundNode = nodeArray[col,row];
		if(foundNode != null) 
		{
			if(size <= foundNode.mClearanceSize)
			{
				return foundNode;
			}
		}
		
		//for no valid node. Search by spreading
		int colStart = col - 1;
		int colEnd = col + 2;
		int rowStart = row - 1;
		int rowEnd = row + 2;
		
		while (colStart >= 0 || colEnd < sSmallAiCol || rowStart >= 0 || rowEnd < sSmallAiRow)	
		{
			float closestDist = Mathf.Infinity;
			Node closestNode = null;
			for(int i = colStart; i < colEnd; i++)
			{
				//skip when out of bound
				if(i < 0 || i >= sSmallAiCol) continue;
				
				for(int j = rowStart; j < rowEnd; j++)
				{
					//skip when out of bound
					if(j < 0 || j >= sSmallAiRow) continue;
					
					//! skip the middle node
					if(j > rowStart && j < (rowEnd - 1)
					&& i > colStart && i < (colEnd - 1))continue;
					
					foundNode = nodeArray[i,j];
					if(foundNode != null)
					{
						if(size > foundNode.mClearanceSize)continue;
						float distance = Vector3.SqrMagnitude(pos - foundNode.mPoint);
						if(distance < closestDist)
						{
							closestDist = distance;
							closestNode = foundNode;
						}
					}
				}
			}
			
			if(closestNode != null)return closestNode;
			
			--colStart;
			--rowStart;
			++colEnd;
			++rowEnd;
		}
		return null;
	}
	
	public static Node GetBigNodeAI(Vector3 pos, Node[,] nodeArray, int size)
	{
		//! for every node of Ai cell is one unit apart
		int row = (int)Mathf.Round(pos.x / sTileSize);
		int col = (int)Mathf.Round((-1 * pos.z) / sTileSize);
//		Debug.Log("row: " + row);
//		Debug.Log("col: " + col);
		
		Node foundNode = nodeArray[col,row];
		if(foundNode != null) 
		{
			if(size <= foundNode.mClearanceSize)
			{
				return foundNode;
			}
		}
		
		//for no valid node. Search by spreading
		int colStart = col - 1;
		int colEnd = col + 2;
		int rowStart = row - 1;
		int rowEnd = row + 2;
		
		while (colStart >= 0 || colEnd < sBigAiCol || rowStart >= 0 || rowEnd < sBigAiRow)	
		{
			float closestDist = Mathf.Infinity;
			Node closestNode = null;
			for(int i = colStart; i < colEnd; i++)
			{
				//skip when out of bound
				if(i < 0 || i >= sBigAiCol) continue;
				
				for(int j = rowStart; j < rowEnd; j++)
				{
					//skip when out of bound
					if(j < 0 || j >= sBigAiRow) continue;
					
					//! skip the middle node
					if(j > rowStart && j < (rowEnd - 1)
					&& i > colStart && i < (colEnd - 1))continue;
					
					foundNode = nodeArray[i,j];
					if(foundNode != null)
					{
						if(size > foundNode.mClearanceSize)continue;
						float distance = Vector3.SqrMagnitude(pos - foundNode.mPoint);
						if(distance < closestDist)
						{
							closestDist = distance;
							closestNode = foundNode;
						}
					}
				}
			}
			
			if(closestNode != null)return closestNode;
			
			--colStart;
			--rowStart;
			++colEnd;
			++rowEnd;
		}
		return null;
	}
	
	static bool CheckInSearchNode(List<SearchNode> searchNodeList, int row, int col, Node[,] nodeArray)
	{
		foreach(SearchNode searchNode in searchNodeList)
		{
			if(searchNode.mNode == nodeArray[col,row])
			{
				return true;
			}
		}
		return false;
	}
	
	static float GetRelativeCost(List<SearchNode> searchNodeList, int row, int col, Node[,] nodeArray)
	{
		foreach(SearchNode searchNode in searchNodeList)
		{
			if(searchNode.mNode == nodeArray[col,row])
			{
				return searchNode.mRelativeCost;
			}
		}
		
		Debug.LogWarning("The GetRelativeCost function is not working properly!");
		return 0.0f;
	}
	
	static SearchNode GetSearchNode(List<SearchNode> searchNodeList, int row, int col, Node[,] nodeArray)
	{
		foreach(SearchNode searchNode in searchNodeList)
		{
			if(searchNode.mNode == nodeArray[col,row])
			{
				return searchNode;
			}
		}
		
		Debug.LogWarning("The GetSearchNode function is returning null!");
		return null;
	}
	
	//! pathfinding for the link (return with reversing the list)
	public List<BlockNode> GetPath(Vector3 start, Vector3 goal, Node[,] nodeArray)
	{
		List<SearchNode> openList = new List<SearchNode>();
		List<SearchNode> closeList = new List<SearchNode>();
		Node startNode = GetNode(start,nodeArray);
		Node goalNode = GetNode(goal,nodeArray);
		//Debug.Log(goalNode.mPoint);
		SearchNode currentNode = new SearchNode();
		currentNode.mNode = startNode;
		//add the start node into the OPEN
		openList.Add(currentNode);
		
		int infiniteCounter = 0;
		
		while(currentNode.mNode != goalNode && openList.Count > 0)
		{
			//switch open to close
			//Debug.Log("OpenList count: " + openList.Count);
			//Debug.Log("The value: " + openList.First.Value);
			closeList.Add(currentNode);
			openList.Remove(currentNode);
			
			foreach(NodeLink nodeLink in currentNode.mNode.mNodeLinkNeighbour)
			{
				int row = nodeLink.neighbourRow;
				int col = nodeLink.neighbourCol;
				float costToNeighbour = currentNode.mRelativeCost + nodeLink.cost;
				
				//! if the neighbour is in close, ignore
				if(CheckInSearchNode(closeList,row,col,nodeArray))continue;
				
				//! if the neighbour is in open and cost less than neighbour
				if(CheckInSearchNode(openList,row,col,nodeArray) && costToNeighbour < GetRelativeCost(openList,row,col,nodeArray))
				{
					openList.Remove(GetSearchNode(openList,row,col,nodeArray));
				}
				
				//! if the neighbour is neither in open or close
				if(!CheckInSearchNode(openList,row,col,nodeArray))
				{
					SearchNode sNode = new SearchNode();
					sNode.mNode = mNodes[col,row];
					sNode.mRelativeCost = costToNeighbour;
					sNode.mParentNode = currentNode;
					sNode.mHeuristic = Vector3.Distance(nodeArray[col,row].mPoint, goalNode.mPoint);
					openList.Add(sNode);
				}
			}
			
			float lowestNode = Mathf.Infinity;
			foreach(SearchNode searchNode in openList)
			{
				float rank = searchNode.mRelativeCost + searchNode.mHeuristic;
				if(rank < lowestNode)
				{
					currentNode = searchNode;
					lowestNode = rank;
				}
			}
			
			infiniteCounter++;
			if(infiniteCounter > 1000000)
			{
				Debug.LogError("Infinite loop occured in GetPath(Vector3 start, Vector3 goal)");
				break;
			}
		}
		
		openList.Remove(currentNode);
		closeList.Add(currentNode);
		
		List<BlockNode> resultPath = new List<BlockNode>();
		SearchNode nodeToBeCheck = currentNode;
		
		BlockNode blockNode = new BlockNode();
		blockNode.mPoint = currentNode.mNode.mPoint;
		blockNode.id = 1;
		
		resultPath.Add(blockNode);
		infiniteCounter = 0;
		
		while(nodeToBeCheck.mNode != startNode)
		{
			nodeToBeCheck = nodeToBeCheck.mParentNode;
			
			BlockNode blkNode = new BlockNode();
			blkNode.mPoint = nodeToBeCheck.mNode.mPoint;
			blkNode.id = 1;
			
			resultPath.Add(blkNode);
			
			infiniteCounter++;
			if(infiniteCounter > 100)
			{
				Debug.LogError("Infinite loop occured in GetPath(Vector3 start, Vector3 goal)");
				break;
			}
		}
		resultPath.Reverse();
		
		return resultPath;
	}
	
	//! pathfinding for general use (return without reversing the result path)
	public static List<Vector3> GetPathAi(Vector3 start, Vector3 goal, Node[,] nodeArray, int size, AI_NODE_TYPE type)
	{
		List<SearchNode> openList = new List<SearchNode>();
		List<SearchNode> closeList = new List<SearchNode>();
		
		Node startNode = null;
		Node goalNode = null;
		
		if(type == AI_NODE_TYPE.SMALL)
		{
			startNode = GetSmallNodeAI(start,nodeArray,size);
			goalNode = GetSmallNodeAI(goal,nodeArray,size);
			TestAStarScript.startNode = startNode;
		}
		else
		{
			startNode = GetBigNodeAI(start,nodeArray,size);
			goalNode = GetBigNodeAI(goal,nodeArray,size);
			TestAStarScript.startNode = startNode;
		}
		//Debug.Log(goalNode.mPoint);
		SearchNode currentNode = new SearchNode();
		currentNode.mNode = startNode;
		//add the start node into the OPEN
		openList.Add(currentNode);
		
		int infiniteCounter = 0;
		
		while(currentNode.mNode != goalNode && openList.Count > 0)
		{
			//switch open to close
			//Debug.Log("OpenList count: " + openList.Count);
			//Debug.Log("The value: " + openList.First.Value);
			closeList.Add(currentNode);
			openList.Remove(currentNode);
			
			foreach(NodeLink nodeLink in currentNode.mNode.mNodeLinkNeighbour)
			{
				int row = nodeLink.neighbourRow;
				int col = nodeLink.neighbourCol;
				float costToNeighbour = currentNode.mRelativeCost + nodeLink.cost;
				
				//! if the neighbour is in close, ignore
				if(CheckInSearchNode(closeList,row,col,nodeArray))continue;
				
				//! if the neighbour is in open and cost less than neighbour
				if(CheckInSearchNode(openList,row,col,nodeArray) && costToNeighbour < GetRelativeCost(openList,row,col,nodeArray))
				{
					openList.Remove(GetSearchNode(openList,row,col,nodeArray));
				}
				
				//! if the neighbour is neither in open or close
				if(!CheckInSearchNode(openList,row,col,nodeArray) && size <= nodeArray[col,row].mClearanceSize)
				{
					SearchNode sNode = new SearchNode();
					sNode.mNode = nodeArray[col,row];
					sNode.mRelativeCost = costToNeighbour;
					sNode.mParentNode = currentNode;
					sNode.mHeuristic = Vector3.Distance(nodeArray[col,row].mPoint, goalNode.mPoint);
					openList.Add(sNode);
				}
			}
			
			float lowestNode = Mathf.Infinity;
			foreach(SearchNode searchNode in openList)
			{
				float rank = searchNode.mRelativeCost + searchNode.mHeuristic;
				if(rank < lowestNode)
				{
					currentNode = searchNode;
					lowestNode = rank;
				}
			}
			
			infiniteCounter++;
			if(infiniteCounter > 1000000)
			{
				Debug.LogError("Infinite loop occured in GetPath(Vector3 start, Vector3 goal)");
				break;
			}
		}
		
		openList.Remove(currentNode);
		closeList.Add(currentNode);
		
		//! DEBUGGING
		TestAStarScript.mOpenList = openList;
		TestAStarScript.mCloseList = closeList;
		
		List<Vector3> resultPath = new List<Vector3>();
		SearchNode nodeToBeCheck = currentNode;
		
		if(currentNode.mNode == null)
		{
			Debug.LogError("HEY YOUR NODE IS NULL!");
		}
		//Debug.LogWarning("currentNode: " + currentNode.mNode.mPoint);
		
		resultPath.Add(currentNode.mNode.mPoint);
		infiniteCounter = 0;
		
		while(nodeToBeCheck.mNode != startNode)
		{
			nodeToBeCheck = nodeToBeCheck.mParentNode;
			
			//! do not include the start node
			if(nodeToBeCheck.mNode == startNode)break;
			
			resultPath.Add(nodeToBeCheck.mNode.mPoint);
			
			infiniteCounter++;
			if(infiniteCounter > 100000)
			{
				Debug.LogError("Infinite loop occured in GetPath(Vector3 start, Vector3 goal)");
				break;
			}
		}
		//resultPath.Reverse();
		
		return resultPath;
	}
	
	GameObject GetNeighbourBlock(NodeAnchor.NODE_DIRECTION direction, int row, int col)
	{
		GameObject gameObj = null;
		if(direction == NodeAnchor.NODE_DIRECTION.NORTH)
		{
			//! get the neighbour of north
			int neighbourCol = col - 1;
			
			//! if checking on end of boundary
			if(neighbourCol < 0)
			{
				return null;
			}
			
			gameObj = sInitBlockSpawned[neighbourCol,row];
		}
		else if(direction == NodeAnchor.NODE_DIRECTION.EAST)
		{
			//! get the neighbour of north
			int neighbourRow = row + 1;
			
			//! if checking on end of boundary
			if(neighbourRow > mRow - 1)
			{
				return null;
			}
			
			gameObj = sInitBlockSpawned[col,neighbourRow];
		}
		else if(direction == NodeAnchor.NODE_DIRECTION.SOUTH)
		{
			//! get the neighbour of north
			int neighbourCol = col + 1;
			
			//! if checking on end of boundary
			if(neighbourCol > mCol - 1)
			{
				return null;
			}
			
			gameObj = sInitBlockSpawned[neighbourCol,row];
		}
		else if(direction == NodeAnchor.NODE_DIRECTION.WEST)
		{
			//! get the neighbour of north
			int neighbourRow = row - 1;
			
			//! if checking on end of boundary
			if(neighbourRow < 0)
			{
				return null;
			}
			
			gameObj = sInitBlockSpawned[col,neighbourRow];
		}
		
		return gameObj;
	}
	
	//! for on gizmos to draw the level nodes and the boundary
	void DrawLevelNodes()
	{
		int maxCol = mMaxBoundary * mCol;
		int maxRow = mMaxBoundary * mRow;
		
		Gizmos.color = Color.red;
		
		//! Draw the map
		Vector3 offsetX = new Vector3(sTileSize * 0.5f, 0.0f, 0.0f);
		Vector3 offsetZ = new Vector3(0.0f , 0.0f, sTileSize * 0.5f);
		//! get the left most line
		Gizmos.DrawLine(mNodes[0,0].mPoint - offsetX + offsetZ, mNodes[maxCol - 1,0].mPoint - offsetX - offsetZ);
		//! get the top most line
		Gizmos.DrawLine(mNodes[0,0].mPoint + offsetZ - offsetX, mNodes[0,maxRow - 1].mPoint + offsetZ + offsetX);
		//! draw remaining vertical line
		for(int i = 1; i <= mRow; i++)
		{
			int step = i * mMaxBoundary - 1;
			Gizmos.DrawLine(mNodes[0,step].mPoint + offsetX + offsetZ, mNodes[maxCol - 1,step].mPoint + offsetX - offsetZ);
		}
		//! draw remaining horizontal line
		for(int i = 1; i <= mCol; i++)
		{
			int step = i * mMaxBoundary - 1;
			Gizmos.DrawLine(mNodes[step,0].mPoint - offsetZ - offsetX, mNodes[step, maxRow - 1].mPoint + offsetX - offsetZ);
		}
		
		Gizmos.color = Color.blue;
		//! draw node
		foreach(Node node in mNodes)
		{
			Gizmos.DrawIcon(node.mPoint,"LevelRelated/NodeIconBig");
			
			foreach(NodeLink nodeLink in node.mNodeLinkNeighbour)
			{
				Node neighbour = mNodes[nodeLink.neighbourCol,nodeLink.neighbourRow];
				Gizmos.DrawLine(node.mPoint, neighbour.mPoint);
			}
		}
		
		//! just to draw the boundary offset to the btm left position
		for(int i = 1; i <= mCol; i++)
		{
			for(int j = 0; j < mRow; j++)
			{
				//int id = mMaxBoundary * mCol * mMaxBoundary * i + j * mMaxBoundary;
				//int id = (mMaxBoundary * mCol * mMaxBoundary * i - (mMaxBoundary * mCol)) + j * mMaxBoundary;
				int colStep = i * mMaxBoundary - 1;
				int rowStep = j * mMaxBoundary;
				//Vector3 length = new Vector3((mMaxBoundary - 2) * mTileSize, 0.0f, (mMaxBoundary - 2) * mTileSize);
				Gizmos.DrawIcon(mNodes[colStep,rowStep].mPoint,"LevelRelated/RedNode");
			}
		}
		
		//! just to draw the inital spawn block offset to the btm left position
		if(mLevelGenerator.mSpawnList.Count > 0)
		{
			foreach(GameObject gameObject in mLevelGenerator.mSpawnList)
			{
				//! this only work if the collider is active
				Vector3 objPos = gameObject.transform.position - gameObject.collider.bounds.size * 0.5f;
				Gizmos.DrawIcon(objPos,"LevelRelated/ObjPos");
				Gizmos.DrawIcon(gameObject.collider.bounds.min, "LevelRelated/Collider_min");
				Gizmos.DrawIcon(gameObject.collider.bounds.max, "LevelRelated/Collider_max");
			}
		}
	}
	
	void DrawPathfindingLink()
	{
		Gizmos.color = Color.yellow;
		
		//! to draw the pathlink debugNodes
		foreach(PathLink pathLinks in mPathLinks)
		{
			//Debug.Log(pathLinks.mBlockNodes.Count);
			for(int i = 0; i < pathLinks.mDebugNodes.Count - 1; i++)
			{
				Gizmos.DrawLine(pathLinks.mDebugNodes[i].mPoint, pathLinks.mDebugNodes[i + 1].mPoint);
			}
		}
	}
	
	void DrawBigAiNodes()
	{
		Gizmos.color = Color.cyan;
		foreach(Node node in sBigAiNodes)
		{
			if(node == null)continue;
			Gizmos.DrawIcon(node.mPoint,"LevelRelated/BlueNode");
			
			foreach(NodeLink nodeLink in node.mNodeLinkNeighbour)
			{
				Node neighbour = sBigAiNodes[nodeLink.neighbourCol,nodeLink.neighbourRow];
				Gizmos.DrawLine(node.mPoint, neighbour.mPoint);
			}
		}
	}
	
	void DrawSmallAiNodes()
	{
		Gizmos.color = Color.yellow;
		
		//! to draw the pathfinding nodes
		foreach(Node node in sSmallAiNodes)
		{
			if(node == null)continue;
			Gizmos.DrawIcon(node.mPoint,"LevelRelated/" + node.mClearanceSize);
			
			foreach(NodeLink nodeLink in node.mNodeLinkNeighbour)
			{
				Node neighbour = sSmallAiNodes[nodeLink.neighbourCol,nodeLink.neighbourRow];
				Gizmos.DrawLine(node.mPoint, neighbour.mPoint);
			}
		}
	}
	
	void OnGUI()
	{
		mDrawLevelNodes = GUI.Toggle(new Rect(0,0,300,50),mDrawLevelNodes,"Draw Level Nodes");
		mDrawPathFindingLink = GUI.Toggle(new Rect(0,50,300,50),mDrawPathFindingLink,"Draw linkage between initial block");
		mDrawBigAiNodes = GUI.Toggle(new Rect(0,100,300,50),mDrawBigAiNodes,"Draw Big Ai Nodes");
		mDrawSmallAiNodes = GUI.Toggle(new Rect(0,150,300,50),mDrawSmallAiNodes,"Draw Small Ai Nodes");
		EventManager.mDrawInitialBlockMap = GUI.Toggle(new Rect(0,200,300,50),EventManager.mDrawInitialBlockMap,"Draw the event manager map");
	}
	
	void OnDrawGizmos()
	{
		if(mNodes.Length <= 0)
		{
			return;
		}
		
		if(mDrawLevelNodes)
		{
			DrawLevelNodes();
		}
		if(mDrawPathFindingLink)
		{
			DrawPathfindingLink();
		}
		if(mDrawBigAiNodes)
		{
			DrawBigAiNodes();
		}
		if(mDrawSmallAiNodes)
		{
			DrawSmallAiNodes();
		}
		
		//! check the node linkage
//		foreach(NodeLink nodeLink in mNodes[4,4].mNodeLinkNeighbour)
//		{
//				Node neighbour = mNodes[nodeLink.neighbourCol,nodeLink.neighbourRow];
//				Gizmos.DrawLine(mNodes[4,4].mPoint, neighbour.mPoint);
//		}
		
		
		
		
		//Gizmos.color = Color.green;
		
		//! draw the pathLinks
//		foreach(PathLink pathLinks in mPathLinks)
//		{
//			Gizmos.DrawIcon(pathLinks.mStart,"LevelRelated/Start_Icon");
//			Gizmos.DrawIcon(pathLinks.mGoal,"LevelRelated/BullEye_Icon");
//			Gizmos.DrawLine(pathLinks.mStart, pathLinks.mGoal);
//		}
	}
}
