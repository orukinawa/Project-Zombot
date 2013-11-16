using UnityEngine;
using System.Collections;

public class TestPingPongMotion : MonoBehaviour
{
	public float xOffset;
	public float zOffset;
	
	Vector3 start;
	Vector3 end;
	
	// Use this for initialization
	void Start ()
	{
		start = transform.position;
		end = new Vector3(transform.position.x+xOffset,transform.position.y,transform.position.z+zOffset);
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = Vector3.Lerp(start, end, Mathf.PingPong(Time.time, 1.0f));
	}
}
