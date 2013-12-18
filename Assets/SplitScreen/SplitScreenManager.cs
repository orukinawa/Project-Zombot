using UnityEngine;
using System.Collections;

//! Manages the movement of cameras relative to the players and culling of the viewports accordingly
public class SplitScreenManager : MonoBehaviour
{
	//! Transform for player character 1.
	public Transform playerOneTransform;
		
	//! Transform for player character 2.
	public Transform playerTwoTransform;
	
	//! Transform for Player 1's camera holder.
	public Transform playerOneCameraHolderPrefab;
	
	//! Transform for Player 2's camera holder.
	public Transform playerTwoCameraHolderPrefab;
	
	//! Transform for Player 1's camera holder.
	Transform playerOneCameraHolder;
	
	//! Transform for Player 2's camera holder.
	Transform playerTwoCameraHolder;
		
	//! How high the cameras should be.
	public float cameraHeight;
	
	//! How far the cameras should be from the players along the line between them.
	public float cameraDistance;
	
	//! How far the camers should be BEHIND the players.
	public float cameraOffset;
	
	//! Smoothing for camera movement.
	public float cameraSmoothing;
	
	//! Reference to camera 2 for enable/disable.
	Transform playerTwoCamera;
	
	//! Cache this tranform.
	Transform mTransform;
	
	//! Transform of the culling plane for Player 1's Camera
	Transform playerOneCullPlane;
	
	//! Transform of the culling plane for Player 2's Camera
	Transform playerTwoCullPlane;
	
	//! Square distance between the players.
	float sqrDistance;
	
	//! How close the players should be for the split screens to merge.
	float closeEnoughSqrDistance;
	
	//! Cache the camera velocities for smooth damping.
	Vector3 cameraOneVel = Vector3.zero;
	Vector3 cameraTwoVel = Vector3.zero;
	
	//! Flag to check if players are close enough to merge split screens.
	bool isCloseEnough = false;
	
	//! Cache previous frame flag.
	bool isCloseEnoughPrevious = false;
	
	//! Flag to check if the cameras already moved to player position
	bool cameraInitialPosition = false;
	
	void Start()
	{
		mTransform = transform;
		mTransform.position = new Vector3(mTransform.position.x,cameraHeight,mTransform.position.z);
		closeEnoughSqrDistance = (2*cameraDistance)*(2*cameraDistance);
		playerOneTransform = GameObject.Find("Player1").transform;
		playerTwoTransform = GameObject.Find("Player2").transform;
	}
	
	void LateUpdate()
	{
		if(!playersSet()) return;
		if(!cameraInitialPosition)
		{
			//instantiate the camera holder prefabs
			playerOneCameraHolder = (Transform)Instantiate(playerOneCameraHolderPrefab, playerOneTransform.position, Quaternion.identity);
			playerTwoCameraHolder = (Transform)Instantiate(playerTwoCameraHolderPrefab, playerTwoTransform.position, Quaternion.identity);
			playerOneCullPlane = playerOneCameraHolder.GetChild(0).GetChild(0);
			playerTwoCamera = playerTwoCameraHolder.GetChild(0);
			playerTwoCullPlane = playerTwoCamera.GetChild(0);
			cameraInitialPosition = true;
		}
		//Cache
		float tempPosX = playerOneTransform.position.x;
		float tempPosZ = playerOneTransform.position.z;
		float xDist = playerTwoTransform.position.x - tempPosX;
		float zDist = playerTwoTransform.position.z - tempPosZ;
		
		//calculate square distance between players
		sqrDistance = xDist*xDist + zDist*zDist;
		
		//update closeEnough flags
		isCloseEnoughPrevious = isCloseEnough;
		isCloseEnough = sqrDistance < closeEnoughSqrDistance;
		
		//move this object to the midpoint between players
		mTransform.position = new Vector3(
			tempPosX + xDist * 0.5f,
			cameraHeight,
			tempPosZ + zDist * 0.5f);
		
		//rotate so that its forwards faces player 1
		mTransform.LookAt(new Vector3(tempPosX, mTransform.position.y, tempPosZ));
		
		if(!isCloseEnough)
		{
			if(isCloseEnoughPrevious)
			{
				playerTwoCamera.gameObject.SetActive(true);
				playerOneCullPlane.gameObject.SetActive(true);
			}
			
			Vector3 camPos1 = playerOneTransform.position - mTransform.forward * cameraDistance + Vector3.back * cameraOffset;
			camPos1.y = cameraHeight;
//			playerOneCameraHolder.transform.position = camPos1;
			playerOneCameraHolder.transform.position = Vector3.SmoothDamp(playerOneCameraHolder.transform.position, camPos1, ref cameraOneVel, cameraSmoothing);
			
			Vector3 camPos2 = playerTwoTransform.position + mTransform.forward * cameraDistance + Vector3.back * cameraOffset;
			camPos2.y = cameraHeight;		
			//playerTwoCameraHolder.transform.position = camPos2;
			playerTwoCameraHolder.transform.position = Vector3.SmoothDamp(playerTwoCameraHolder.transform.position, camPos2, ref cameraTwoVel, cameraSmoothing);			
			
			Vector3 tempEuler = playerOneCullPlane.localEulerAngles;
			tempEuler.z = -mTransform.eulerAngles.y;
			playerOneCullPlane.localEulerAngles = tempEuler;
			
			tempEuler = playerTwoCullPlane.localEulerAngles;
			tempEuler.z = 180.0f - mTransform.eulerAngles.y;
			playerTwoCullPlane.localEulerAngles = tempEuler;
		}
		
		else
		{
			if(!isCloseEnoughPrevious)
			{
				playerTwoCamera.gameObject.SetActive(false);
				playerOneCullPlane.gameObject.SetActive(false);
			}			
			Vector3 camPos1 = mTransform.position + Vector3.back * cameraOffset;
			camPos1.y = cameraHeight;
			playerOneCameraHolder.transform.position = Vector3.SmoothDamp(playerOneCameraHolder.transform.position, camPos1, ref cameraOneVel, cameraSmoothing);
			playerTwoCameraHolder.transform.position = Vector3.SmoothDamp(playerTwoCameraHolder.transform.position, camPos1, ref cameraOneVel, cameraSmoothing);
		}		
		//mainCameraTransform.position = Vector3.SmoothDamp (mainCameraTransform.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothing);
	}
	
	bool playersSet()
	{
		if(playerOneTransform == null) return false;
		if(playerTwoTransform == null) return false;
		return true;
	}
}
