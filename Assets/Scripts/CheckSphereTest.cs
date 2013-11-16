using UnityEngine;
using System.Collections;

public class CheckSphereTest : MonoBehaviour {

	// Update is called once per frame
	void Update ()
	{
		if(Physics.CheckSphere(transform.position,1.2f))
		{
			Debug.Log("Check sphere succeeds");
		}
	}
}
