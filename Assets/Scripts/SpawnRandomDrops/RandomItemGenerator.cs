using UnityEngine;
using System.Collections;

public class RandomItemGenerator : MonoBehaviour {

	public GameObject[] randomItem;
	GameObject itemSpawned;
	
	void Start()
	{
		int random = Random.Range(0,randomItem.Length);
		Debug.Log("random length: " + randomItem.Length);
		itemSpawned = (GameObject)Instantiate(randomItem[random], transform.position, Quaternion.identity);
	}
}
