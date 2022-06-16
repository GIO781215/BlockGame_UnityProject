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
        image = GetComponent<Image>(); //��o�� gameObject �� Image �ե�
        StartCoroutine(Effect());
    }

    IEnumerator Effect()
    {
        float duration = 0.5f;
        float timer = Time.time;

        while ((Time.time - timer) <= duration) //�j��|�b duration ���]��
        {
            image.color = SetAlpha(image.color, curve.Evaluate((Time.time - timer)/ duration));
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
