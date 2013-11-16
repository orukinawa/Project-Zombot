using UnityEngine;
using System.Collections.Generic;

public class NeighbourInitialBlockLink
{
	public int mCol;
	public int mRow;
	public NeighbourInitialBlockLink(int col, int row)
	{
		mCol = col;
		mRow = row;
	}
}

public class InitialBlock
{
	public GameObject mInitialBlockObj;
	public List<NeighbourInitialBlockLink> mNeighbourBlock = new List<NeighbourInitialBlockLink>();
	//! cache all generated block in this row/col boundary
	public List<GameObject> mGeneratedBlock = new List<GameObject>();
}

public class PlayerData
{
	public GameObject mPlayerObj;
	public int mCurrRow = 0;
	public int mCurrCol = 0;
}

[System.Serializable]
public class EnemySpawnManager
{
	//! the require talentpoint to spawn this type of enemy
	public int mTargetTalentPoint;
	//! attach prefabs via inspector
	public GameObject mEnemyPrefab;
	//! checks whether it's added to the list
	public bool mAdded;
}
	

public class EventManager : MonoBehaviour
{
	//! list of monster allow in this scene(level)
	public EnemySpawnManager[] mEnemySpawnManager;
	List<GameObject> mEnemyTypeList = new List<GameObject>();
	
	//! debugging purpose
	public static bool mDrawInitialBlockMap;
	
	//! talent point determines difficulty of the gsame
	static float mTalentPoint = 0;
	//! the talent point increment based on kills
	public float mKillTalentPts = 1.0f;
	public static float sKillPts;
	//! for every second add to talent point
	public float mTimeIncrementTalentPoint = 0.5f;
	float mCurrentTime = 0.0f;
	
	//! the wave spawning
	public int mNumPerWave = 10;
	int mSpawnedWave = 0;
	//! in seconds
	public float mWaveSpawnDuration = 60.0f;
	float mWaveSpawnTimer = 0.0f;
	float mSpawnDelayDuration = 1.0f;
	
	//! signal to tell the players have open the start door
	public static bool mStartGame;
	
	public float mMaxDistSpawn;
	float mMaxDistSpawnSqr;
	
	//! this is for a single player test
	public GameObject mPlayerPrefab;
	
	public int mNumPlayer = 1;
	
	//! keep players references
	public List<PlayerData> mPlayerInGame = new List<PlayerData>();
	
	//! abstarct map of the initial block and it's linkages
	public static InitialBlock[,] sInitialBlockMap = new InitialBlock[0,0];
	
	public EventMap mEventMap;
	
	//! to update various things
	float mUpdateTimer = 1.0f;
	
	void Start()
	{
		mMaxDistSpawnSqr = mMaxDistSpawn * mMaxDistSpawn;
		sKillPts = mKillTalentPts;
		mSpawnedWave = mNumPerWave;
		//! update the first time
		mUpdateTimer = 0.0f;
		SpawnPlayers();
	}
	
	void Update()
	{
		if(mStartGame)
		{
			mUpdateTimer -= Time.deltaTime;
			if(mUpdateTimer <= 0.0f)
			{
				UpdateSpawnList();
				mUpdateTimer = 1.0f;
			}
			UpdTimeProgression();
			UpdWaveTimer();
			SpawnEnemies();
		}
	}
	
	void UpdTimeProgression()
	{
		//! increse the difficulty of the game over time
		mCurrentTime += Time.deltaTime;
		if(mCurrentTime >= 1.0f)
		{
			mTalentPoint += mTimeIncrementTalentPoint;
			mCurrentTime = 0.0f;
		}
	}
	
	void UpdWaveTimer()
	{
		mWaveSpawnTimer += Time.deltaTime;
		if(mWaveSpawnTimer >= mWaveSpawnDuration)
		{
			mSpawnDelayDuration -= Time.deltaTime;
			if(mSpawnDelayDuration <= 0.0f)
			{
				SpawnEnemyWave();
				mSpawnDelayDuration = 1.0f;
				mSpawnedWave -= 1;
			}
			
			if(mSpawnedWave <= 0)
			{
				mWaveSpawnTimer = 0.0f;
				mSpawnedWave = mNumPerWave;
			}
		}
	}
	
	public void SpawnEnemyWave()
	{
		int col = EventMap.sTileSize * EventMap.mMaxBoundary;
		int row = EventMap.sTileSize * EventMap.mMaxBoundary;
		
		foreach(PlayerData player in mPlayerInGame)
		{
			GameObject playerObj = player.mPlayerObj;
			int playerRow = (int)(playerObj.transform.position.x / row);
			int playerCol =(int)(-1 * playerObj.transform.position.z / col);
			
			List<EventSpawnEnemies> enemySpawners = GetAllSpawnEnemyBlocks(playerCol,playerRow);
			
			if(enemySpawners.Count == 0)
			{
				break;
			}
			
			int rand = Random.Range(0, enemySpawners.Count);
			enemySpawners[rand].SpawnEnemy("PURSUE",playerObj);
		}
	}
	
	//! determines which block the players will be spawn
	public void SpawnPlayers()
	{
		List<EventSpawnPlayer> mStartBlocks = new List<EventSpawnPlayer>();
		for(int i = 0; i < mEventMap.mCol; i++)
		{
			for(int j = 0; j < mEventMap.mRow; j++)
			{
				if(sInitialBlockMap[i,j] == null)continue;
				EventSpawnPlayer spawnPlayerEvt = sInitialBlockMap[i,j].mInitialBlockObj.GetComponent<EventSpawnPlayer>();
				
				if(spawnPlayerEvt)
				{
					mStartBlocks.Add(spawnPlayerEvt);
				}
			}
		}
		
		//! spawn the player
		for(int i = 0; i < mNumPlayer; i++)
		{
			int numRand = Random.Range(0, mStartBlocks.Count);
			mStartBlocks[numRand].SpawnPlayer(mPlayerPrefab,this);
		}
	}
	
	//! add/subtract talent point
	public static void ConfigureTalentPoint(float talentPoint)
	{
		mTalentPoint += talentPoint;
	}
	
	//! gets all the blocks with EventSpawnEnemies script attached in only specify region
	public List<EventSpawnEnemies> GetAllSpawnEnemyBlocks(int col, int row)
	{
		List<EventSpawnEnemies> evtSpawnEnemyList = new List<EventSpawnEnemies>();
		
		List<GameObject> blockList = sInitialBlockMap[col,row].mGeneratedBlock;
		
		foreach(GameObject obj in blockList)
		{
			EventSpawnEnemies evt = obj.GetComponent<EventSpawnEnemies>();
			if(evt != null)
			{
				evtSpawnEnemyList.Add(evt);
			}
		}
		
		return evtSpawnEnemyList;
	} 
	
	//! updates the spawn list for eventSpawnEnemies
	public void UpdateSpawnList()
	{
		foreach(EnemySpawnManager evtManager in mEnemySpawnManager) 
		{
			if(mTalentPoint >= evtManager.mTargetTalentPoint)	
			{
				if(!evtManager.mAdded)
				{
					evtManager.mAdded = true;
					mEnemyTypeList.Add(evtManager.mEnemyPrefab);
				}
			}
		}
	}
	
	//! determines which block the enemy will be spawn
	public void SpawnEnemies()
	{
		int col = EventMap.sTileSize * EventMap.mMaxBoundary;
		int row = EventMap.sTileSize * EventMap.mMaxBoundary;
		
		//Debug.Log("spawnEnemy running");
		
		foreach(PlayerData player in mPlayerInGame)
		{
			GameObject playerObj = player.mPlayerObj;
			int playerRow = (int)(playerObj.transform.position.x / row);
			int playerCol =(int)(-1 * playerObj.transform.position.z / col);
			
			List<EventSpawnEnemies> enemySpawners = GetAllSpawnEnemyBlocks(playerCol,playerRow);
			
			if(enemySpawners.Count == 0)
			{
				break;
			}
			
			foreach(EventSpawnEnemies spawner in enemySpawners)
			{
				if(spawner.mSpawnState == EventSpawnEnemies.SPAWN_STATE.ALLOW_SPAWN)
				{
					spawner.UpdatePrefabList(mEnemyTypeList);
					spawner.SpawnEnemy();
				}
			}
		}
	}
	
	//! make this a static function and do it at link path function at eventMap
	public static void LinkInitialBlock(GameObject selfBlock, int selfCol, int selfRow, NodeAnchor.NODE_DIRECTION direction)
	{
		//! for self
		if(sInitialBlockMap[selfCol,selfRow] == null)
		{
			InitialBlock initialBlock = new InitialBlock();
			if(selfBlock == null)
			{
				Debug.Log("The one with self block failure: " + selfBlock.name);
			}
			initialBlock.mInitialBlockObj = selfBlock;
			sInitialBlockMap[selfCol,selfRow] = initialBlock;
		}
		
		int neighbourCol = 0;
		int neighbourRow = 0;
		
		//! add the neighbourBlock
		if(direction == NodeAnchor.NODE_DIRECTION.NORTH)
		{
			neighbourCol = selfCol - 1;
			neighbourRow = selfRow;
		}
		if(direction == NodeAnchor.NODE_DIRECTION.EAST)
		{
			neighbourCol = selfCol;
			neighbourRow = selfRow + 1;
		}
		if(direction == NodeAnchor.NODE_DIRECTION.SOUTH)
		{
			neighbourCol = selfCol + 1;
			neighbourRow = selfRow;
		}
		if(direction == NodeAnchor.NODE_DIRECTION.WEST)
		{
			neighbourCol = selfCol;
			neighbourRow = selfRow - 1;
		}
		
		//! for the neighbour
		if(sInitialBlockMap[neighbourCol,neighbourRow] == null)
		{
			InitialBlock initialBlock = new InitialBlock();
			initialBlock.mInitialBlockObj = EventMap.sInitBlockSpawned[neighbourCol,neighbourRow];
			sInitialBlockMap[neighbourCol,neighbourRow] = initialBlock;
		}
		//! assign linkage
		NeighbourInitialBlockLink currlink = new NeighbourInitialBlockLink(selfCol,selfRow);
		NeighbourInitialBlockLink neighbourlink = new NeighbourInitialBlockLink(neighbourCol,neighbourRow);
		sInitialBlockMap[selfCol,selfRow].mNeighbourBlock.Add(neighbourlink);
		sInitialBlockMap[neighbourCol,neighbourRow].mNeighbourBlock.Add(currlink);
	}
	
	void DrawInitialBlockMap()
	{
		foreach(InitialBlock initBlock in sInitialBlockMap)	
		{
			if(initBlock == null)
			{
				continue;
			}
			
			if(initBlock.mInitialBlockObj == null)
			{
				continue;
			}
			Vector3 selfPos = initBlock.mInitialBlockObj.transform.position;
			Gizmos.DrawIcon(selfPos,"LevelRelated/Anchor_Exit");
			foreach(NeighbourInitialBlockLink link in initBlock.mNeighbourBlock)
			{
				Vector3 neighbourPos = EventMap.sInitBlockSpawned[link.mCol,link.mRow].transform.position;
				Gizmos.DrawLine(selfPos,neighbourPos);
			}
		}
		
		for(int i = 0; i < mEventMap.mCol; i++)
		{
			for(int j = 0; j < mEventMap.mRow; j++)
			{
				foreach(GameObject obj in sInitialBlockMap[i,j].mGeneratedBlock)
				{
					if(obj == null)
					{
						mDrawInitialBlockMap = false;
						break;
					}
					Gizmos.DrawIcon(obj.transform.position - Vector3.right + Vector3.up,"LevelRelated/" + i);
					Gizmos.DrawIcon(obj.transform.position + Vector3.right + Vector3.up,"LevelRelated/" + j);
				}
			}
		}
		
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(300,0,200,50),"TalentPts: " + mTalentPoint);
		GUI.Label(new Rect(300,60,200,50),"WaveTimer: " + mWaveSpawnTimer);
	}
	
	void OnDrawGizmos()
	{
		if(mDrawInitialBlockMap)
		{
			DrawInitialBlockMap();
		}
	}
}
