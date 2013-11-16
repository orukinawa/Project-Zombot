using UnityEngine;
using System.Collections;

public class HackFollowTransform : MonoBehaviour
{
	Vector3 newPos;
	
	void Awake()
	{
		newPos = new Vector3 (0f,transform.localPosition.y,0f);
	}
	
	void Update()
	{
		transform.localPosition = newPos;
	}
}
