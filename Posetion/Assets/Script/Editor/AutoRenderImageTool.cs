using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Animations;
using System;

public class AutoRenderImageTool001 : Editor
{

    [MenuItem("Import/test")]
    public static void Init(){
        //myTest(); 
        Quaternion rotation = new Quaternion(0f,0f,1.0f,0);

        Debug.Log(rotation.eulerAngles);
    }
    static IEnumerator WaitAndPrint()
    {
        Debug.Log("WaitAndPrint1" + "|" + Time.time);
        yield return new WaitForSeconds(4.0f);
        Debug.Log("WaitAndPrint2" +"|"+ Time.time);
    }
    public static void myTest()
    {
        EditorCoroutineRunner.StartEditorCoroutine(WaitAndPrint());
        Debug.Log("WaitAndPrint3" + "|" + Time.time);
    }

    string fbxFolderPath;
    string outPutFolderPath;
    
    public void ImportAnim(string path)
    {
        string prePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        string animationPath = prePath + string.Format("/Assets/Animation");          
        //检测是否有animation文件夹，如果有，则删除文件夹 ,创建新的文件夹     
        if(Directory.Exists(animationPath)){
            Directory.Delete(animationPath,true);
        }
        Directory.CreateDirectory(animationPath);

        string[] splitStrings = new string[2];
        AnimationClip aniClipSrc = null;

        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }

                splitStrings = files[i].Name.Split('.');
                Debug.Log("Name:" + splitStrings[0]);


                aniClipSrc = AssetDatabase.LoadAssetAtPath<AnimationClip>(string.Format("Assets/AnimationMode/{0}", files[i].Name));
                if (null == aniClipSrc)
                {
                    continue;
                }

                AnimationClip tmp = new AnimationClip();
                EditorUtility.CopySerialized(aniClipSrc, tmp);

                // AssetDatabase.CreateAsset(temp, "Assets/Animation/lanhai02.anim"); 
                AssetDatabase.CreateAsset(tmp, string.Format("Assets/Animation/{0}.anim", splitStrings[0]));
            }
        }

        AssetDatabase.Refresh();
    }

    public void AddAnimToAnimController()
    {
        string prePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        string path = prePath + string.Format("/Assets/Animation");

        if (false == Directory.Exists(path))
        {
            return;
        }

        string[] splitStrings = new string[2];
        AnimationClip aniClip = null;
        //判断是否有controller文件，有的话就删除，再创建新的文件
        
        string animControllerPath = prePath + string.Format("/Assets/Resources/Animator/PosetionEx.controller");   
        if(File.Exists(animControllerPath)){
            File.Delete(animControllerPath);
        }
        
        var animController = AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/Animator/PosetionEx.controller");
        
        UnityEditor.Animations.AnimatorControllerLayer layer = animController.layers[0];
        UnityEditor.Animations.AnimatorStateMachine stateMachine = layer.stateMachine;

        AssetDatabase.Refresh();

        Vector3 statepos = new Vector3(50, -300, 0);
        UnityEditor.Animations.AnimatorState state = null;

        DirectoryInfo direction = new DirectoryInfo(path);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        int nCount = 0;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta"))
            {
                continue;
            }

            splitStrings = files[i].Name.Split('.');
            Debug.Log("Name:" + splitStrings[0]);

            // 加载动画
            aniClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(string.Format("Assets/Animation/{0}.anim", splitStrings[0]));
            if (null == aniClip)
            {
                continue;
            }

            // 设置状态位置
            statepos.y += 60;

            // 添加状态
            state = stateMachine.AddState(aniClip.name, statepos);
            state.motion = aniClip;

            nCount++;
            if (nCount == 30)
            {
                nCount = 0;
                statepos.y = -300;
                statepos.x += 300;
            }
        }

        AssetDatabase.Refresh();
    }

    public void WriteCameraData(){
       
        string m_prePath = Application.dataPath.Substring(0,  Application.dataPath.LastIndexOf("/"));
        string outPutPath = m_prePath + string.Format("/OutPut/Camera.json");
        if(File.Exists(outPutPath)){
            File.Delete(outPutPath);
        } 

        StreamWriter writer = new StreamWriter(outPutPath,true);

        // 记录摄像机对应的世界坐标系到摄像机坐标系矩阵
        for (int i = 0; i < 8; i++)
        {
            GameObject obj =   GameObject.Find("CameraRT0"+(i+1));
            Transform objTransf = obj.GetComponent<Transform>();
            writer.Write("\""+obj.name+"\": ");
            CameraData camObj = new CameraData();
            camObj.translation[0] = objTransf.position.x;
            camObj.translation[1] = objTransf.position.y;
            camObj.translation[2] = objTransf.position.z;

            camObj.orientation[0] = objTransf.rotation.x;
            camObj.orientation[1] = objTransf.rotation.y;
            camObj.orientation[2] = objTransf.rotation.z;
            camObj.orientation[3] = objTransf.rotation.w;

            string json = JsonUtility.ToJson(camObj);
            writer.Write(json);
            if(i!=7){
                writer.Write(",\n");
            }
            
            // string a = string.Format("{0}:\n          pos:[{1},{2},{3}]\n          rotation:[{4},{5},{6},{7}]\n",obj.name,objTransf.position.x,objTransf.position.y,objTransf.position.z,
            // objTransf.rotation.x,objTransf.rotation.y,objTransf.rotation.z,objTransf.rotation.w);
            // Debug.Log(a);
            // writer.WriteLine(a);

        }
        GameObject camera = GameObject.Find("CameraRT01");
        Camera c = camera.GetComponent<Camera>();
        Debug.Log(c.focalLength);
        writer.WriteLine("\nCamera focalLength:"+c.focalLength);
        writer.Close();        
    } 

    public void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("FBX文件夹路径：")){
            Debug.Log("FBX文件夹路径：");
            fbxFolderPath = EditorUtility.OpenFolderPanel("Open Folder Dialog","D:\\","");
        };
        EditorGUILayout.TextField(fbxFolderPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("输出文件夹路径：")){
            outPutFolderPath = EditorUtility.OpenFolderPanel("Open Folder Dialog","D:\\","");
        };
        EditorGUILayout.TextField(outPutFolderPath);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(20);
        if(GUILayout.Button("一键渲染")){
            //批量分离FBX动画片段
            ImportAnim(fbxFolderPath);
            //添加动画片段到controller文件
            AddAnimToAnimController();
        };
        GUILayout.Space(50);
        if(GUILayout.Button("导出相机数据")){
            WriteCameraData();
        }
    }

}

[Serializable]
public class CameraData{
    public float[] translation = new float[3];
    public float[] orientation = new float[4];
}
