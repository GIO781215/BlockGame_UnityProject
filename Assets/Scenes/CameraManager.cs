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

    public void GoUp() //����v�����W���ʤ@���
    {
        NextPostion = transform.position + Vector3.up;
    }

    public void ToStartPos() //����v�����^�C���}�l�ɪ���m
    {
        NextPostion = new Vector3(30, 35, -30);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, NextPostion, Time.deltaTime * 2);       
    }
}
