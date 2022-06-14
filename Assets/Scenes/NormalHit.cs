using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormalHit : MonoBehaviour
{
    public AnimationCurve curve;
    private Image image;


    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>(); //獲得此 gameObject 的 Image 組件
        StartCoroutine(Effect());
    }

    IEnumerator Effect()
    {
        float duration = 0.5f;
        float timer = Time.time;

        while ((Time.time - timer) <= duration) //迴圈會在 duration 內跑完
        {
            image.color = SetAlpha(image.color, curve.Evaluate((Time.time - timer)/ duration));
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
