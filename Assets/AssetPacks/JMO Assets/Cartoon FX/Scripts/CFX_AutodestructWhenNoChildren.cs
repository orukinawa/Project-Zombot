using UnityEngine;
using System.Collections;

/// <summary>
/// Automatically destroys the GameObject when there are no children left.
/// </summary>

public class CFX_AutodestructWhenNoChildren : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
	{
#if UNITY_4_2
		if( transform.childCount == 0)
		{
			GameObject.Destroy(this.gameObject);
		}
#else
		if( transform.GetChildCount() == 0)
		{
			GameObject.Destroy(this.gameObject);
		}
#endif
	}
}
