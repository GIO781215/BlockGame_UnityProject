using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//WebGL branch 
/*------------------------------------------------------------

遊戲主循環控制器:

執行 Start() 時進入遊戲主循環函數 GameLoop()
{
    初始化 : Init() // 設置 FirstBlock 的位置、大小，和設置將要生成方塊的位置、大小、移動方向 CurrentCenter、CurrentScale、CurrentMoveDirection
        while (!isFail)
        {
            生成方塊 : SpawnBlock() //  用參數 CurrentCenter、CurrentScale、CurrentMoveDirection 生成新方塊，並更新 MovingBlock 為此新生方塊

            等待按鈕按下 : yield return new WaitUntil(() => Input.GetMouseButtonDown(0)) 
　
            移動相機 : CameraManager.GoUp() //攝影機往上移動一單位

   　       計算方塊裁切結果 : CutBlockForward() 或 CutBlockRight() //三種結果: 1.完全沒對上，失敗遊戲結束    2.完全對上 { 更新 CurrentCenter; Blocks.Add(aboveBlock); }   3.部分對上 { 更新 CurrentCenter; 更新 CurrentScale; Blocks.Add(aboveBlock); RecoverBlocks.Push(aboveBlock); } 
           
            判斷遊戲是否結束 : if (isFail) break;

            改變方塊移動方向 : ChangeBlockDirection()

            判斷是否達成 Combo 與復原方塊 : RecoverBlock(); //再次更新 CurrentCenter 與 CurrentScale ，並改變當前移動(但現在其實已經停止了)方塊的中心位置與大小
        }
}

----------------------------------------------------------------*/




public class GameManager : MonoBehaviour
{
    //關鍵字 region 可以設定摺疊區塊
    #region Variable 

    public static GameManager Instance; 
    public GameObject BlockPerfab;  //之要複製方塊時的模板物件(使用一樣屬性、材質等)
    public GameObject FirstBlock; //第一個方塊
    public List<GameObject> Blocks = new List<GameObject>(); //到目前為止生成的所有方塊
    public Stack<GameObject> RecoverBlocks = new Stack<GameObject>(); //要復原的方塊堆疊
    [HideInInspector] public Vector3 CurrentCenter; //生成方塊的中心位置
    [HideInInspector] public Vector3 CurrentScale; //生成方塊的大小
    [HideInInspector] public Vector3 CurrentMoveDirection; //生成方塊的方向
    public Text scoreText;
    public int Score = 0;
    public int Combo = 0;
    public GameObject MovingBlock; //正在移動的方塊
    public GameObject StartButton;
    public GameObject RestartButton;
    private bool isFail = false;

    #endregion 

    void Awake()
    { 
        if (Instance == null) //原則上 GameManager 物件只會存在一個，並指定給 instance，來讓在其他地方的程式能夠方便地使用到 GameManager 內的其他函數，此種設計方式叫做單例模式
            Instance = this; //當 Unity 初始化生成本 GameManager 物件時，將其指定給 instance 
   /*   
        else
        {
            Destroy(this);
            Debug.LogWarning("本該只存在一個的 GameManager 莫名其妙多生成了一個，請檢察程式碼");
        }
   */
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameLoop());  // <!>.1 執行 IEnumerator 函數的方法
    }

    void Init()
    {
        //設置 FirstBlock 的位置、大小
        FirstBlock.transform.position = new Vector3(0, 0, 0);
        FirstBlock.transform.localScale = new Vector3(7, 1, 7);

        //指定即將要生成方塊的位置、大小、移動方向
        CurrentCenter = new Vector3(0, 1, 0);  
        CurrentScale = FirstBlock.transform.localScale;  
        CurrentMoveDirection = Vector3.forward;  

        Blocks.Add(FirstBlock); //將第一個方塊加入所有方塊 List
        RecoverBlocks.Push(FirstBlock); //將第一個方塊加入復原方塊Stack
    }
 
    void SpawnBlock() //方塊生成函數
    {
        GameObject newMovingBlock = Instantiate(BlockPerfab, CurrentCenter, Quaternion.identity, this.transform.Find("tempBlocks"));  //實例化 GameObject 物件 (第四個參數是生成的實體要把誰當作父對象)                                                                                                        
        newMovingBlock.GetComponent<BlockBehavior>().SpawnInit(CurrentCenter, CurrentMoveDirection, CurrentScale); //因為實例化時指定了 BlockPerfab 做為模板物件 , 且在 Unity 中有將其設為 FirstBlock , 且有把 BlockBehavior 腳本掛在 FirstBlock 下 , 所以能夠用 GetComponent<BlockBehavior> 取用其中的函數與變數    
        MovingBlock = newMovingBlock; 

    }

    void CutBlockForward(GameObject aboveBlock, GameObject belowBlock) //方塊裁切計算(Forward方向)
    {
        Vector3 belowBlockCenter = belowBlock.transform.position; //下方方塊的中心位置
        Vector3 belowBlockScale = belowBlock.transform.localScale; //下方方塊的大小
        Vector3 aboveBlockCenter = aboveBlock.transform.position; //上方方塊的中心位置
        Vector3 aboveBlockScale = aboveBlock.transform.localScale; //上方方塊的大小
        float offset = (aboveBlockCenter - belowBlockCenter).z; //兩方塊的偏移量

        //如果偏移量大於下方方塊的寬度則遊戲結束
        if (Mathf.Abs(offset) > belowBlockScale.z)
        {
            aboveBlock.AddComponent<Rigidbody>(); //給方塊加上剛體組件使其墜落
            aboveBlock.GetComponent<BlockBehavior>().Moving = false; //將 aboveBlock 的腳本 BlockBehavior 中的 Moving 屬性設為 false , 這樣就不會再執行其 Update() 裡的函數 , 就不會再改變位置了
            isFail=true;
            return;
        }

        //如果幾乎沒有偏移
        if (Mathf.Abs(offset) < 0.5f)
        {
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;
            aboveBlock.transform.position = CurrentCenter;
            CurrentCenter += Vector3.up;
            Blocks.Add(aboveBlock);
            Score++;
            Combo++;

            //點中音效
            AudioManager.Instance.PlayComboSound(Combo);
            //點中特效
            HitManager.instance.ShowNormalHitEffect(aboveBlock.transform.position - Vector3.up*0.5f, aboveBlock.transform.localScale);
            if (Combo >= 3) //點中大特效
            {
                int effectCount = Mathf.Min(Combo-2, 3);
                StartCoroutine( HitManager.instance.ShowPerfectHitEffectCombo(effectCount, aboveBlock.transform.position - Vector3.up * 0.5f, aboveBlock.transform.localScale) );                      
            }
            return;
        }

        //如果是點擊時有偏移的情況 
        {
            //計算一分為二的 A、B 方塊的中心位置與大小
            Vector3 A_Center = (belowBlockCenter + aboveBlockCenter) / 2f + Vector3.up * 0.5f; //A方塊中心位置
            Vector3 A_Scale = new Vector3(belowBlockScale.x, 1f, belowBlockScale.z - Mathf.Abs(offset)); //A方塊大小
            Vector3 B_Center = Vector3.zero; //B方塊中心位置
            if (offset > 0)
                B_Center = belowBlockCenter + ((belowBlockScale.z + offset) / 2f) * Vector3.forward + Vector3.up;
            if (offset < 0)
                B_Center = belowBlockCenter + ((-belowBlockScale.z + offset) / 2f) * Vector3.forward + Vector3.up;
            Vector3 B_Scale = new Vector3(belowBlockScale.x, 1f, Mathf.Abs(offset)); //B方塊大小

            //將 aboveBlock 當做 A 方塊
            aboveBlock.transform.position = A_Center;
            aboveBlock.transform.localScale = A_Scale;
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;

            //新增一個 Block 做為 B 方塊
            GameObject B_Block = Instantiate(BlockPerfab, B_Center, Quaternion.identity, this.transform.Find("tempBlocks"));
            B_Block.transform.localScale = B_Scale;
            B_Block.AddComponent<Rigidbody>();

            //變數更新
            CurrentCenter = aboveBlock.transform.position + Vector3.up;
            CurrentScale = aboveBlock.transform.localScale;
            Score++;
            Combo = 0;

            Blocks.Add(aboveBlock);
            RecoverBlocks.Push(aboveBlock);
        }

    }

    void CutBlockRight(GameObject aboveBlock, GameObject belowBlock) //方塊裁切計算(Right方向)
    {
        Vector3 belowBlockCenter = belowBlock.transform.position; //下方方塊的中心位置
        Vector3 belowBlockScale = belowBlock.transform.localScale; //下方方塊的大小
        Vector3 aboveBlockCenter = aboveBlock.transform.position; //上方方塊的中心位置
        Vector3 aboveBlockScale = aboveBlock.transform.localScale; //上方方塊的大小
        float offset = (aboveBlockCenter - belowBlockCenter).x; //兩方塊的偏移量

        //如果偏移量大於下方方塊的寬度則遊戲結束
        if (Mathf.Abs(offset) > belowBlockScale.x) 
        {
            aboveBlock.AddComponent<Rigidbody>(); //給方塊加上剛體組件使其墜落
            aboveBlock.GetComponent<BlockBehavior>().Moving = false; //將 aboveBlock 的腳本 BlockBehavior 中的 Moving 屬性設為 false , 這樣就不會再執行其 Update() 裡的函數 , 就不會再改變位置了
            isFail = true;
            return;
        }

        //如果幾乎沒有偏移
        if (Mathf.Abs(offset) < 0.5f) 
        {
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;
            aboveBlock.transform.position = CurrentCenter;
            CurrentCenter += Vector3.up;
            Blocks.Add(aboveBlock);
            Score++;
            Combo++;

            //點中音效
            AudioManager.Instance.PlayComboSound(Combo);
            //點中特效
            HitManager.instance.ShowNormalHitEffect(aboveBlock.transform.position - Vector3.up * 0.5f, aboveBlock.transform.localScale);
            if (Combo >= 3) //點中大特效
            {
                int effectCount = Mathf.Min(Combo - 2, 3);
                StartCoroutine(HitManager.instance.ShowPerfectHitEffectCombo(effectCount, aboveBlock.transform.position - Vector3.up * 0.5f, aboveBlock.transform.localScale));
            }
            return;
        }

        //如果是點擊時有偏移的情況 
        {
            //計算一分為二的 A、B 方塊的中心位置與大小
            Vector3 A_Center = (belowBlockCenter + aboveBlockCenter) / 2f + Vector3.up * 0.5f; //A方塊中心位置
            Vector3 A_Scale = new Vector3(belowBlockScale.x - Mathf.Abs(offset), 1f, belowBlockScale.z); //A方塊大小
            Vector3 B_Center = Vector3.zero; //B方塊中心位置
            if (offset > 0)
                B_Center = belowBlockCenter + ((belowBlockScale.x + offset) / 2f) * Vector3.right + Vector3.up;
            if (offset < 0)
                B_Center = belowBlockCenter + ((-belowBlockScale.x + offset) / 2f) * Vector3.right + Vector3.up;
            Vector3 B_Scale = new Vector3(Mathf.Abs(offset), 1f, belowBlockScale.z); //B方塊大小

            //將 aboveBlock 當做 A 方塊
            aboveBlock.transform.position = A_Center;
            aboveBlock.transform.localScale = A_Scale;
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;

            //新增一個 Block 做為 B 方塊
            GameObject B_Block = Instantiate(BlockPerfab, B_Center, Quaternion.identity, this.transform.Find("tempBlocks"));
            B_Block.transform.localScale = B_Scale;
            B_Block.AddComponent<Rigidbody>();

            //變數更新
            CurrentCenter = aboveBlock.transform.position + Vector3.up;
            CurrentScale = aboveBlock.transform.localScale;
            Score++;
            Combo = 0;

            Blocks.Add(aboveBlock);
            RecoverBlocks.Push(aboveBlock);
        }

    }

    void ChangeBlockDirection()
    {
        if (CurrentMoveDirection == Vector3.forward)
            CurrentMoveDirection = Vector3.right;
        else
            CurrentMoveDirection = Vector3.forward;

    }

    public void RecoverBlock()
    {
        if (RecoverBlocks.Count > 1) //第一個一定是 FirstBlock
            RecoverBlocks.Pop(); //先推掉剛剛加入的復原形狀，真正要復原的形狀再前一個!

        CurrentScale = RecoverBlocks.Peek().transform.localScale;
        CurrentCenter = new Vector3(RecoverBlocks.Peek().transform.position.x, CurrentCenter.y, RecoverBlocks.Peek().transform.position.z);
        StartCoroutine(  MovingBlock.GetComponent<BlockBehavior>().Recover(CurrentScale, (CurrentCenter - Vector3.up))  );

        //雖然剛剛讓 MovingBlock 變形了，但不必再更新陣列裡的方塊 Blocks[Score] 一次，因為 List 裡紀錄的方塊跟當前的是同一個物件，並不是副本，所以改變 List 裡的方塊畫面中之前的方塊也都會被改變!!!
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@好像有不是耶，用 Blocks.Clear 清空了但畫面上還是殘留著之前的方塊
    }

    public void Restart()
    {
        //把所有在 tempBlocks 下的方塊全部都抓出來銷毀 //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@這寫法再研究
        foreach (var gameObj in GetComponentsInChildren<BlockBehavior>()) //獲取所有擁有指定組件（BlockBehavior）的子項，返回那些子項組成的陣列。如果子項有的 SetActive 是 false 則獲取不到 (有第二個參數 GetComponentsInChildren<組件>(true) 讓此也能獲取的到)
        {
            Destroy(gameObj.gameObject);
        }

        Blocks.Clear();
        RecoverBlocks.Clear();
        Score = 0;
        Combo = 0;
        isFail = false;

        CameraManager.Instance.ToStartPos();  
    }

    IEnumerator GameLoop()  // <!>.1 用 IEnumerator 宣告的函數可用 Wait 類的 yield return 來讓某些會造成 delay 的函數不要一直卡著
    {

        while (true)
        {

            Init();
            yield return new WaitUntil(() => !StartButton.activeInHierarchy); //如果開始按鈕按下把自己取消 SetActive 後才放行
            AudioManager.Instance.PlayStartSound(); //播放遊戲開始音效
            while (!isFail)
            {

                SpawnBlock();

                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

                CameraManager.Instance.GoUp();

                if (CurrentMoveDirection == Vector3.forward)
                    CutBlockForward(MovingBlock, Blocks[Score]);
                else if (CurrentMoveDirection == Vector3.right)
                    CutBlockRight(MovingBlock, Blocks[Score]);

                if (isFail) break;

                ChangeBlockDirection();

                if (Combo >= 5)
                {
                    RecoverBlock();
                    //Combo=0; //讓玩家連續 Combo 的話也能一直復原方塊好了
                }

                yield return null; //這行是為了讓 Input.GetMouseButtonDown(0) 更新而加的，先跳出去迴圈讓其值重置，不然立刻重頭 loop 的話會直接通過 yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }

            //Print Fail UI
            RestartButton.SetActive(true); //讓重新開始按鈕顯示出來
            yield return new WaitUntil(() => !RestartButton.activeInHierarchy); //等待重新開始按鈕被按下
            Restart();

        }
    }


    // Update is called once per frame
    void Update()
    {
        scoreText.text = Score.ToString(); //一直讓 scoreText 顯示當前的分數

        //#if UNITY_EDITOR 裡的內容在 Unity 輸出成遊戲時不會被編譯，可放心地把 debug code 寫在裡面
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.A)) //遊戲開始後按下 A 鍵可以執行某函數，這樣 debug 很方便
        {
            //某函數
        }
        #endif
    }
}
