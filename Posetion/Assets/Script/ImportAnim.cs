using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Animations;
public static class CImportAnim
{
    public static string prePath= Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
    [MenuItem("Import/ImportAnimToAnimController")]
    public static void AddAnimToAnimController()
    {

        string path = prePath + string.Format("/Assets/Animation");

        if (false == Directory.Exists(path))
        {
            return;
        }

        string[] splitStrings = new string[2];
        AnimationClip aniClip = null;
        UnityEditor.Animations.AnimatorController animController = AssetDatabase.LoadAssetAtPath("Assets/Resources/Animator/PosetionEx.controller", typeof(UnityEditor.Animations.AnimatorController)) as UnityEditor.Animations.AnimatorController;
        UnityEditor.Animations.AnimatorControllerLayer layer = animController.layers[0];
        UnityEditor.Animations.AnimatorStateMachine stateMachine = layer.stateMachine;

        // 清空所有的状态
        //for (int i = 0; i < stateMachine.states.Length; i++)
        //{
        //    stateMachine.RemoveState(stateMachine.states[i] as UnityEditor.Animations.AnimatorState);
        //}

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


    public static void ImportAnim(string folderPath)
    {

        string[] str = folderPath.Split('\\');
        string folderName = str[str.Length-1];

        string[] splitStrings = new string[2];
        AnimationClip aniClipSrc = null;

        if (Directory.Exists(folderPath))
        {
            DirectoryInfo direction = new DirectoryInfo(folderPath);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }

                splitStrings = files[i].Name.Split('.');

                aniClipSrc = AssetDatabase.LoadAssetAtPath<AnimationClip>(string.Format("Assets/AnimationMode/"+folderName+"/{0}", files[i].Name));
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

[MenuItem("Import/CreatBGImage")]
    public static void CreatBGImage()
    {
        string imageFolder = prePath + string.Format("/BGImage");
        string bgTxtPath = prePath + string.Format("/BGImage.txt");

        if (Directory.Exists(imageFolder))
        {

            if (File.Exists(bgTxtPath))
            {
                File.Delete(bgTxtPath);
            }
            StreamWriter sw;
            FileInfo fi = new FileInfo(bgTxtPath);
            sw = fi.CreateText();

            DirectoryInfo direction = new DirectoryInfo(imageFolder);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta"))
                {
                    continue;
                }
                //Debug.Log(imageFolder + string.Format("/" + files[i].Name));
                // Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(imageFolder+files[i].Name,typeof (Texture2D));
                sw.WriteLine(imageFolder + string.Format("/" + files[i].Name));
                //Debug.Log( "FullName:" + files[i].FullName );  
                //Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
            }
            sw.Close();
            sw.Dispose();
        }
    }

    [MenuItem("Import/RandomMoveBGImage")]
    public static void RandomMoveBGImage()
    {
        CreatBGImage();
        string bgTxtPath = prePath + string.Format("/BGImage.txt");
        List<string> bgImagePathList = new List<string>();
        FileInfo fi = new FileInfo(bgTxtPath);
        if (!fi.Exists)
        {
            Debug.Log("文件不存在");
        }
        else
        {
            Debug.Log("文件存在");
            StreamReader sr = new StreamReader(bgTxtPath);
            string s = sr.ReadLine();
            while (s != null)
            {
                //  Debug.Log(s);
                bgImagePathList.Add(s);
                s = sr.ReadLine();
            }
            sr.Close();
        }

        for (int i=0;i<4;i++) {
            string bgPath = bgImagePathList[Random.Range(0, bgImagePathList.Count)];
            string newPath = bgPath.Replace("BGImage", "Assets\\SkyBox\\Image");
            File.Copy(bgPath, newPath, true);
        }

        AssetDatabase.Refresh();
    }
    static IEnumerator EditorApplicationStart()
    { 
        EditorApplication.isPlaying = true;
 
        yield return new WaitForSeconds(4.0f);
    }
   
    public static void AutoRenderImageTool(string folderPath)
    {
        //删除动画文件夹
        string animationFolder = Application.dataPath+"/Animation";
        if(Directory.Exists(animationFolder)){
            Directory.Delete(animationFolder, true);
            Directory.CreateDirectory(animationFolder);
        }else{
            Directory.CreateDirectory(animationFolder);
        }
        //删除动画控制器
        string animationControllerFile = Application.dataPath+"/Resources/Animator/PosetionEx.controller";
        if(File.Exists(animationControllerFile)){
            File.Delete(animationControllerFile);
            AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/Animator/PosetionEx.controller");
        }else{
           AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/Animator/PosetionEx.controller");
        }
        //删除背景图
        string bgFolder = Application.dataPath + "/SkyBox/Image";
        if (Directory.Exists(bgFolder))
        {
            Directory.Delete(bgFolder, true);
            Directory.CreateDirectory(bgFolder);
        }
        else
        {
            Directory.CreateDirectory(bgFolder);
        }


        RandomMoveBGImage();
        ImportAnim(folderPath);
        AddAnimToAnimController();
        EditorCoroutineRunner.StartEditorCoroutine(EditorApplicationStart());
    }

    [MenuItem("Import/BathFolderRender")]
    public static void BathFolderRender()
    {
        //创建输出目录
        string outPutPath = prePath + string.Format("/OutPut");
        // 创建OutPut目录
        if (false == Directory.Exists(outPutPath))
        {
            Directory.CreateDirectory(outPutPath);
        }
        
        //存储文件夹路径
        string allFolderTxtPath = prePath + string.Format("/AllFolderPath.txt");

        if (File.Exists(allFolderTxtPath))
        {
            File.Delete(allFolderTxtPath);
        }
        StreamWriter sw;
        FileInfo fi = new FileInfo(allFolderTxtPath);
        sw = fi.CreateText();

        string aniModeFolderPath = prePath + string.Format("/Assets/AnimationMode"); // Assets/AnimationMode/
        DirectoryInfo dir = new DirectoryInfo(aniModeFolderPath);
        DirectoryInfo[] allAniFolder = dir.GetDirectories();
        string modelfolderPath=null;
        for(int i=0;i<allAniFolder.Length;i++){
            sw.WriteLine(allAniFolder[i].ToString());
            modelfolderPath = allAniFolder[0].ToString();
        }
        sw.Close();
        sw.Dispose();

        AutoRenderImageTool(modelfolderPath);
        // ImportAnim(modelfolderPath);

    }
    [MenuItem("Import/WriteCameraData")]
    public static void WriteCameraData()
    {

        string m_prePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        string outPutPath = m_prePath + string.Format("/OutPut/Camera.json");
        if (File.Exists(outPutPath))
        {
            File.Delete(outPutPath);
        }

        StreamWriter writer = new StreamWriter(outPutPath, true);

        // 记录摄像机对应的世界坐标系到摄像机坐标系矩阵
        for (int i = 0; i < 8; i++)
        {
            GameObject obj = GameObject.Find("CameraRT0" + (i + 1));
            Transform objTransf = obj.GetComponent<Transform>();
            Vector3 euler = objTransf.rotation.eulerAngles;
            Vector3 newEuler = new Vector3(euler.x, euler.y, euler.z+180.0f);
            Quaternion newQuat = Quaternion.Euler(newEuler);
            //Debug.Log( "quat:    "+objTransf.rotation.x+"    newQuat:    "+newQuat.x);
            writer.Write("\"" + obj.name + "\": ");
            CameraData camObj = new CameraData();
            camObj.translation[0] = objTransf.position.x;
            camObj.translation[1] = objTransf.position.y;
            camObj.translation[2] = objTransf.position.z;

            

            camObj.orientation[0] = newQuat.x;
            camObj.orientation[1] = newQuat.y;
            camObj.orientation[2] = newQuat.z;
            camObj.orientation[3] = newQuat.w;

            string json = JsonUtility.ToJson(camObj);
            writer.Write(json);
            if (i != 7)
            {
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
        writer.WriteLine("\nCamera focalLength:" + c.focalLength);
        writer.Close();
    }

}


public class CameraData{
    public float[] translation = new float[3];
    public float[] orientation = new float[4];
}
