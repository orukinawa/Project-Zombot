using UnityEngine;
using System.Collections;

public class ObjectActiveToggle : MonoBehaviour
{
	public GameObject toggleObject;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.RightShift))
		{
			if(toggleObject.activeSelf)
			{
				toggleObject.SetActive(false);
				return;
			}
			toggleObject.SetActive(true);
		}
	}
}
