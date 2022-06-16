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
        image = GetComponent<Image>(); //��o�� gameObject �� Image �ե�
        //this.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1000);
        StartCoroutine(Effect());
    }

    IEnumerator Effect()
    {
        float duration = 0.5f;
        float timer = Time.time;
        //Vector3 originalScale = transform.localScale;
        Vector2 originalscale = this.GetComponent<RectTransform>().sizeDelta;

        while ((Time.time - timer) <= duration) //�j��|�b duration ���]��
        {
            //�ϥ� transform.localScale ���ܴ��j�p���ܡA���|�� Image Type �]�� Sliced ���ĪG�A�Ѯv�o��n���S�`�N��?
            //transform.localScale = originalScale + originalScale * curve.Evaluate((Time.time - timer) / duration); //�j�p�H�ɶ��ܦ��⭿
            this.GetComponent<RectTransform>().sizeDelta = originalscale + originalscale * curve.Evaluate((Time.time - timer) / duration); //�j�p�H�ɶ��ܦ��⭿
            image.color = SetAlpha(image.color,  1 - ((Time.time - timer) / duration));  //�z���ת����u���ܬ��s
            yield return null;
        }
        Destroy(gameObject); //�P���� gameObject
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
