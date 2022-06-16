using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    public static HitManager instance;

    public GameObject NormalHitSquare;
    public GameObject PerfectHitSquare;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }
     
    public void ShowNormalHitEffect(Vector3 pos, Vector3 Scale) //參數為方形特效生成的位置、大小
    {
        GameObject effect = Instantiate(NormalHitSquare, pos, Quaternion.identity, this.transform.Find("Hit_Canvas")); //第四個參數是生成的實體要把誰當作父對象，自己的 Transform 也會以父對象做為參考座標基準 (NormalHitSquare 是 Image 物件，生成時也要在 Canvas 底下才有用)
        effect.transform.position = pos; //即使 Instantiate 時有指定位置與轉向，但 Unity 把 image 轉換到 WorldSpace 的時候有時位置、轉向會跑掉，所以再重新指定一次最保險
        effect.transform.localRotation = Quaternion.identity; //即使 Instantiate 時有指定位置與轉向，但 Unity 把 image 轉換到 WorldSpace 的時候有時位置、轉向會跑掉，所以再重新指定一次最保險
        effect.GetComponent<RectTransform>().sizeDelta = new Vector2(71 * Scale.x, 71 * Scale.z); //71是老師測出來的縮放比例
    }

    public IEnumerator ShowPerfectHitEffectCombo(int combo, Vector3 pos, Vector3 scale) //參數為方形特效生成的個數、位置、大小
    {
        for(int i = 0; i < combo; i++)
        {
            ShowPerfectHitEffect(pos, scale);
            yield return new WaitForSeconds(0.12f); //每個方形特效間隔0.12秒
        }
    }

    public void ShowPerfectHitEffect(Vector3 pos, Vector3 Scale) //參數為方形特效生成的位置、大小
    {
        GameObject effect = Instantiate(PerfectHitSquare, pos, Quaternion.identity, this.transform.Find("Hit_Canvas"));
        effect.transform.position = pos;
        effect.transform.localRotation = Quaternion.identity;  
        effect.GetComponent<RectTransform>().sizeDelta = new Vector2(71 * Scale.x, 71 * Scale.z);  
    }

}
