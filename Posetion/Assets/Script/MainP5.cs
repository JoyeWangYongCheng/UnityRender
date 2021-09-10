
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class MainP5 : MonoBehaviour
{
    
    public GameObject[] m_arrCamera = null;           // 拍摄图片相机
    private int m_CameraIndex;
    private string m_manPath;
    private GameObject m_man;
    private Animator m_animator;
    private Camera [] m_mainCamera;          // 主摄像机
    private Transform[] m_BoneTransform;
    private int m_curFrame;
    private int m_totalFrame;

    private string m_curAinmationName;
    private Texture2D [] m_texture2D;

    private AnimationClip[] m_animationClip;
    private UnityEditor.Animations.AnimatorController m_Ac;
    private UnityEditor.Animations.ChildAnimatorState [] m_AnimationState;

    private int m_curStateIndex;
    private bool m_needChangAnimation;
    // private bool m_end = false;
    private int m_wait;
    private FileHelper m_CSV;
    private FileHelper [] m_CSVCamera;
	private FileHelper [] m_CSVCamera2d;

    private string m_prePath;

    private Transform m_moveControll;

    string[] m_BonesName = {
                           "Bip001",        // hip 0
                           "Bip001 R Thigh",       // r-hip 1
                           "Bip001 R Calf",        // r-keen 2
                           "Bip001 R Foot",        // r-foot 3
                           "Bip001 R Toe0Nub",    // Toe0Nub.R
                           "Bip001 L Thigh",       // l-hip 4
                           "Bip001 L Calf",        // l-keen 5
                           "Bip001 L Foot",        // l-foot 6
                           "Bip001 L Toe0Nub",    // Toe0Nub.L
                           "Bip001 Spine",        // Spine 7 
                           "Bip001 Spine1",        // Spine 19 
                           "Bip001 Spine2",        // Chest 8
                           "Bip001 Neck",          // Neck 9
                           "Bip001 Head",          // Head 10
                           "Bip001 HeadNub",		// HeadNub 20
                        //    "Bip001 R Clavicle",        // R-clavicle    18
                           "Bip001 R UpperArm",    // R-shoulder 14
                           "Bip001 R Forearm",     // R-elbow    15
                           "Bip001 R Hand",        // R-wrist    16 
                        //    "Bip001 L Clavicle",        // L-clavicle    17                          
                           "Bip001 L UpperArm",    // L-shoulder 11
                           "Bip001 L Forearm",     // L-elbow    12
                           "Bip001 L Hand",        // L-wrist    13				   
    };

    string[] m_BonesPosName = {
                           "FrameID",
                           "Hip.Center.x",  "Hip.Center.y",  "Hip.Center.z",                    // hip
                           "Hip.R.x",  "Hip.R.y",  "Hip.R.z",              // r-hip
                           "Knee.R.x",  "Knee.R.y",  "Knee.R.z",           // r-keen
                           "Ankle.R.x",  "Ankle.R.y",  "Ankle.R.z",           // r-foot
                           "Toe0Nub.R.x",  "Toe0Nub.R.y",  "Toe0Nub.R.z",           // r-foot
                           "Hip.L.x",  "Hip.L.y",  "Hip.L.z",              // l-hip
                           "Knee.L.x",  "Knee.L.y",  "Knee.L.z",           // l-keen
                           "Ankle.L.x",  "Ankle.L.y",  "Ankle.L.z",           // l-foot
                           "Toe0Nub.L.x",  "Toe0Nub.L.y",  "Toe0Nub.L.z",           // r-foot
                           "Spine.x",  "Spine.y",  "Spine.z",              // Spine
                           "Spine1.x",  "Spine1.y",  "Spine1.z",              // Spine
                           "Spine2.x",  "Spine2.y",  "Spine2.z",              // Spine
                           "Neck.x",  "Neck.y",  "Neck.z",                 // Neck
                           "Head.x",  "Head.y",  "Head.z",                 // Head
                           "HeadNub.x",  "HeadNub.y",  "HeadNub.z",        // HeadNub
                           "Shoulder.L.x", "Shoulder.L.y", "Shoulder.L.z", // L-shoulder
                           "Elbow.L.x",  "Elbow.L.y",  "Elbow.L.z",        // L-elbow
                           "Wrist.L.x",  "Wrist.L.y",  "Wrist.L.z",        // L-wrist
                           "Shoulder.R.x", "Shoulder.R.y", "Shoulder.R.z", // R-shoulder
                           "Elbow.R.x",  "Elbow.R.y",  "Elbow.R.z",        // R-elbow
                           "Wrist.R.x",  "Wrist.R.y",  "Wrist.R.z",        // R-wrist
                        //    "L-clavicle.x",  "L-clavicle.y",  "L-clavicle.z",        // L-clavicle
                        //    "R-clavicle.x",  "R-clavicle.y",  "R-clavicle.z",        // R-clavicle                      
    };

    private Vector3[] m_postionCamera;
    private string m_controlBoneName;
    private List<Texture2D> texList;
    public Material skyMat;
    private string allFolderTxtPath;
    // Use this for initialization

    public static Transform FindChild(Transform trans, string goName)
    {
        Transform child = trans.Find(goName);
        if (child != null)
            return child;

        Transform go = null;
        for (int i = 0; i < trans.childCount; i++)
        {
            child = trans.GetChild(i);
            go = FindChild(child, goName);
            if (go != null)
                return go;
        }
        return null;
    }


    void Start ()
    {
        InitArgs();
        //查找所有背景图
        string imageFolder = "Assets/SkyBox/Image/";
        if(Directory.Exists(imageFolder)){
            DirectoryInfo direction = new DirectoryInfo(imageFolder);
            FileInfo[] files = direction.GetFiles("*",SearchOption.AllDirectories); 

            for(int i=0;i<files.Length;i++){  
                if (files[i].Name.EndsWith(".meta")){  
                    continue;  
                }  
                // Debug.Log( "Name:" + files[i].Name ); 
                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(imageFolder+files[i].Name,typeof (Texture2D));
                texList.Add(tex);
                //Debug.Log( "FullName:" + files[i].FullName );  
                //Debug.Log( "DirectoryName:" + files[i].DirectoryName );  
            }  
        }

        // 加载角色
        Object manPreb = Resources.Load(m_manPath, typeof(Object));
        m_man = Instantiate(manPreb) as GameObject;


        int index = 0;
        for (index = 0; index < m_BoneTransform.Length; index++)
        {
            m_BoneTransform[index] = FindChild(m_man.transform, m_BonesName[index]);
            if (null == m_BoneTransform[index])
            {
                print("load Bone error" + m_BonesName[index]);
            }
        }
        m_moveControll = FindChild(m_man.transform, m_controlBoneName);

        //读取第一行
        m_prePath = CImportAnim.prePath + string.Format("/OutPut");
        // 创建OutPut目录
        if (false == Directory.Exists(m_prePath))
        {
            Directory.CreateDirectory(m_prePath);
        }

        string outPutPath;
        //删除第一行
        List<string> lines = new List<string>(File.ReadAllLines(allFolderTxtPath));
        lines.RemoveAt(0);
        File.WriteAllLines(allFolderTxtPath,lines.ToArray());

        // 获取摄像机
        // 先将所有拍摄摄像机全部禁用, 并将摄像机参数输出
        Transform ts = null;

        // m_texture2D = new Texture2D[m_arrCamera.Length];
        
        m_mainCamera = new Camera[m_arrCamera.Length];

        for (int i = 0; i < m_arrCamera.Length; i++)
        {
            m_arrCamera[i].SetActive(true);
			
            m_mainCamera[i] = m_arrCamera[i].GetComponent<Camera>();

           
            // 创建纹理
            m_texture2D[i] = new Texture2D(m_mainCamera[i].targetTexture.width, m_mainCamera[i].targetTexture.height, TextureFormat.ARGB32, false);

            ts = m_arrCamera[i].transform;            

            
            // 创建对应的摄像机目录用于保存2D图片
            outPutPath = m_prePath + string.Format("/Images/{0}", m_arrCamera[i].name);
            if (false == Directory.Exists(outPutPath))
            {
                Directory.CreateDirectory(outPutPath);
            }
        }

        // 动画组件
        m_animator = m_man.GetComponent<Animator>();
        m_animator.runtimeAnimatorController = Resources.Load("Animator/PosetionEx") as RuntimeAnimatorController;  
        // 获取动画状态
        m_Ac = m_animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
        Debug.Log(m_Ac);
        m_animationClip = m_Ac.animationClips;
        m_AnimationState = m_Ac.layers[0].stateMachine.states;

        m_curStateIndex = 0;
        m_needChangAnimation = true;
        // m_end = false;

        // 3D顶点保存
        m_CSV = new FileHelper();
        m_CSVCamera = new FileHelper[m_arrCamera.Length];
		m_CSVCamera2d = new FileHelper[m_arrCamera.Length];
        // 创建保存3D顶点数据的目录
        outPutPath = m_prePath + string.Format("/{0}", "3DPostion");
        if (false == Directory.Exists(outPutPath))
        {
            Directory.CreateDirectory(outPutPath);
        }

        for (int i = 0; i < m_arrCamera.Length; i++)
        {
            m_CSVCamera[i] = new FileHelper();
			m_CSVCamera2d[i] = new FileHelper();
            // 创建对应的摄像机目录用于保存摄像机下的3D姿势
            outPutPath = m_prePath + string.Format("/3DPostion/{0}", m_arrCamera[i].name);
            if (false == Directory.Exists(outPutPath))
            {
                Directory.CreateDirectory(outPutPath);
            }
			
			outPutPath = m_prePath + string.Format("/2DPostion/{0}", m_arrCamera[i].name);
            if (false == Directory.Exists(outPutPath))
            {
                Directory.CreateDirectory(outPutPath);
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // 确定是否需要切换新动作
        if (true == m_needChangAnimation)
        {
            m_animator.Play(m_AnimationState[m_curStateIndex].state.name, 0, 0);
            m_curAinmationName = m_AnimationState[m_curStateIndex].state.name;
            Debug.Log(" 当前动画名字   :"+m_curAinmationName);
            m_needChangAnimation = false;

            m_curFrame = 0;
            
            
            foreach(var item in m_animationClip){
                if(item.name == m_curAinmationName){
                    m_totalFrame = (int)(30*item.length);  
                }
            }

            // 创建对应的文件夹，保存文件
            string folderPath = null;
            for (int i = 0; i < m_arrCamera.Length; i++)
            {
                folderPath = m_prePath + string.Format("/Images/{0}/{1}", m_arrCamera[i].name, m_AnimationState[m_curStateIndex].state.name);
                if (false == Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }

            // 创建对应的CSV文件保存3D POS
            folderPath = m_prePath + string.Format("/3DPostion/{0}.csv", m_AnimationState[m_curStateIndex].state.name);
            m_CSV.CreateFile(folderPath, m_BonesPosName);
            for (int i = 0; i < m_arrCamera.Length; i++)
            {
                folderPath = m_prePath + string.Format("/3DPostion/{0}/{1}.csv", m_arrCamera[i].name, m_AnimationState[m_curStateIndex].state.name);
                m_CSVCamera[i].CreateFile(folderPath, m_BonesPosName);
				
				folderPath = m_prePath + string.Format("/2DPostion/{0}/{1}.csv", m_arrCamera[i].name, m_AnimationState[m_curStateIndex].state.name);
                m_CSVCamera2d[i].CreateFile(folderPath, m_BonesPosName);
            }

        }

        StartCoroutine(SaveImageAndData());
        m_animator.speed = 1.0f;
        m_animator.Update(1f/30f); 
        m_animator.speed = 0.0f;     
    }

     private  IEnumerator SaveImageAndData()
     {
         yield return new WaitForEndOfFrame();

       // 判断当前动画是否播放完成
        Debug.Log("m_curframe: "+m_curFrame+"      m_total:"+m_totalFrame);
        if(m_curFrame>=m_totalFrame+1)
        {
            Debug.Log("Animator Count:"+m_AnimationState.Length);
            //转换下一个动作
            m_needChangAnimation = true;

            // 动画数据采集完成，保存3D数据到CSV文件
            string csvPath = m_prePath + string.Format("/3DPostion/{0}.csv", m_curAinmationName);
            m_CSV.SaveFile(csvPath);
            for (int i = 0; i < m_arrCamera.Length; i++)
            {
                csvPath = m_prePath + string.Format("/3DPostion/{0}/{1}.csv", m_arrCamera[i].name, m_curAinmationName);
                m_CSVCamera[i].SaveFile(csvPath);
                
                csvPath = m_prePath + string.Format("/2DPostion/{0}/{1}.csv", m_arrCamera[i].name, m_curAinmationName);
                m_CSVCamera2d[i].SaveFile(csvPath);
            }

            //判断所有动画是否转换完成
            if (m_curStateIndex < m_AnimationState.Length - 1)
            {
                m_curStateIndex++;
            }
            else
            {
                Debug.Log("Done  Time :"+Time.time);
                //关闭脚本
                EditorApplication.isPlaying = false;
                InitArgs();
                
                StreamReader sr = new StreamReader(allFolderTxtPath);
                string s = sr.ReadLine();

                if(s!=null){
                   Debug.Log("下一个动画文件渲染"+s);
                   
                   if (Application.isEditor)
                    {
                       
                        CImportAnim.AutoRenderImageTool(s);
                        print("We are running this from inside of the editor!");
                    }
                  
                }else
                {
                    enabled = false;
                    Debug.Log("所有文件夹动画渲染结束");
                }
                sr.Close();
            }            
        }
        else{
            Debug.Log("输出Frame "+m_curFrame);
            // 保存每帧图片到文件
            Save();

            m_curFrame++;
            
        }
     }

    void Save()
    {
        string pngFilePath = null; 

        // 记录3D POS
        Recode3DPos();

        // 记录2D图片
        for (int i = 0; i < m_arrCamera.Length; i++)
        {
            pngFilePath = m_prePath + string.Format("/Images/{0}/{1}/{2:D4}.jpg", m_arrCamera[i].name, m_curAinmationName, m_curFrame);
            // Debug.Log(pngFilePath);
            // enabled = false;
            // bgCanvas.worldCamera = m_mainCamera[i];
            // Debug.Log(texIndex);
            //更换背景图
            // Shader.SetGlobalTexture("_SkyMainTex",texList[Random.Range(0,texList.Count-1)]);
            skyMat.SetTexture("_SkyMainTex",texList[Random.Range(0,texList.Count)]);
            CreateFrom(i, m_mainCamera[i].targetTexture);

            var bytes = m_texture2D[i].EncodeToJPG();
            System.IO.File.WriteAllBytes(pngFilePath, bytes);
        }
    }

    //  拷贝屏幕像素到纹理
    void CreateFrom(int indexCam, RenderTexture renderTexture)
    {
        var previous = RenderTexture.active;
        RenderTexture.active = renderTexture;

        m_texture2D[indexCam].ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        RenderTexture.active = previous;
        
        m_texture2D[indexCam].Apply();
        m_texture2D[indexCam].GetPixels32();
    }

    void Recode3DPos()
    {
        // 世界坐标系3D姿势
        // 写入帧号
        m_CSV.WriteFile(m_curFrame, 0, string.Format("{0}", m_curFrame));

        // 写入3D POS
        for (int i = 0; i < m_BoneTransform.Length; i++)
        {
            m_CSV.WriteFile(m_curFrame, i * 3 + 1, string.Format("{0}", -m_BoneTransform[i].position.x));
            m_CSV.WriteFile(m_curFrame, i * 3 + 2, string.Format("{0}", m_BoneTransform[i].position.z));
            m_CSV.WriteFile(m_curFrame, i * 3 + 3, string.Format("{0}", -m_BoneTransform[i].position.y));
        }

        // 摄像机坐标系3D姿势
        for (int i = 0; i < m_arrCamera.Length; i++)
        {
            m_CSVCamera[i].WriteFile(m_curFrame, 0, string.Format("{0}", m_curFrame));
			m_CSVCamera2d[i].WriteFile(m_curFrame, 0, string.Format("{0}", m_curFrame));

            Matrix4x4 m1 = m_mainCamera[i].worldToCameraMatrix;
            Matrix4x4 m2 = Matrix4x4.Rotate(new Quaternion(m1.rotation.x, m1.rotation.y, m1.rotation.z, m1.rotation.w));

            // 将世界坐标系的点转换到摄像机坐标系
            for (int j = 0; j < m_BoneTransform.Length; j++)
            {
                m_postionCamera[j] = m1.MultiplyPoint(m_BoneTransform[j].position);

                m_CSVCamera[i].WriteFile(m_curFrame, j * 3 + 1, string.Format("{0}", -m_postionCamera[j].x));
                m_CSVCamera[i].WriteFile(m_curFrame, j * 3 + 2, string.Format("{0}", m_postionCamera[j].z));
                m_CSVCamera[i].WriteFile(m_curFrame, j * 3 + 3, string.Format("{0}", -m_postionCamera[j].y));
				
				Vector3 screenPos = m_mainCamera[i].WorldToViewportPoint(m_BoneTransform[j].position); 
				m_CSVCamera2d[i].WriteFile(m_curFrame, j * 3 + 1, string.Format("{0}", screenPos[0]*1024));
				m_CSVCamera2d[i].WriteFile(m_curFrame, j * 3 + 2, string.Format("{0}", 1024-screenPos[1]*1024));
				m_CSVCamera2d[i].WriteFile(m_curFrame, j * 3 + 3, string.Format("{0}", 0));
            }
        }
    }

    void InitArgs(){
        m_CameraIndex = 0;
        m_manPath = "Prebs/Standard_Model_skin";
        m_man = null;
        m_animator = null;
        m_mainCamera = null;          // 主摄像机
        m_BoneTransform = new Transform[21];
        m_curFrame = 0;
        m_totalFrame=0;

        m_curAinmationName = "Temp";
        m_texture2D=new Texture2D[m_arrCamera.Length];

        m_animationClip = null;
        m_Ac = null;
        m_AnimationState = null;

        m_curStateIndex = 0;
        m_needChangAnimation = false;
            // private bool m_end = false;
        m_wait = 0;
        m_CSV = null;
        m_CSVCamera = null;
        m_CSVCamera2d = null;

        m_prePath = null;

        m_moveControll = null;

        m_postionCamera = new Vector3[21];
        m_controlBoneName = "Bip001";
        texList= new List<Texture2D>();
        allFolderTxtPath = CImportAnim.prePath+ string.Format("/AllFolderPath.txt");
    }
}
