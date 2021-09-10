using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[InitializeOnLoad]
public class EditorCoroutineTest  : Editor {
 
    static float time01;
    [MenuItem("TestEditorUpdata/Test")]
    public static void TestCoroutine()
    {
        EditorApplication.update+=Update;
        myTest();
    }
    static IEnumerator WaitAndPrint()
    { 
        
        

        EditorApplication.isPlaying = true;
 
        yield return new WaitForSeconds(4.0f);
		// EditorApplication.isPlaying = false;
        
    }
    public static void myTest()
    {
        EditorCoroutineRunner.StartEditorCoroutine(WaitAndPrint());
        // EditorApplication.isPlaying = true;
        Debug.Log("WaitAndPrint3" + "|" + Time.time);
    }

    static void Update() {
        Debug.Log("Editor causes this Update");
		// time01+=Time.deltaTime;
		// if(time01>10){
		// 	EditorApplication.isPlaying = false;
		// }         
    }
}
