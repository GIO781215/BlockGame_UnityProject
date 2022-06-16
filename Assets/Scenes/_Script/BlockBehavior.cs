using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*------------------------------------------------------------

����}��:

Update() : �Y Moving = ture �A��s�ɷ|��s��m������

SpawnInit() : ����ͦ��ɡB�]�m���m�B�j�p�B���ʤ�V��

Recover(): ����_��ɡB�]�m���m�B�j�p

----------------------------------------------------------------*/




public class BlockBehavior : MonoBehaviour 
{
    [HideInInspector] public Vector3 Center; //������ߦ�m
    [HideInInspector] public Vector3 MoveDirection; //���ʤ�V 
    [HideInInspector] public bool Moving = false; //������ʺX��(����O�_�w�����٭n�~�򲾰�)

    private float MovingSpeed = 10; //������ʳt�סA�p�߳]�� public �b�o���ȴN�S�ĪG�F�A�ܦ��n�� Unity �̧�
    private float StartPositionOffset = 22; //����ͦ��ɻP���ߦ�m�������q
    private float StartTime;
    [HideInInspector] public bool Y_need_to_add_100 = false; //�N�ͦ��� Y �b��� 100 ���ƭ��٭�Ϊ��Ѽ�


    //===================================== �� GameObject �������欰 =====================================

    void Start()
    {
       // Moving = true;  MoveDirection = Vector3.forward; //�ϲĤ@�Ӥ���]�}�l��
    }

    void Update()
    {
        if (Y_need_to_add_100 == true)
        {
            transform.position += new Vector3(0, 100, 0);
            Y_need_to_add_100 = false;
        }

        if (Moving)
        {
            if(MoveDirection == Vector3.forward)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, this.Center.z +  Mathf.PingPong((Time.time - StartTime) * MovingSpeed, StartPositionOffset) - StartPositionOffset/2f); //�o�䤣��g���� transform.position.z +  Mathf.PingPong() �o�ˡA�o�˦�m�|�@���[�U�h�A�o�O�ܳt���ʪ��g�k
            }
            if(MoveDirection == Vector3.right)
            {
                transform.position = new Vector3(this.Center.x + Mathf.PingPong((Time.time - StartTime) * MovingSpeed, StartPositionOffset) - StartPositionOffset/2f, transform.position.y, transform.position.z);
            }
        }    
    }




    //===================================== �� GameManager �ϥΪ���� =====================================

    public void SpawnInit(Vector3 center, Vector3 moveDirection, Vector3 scale) //�����l�ͦ����
    {
        Center = center;
        MoveDirection = moveDirection;
        StartTime = Time.time; //��������ͦ��ɪ��ɶ�
        transform.localScale = scale; //�o�䪺 transform.localScale ���]�O this.transform.localScale �]�N�O�����}���� GameObjects
    }


    public  IEnumerator Recover(Vector3 recoverScale, Vector3 recoverCenter) //����_����
    {
        float recoverTime = Time.time;
        Vector3 scaleDifference = recoverScale - transform.localScale;
        Vector3 posDifference = recoverCenter - transform.position;
        /*��@�����]���o�� while �A�B���פj�p��n�[���t��  
        while ((Time.time - recoverTime) < 1.0f) //�@�I�@�I�[���A�s�y�ʵe�ĪG
        {
            transform.localScale += scaleDifference * Time.deltaTime ;
            transform.position += posDifference * Time.deltaTime;
            yield return null; //�� return ��������v��X�h�A���@���ŴN�n�^���~��]�A�]�� while �� false ���X�h�]�� Recover() �̩���Ӷ]���~�⵲�����o�Ө��
        }
        */


        Vector3 oldLocalScale = transform.localScale; 
        //��g���� 0.25 ��]���o�� while
        while ((Time.time - recoverTime) <= 0.25f)  
        {
            transform.localScale = oldLocalScale + scaleDifference * ((Time.time - recoverTime) / 0.25f); //�̮ɶ��ӼW�[�ƭȪ��t�@�ؼg�k
            transform.position += posDifference * Time.deltaTime * 4;
            yield return null;
        }
        
        transform.localScale = recoverScale;
        transform.position = recoverCenter;
    }




}
