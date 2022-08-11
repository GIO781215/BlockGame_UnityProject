using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; //�o��Ѯv�Ϊ����@�ˡA�ڥΪ� PostProcessing �O���ӴN���b Unity �� Package ���� (�ҥH���Τ�k�]�����P)

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public Vector3 startPosition; //��v�����_�l�y��
    public Vector3 NextPostion;
    //public PostProcessProfile profile; //�ڥΪ� PostProcessing ���ݭn

    private AmbientOcclusion _ao;
    private DepthOfField myDepthOfField;
    private PostProcessVolume myPostProcessVolume;
      

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

   void Start()
    {
        startPosition = transform.position;
        NextPostion = transform.position;

        myPostProcessVolume = GetComponent<PostProcessVolume>(); //�o��b Unity �̪� PostProcessVolume �ե�
        myPostProcessVolume.profile.TryGetSettings(out myDepthOfField); //�o�� PostProcessVolume �̪� DepthOfField ����
    }

    public void GoUp() //����v�����W���ʤ@���
    {
        if (GameManager.Instance.Score > 3)
            NextPostion = startPosition + (GameManager.Instance.Score - 3) * Vector3.up;
    }

    public void ToStartPos() //����v�����^�C���}�l�ɪ���m
    {
        NextPostion = new Vector3(30, 35, -30);
    }

    public void ResetFocusDistance() //�C���}�l�ɱN���Y�J�Z�]��5
    {
        myDepthOfField.focusDistance.value = 5f;
    }

    public IEnumerator Blur() //�N���Y�J�Z�ܼҽk
    {
        myDepthOfField.focusDistance.value = 5f; //�p�G�ƨg���I�鱼�S���W���ӴN�|�����Y���J�Z�ƭȶö]�A�ҥH�������N��l�ȳ]���T�w

        float startTime = Time.time;
        float duration = 1f; //�N�ɶ����j�]�� 1 ��
        float deltaDepthOfFiel = -3f; //�N�J�Z�Y�p 3
        float _DOF = myDepthOfField.focusDistance.value;
        while ((Time.time - startTime) < duration) //�b�̭������ duration ����
        {
            myDepthOfField.focusDistance.value = _DOF + ((Time.time - startTime) / duration) * deltaDepthOfFiel; //�b duration ���C�C�a��J�Z�ܤp
            yield return null;
        }

        myDepthOfField.focusDistance.value = 2f;
    }

    public IEnumerator Focus() //�N���Y��J�ܲM��
    {
        myDepthOfField.focusDistance.value = 2f; //�p�G�ƨg���I�鱼�S���W���ӴN�|�����Y���J�Z�ƭȶö]�A�ҥH�������N��l�ȳ]���T�w

        float startTime = Time.time;
        float duration = 1f; //�N�ɶ����j�]�� 1 ��
        float deltaDepthOfFiel = 3f; //�N�J�Z��j 3
        float _DOF = myDepthOfField.focusDistance.value;
        while ((Time.time - startTime) < duration) //�b�̭������ duration ����
        {
            myDepthOfField.focusDistance.value = _DOF + ((Time.time - startTime) / duration) * deltaDepthOfFiel; //�b duration ���C�C�a��J�Z��j
            yield return null;
        }

        myDepthOfField.focusDistance.value = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, NextPostion, Time.deltaTime * 2);       
    }
}
