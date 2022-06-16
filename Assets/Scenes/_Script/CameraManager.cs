using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; //這跟老師用的不一樣，我用的 PostProcessing 是本來就有在 Unity 的 Package 中的 (所以取用方法也有不同)

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public Vector3 NextPostion;
    //public PostProcessProfile profile; //我用的 PostProcessing 不需要

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
        NextPostion = transform.position;

        myPostProcessVolume = GetComponent<PostProcessVolume>(); //得到在 Unity 裡的 PostProcessVolume 組件
        myPostProcessVolume.profile.TryGetSettings(out myDepthOfField); //得到 PostProcessVolume 裡的 DepthOfField 項目
    }

    public void GoUp() //讓攝影機往上移動一單位
    {
        NextPostion = transform.position + Vector3.up;
    }

    public void ToStartPos() //讓攝影機移回遊戲開始時的位置
    {
        NextPostion = new Vector3(30, 35, -30);
    }

    public void ResetFocusDistance() //遊戲開始時將鏡頭焦距設為5
    {
        myDepthOfField.focusDistance.value = 5f;
    }

    public IEnumerator Blur() //將鏡頭焦距變模糊
    {
        myDepthOfField.focusDistance.value = 5f; //如果瘋狂亂點輸掉又馬上重來就會讓鏡頭的焦距數值亂跑，所以先直接將初始值設為固定

        float startTime = Time.time;
        float duration = 1f; //將時間間隔設為 1 秒
        float deltaDepthOfFiel = -3f; //將焦距縮小 3
        float _DOF = myDepthOfField.focusDistance.value;
        while ((Time.time - startTime) < duration) //在裡面執行到 duration 秒為止
        {
            myDepthOfField.focusDistance.value = _DOF + ((Time.time - startTime) / duration) * deltaDepthOfFiel; //在 duration 秒間慢慢地把焦距變小
            yield return null;
        }

        myDepthOfField.focusDistance.value = 2f;
    }

    public IEnumerator Focus() //將鏡頭對焦變清晰
    {
        myDepthOfField.focusDistance.value = 2f; //如果瘋狂亂點輸掉又馬上重來就會讓鏡頭的焦距數值亂跑，所以先直接將初始值設為固定

        float startTime = Time.time;
        float duration = 1f; //將時間間隔設為 1 秒
        float deltaDepthOfFiel = 3f; //將焦距放大 3
        float _DOF = myDepthOfField.focusDistance.value;
        while ((Time.time - startTime) < duration) //在裡面執行到 duration 秒為止
        {
            myDepthOfField.focusDistance.value = _DOF + ((Time.time - startTime) / duration) * deltaDepthOfFiel; //在 duration 秒間慢慢地把焦距放大
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
