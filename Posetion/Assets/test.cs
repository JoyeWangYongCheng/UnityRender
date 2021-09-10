using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    // Use this for initialization
    private Animator m_animator;
    private int m_curFrame;
    private Transform bipTransform;
    void Start () {
        m_animator = transform.GetComponent<Animator>();
        bipTransform = transform.GetComponentsInChildren<Transform>()[1];
        Debug.Log("输出Frame " + m_curFrame + "            bip001：" + bipTransform.position.x);
    }
	
	// Update is called once per frame
	void Update () {
        m_animator.speed = 1.0f;
       
        Debug.Log("输出Frame " + m_curFrame + "            bip001：" + bipTransform.position.x);
        m_curFrame++;
        //StartCoroutine(Test());

        m_animator.Update(1f / 30f);
        m_animator.speed = 0.0f;

    }

    private IEnumerator Test()
    {
        Debug.Log("输出Frame " + m_curFrame + "            bip001：" + bipTransform.position.x);
        m_curFrame++;

        yield return new WaitForEndOfFrame();


    }
}
