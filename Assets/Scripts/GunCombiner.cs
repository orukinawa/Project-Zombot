using UnityEngine;
using System.Collections;

public class GunCombiner : MonoBehaviour
{
	public GameObject[] gunArray;
	public GameObject[] bulletArray;
	public GameObject[] effectArray;
	
	private int gunIndex = 0;
	private int bulletIndex = 0;
	private int effectIndex = 0;
	
	bool isOn = false;
	
	public InventoryManager invManager;
	
	void Start()
	{
		//invManager = GetComponent<InventoryManager>();
	}
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Y))
		{
			isOn = !isOn;
			if(Time.timeScale == 0)
			{
				Time.timeScale = 1;
			}
			else if (Time.timeScale == 1)
			{
				Time.timeScale = 0;
			}
		}
	}
	
	void OnGUI()
	{
		if(isOn)
		{
			GUILayout.BeginVertical();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Gun");
			if(GUILayout.Button("<-")) gunIndex = Previous(gunIndex,gunArray);
			if(GUILayout.Button("->")) gunIndex = Next(gunIndex,gunArray);
			GUILayout.Label(gunArray[gunIndex].name);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Bullet");
			if(GUILayout.Button("<-")) bulletIndex = Previous(bulletIndex,bulletArray);
			if(GUILayout.Button("->")) bulletIndex = Next(bulletIndex,bulletArray);
			GUILayout.Label(bulletArray[bulletIndex].name);
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Effect");
			if(GUILayout.Button("<-")) effectIndex = Previous(effectIndex,effectArray);
			if(GUILayout.Button("->")) effectIndex = Next(effectIndex,effectArray);
			GUILayout.Label(effectArray[effectIndex].name);
			GUILayout.EndHorizontal();
			
			if(GUILayout.Button("Combine!")) CombineGun();
		}
	}
	
	int Previous(int index, GameObject[] array)
	{
		int tempInt = index-1;
		if(tempInt < 0) tempInt = array.Length -1;
		return tempInt;		
	}
	
	int Next(int index, GameObject[] array)
	{
		int tempInt = index+1;
		if(tempInt > array.Length -1) tempInt = 0;
		return tempInt;		
	}
	
	void CombineGun()
	{
		GameObject tempGun = gunArray[gunIndex];
		tempGun.GetComponent<GunBase>().bulletMod = bulletArray[bulletIndex];
		tempGun.GetComponent<GunBase>().effectMod = effectArray[effectIndex];
		
//		commando.equippedWeapons[0] = tempGun;
//		commando.InitializeGuns();
		
		invManager.ReplaceWeapon(tempGun);
		
		isOn = false;
		Time.timeScale = 1;
	}
}
