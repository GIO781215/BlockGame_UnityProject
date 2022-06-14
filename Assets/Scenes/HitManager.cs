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
     
    public void ShowNormalHitEffect(Vector3 pos, Vector3 Scale) //�ѼƬ���ίS�ĥͦ�����m�B�j�p
    {
        GameObject effect = Instantiate(NormalHitSquare, pos, Quaternion.identity, this.transform.Find("Hit_Canvas")); //�ĥ|�ӰѼƬO�ͦ�������n��ַ�@����H�A�ۤv�� Transform �]�|�H����H�����ѦҮy�а�� (NormalHitSquare �O Image ����A�ͦ��ɤ]�n�b Canvas ���U�~����)
        effect.transform.position = pos; //�Y�� Instantiate �ɦ����w��m�P��V�A�� Unity �� image �ഫ�� WorldSpace ���ɭԦ��ɦ�m�B��V�|�]���A�ҥH�A���s���w�@���̫O�I
        effect.transform.localRotation = Quaternion.identity; //�Y�� Instantiate �ɦ����w��m�P��V�A�� Unity �� image �ഫ�� WorldSpace ���ɭԦ��ɦ�m�B��V�|�]���A�ҥH�A���s���w�@���̫O�I
        effect.GetComponent<RectTransform>().sizeDelta = new Vector2(71 * Scale.x, 71 * Scale.z); //71�O�Ѯv���X�Ӫ��Y����
    }

    public IEnumerator ShowPerfectHitEffectCombo(int combo, Vector3 pos, Vector3 scale) //�ѼƬ���ίS�ĥͦ����ӼơB��m�B�j�p
    {
        for(int i = 0; i < combo; i++)
        {
            ShowPerfectHitEffect(pos, scale);
            yield return new WaitForSeconds(0.12f); //�C�Ӥ�ίS�Ķ��j0.12��
        }
    }

    public void ShowPerfectHitEffect(Vector3 pos, Vector3 Scale) //�ѼƬ���ίS�ĥͦ�����m�B�j�p
    {
        GameObject effect = Instantiate(PerfectHitSquare, pos, Quaternion.identity, this.transform.Find("Hit_Canvas"));
        effect.transform.position = pos;
        effect.transform.localRotation = Quaternion.identity;  
        effect.GetComponent<RectTransform>().sizeDelta = new Vector2(71 * Scale.x, 71 * Scale.z);  
    }

}
