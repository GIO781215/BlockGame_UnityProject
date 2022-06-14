using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public Vector3 NextPostion; 

    void Awake()
    {
        if (Instance == null)
            Instance = this;  
    }

   void Start()
    {
        NextPostion = transform.position;
    }

    public void GoUp() //讓攝影機往上移動一單位
    {
        NextPostion = transform.position + Vector3.up;
    }

    public void ToStartPos() //讓攝影機移回遊戲開始時的位置
    {
        NextPostion = new Vector3(30, 35, -30);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, NextPostion, Time.deltaTime * 2);       
    }
}
