using UnityEngine;
using System.Collections;

public class DumbMovement : MonoBehaviour {

	// Update is called once per frame
	void Update ()
	{
		transform.position += transform.forward * 2 * Time.deltaTime;
	}
}
