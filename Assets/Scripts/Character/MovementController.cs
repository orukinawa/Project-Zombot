using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour
{
	public enum MOTORTYPE
	{
		NORMAL = 0,
		IMMOBILE = 1,
		FROZEN = 2
	}
	
	//Motors
	public MotorBase currMotor;
	public MotorBase[] MotorList;
	public MOTORTYPE currMotortype = MOTORTYPE.NORMAL;
	
	// Objects to drag in
	public Transform character;
	public GameObject cursorPrefab;
	
	// Settings
	public float cameraSmoothing = 0.01f;
	public float cameraPreview = 2.0f;
	
	// Cursor settings
	public float cursorPlaneHeight = 0.0f;
	public float cursorFacingCamera = 0.0f;
	public float cursorSmallerWithDistance = 0.0f;
	public float cursorSmallerWhenClose = 1.0f;
	
	// Private memeber data
	private Camera mainCamera;
	
	private Transform cursorObject;
	
	private Transform mainCameraTransform;
	private Vector3 cameraVelocity = Vector3.zero;
	//public Vector3 cameraOffset = Vector3.zero;
	Vector3 initOffsetToPlayer;
	
	private Plane playerMovementPlane;
	
	private Quaternion screenMovementSpace;
	private Vector3 screenMovementForward;
	private Vector3 screenMovementRight;
	
	private Vector3 movementDirection;
	private Vector3 facingDirection;
	private Vector3 dashDirection;
	
	public float cameraDistance = 10.0f;
	
	bool isDashing;
	//public float dashSpeed;
	public float dashDuration;
	float dashTimer;
	public float dashCooldown;
	float dashCooldownTimer;
	bool canDash;
	
	public float moveSpeedMultiplier = 1.0f;
	public float invertMultiplier = 1.0f;
	public float dashSpeedMultiplier = 1.0f;
	public float rotationMultiplier = 1.0f;
	
	void Awake () 
	{		
		// Set main camera
		mainCamera = Camera.main;
		mainCameraTransform = mainCamera.transform;
		
		// Ensure we have character set
		// Default to using the transform this component is on
		if (!character)
			character = transform;
		
		currMotor = MotorList[(int)MOTORTYPE.NORMAL];
		
		movementDirection = Vector2.zero;
		facingDirection = Vector2.zero;
		dashDirection = Vector2.zero;
		
		//initOffsetToPlayer = mainCameraTransform.position - character.position;
		
		//Calculate initialoffset here		
		Vector3 reverseForward = mainCameraTransform.forward * -1.0f;
		initOffsetToPlayer = reverseForward.normalized * cameraDistance;	
		
		mainCameraTransform.position = character.position + initOffsetToPlayer;
		
		if (cursorPrefab) 
		{
			cursorObject = (Instantiate (cursorPrefab) as GameObject).transform;
		}
		
		// Save camera offset so we can use it in the first frame
		//cameraOffset = mainCameraTransform.position - character.position;
		//mainCameraTransform.position = character.position + cameraOffset;
		//Debug.Log(mainCameraTransform.position);
		
		// Set the initial cursor position to the center of the screen
		//cursorScreenPosition =  new Vector3 (0.5f * Screen.width, 0.5f * Screen.height, 0);
		
		// caching movement plane
		// ORU: need this to convert screenposition to worldposition
		// plane height can be adjusted
		playerMovementPlane = new Plane (character.up, character.position + character.up * cursorPlaneHeight);
	}
	
	void Start () 
	{		
		// it's fine to calculate this on Start () as the camera is static in rotation		
		//ORU : To catter for camera rotation other than euler.y (movement and rotation is relative to level and not on camera)
		// This allow turning the camera around and still have a good movement,rotation
		// Camera's euler y only!!!
		screenMovementSpace = Quaternion.Euler (0, mainCameraTransform.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;
		isDashing = false;
		dashCooldownTimer = 0.0f;
		canDash = true;
	}
	
	void Update ()
	{
		//ORU need this to move
		movementDirection = Input.GetAxis ("Horizontal") * screenMovementRight + Input.GetAxis ("Vertical") * screenMovementForward;
		
		// Make sure the direction vector doesn't exceed a length of 1
		// so the character can't move faster diagonally than horizontally or vertically
		if (movementDirection.sqrMagnitude > 1)
		{
			movementDirection.Normalize();
		}
		
		if(!isDashing)
		{
			currMotor.Move(movementDirection*moveSpeedMultiplier*invertMultiplier);
		}
		
		// HANDLE CHARACTER FACING DIRECTION AND SCREEN FOCUS POINT
		
		// First update the camera position to take into account how much the character moved since last frame
		//mainCameraTransform.position = Vector3.Lerp (mainCameraTransform.position, character.position + cameraOffset, Time.deltaTime * 45.0f * deathSmoothoutMultiplier);
		
		// Set up the movement plane of the character, so screenpositions
		// can be converted into world positions on this plane
		
		playerMovementPlane.normal = character.up;
		playerMovementPlane.distance = -character.position.y + cursorPlaneHeight;
		
		// used to adjust the camera based on cursor or joystick position
		
		Vector3 cameraAdjustmentVector = Vector3.zero;
		
		// On PC, the cursor point is the mouse position
		Vector3 cursorScreenPosition = Input.mousePosition;
					
		// Find out where the mouse ray intersects with the movement plane of the player
		Vector3 cursorWorldPosition = CursorManager.ScreenPointToWorldPointOnPlane (cursorScreenPosition, playerMovementPlane, mainCamera);
		
		float halfWidth = Screen.width / 2.0f;
		float halfHeight = Screen.height / 2.0f;
		float maxHalf = Mathf.Max (halfWidth, halfHeight);
		
		// Acquire the relative screen position
		// ORU: This is done so that the camera offset in a direction depends on the screen width/height
		// On a typical 16:9 aspect ratio, the camera will get offset more horizontally than vertically
		Vector3 posRel = cursorScreenPosition - new Vector3 (halfWidth, halfHeight, cursorScreenPosition.z);		
		posRel.x /= maxHalf; 
		posRel.y /= maxHalf;
					
		cameraAdjustmentVector = posRel.x * screenMovementRight + posRel.y * screenMovementForward;
		cameraAdjustmentVector.y = 0.0f;	
								
		// The facing direction is the direction from the character to the cursor world position
		if(!isDashing)
		{
			facingDirection = (cursorWorldPosition - character.position);
			facingDirection.y = 0;
			currMotor.Rotate(facingDirection*rotationMultiplier*invertMultiplier);
		}
		
		// Draw the cursor nicely
		HandleCursorAlignment (cursorWorldPosition);
		
		// HANDLE CAMERA POSITION
			
		// Set the target position of the camera to point at the focus point
		Vector3 cameraTargetPosition = character.position + initOffsetToPlayer + cameraAdjustmentVector * cameraPreview;
		
		// Apply some smoothing to the camera movement
		mainCameraTransform.position = Vector3.SmoothDamp (mainCameraTransform.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothing);
		
		// Save camera offset so we can use it in the next frame
		//cameraOffset = mainCameraTransform.position - character.position;
		if(isDashing)
		{
			if(dashTimer > dashDuration)
			{
				isDashing = false;
				dashTimer = 0.0f;
				canDash = false;
				return;
			}
			dashTimer += Time.deltaTime;
			currMotor.Dash(dashDirection*dashSpeedMultiplier*invertMultiplier);
		}
		
		if(!canDash)
		{
			dashCooldownTimer += Time.deltaTime;
			if(dashCooldownTimer > dashCooldown)
			{
				dashCooldownTimer = 0.0f;
				canDash = true;
			}
		}
	}
	
//	public static Vector3 PlaneRayIntersection (Plane plane, Ray ray)
//	{
//		float dist = 0.0f;
//		plane.Raycast (ray, out dist);
//		return ray.GetPoint (dist);
//	}
//	
//	public static Vector3 ScreenPointToWorldPointOnPlane (Vector3 screenPoint, Plane plane , Camera camera)
//	{
//		// Set up a ray corresponding to the screen position
//		Ray ray = camera.ScreenPointToRay (screenPoint);
//		
//		// Find out where the ray intersects with the plane
//		return PlaneRayIntersection (plane, ray);
//	}
	
	void HandleCursorAlignment (Vector3 cursorWorldPosition)
	{
		if (!cursorObject)
			return;
		
		// HANDLE CURSOR POSITION
		
		// Set the position of the cursor object
		cursorObject.position = cursorWorldPosition;
		
		// Hide mouse cursor when within screen area, since we're showing game cursor instead
		Screen.showCursor = (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height);
		
		
		// HANDLE CURSOR ROTATION
		
		Quaternion cursorWorldRotation = cursorObject.rotation;
		if (facingDirection != Vector3.zero)
			cursorWorldRotation = Quaternion.LookRotation (facingDirection);
		
		// Calculate cursor billboard rotation
		Vector3 cursorScreenspaceDirection= Input.mousePosition - mainCamera.WorldToScreenPoint (transform.position + character.up * cursorPlaneHeight);
		cursorScreenspaceDirection.z = 0.0f;
		Quaternion cursorBillboardRotation = mainCameraTransform.rotation * Quaternion.LookRotation (cursorScreenspaceDirection, -Vector3.forward);
		
		// Set cursor rotation
		cursorObject.rotation = Quaternion.Slerp (cursorWorldRotation, cursorBillboardRotation, cursorFacingCamera);
		
		
		// HANDLE CURSOR SCALING
		
		// The cursor is placed in the world so it gets smaller with perspective.
		// Scale it by the inverse of the distance to the camera plane to compensate for that.
		float compensatedScale = 0.1f * Vector3.Dot (cursorWorldPosition - mainCameraTransform.position, mainCameraTransform.forward);
		
		// Make the cursor smaller when close to character
		float cursorScaleMultiplier = Mathf.Lerp (0.7f, 1.0f, Mathf.InverseLerp (0.5f, 4.0f, facingDirection.magnitude));
		
		// Set the scale of the cursor
		cursorObject.localScale = Vector3.one * Mathf.Lerp (compensatedScale, 1.0f, cursorSmallerWithDistance) * cursorScaleMultiplier;
		
		// DEBUG - REMOVE LATER
//		if (Input.GetKey(KeyCode.O)) cursorFacingCamera += Time.deltaTime * 0.5f;
//		if (Input.GetKey(KeyCode.P)) cursorFacingCamera -= Time.deltaTime * 0.5f;
//		cursorFacingCamera = Mathf.Clamp01(cursorFacingCamera);
//		
//		if (Input.GetKey(KeyCode.K)) cursorSmallerWithDistance += Time.deltaTime * 0.5f;
//		if (Input.GetKey(KeyCode.L)) cursorSmallerWithDistance -= Time.deltaTime * 0.5f;
//		cursorSmallerWithDistance = Mathf.Clamp01(cursorSmallerWithDistance);
	}
	
	public void Dash()
	{
		if(canDash)
		{			
			isDashing = true;
			if(movementDirection.sqrMagnitude > 0)
			{
				dashDirection = movementDirection.normalized;
				return;
			}
			dashDirection = facingDirection.normalized;
		}
	}
	
	public void SetMotor(MOTORTYPE mType)
	{
		currMotor = MotorList[(int)mType];
		currMotortype = mType;
	}
}
