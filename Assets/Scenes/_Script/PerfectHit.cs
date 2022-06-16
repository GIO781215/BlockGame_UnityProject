using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerfectHit : MonoBehaviour
{
    public AnimationCurve curve;
    private Image image;


    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>(); //獲得此 gameObject 的 Image 組件
        //this.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1000);
        StartCoroutine(Effect());
    }

    IEnumerator Effect()
    {
        float duration = 0.5f;
        float timer = Time.time;
        //Vector3 originalScale = transform.localScale;
        Vector2 originalscale = this.GetComponent<RectTransform>().sizeDelta;

        while ((Time.time - timer) <= duration) //迴圈會在 duration 內跑完
        {
            //使用 transform.localScale 來變換大小的話，不會有 Image Type 設為 Sliced 的效果，老師這邊好像沒注意到?
            //transform.localScale = originalScale + originalScale * curve.Evaluate((Time.time - timer) / duration); //大小隨時間變成兩倍
            this.GetComponent<RectTransform>().sizeDelta = originalscale + originalscale * curve.Evaluate((Time.time - timer) / duration); //大小隨時間變成兩倍
            image.color = SetAlpha(image.color,  1 - ((Time.time - timer) / duration));  //透明度直接線性變為零
            yield return null;
        }
        Destroy(gameObject); //銷毀本 gameObject
    }

    Color SetAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
