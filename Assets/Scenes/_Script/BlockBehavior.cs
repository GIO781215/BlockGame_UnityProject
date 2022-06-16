using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*------------------------------------------------------------

方塊腳本:

Update() : 若 Moving = ture ，刷新時會更新位置做移動

SpawnInit() : 方塊生成時、設置其位置、大小、移動方向等

Recover(): 方塊復原時、設置其位置、大小

----------------------------------------------------------------*/




public class BlockBehavior : MonoBehaviour 
{
    [HideInInspector] public Vector3 Center; //方塊中心位置
    [HideInInspector] public Vector3 MoveDirection; //移動方向 
    [HideInInspector] public bool Moving = false; //方塊移動旗標(方塊是否已到位或還要繼續移動)

    private float MovingSpeed = 10; //方塊移動速度，小心設成 public 在這邊改值就沒效果了，變成要到 Unity 裡改
    private float StartPositionOffset = 22; //方塊生成時與中心位置的偏移量
    private float StartTime;
    [HideInInspector] public bool Y_need_to_add_100 = false; //將生成時 Y 軸先減掉 100 的數值還原用的參數


    //===================================== 此 GameObject 本身的行為 =====================================

    void Start()
    {
       // Moving = true;  MoveDirection = Vector3.forward; //使第一個方塊也開始動
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
                transform.position = new Vector3(transform.position.x, transform.position.y, this.Center.z +  Mathf.PingPong((Time.time - StartTime) * MovingSpeed, StartPositionOffset) - StartPositionOffset/2f); //這邊不能寫成像 transform.position.z +  Mathf.PingPong() 這樣，這樣位置會一直加下去，這是變速移動的寫法
            }
            if(MoveDirection == Vector3.right)
            {
                transform.position = new Vector3(this.Center.x + Mathf.PingPong((Time.time - StartTime) * MovingSpeed, StartPositionOffset) - StartPositionOffset/2f, transform.position.y, transform.position.z);
            }
        }    
    }




    //===================================== 給 GameManager 使用的函數 =====================================

    public void SpawnInit(Vector3 center, Vector3 moveDirection, Vector3 scale) //方塊初始生成函數
    {
        Center = center;
        MoveDirection = moveDirection;
        StartTime = Time.time; //紀錄方塊生成時的時間
        transform.localScale = scale; //這邊的 transform.localScale 其實也是 this.transform.localScale 也就是掛載腳本的 GameObjects
    }


    public  IEnumerator Recover(Vector3 recoverScale, Vector3 recoverCenter) //方塊復原函數
    {
        float recoverTime = Time.time;
        Vector3 scaleDifference = recoverScale - transform.localScale;
        Vector3 posDifference = recoverCenter - transform.position;
        /*花一秒鐘跑完這個 while ，且長度大小剛好加完差值  
        while ((Time.time - recoverTime) < 1.0f) //一點一點加完，製造動畫效果
        {
            transform.localScale += scaleDifference * Time.deltaTime ;
            transform.position += posDifference * Time.deltaTime;
            yield return null; //假 return 先把執行權交出去，但一有空就要回來繼續跑，跑到 while 為 false 跳出去跑到 Recover() 最底整個跑完才算結束掉這個函數
        }
        */


        Vector3 oldLocalScale = transform.localScale; 
        //改寫成花 0.25 秒跑完這個 while
        while ((Time.time - recoverTime) <= 0.25f)  
        {
            transform.localScale = oldLocalScale + scaleDifference * ((Time.time - recoverTime) / 0.25f); //依時間來增加數值的另一種寫法
            transform.position += posDifference * Time.deltaTime * 4;
            yield return null;
        }
        
        transform.localScale = recoverScale;
        transform.position = recoverCenter;
    }




}
