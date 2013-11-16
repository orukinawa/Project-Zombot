using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EventMap))]
public class EventMapEditor : Editor
{
	EventMap mEventMap;
//	public GameObject instance;
	//bool mapEditorCreate = false;
	//string[] mString = {"1","2","3","4","5","6"};
	
	public void Awake()
	{
		mEventMap = (EventMap)target;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		if(GUILayout.Button("Create map"))
		{
			mEventMap.CreateMap();
//			instance = mEventMap.mDefaultBlock;
//			PrefabUtility.ReplacePrefab(instance, PrefabUtility.GetPrefabParent(instance), ReplacePrefabOptions.ConnectToPrefab);
			//mapEditorCreate = true;
		}
		
//		if(mapEditorCreate)
//		{
//			//GUI.enabled = false;
//			GUILayout.SelectionGrid(-1,mString,mEventMap.mRow);
//		}
	}
}
