using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MyTest : MonoBehaviour {
	float time01;
	// Use this for initialization
	void Start () {
		Debug.Log("重新开始");
	}
	
	// Update is called once per frame
	void Update () {
		time01+=Time.deltaTime;
		if(time01>5){
			EditorApplication.isPlaying = false;
			EditorCoroutineTest.myTest();
		}
	}
}
