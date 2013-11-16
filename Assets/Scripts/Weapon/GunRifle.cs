using UnityEngine;
using System.Collections;

public class GunRifle : GunBase
{
	void Start()
	{
		base.Initialize();
	}
	
	void Update()
	{
		base.GunUpdate();
	}
}
