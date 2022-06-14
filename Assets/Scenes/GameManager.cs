using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//WebGL branch
//GitHub test
/*------------------------------------------------------------

�C���D�`�����:

���� Start() �ɶi�J�C���D�`����� GameLoop()
{
    ��l�� : Init() // �]�m FirstBlock ����m�B�j�p�A�M�]�m�N�n�ͦ��������m�B�j�p�B���ʤ�V CurrentCenter�BCurrentScale�BCurrentMoveDirection
        while (!isFail)
        {
            �ͦ���� : SpawnBlock() //  �ΰѼ� CurrentCenter�BCurrentScale�BCurrentMoveDirection �ͦ��s����A�ç�s MovingBlock �����s�ͤ��

            ���ݫ��s���U : yield return new WaitUntil(() => Input.GetMouseButtonDown(0)) 
�@
            ���ʬ۾� : CameraManager.GoUp() //��v�����W���ʤ@���

   �@       �p�����������G : CutBlockForward() �� CutBlockRight() //�T�ص��G: 1.�����S��W�A���ѹC������    2.������W { ��s CurrentCenter; Blocks.Add(aboveBlock); }   3.������W { ��s CurrentCenter; ��s CurrentScale; Blocks.Add(aboveBlock); RecoverBlocks.Push(aboveBlock); } 
           
            �P�_�C���O�_���� : if (isFail) break;

            ���ܤ�����ʤ�V : ChangeBlockDirection()

            �P�_�O�_�F�� Combo �P�_���� : RecoverBlock(); //�A����s CurrentCenter �P CurrentScale �A�ç��ܷ�e����(���{�b���w�g����F)��������ߦ�m�P�j�p
        }
}

----------------------------------------------------------------*/




public class GameManager : MonoBehaviour
{
    //����r region �i�H�]�w�P�|�϶�
    #region Variable 

    public static GameManager Instance; 
    public GameObject BlockPerfab;  //���n�ƻs����ɪ��ҪO����(�ϥΤ@���ݩʡB���赥)
    public GameObject FirstBlock; //�Ĥ@�Ӥ��
    public List<GameObject> Blocks = new List<GameObject>(); //��ثe����ͦ����Ҧ����
    public Stack<GameObject> RecoverBlocks = new Stack<GameObject>(); //�n�_�쪺������|
    [HideInInspector] public Vector3 CurrentCenter; //�ͦ���������ߦ�m
    [HideInInspector] public Vector3 CurrentScale; //�ͦ�������j�p
    [HideInInspector] public Vector3 CurrentMoveDirection; //�ͦ��������V
    public Text scoreText;
    public int Score = 0;
    public int Combo = 0;
    public GameObject MovingBlock; //���b���ʪ����
    public GameObject StartButton;
    public GameObject RestartButton;
    private bool isFail = false;

    #endregion 

    void Awake()
    { 
        if (Instance == null) //��h�W GameManager ����u�|�s�b�@�ӡA�ë��w�� instance�A�����b��L�a�誺�{�������K�a�ϥΨ� GameManager ������L��ơA���س]�p�覡�s����ҼҦ�
            Instance = this; //�� Unity ��l�ƥͦ��� GameManager ����ɡA�N����w�� instance 
   /*   
        else
        {
            Destroy(this);
            Debug.LogWarning("���ӥu�s�b�@�Ӫ� GameManager ���W�䧮�h�ͦ��F�@�ӡA���˹�{���X");
        }
   */
    }


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameLoop());  // <!>.1 ���� IEnumerator ��ƪ���k
    }

    void Init()
    {
        //�]�m FirstBlock ����m�B�j�p
        FirstBlock.transform.position = new Vector3(0, 0, 0);
        FirstBlock.transform.localScale = new Vector3(7, 1, 7);

        //���w�Y�N�n�ͦ��������m�B�j�p�B���ʤ�V
        CurrentCenter = new Vector3(0, 1, 0);  
        CurrentScale = FirstBlock.transform.localScale;  
        CurrentMoveDirection = Vector3.forward;  

        Blocks.Add(FirstBlock); //�N�Ĥ@�Ӥ���[�J�Ҧ���� List
        RecoverBlocks.Push(FirstBlock); //�N�Ĥ@�Ӥ���[�J�_����Stack
    }
 
    void SpawnBlock() //����ͦ����
    {
        GameObject newMovingBlock = Instantiate(BlockPerfab, CurrentCenter, Quaternion.identity, this.transform.Find("tempBlocks"));  //��Ҥ� GameObject ���� (�ĥ|�ӰѼƬO�ͦ�������n��ַ�@����H)                                                                                                        
        newMovingBlock.GetComponent<BlockBehavior>().SpawnInit(CurrentCenter, CurrentMoveDirection, CurrentScale); //�]����ҤƮɫ��w�F BlockPerfab �����ҪO���� , �B�b Unity �����N��]�� FirstBlock , �B���� BlockBehavior �}�����b FirstBlock �U , �ҥH����� GetComponent<BlockBehavior> ���Ψ䤤����ƻP�ܼ�    
        MovingBlock = newMovingBlock; 

    }

    void CutBlockForward(GameObject aboveBlock, GameObject belowBlock) //��������p��(Forward��V)
    {
        Vector3 belowBlockCenter = belowBlock.transform.position; //�U���������ߦ�m
        Vector3 belowBlockScale = belowBlock.transform.localScale; //�U�������j�p
        Vector3 aboveBlockCenter = aboveBlock.transform.position; //�W���������ߦ�m
        Vector3 aboveBlockScale = aboveBlock.transform.localScale; //�W�������j�p
        float offset = (aboveBlockCenter - belowBlockCenter).z; //�����������q

        //�p�G�����q�j��U�������e�׫h�C������
        if (Mathf.Abs(offset) > belowBlockScale.z)
        {
            aboveBlock.AddComponent<Rigidbody>(); //������[�W����ե�Ϩ�Y��
            aboveBlock.GetComponent<BlockBehavior>().Moving = false; //�N aboveBlock ���}�� BlockBehavior ���� Moving �ݩʳ]�� false , �o�˴N���|�A����� Update() �̪���� , �N���|�A���ܦ�m�F
            isFail=true;
            return;
        }

        //�p�G�X�G�S������
        if (Mathf.Abs(offset) < 0.5f)
        {
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;
            aboveBlock.transform.position = CurrentCenter;
            CurrentCenter += Vector3.up;
            Blocks.Add(aboveBlock);
            Score++;
            Combo++;

            //�I������
            AudioManager.Instance.PlayComboSound(Combo);
            //�I���S��
            HitManager.instance.ShowNormalHitEffect(aboveBlock.transform.position - Vector3.up*0.5f, aboveBlock.transform.localScale);
            if (Combo >= 3) //�I���j�S��
            {
                int effectCount = Mathf.Min(Combo-2, 3);
                StartCoroutine( HitManager.instance.ShowPerfectHitEffectCombo(effectCount, aboveBlock.transform.position - Vector3.up * 0.5f, aboveBlock.transform.localScale) );                      
            }
            return;
        }

        //�p�G�O�I���ɦ����������p 
        {
            //�p��@�����G�� A�BB ��������ߦ�m�P�j�p
            Vector3 A_Center = (belowBlockCenter + aboveBlockCenter) / 2f + Vector3.up * 0.5f; //A������ߦ�m
            Vector3 A_Scale = new Vector3(belowBlockScale.x, 1f, belowBlockScale.z - Mathf.Abs(offset)); //A����j�p
            Vector3 B_Center = Vector3.zero; //B������ߦ�m
            if (offset > 0)
                B_Center = belowBlockCenter + ((belowBlockScale.z + offset) / 2f) * Vector3.forward + Vector3.up;
            if (offset < 0)
                B_Center = belowBlockCenter + ((-belowBlockScale.z + offset) / 2f) * Vector3.forward + Vector3.up;
            Vector3 B_Scale = new Vector3(belowBlockScale.x, 1f, Mathf.Abs(offset)); //B����j�p

            //�N aboveBlock �� A ���
            aboveBlock.transform.position = A_Center;
            aboveBlock.transform.localScale = A_Scale;
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;

            //�s�W�@�� Block ���� B ���
            GameObject B_Block = Instantiate(BlockPerfab, B_Center, Quaternion.identity, this.transform.Find("tempBlocks"));
            B_Block.transform.localScale = B_Scale;
            B_Block.AddComponent<Rigidbody>();

            //�ܼƧ�s
            CurrentCenter = aboveBlock.transform.position + Vector3.up;
            CurrentScale = aboveBlock.transform.localScale;
            Score++;
            Combo = 0;

            Blocks.Add(aboveBlock);
            RecoverBlocks.Push(aboveBlock);
        }

    }

    void CutBlockRight(GameObject aboveBlock, GameObject belowBlock) //��������p��(Right��V)
    {
        Vector3 belowBlockCenter = belowBlock.transform.position; //�U���������ߦ�m
        Vector3 belowBlockScale = belowBlock.transform.localScale; //�U�������j�p
        Vector3 aboveBlockCenter = aboveBlock.transform.position; //�W���������ߦ�m
        Vector3 aboveBlockScale = aboveBlock.transform.localScale; //�W�������j�p
        float offset = (aboveBlockCenter - belowBlockCenter).x; //�����������q

        //�p�G�����q�j��U�������e�׫h�C������
        if (Mathf.Abs(offset) > belowBlockScale.x) 
        {
            aboveBlock.AddComponent<Rigidbody>(); //������[�W����ե�Ϩ�Y��
            aboveBlock.GetComponent<BlockBehavior>().Moving = false; //�N aboveBlock ���}�� BlockBehavior ���� Moving �ݩʳ]�� false , �o�˴N���|�A����� Update() �̪���� , �N���|�A���ܦ�m�F
            isFail = true;
            return;
        }

        //�p�G�X�G�S������
        if (Mathf.Abs(offset) < 0.5f) 
        {
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;
            aboveBlock.transform.position = CurrentCenter;
            CurrentCenter += Vector3.up;
            Blocks.Add(aboveBlock);
            Score++;
            Combo++;

            //�I������
            AudioManager.Instance.PlayComboSound(Combo);
            //�I���S��
            HitManager.instance.ShowNormalHitEffect(aboveBlock.transform.position - Vector3.up * 0.5f, aboveBlock.transform.localScale);
            if (Combo >= 3) //�I���j�S��
            {
                int effectCount = Mathf.Min(Combo - 2, 3);
                StartCoroutine(HitManager.instance.ShowPerfectHitEffectCombo(effectCount, aboveBlock.transform.position - Vector3.up * 0.5f, aboveBlock.transform.localScale));
            }
            return;
        }

        //�p�G�O�I���ɦ����������p 
        {
            //�p��@�����G�� A�BB ��������ߦ�m�P�j�p
            Vector3 A_Center = (belowBlockCenter + aboveBlockCenter) / 2f + Vector3.up * 0.5f; //A������ߦ�m
            Vector3 A_Scale = new Vector3(belowBlockScale.x - Mathf.Abs(offset), 1f, belowBlockScale.z); //A����j�p
            Vector3 B_Center = Vector3.zero; //B������ߦ�m
            if (offset > 0)
                B_Center = belowBlockCenter + ((belowBlockScale.x + offset) / 2f) * Vector3.right + Vector3.up;
            if (offset < 0)
                B_Center = belowBlockCenter + ((-belowBlockScale.x + offset) / 2f) * Vector3.right + Vector3.up;
            Vector3 B_Scale = new Vector3(Mathf.Abs(offset), 1f, belowBlockScale.z); //B����j�p

            //�N aboveBlock �� A ���
            aboveBlock.transform.position = A_Center;
            aboveBlock.transform.localScale = A_Scale;
            aboveBlock.GetComponent<BlockBehavior>().Moving = false;

            //�s�W�@�� Block ���� B ���
            GameObject B_Block = Instantiate(BlockPerfab, B_Center, Quaternion.identity, this.transform.Find("tempBlocks"));
            B_Block.transform.localScale = B_Scale;
            B_Block.AddComponent<Rigidbody>();

            //�ܼƧ�s
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
        if (RecoverBlocks.Count > 1) //�Ĥ@�Ӥ@�w�O FirstBlock
            RecoverBlocks.Pop(); //���������[�J���_��Ϊ��A�u���n�_�쪺�Ϊ��A�e�@��!

        CurrentScale = RecoverBlocks.Peek().transform.localScale;
        CurrentCenter = new Vector3(RecoverBlocks.Peek().transform.position.x, CurrentCenter.y, RecoverBlocks.Peek().transform.position.z);
        StartCoroutine(  MovingBlock.GetComponent<BlockBehavior>().Recover(CurrentScale, (CurrentCenter - Vector3.up))  );

        //���M����� MovingBlock �ܧΤF�A�������A��s�}�C�̪���� Blocks[Score] �@���A�]�� List �̬�����������e���O�P�@�Ӫ���A�ä��O�ƥ��A�ҥH���� List �̪�����e�������e������]���|�Q����!!!
        //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@�n�������O�C�A�� Blocks.Clear �M�ŤF���e���W�٬O�ݯd�ۤ��e�����
    }

    public void Restart()
    {
        //��Ҧ��b tempBlocks �U�������������X�ӾP�� //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@�o�g�k�A��s
        foreach (var gameObj in GetComponentsInChildren<BlockBehavior>()) //����Ҧ��֦����w�ե�]BlockBehavior�^���l���A��^���Ǥl���զ����}�C�C�p�G�l������ SetActive �O false �h������� (���ĤG�ӰѼ� GetComponentsInChildren<�ե�>(true) �����]���������)
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

    IEnumerator GameLoop()  // <!>.1 �� IEnumerator �ŧi����ƥi�� Wait ���� yield return �����Y�Ƿ|�y�� delay ����Ƥ��n�@���d��
    {

        while (true)
        {

            Init();
            yield return new WaitUntil(() => !StartButton.activeInHierarchy); //�p�G�}�l���s���U��ۤv���� SetActive ��~���
            AudioManager.Instance.PlayStartSound(); //����C���}�l����
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
                    //Combo=0; //�����a�s�� Combo ���ܤ]��@���_�����n�F
                }

                yield return null; //�o��O���F�� Input.GetMouseButtonDown(0) ��s�ӥ[���A�����X�h�j������ȭ��m�A���M�ߨ譫�Y loop ���ܷ|�����q�L yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            }

            //Print Fail UI
            RestartButton.SetActive(true); //�����s�}�l���s��ܥX��
            yield return new WaitUntil(() => !RestartButton.activeInHierarchy); //���ݭ��s�}�l���s�Q���U
            Restart();

        }
    }


    // Update is called once per frame
    void Update()
    {
        scoreText.text = Score.ToString(); //�@���� scoreText ��ܷ�e������

        //#if UNITY_EDITOR �̪����e�b Unity ��X���C���ɤ��|�Q�sĶ�A�i��ߦa�� debug code �g�b�̭�
        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.A)) //�C���}�l����U A ��i�H����Y��ơA�o�� debug �ܤ�K
        {
            //�Y���
        }
        #endif
    }
}
