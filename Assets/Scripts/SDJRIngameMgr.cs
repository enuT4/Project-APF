using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class SDJRIngameMgr : MonoBehaviour
{
    public static SDJRIngameMgr Inst = null;

    [Header("-------- GameUI --------")]
    public Text m_ScoreText = null;
    public Image m_GuageBarBack = null;
    public Image m_GuageBar = null;
    public Image m_TimeBar = null;
    Vector3 m_TimeBarPos;
    public Text m_TimerText = null;

    float m_GameTime = 60.0f;
    float m_MaxTime = 80.0f;
    public int m_CurScore = 0;

    float m_MaxGuage = 100.0f;
    float m_CurGuage = 0.0f;
    float guageAmount = 0.0f;
    public int m_BombCount = 0;
    public Image m_PlusSecImg = null;
    float m_PlusShowTimer = 0.0f;

    public Button m_SpawnLineBtn = null;
    public Image m_SpawnTutImg = null;

    public GameObject TxtPrefabParent;
    GameObject a_TimeTxtClone;
    TimerText a_TimerTx;
    public GameObject m_TimerObj = null;
    bool[] m_isTimeShow = new bool[5];

    //업그레이드
    int m_BonusScore = 0;
    float m_FeverRate = 1.0f;
    float m_SuperRate = 0.0f;

    //게임 준비
    [Header("-------- Game Ready --------")]
    public GameObject m_ReadyPanelObj = null;
    public Image m_ReadyImg = null;
    public Image m_GoImg = null;
    float m_ReadyTimer = 1.0f;
    float m_GoTimer = 0.0f;
    bool m_IsGameStart = false;
    Color m_ReadyImgColor;

    //일시정지
    [Header("-------- PausePanel --------")]
    public Button m_PauseBtn = null;
    public GameObject m_PausePanelObj = null;
    public Button m_PPContinueBtn = null;
    public Button m_PPRestartBtn = null;
    public Button m_PPTutorialBtn = null;
    public Button m_PPGotoLobbyBtn = null;
    public GameObject m_MessageBoxObj = null;
    public string m_PPStr = "";
    public GameObject Msg_Canvas;
    public GameObject m_TutPanelObj = null;
    public Button m_TutCloseBtn = null;

    //인게임
    public int m_Level = 1;
    public GameObject m_TilePrefab = null;
    public GameObject a_TileGroup;
    float a_TGVelocity = 500.0f;
    bool m_isTGMove = false;
    Vector2 m_TGCurPos = Vector2.zero;
    bool m_is1stLineSpawn = true;
    float m_LocalPosy = 0.0f;
    public bool m_OnlySel = false;
    string m_SelString = "";
    GameObject m_SelectedTile;
    int[] m_MovedTileNum = new int[2];
    string m_KeyStr = "";
    int m_TileCount = 0;
    int m_BombGuageCount = 0;

    //보너스 텍스트
    public GameObject m_BonusTxtObj;
    GameObject m_BonusTxtClone;
    BonusText m_BonusTx;

    //라인 점수 텍스트
    public GameObject m_LineSpawnTextObj;
    GameObject m_LSTxtClone;
    LineSpawnText m_LSTx;
    Vector3 m_LSTxtPos;

    //슈퍼 피버
    [Header("-------- Super Fever --------")]
    int m_FireCombo = 13;
    float m_SFTimer = 0.0f;
    bool isSFOn = false;
    bool isSFLineSpawn = false;
    int m_TempLv = 0;
    public GameObject m_SuperFeverImgObj;
    public GameObject m_FyahImgObj;
    GameObject a_SFImgClone;
    GameObject a_FyahClone;

    //콤보 텍스트
    [Header("-------- Combo Text --------")]
    public GameObject m_ComboObj = null;
    public Transform m_Canvas = null;
    GameObject a_ComboClone;
    ComboText a_ComboTx;
    //public Transform m_HUD_Canvas = null;
    public int m_Combo = 0;
    int m_JudgeCombo = 0;


    //타일 리스트
    List<GameObject>[] TileListArr = new List<GameObject>[3];

    //타일 제거 리스트
    List<int>[] DelTileListArr = new List<int>[3];

    //타일 제거 효과
    public GameObject ExpEffect;
    public GameObject EffectGroup;

    //페널티
    [HideInInspector] public bool m_isReverse = false;
    [HideInInspector] public bool m_isTileHide = false;
    public Image m_BadTile1ShowImg;

    //타이머 변수들
    [Header("-------- Timer --------")]
    public float m_BonusTTimer = 0.0f;
    public float m_BonusSpawnTime = 0.0f;      //보너스 레벨에 따른 스폰 주기 상수
    public float m_EraserTTimer = 0.0f;
    public float m_HammerTTimer = 0.0f;
    public float m_HammerDurTime = 0.0f;
    public float m_LineTTimer = 0.0f;
    public float m_Bad1TTimer = 0.0f;
    public float m_Bad2TTimer = 0.0f;
    public static float m_Bad1EffTimer = 0.0f;
    public static float m_Bad2EffTimer = 0.0f;
    float m_LineSpawnTick = 0.0f;
    bool isHammerOn = false;
    bool[] m_isSpawn = new bool[7];
    public float m_AutoLineSpawnTimer = 0.0f;
    bool m_isSpaceKey = false;
    
    [HideInInspector] public bool m_Switchbool = false;

    //게임 오버
    [Header("-------- GameOver --------")]
    public GameObject m_GOPanelObj = null;
    public Image m_GOPanelImg;
    public Image m_TUPanelImg;
    bool m_isGameOver = false;
    bool m_isTimeUp = false;
    float m_GOImgShowTimer = 3.0f;

    //아이템 변수
    bool isSFItemOn = false;
    public GameObject m_HammerPrefabObj;
    GameObject durHammerObj;

    void Awake()
	{
        Inst = this;
	}

	// Start is called before the first frame update
	void Start()
	{
        CheckItem();
        CheckUGRate();
        Application.targetFrameRate = 60;
        //스폰 초기화
        for (int ii = 0; ii < m_isSpawn.Length; ii++)
		{
            m_isSpawn[ii] = false;
		}

        InitTimer();
        InitTileList();
        InitLineSpawn();

        if (m_GOPanelObj != null)
            m_GOPanelObj.SetActive(false);

        if (m_PlusSecImg != null)
            m_PlusSecImg.gameObject.SetActive(false);

        if (m_ReadyPanelObj != null && m_IsGameStart == false)
            m_ReadyPanelObj.SetActive(true);

        //라인 스폰
        if (m_SpawnLineBtn != null)
            m_SpawnLineBtn.onClick.AddListener(SpawnLineFunc);

        //일시정지
        if (m_PauseBtn != null)
            m_PauseBtn.onClick.AddListener(()=> 
            {
                PauseBtnFunc(true);
            });

        if (m_PPContinueBtn != null)
            m_PPContinueBtn.onClick.AddListener(()=> 
            {
                PauseBtnFunc(false);
            });

        if (m_PPRestartBtn != null)
            m_PPRestartBtn.onClick.AddListener(PPRestartBtnFunc);

        if (m_PPTutorialBtn != null)
            m_PPTutorialBtn.onClick.AddListener(() =>
            {
                m_TutPanelObj.SetActive(true);
            });

        if (m_PPGotoLobbyBtn != null)
            m_PPGotoLobbyBtn.onClick.AddListener(PPGotoLobbyBtnFunc);

        if (m_TutCloseBtn != null)
            m_TutCloseBtn.onClick.AddListener(() =>
            {
                m_TutPanelObj.SetActive(false);
            });

        m_LSTxtPos = new Vector3(0.0f, -310.0f, 0.0f);
        guageAmount = 3.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsGameStart)
		{
            if (0.0f < m_ReadyTimer)
            {
                m_ReadyTimer -= Time.deltaTime;
                m_ReadyImgColor = m_ReadyImg.color;
                m_ReadyImgColor.a = Mathf.Lerp(1.0f - m_ReadyTimer, 2.0f, 0.2f);
                if (1.0f <= m_ReadyImgColor.a)
                    m_ReadyImgColor.a = 1.0f;
                m_ReadyImg.color = m_ReadyImgColor;

                if (m_ReadyTimer < 0.0f)
                {
                    m_ReadyImg.gameObject.SetActive(false);
                    m_GoImg.gameObject.SetActive(true);
                    m_GoTimer = 1.0f;
                    m_ReadyTimer = 0.0f;
                }
            }

            if (0.0f < m_GoTimer)
            {
                if (m_ReadyImg.gameObject.activeSelf == true ||
                    m_GoImg.gameObject.activeSelf == false)
                {
                    return;
                }

                m_GoTimer -= Time.deltaTime;
                if (m_GoTimer <= 0.0f)
                {
                    m_ReadyPanelObj.SetActive(false);
                    m_IsGameStart = true;
                    if (isSFItemOn == true)
					{
                        m_Combo = 100;
                        m_FireCombo = 113;
					}
                }
            }
        }
        else
		{
            m_GameTime -= Time.deltaTime;
            m_TimerText.text = ((int)m_GameTime).ToString();
            if (Time.timeScale != 0.0f && (0.0f < m_GameTime && m_GameTime <= 60.0f)) 
                m_TimeBar.transform.Translate(-0.055f * 0.015f, 0, 0);

            CheckTimeState(m_GameTime);

            if (0.0f < m_PlusShowTimer)
            {
                m_PlusSecImg.gameObject.SetActive(true);
                m_PlusShowTimer -= Time.deltaTime;
                if (m_PlusShowTimer <= 0.0f)
                {
                    m_PlusShowTimer = 0.0f;
                    m_PlusSecImg.gameObject.SetActive(false);
                }
            }

            if (0.0f < m_LineSpawnTick && m_isGameOver == false && 0.0f < m_GameTime)     //라인이 저절로 내려오는 주기 계산
            {   //게임 시작 후 부터 자동으로 내려오도록
                m_LineSpawnTick -= Time.deltaTime;
                if (m_LineSpawnTick <= 0.0f)
                {
                    m_LineSpawnTick = 0.0f;
                }
            }
        }

        TileTimerFunc();
        TGTranslate(m_TGCurPos.y);
        DurTimerFunc();
        BadTileEffUpdate();
        //if (isSFLineSpawn)
        //    RefreshTile();
        ComboUpdate();

        if (Input.GetKeyDown(KeyCode.DownArrow) ||
			Input.GetKeyDown(KeyCode.S))
		{
            m_KeyStr = "Down";
            if (isHammerOn)
                HammerSelFunc(m_KeyStr);
            else
                TileSelOnOff(m_KeyStr);
		}

        if (Input.GetKeyDown(KeyCode.Space))
		{
            m_isSpaceKey = true;
            //LV 1 ~ 6 : 3.0초 ~ 2.0초
            m_AutoLineSpawnTimer = 3.0f - (m_Level - 1) * 0.2f;        
            SpawnLineFunc();
		}

        GameOverFunc();
        RefreshTile();
        
    }

    void CheckItem()
    {
        if (ReadySceneMgr.m_isChecked[0])
        {
        }
        if (ReadySceneMgr.m_isChecked[1])
        {
        }
        if (ReadySceneMgr.m_isChecked[2])
        {
        }
        if (ReadySceneMgr.m_isChecked[3])
            isSFItemOn = true;
    }

    void CheckUGRate()
    {
        GlobalValue.SDJRUGAm();
        m_BonusScore = GlobalValue.BonusAmount[GlobalValue.g_SDJRUGLv[0]];
        m_FeverRate = GlobalValue.FeverAmount[GlobalValue.g_SDJRUGLv[1]];
        m_SuperRate = GlobalValue.SuperAmount[GlobalValue.g_SDJRUGLv[2]];

        //보너스 레벨에 따른 스폰 주기 계산
        m_BonusSpawnTime = 20 - ((5 / 15) * GlobalValue.g_SDJRUGLv[0]);
    }

    void InitTimer()
	{
        //스폰 타이머들 초기화
        m_BonusTTimer = m_BonusSpawnTime;
        m_EraserTTimer = 39.0f;
        m_HammerTTimer = 47.0f;
        m_LineTTimer = 27.0f;
        m_Bad1TTimer = Random.Range(25.0f, 30.0f);
        m_Bad2TTimer = Random.Range(20.0f, 25.0f);
        m_AutoLineSpawnTimer = 3.0f;
        m_Level = 1;
        m_CurScore = 0;
        m_Combo = 0;
        m_BombCount = 0;
        m_TileCount = 0;
        m_ScoreText.text = m_CurScore.ToString("N0");
        m_TimeBarPos = m_TimeBar.transform.position;
        m_SFTimer = 0.0f;
    }

    void TGTranslate(float a_CurPosy)
	{
        if (!m_isTGMove) return;

        m_LocalPosy -= Time.deltaTime * a_TGVelocity;
        if (m_LocalPosy <= -45.0f * 5)
		{
            m_LocalPosy = -45.0f * 5;
            m_isTGMove = false;

            //게임오버 판정
            for (int ii = 0; ii < 3; ii++)
            {
                if (TileListArr[ii].Count >= 12)
                    m_isGameOver = true;
            }
        }
        m_TGCurPos = new Vector2(0.0f, 195.0f + m_LocalPosy);
        a_TileGroup.transform.localPosition = m_TGCurPos;
	}

    void InitTileList()
	{
        for (int kk = 0; kk < 3; kk++)
        {
            if (TileListArr[kk] != null)
                TileListArr[kk].Clear();
            TileListArr[kk] = new List<GameObject>();

            if (DelTileListArr[kk] != null)
                DelTileListArr[kk].Clear();
            DelTileListArr[kk] = new List<int>();
        }
    }

    void InitLineSpawn()
	{
        for (int ii = 0; ii < 5; ii++)
		{
            for (int jj = 0; jj < 3; jj++)
			{
                MakeTileNode(TileListArr[jj]);
            }
		}
        m_LocalPosy = -45.0f * 5;
        m_TGCurPos = new Vector2(0.0f, 195.0f + m_LocalPosy);

        for (int ii = 0; ii < 3; ii++)
            UpdateTileList(TileListArr[ii], ii);
    }

    void UpdateTileList(List<GameObject> a_TList, int a_Lineidx)
	{
        a_Lineidx--;
        for (int ii = 0; ii < a_TList.Count; ii++)  
		{
            a_TList[ii].transform.SetParent(a_TileGroup.transform, false);
            a_TList[ii].transform.localPosition = 
                new Vector2(a_Lineidx * 160.0f, 180.0f - 45.0f * ii);
        }
	}

    void SpawnLineFunc()        //라인 스폰 함수
	{
        if (m_PausePanelObj.activeSelf) return;
        if (0.0f < m_LineSpawnTick) return;
        if (m_isGameOver || m_isTimeUp)  return;

        if (m_isSpaceKey)
		{
            if (m_is1stLineSpawn)
            {
                m_SpawnTutImg.gameObject.SetActive(false);
                m_is1stLineSpawn = false;
            }
            m_CurScore += 200;

            //점수 연출
            if (m_LineSpawnTextObj == null) return;

            m_LSTxtClone = Instantiate(m_LineSpawnTextObj);
            m_LSTxtClone.transform.SetParent(TxtPrefabParent.transform);
            m_LSTx = m_LSTxtClone.GetComponent<LineSpawnText>();
            if (m_LSTx != null)
                m_LSTx.LineSpawnTxt();

            m_LSTxtClone.transform.localPosition = m_LSTxtPos;
            m_LSTxtClone.transform.localScale = Vector3.one * 1.5f;

            m_JudgeCombo = m_Level + 2;
            m_isSpaceKey = false;
        }

        int a_RandIdx = Random.Range(0, 3);
        switch(a_RandIdx)
        {//스페셜 타일의 등장 위치를 랜덤하게 나타내주기 위해
            case 0:
                MakeTileNode(TileListArr[0]);
                MakeTileNode(TileListArr[1]);
                MakeTileNode(TileListArr[2]);
                break;
            case 1:
                MakeTileNode(TileListArr[1]);
                MakeTileNode(TileListArr[2]);
                MakeTileNode(TileListArr[0]);
                break;
            case 2:
                MakeTileNode(TileListArr[2]);
                MakeTileNode(TileListArr[0]);
                MakeTileNode(TileListArr[1]);
                break;
        }
        for (int ii = 0; ii < 3; ii++)
            UpdateTileList(TileListArr[ii], ii);

        m_LocalPosy = -45.0f * 4;
        m_TGCurPos = new Vector2(0.0f, 195.0f + m_LocalPosy);
        m_isTGMove = true;

        m_LineSpawnTick = 0.15f;
        m_ScoreText.text = m_CurScore.ToString("N0");
        
    }

    void MakeTileNode(List<GameObject> a_TileList)
	{
        GameObject obj = Instantiate(m_TilePrefab);
        TileNode a_TNode = obj.GetComponent<TileNode>();
        a_TNode.m_TType = ChooseTileType();
        a_TNode.SetTile(a_TNode.m_TType, m_Level);

        //터질 타일로 스폰하지 않게
        if (a_TileList != null && a_TileList.Count >= 2)
		{
            while (true)
			{
                TileNode a_TNode1 = a_TileList[0].GetComponent<TileNode>();
                TileNode a_TNode2 = a_TileList[1].GetComponent<TileNode>();
                if (a_TNode1.m_TileIdx == a_TNode2.m_TileIdx &&
					a_TNode.m_TileIdx == a_TNode1.m_TileIdx)
                    a_TNode.SetTile(a_TNode.m_TType, m_Level);
                else
					break;
            }
		}
        a_TNode.SetTileIdx(a_TNode.m_TileIdx);
        a_TileList.Insert(0, obj);
    }

    TileType ChooseTileType()
	{
        int idx = -1;
        for (int ii = 0; ii < m_isSpawn.Length; ii++)
		{
            if (m_isSpawn[ii])  //스폰될 준비가 되어있다
            {
                idx = ii;
                m_isSpawn[ii] = false;
                break;
            }
            else
                continue;
		}
        if (idx == 0) return TileType.Bonus;            //보너스
        else if (idx == 1) return TileType.Special1;    //지우개
        else if (idx == 2) return TileType.Special2;    //해머
        else if (idx == 3) return TileType.Special3;    //한줄
        else if (idx == 4) return TileType.Bad1;        //방해타일1
        else if (idx == 5) return TileType.Bad2;        //방해타일2
        else if (idx == 6) return TileType.Bomb;        //폭탄
        else return TileType.Normal;                    //일반타일
    }

	#region TimerFunc

	void TileTimerFunc()
	{
        SpawnLineTimerFunc();
        BonusTileTimerFunc();
        EraserTileTimerFunc();
        HammerTileTimerFunc();
        LineTileTimerFunc();
        BadTile1TimerFunc();
        BadTile2TimerFunc();

    }

    void SpawnLineTimerFunc()
	{
        if (m_isGameOver || m_GameTime <= 0.0f)
            return;

        if (0.0f < m_AutoLineSpawnTimer)
		{
            m_AutoLineSpawnTimer -= Time.deltaTime;
            if (m_AutoLineSpawnTimer <= 0.0f)
			{
                SpawnLineFunc();
                //LV 1 ~ 6 : 4초 ~ 2.5초
                m_AutoLineSpawnTimer = 4.0f - (m_Level - 1) * 0.3f; 
			}
		}
	}

    void BonusTileTimerFunc()
	{
        //보너스 타일 스폰 타이머
        if (0.0f < m_BonusTTimer)
        {
            if (GlobalValue.g_SDJRUGLv[0] == 0) return;
            if (m_isSpawn[0]) return;

            m_BonusTTimer -= Time.deltaTime;
            if (m_BonusTTimer <= 0.0f)
            {
                m_isSpawn[0] = true;
                m_BonusTTimer = m_BonusSpawnTime;
            }
        }
    }

    void EraserTileTimerFunc()
	{
        //지우개 아이템 스폰 타이머
        if (0.0f < m_EraserTTimer)
        {
            if (!ReadySceneMgr.m_isChecked[0]) return;
            if (m_isSpawn[1]) return;

            m_EraserTTimer -= Time.deltaTime;
            if (m_EraserTTimer <= 0.0f)
            {
                m_isSpawn[1] = true;
                m_EraserTTimer = 37.0f;
            }
        }
    }

    void HammerTileTimerFunc()
	{
        //해머 아이템 스폰 타이머
        if (0.0f < m_HammerTTimer)
        {
            if (!ReadySceneMgr.m_isChecked[1]) return;
            if (m_isSpawn[2]) return;   //여러번 나오는 거 방지
            if (isHammerOn) return;     //해머가 발동중일 때에는 타이머가 안돌아가게

            m_HammerTTimer -= Time.deltaTime;
            if (m_HammerTTimer <= 0.0f)
            {
                m_HammerTTimer = 29.0f;
                m_isSpawn[2] = true;
            }
        }
    }

    void LineTileTimerFunc()
	{
        //한줄뿅 아이템 스폰 타이머
        if (0.0f < m_LineTTimer)
        {
            if (!ReadySceneMgr.m_isChecked[2]) return;
            if (m_isSpawn[3]) return;

            m_LineTTimer -= Time.deltaTime;
            if (m_LineTTimer <= 0.0f)
            {
                m_isSpawn[3] = true;
                m_LineTTimer = 27.0f;
            }
        }
    }

    void BadTile1TimerFunc()
	{
        //방해 타일 1 스폰 타이머
        if (0.0f < m_Bad1TTimer)
        {
            if (m_Level < 4) return;   //레벨 4부터 등장
            if (m_isSpawn[4]) return;

            m_Bad1TTimer -= Time.deltaTime;
            if (m_Bad1TTimer <= 0.0f)
            {
                m_isSpawn[4] = true;
                m_Bad1TTimer = Random.Range(25.0f, 30.0f);
            }
        }
    }

    void BadTile2TimerFunc()
	{
        //방해 타일 2 스폰 타이머
        if (0.0f < m_Bad2TTimer)
        {
            if (m_Level < 6) return;   //레벨 6부터 등장
            if (m_isSpawn[5]) return;

            m_Bad2TTimer -= Time.deltaTime;
            if (m_Bad2TTimer <= 0.0f)
            {
                m_isSpawn[5] = true;
                m_Bad2TTimer = Random.Range(20.0f, 25.0f);
            }
        }
    }

    void DurTimerFunc()
	{
        HammerTimerFunc();
        BadTileEffTimerFunc();
        SFTimerFunc();
    }

    void SFTimerFunc()
	{
        if (0.0f < m_SFTimer)
		{
            m_SFTimer -= Time.deltaTime;
            if (m_SFTimer <= 0.0f)
			{
                m_SFTimer = 0.0f;
                m_Level = m_TempLv;
                if (!isHammerOn)
                    m_FireCombo = m_Combo + 41;
			}
		}
	}

    void HammerTimerFunc()
    {
        //해머 아이템 지속시간 타이머
        if (0.0f < m_HammerDurTime)
        {
            if (!isHammerOn) return;

            m_HammerDurTime -= Time.deltaTime;
            if (m_HammerDurTime <= 0.0f)
            {
                isHammerOn = false;
                Destroy(durHammerObj);
                if (m_SFTimer <= 0.0f)
				{
                    if (m_Combo < 13)
                        m_FireCombo = 13;
                    else
                        m_FireCombo = m_Combo + 20;
                }
                m_isSpawn[2] = false;
                m_HammerDurTime = 2.5f;
            }
        }
    }

    void BadTileEffTimerFunc()
	{
        if (0.0f < m_Bad1EffTimer)
        {
            m_Bad1EffTimer -= Time.deltaTime;
            if (m_Bad1EffTimer <= 0.0f)
            {
                m_Bad1EffTimer = 0.0f;
                m_isReverse = false;
            }
        }

        if (0.0f < m_Bad2EffTimer)
        {
            m_Bad2EffTimer -= Time.deltaTime;
            if (m_Bad2EffTimer <= 0.0f)
            {
                m_Bad2EffTimer = 0.0f;
                m_isTileHide = false;
                m_Switchbool = true;
            }
        }
    }

	#endregion //TimerFunc

    void BadTileEffUpdate()
	{
        //방해 타일 1 지속시간 중 함수
        if (m_isReverse)    //좌우 반전
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.A))
            {
                m_KeyStr = "Right";
                if (isHammerOn)
                    HammerSelFunc(m_KeyStr);
                else
                    TileSelOnOff(m_KeyStr);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.D))
            {
                m_KeyStr = "Left";
                if (isHammerOn)
                    HammerSelFunc(m_KeyStr);
                else
                    TileSelOnOff(m_KeyStr);
            }
            m_BadTile1ShowImg.gameObject.SetActive(true);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.A))
            {
                m_KeyStr = "Left";
                if (isHammerOn)
                    HammerSelFunc(m_KeyStr);
                else
                    TileSelOnOff(m_KeyStr);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.D))
            {
                m_KeyStr = "Right";
                if (isHammerOn)
                    HammerSelFunc(m_KeyStr);
                else
                    TileSelOnOff(m_KeyStr);
            }
            m_BadTile1ShowImg.gameObject.SetActive(false);
        }

        //방해 타일 2 지속시간 중 함수
        if (m_Switchbool)
        {
            GameObject[] Tiles = GameObject.FindGameObjectsWithTag("Tile");
            foreach (GameObject tile in Tiles)
                tile.GetComponent<TileNode>().m_Bad2TileImg.gameObject.SetActive(m_isTileHide);
            if (!m_isTileHide)
                m_Switchbool = false;
        }
    }

    void SpecialTileUpdate(GameObject a_SelTile)
	{
        TileType a_TType = a_SelTile.GetComponent<TileNode>().m_TType;
        if (a_TType == TileType.Normal) return;
        if (!a_SelTile.GetComponent<TileNode>().m_isSPmoved) return;

        if (a_TType == TileType.Bomb)   //폭탄 타일
        {
            m_BombGuageCount = 0;
            for (int ii = m_MovedTileNum[0] - 1; ii < m_MovedTileNum[0] + 2; ii++)
            {
                if (ii < 0 || ii > 2) continue;  //왼쪽이나 오른쪽일 때 out of range 방지
                for (int jj = m_MovedTileNum[1] - 2; jj < m_MovedTileNum[1] + 3; jj++)
                {
                    if (jj < 0 || jj > TileListArr[ii].Count - 1) continue;
                    DelTileListArr[ii].Add(jj);
                    m_BombGuageCount++;
                }
            }
            m_Combo++;
            m_JudgeCombo = m_Level + 2;
        }
        else if (a_TType == TileType.Bonus) //보너스 타일
        {
            if (m_MovedTileNum[1] != 0 && a_SelTile != null)
            {
                int a_TileIdx = TileListArr[m_MovedTileNum[0]][m_MovedTileNum[1] - 1].GetComponent<TileNode>().m_TileIdx;
                for (int ii = 0; ii < 3; ii++)
                {
                    for (int jj = 0; jj < TileListArr[ii].Count; jj++)
                    {
                        if (TileListArr[ii][jj].GetComponent<TileNode>().m_TileIdx == a_TileIdx)
                            DelTileListArr[ii].Add(jj);
                    }
                }
            }

            DelTileListArr[m_MovedTileNum[0]].Add(m_MovedTileNum[1]);   //자신도 터지게
            m_Combo++;
            m_JudgeCombo = m_Level + 2;
        }
        else if (a_TType == TileType.Special1)  //지우개
        {
            m_BombGuageCount = 0;
            for (int ii = 0; ii < 3; ii++)
            {
                for (int jj = 0; jj < TileListArr[ii].Count; jj++)
                {
                    DelTileListArr[ii].Add(jj);
                    m_BombGuageCount++;
                }
            }
            m_BombGuageCount /= 2;
            m_Combo++;
            m_JudgeCombo = m_Level + 2;
        }
        else if (a_TType == TileType.Special2)  //해머
        {
            DelTileListArr[m_MovedTileNum[0]].Add(m_MovedTileNum[1]);   //자신도 터지게
            isHammerOn = true;
            durHammerObj = null;
            durHammerObj = Instantiate(m_HammerPrefabObj);
            durHammerObj.transform.SetParent(a_TileGroup.transform);
            Vector3 m_HammerPos = 
                TileListArr[m_MovedTileNum[0]][m_MovedTileNum[1]].transform.position;
            durHammerObj.transform.position = 
                new Vector3(m_HammerPos.x + 50.0f, m_HammerPos.y, m_HammerPos.z);
			durHammerObj.transform.localScale = Vector3.one;
            m_HammerDurTime = 2.5f;
			m_Combo++;
            m_JudgeCombo = m_Level + 2;
        }
        else if (a_TType == TileType.Special3)  //한줄
        {
            m_BombGuageCount = 0;
            for (int jj = 0; jj < TileListArr[m_MovedTileNum[0]].Count; jj++)
            {
                DelTileListArr[m_MovedTileNum[0]].Add(jj);
                m_BombGuageCount++;
            }
            m_BombGuageCount /= 2;
            m_Combo++;
            m_JudgeCombo = m_Level + 2;
        }

        for (int ii = 0; ii < 3; ii++)
            DelTileListArr[ii] = DelTileListArr[ii].Distinct().ToList();

        ComboText(m_Combo);
    }

    void NormalTileUpdate(GameObject a_SelTile, bool isComboPlus)
	{
        //제거 되어야 할 3라인의 타일들 선정
        for (int ii = 0; ii < 3; ii++)  //3 라인 반복 계산
        {
            if (TileListArr[ii].Count < 3) continue; //타일이 3개보다 적다면 터질 타일이 없으므로

            for (int jj = 0; jj < TileListArr[ii].Count - 2; jj++)   //리스트 한바퀴를 돌아서 제거해야할 타일 선정
            {
                if (TileListArr[ii][jj].GetComponent<TileNode>().m_TileIdx == TileListArr[ii][jj + 1].GetComponent<TileNode>().m_TileIdx &&
                    TileListArr[ii][jj + 1].GetComponent<TileNode>().m_TileIdx == TileListArr[ii][jj + 2].GetComponent<TileNode>().m_TileIdx)
                {
                    if (jj >= 1)
                    {
                        if (TileListArr[ii][jj - 1].GetComponent<TileNode>().m_TType == TileType.Bad1 ||
                            TileListArr[ii][jj - 1].GetComponent<TileNode>().m_TType == TileType.Bad2 ||
                            TileListArr[ii][jj - 1].GetComponent<TileNode>().m_TType == TileType.GameOver)
                            DelTileListArr[ii].Add(jj - 1); //터트린 타일 위에 방해 타일이 있다면
                    }
                    DelTileListArr[ii].Add(jj);
                    DelTileListArr[ii].Add(jj + 1);
                    DelTileListArr[ii].Add(jj + 2);
                }
            }
            DelTileListArr[ii] = DelTileListArr[ii].Distinct().ToList();    //중복으로 적용된 애들 제거
        }

        if (isComboPlus)
		{
            if (IsSelTileinDelList() && 
                a_SelTile.GetComponent<TileNode>().m_TType == TileType.Normal)
			{
                m_Combo++;
                m_JudgeCombo = m_Level + 2;
                ComboText(m_Combo);
            }
            else
                m_SelectedTile = null;
        }
    }

    bool IsSelTileinDelList()
	{
        for (int jj = 0; jj < DelTileListArr[m_MovedTileNum[0]].Count; jj++) 
		{
            if (DelTileListArr[m_MovedTileNum[0]][jj] == m_MovedTileNum[1])
                return true;
		}

        return false;
    }

    void RefreshTile()
	{
        if (m_OnlySel) return;
        if (m_isTGMove) return;
        if (isSFLineSpawn) return;

        for (int ii = 0; ii < 3; ii++)  //삭제할 리스트 초기화
            DelTileListArr[ii].Clear();

        //스페셜 타일들이 제대로 발동 되지 않았을 때
        if (m_SelectedTile != null)
        {
            SpecialTileUpdate(m_SelectedTile);
            NormalTileUpdate(m_SelectedTile, true);
        }
        else
            NormalTileUpdate(m_SelectedTile, false);

        //선정된 타일들 제거
        if (!(m_Combo == m_FireCombo && !isSFOn) || !isHammerOn)
            DestroyTile(m_SelectedTile, TileListArr, DelTileListArr);

        for (int ii = 0; ii < 3; ii++)
            UpdateTileList(TileListArr[ii], ii);

        //게임오버 판정
        if(!m_isTGMove)
		{
            for (int ii = 0; ii < 3; ii++)
            {
                if (TileListArr[ii].Count >= 12)
                    m_isGameOver = true;
            }
        }
    }

    void DestroyTile(GameObject a_SelTile, List<GameObject>[] a_TList, List<int>[] a_DelTList)
	{
        if (a_SelTile != null)
		{
            TileType a_SelType = a_SelTile.GetComponent<TileNode>().m_TType;

            for (int ii = 0; ii < 3; ii++)
            {
                for (int jj = a_DelTList[ii].Count - 1; jj >= 0; jj--)
                {
                    //본인이 터질 방해타일이면 예외처리
                    if (a_SelType != TileType.Normal ||
                        a_TList[ii][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bad1 ||
                        a_TList[ii][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bad2 ||
                        a_TList[ii][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.GameOver)
                        continue;
                    //터트린 타일 좌우에 방해타일이 있다면 방해타일도 터트림
                    if (ii == 0 || ii == 2)
                    {
                        if (a_TList[1].Count > a_DelTList[ii][jj])
                        {
                            if (a_TList[1][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bad1 ||
                            a_TList[1][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bad2 ||
                            a_TList[1][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.GameOver)
                            {
                                ExplosionEffFunc(a_TList[1][a_DelTList[ii][jj]]);
                                Destroy(a_TList[1][a_DelTList[ii][jj]]);
                                a_TList[1].RemoveAt(a_DelTList[ii][jj]);
                                m_TileCount++;
                                AddScore();
                            }
                        }
                    }
                    else if (ii == 1)
                    {
                        for (int kk = 0; kk < 3; kk = kk + 2)
                        {
                            if (a_TList[kk].Count > a_DelTList[ii][jj])
                            {
                                if (a_TList[kk][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bad1 ||
                                a_TList[kk][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bad2 ||
                                a_TList[kk][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.GameOver)
                                {
                                    ExplosionEffFunc(a_TList[kk][a_DelTList[ii][jj]]);
                                    Destroy(a_TList[kk][a_DelTList[ii][jj]]);
                                    a_TList[kk].RemoveAt(a_DelTList[ii][jj]);
                                    m_TileCount++;
                                    AddScore();
                                }
                            }
                        }
                    }
                    
                }
            }
        }

        for (int ii = 0; ii < 3; ii++)
        {
            if (a_DelTList[ii].Count == 0) continue;

            for (int jj = a_DelTList[ii].Count - 1; jj >= 0; jj--)
            {
                //터트릴 타일을 터트림
                ExplosionEffFunc(a_TList[ii][a_DelTList[ii][jj]]);
                if (a_TList[ii][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bomb)
				{
                    m_BombCount++;
                    m_GameTime += 10.0f;
                    if (m_GameTime >= m_MaxTime) m_GameTime = m_MaxTime;
                    m_PlusShowTimer = 1.0f;
                    float a_TimeBarX = m_TimeBar.transform.position.x + (400.0f / 6.0f) * 0.015f;
                    if (a_TimeBarX > m_TimeBarPos.x) a_TimeBarX = m_TimeBarPos.x;
                    m_TimeBar.transform.position = new Vector3(a_TimeBarX,
                            m_TimeBar.transform.position.y, m_TimeBar.transform.position.z);
                }
                else if(a_TList[ii][a_DelTList[ii][jj]].GetComponent<TileNode>().m_TType == TileType.Bonus)
				{
                    if (m_BonusTxtObj == null) return;

                    m_BonusTxtClone = Instantiate(m_BonusTxtObj);
                    m_BonusTxtClone.transform.position = a_TList[ii][a_DelTList[ii][jj]].transform.position;
                    m_BonusTxtClone.transform.localScale = Vector3.one * 0.02f;
                    m_BonusTx = m_BonusTxtClone.GetComponent<BonusText>();
                    m_CurScore += (int)m_BonusScore;
                    m_ScoreText.text = m_CurScore.ToString("N0");
                    if (m_BonusTx != null) m_BonusTx.InitScore(GlobalValue.g_SDJRUGLv[0]);
                }
                Destroy(a_TList[ii][a_DelTList[ii][jj]]);
                a_TList[ii].RemoveAt(a_DelTList[ii][jj]);
                m_TileCount++;
                AddScore();
            }
        }

        MinusGuage();
    }

    void MinusGuage()
	{
        if (m_BombGuageCount == 0) return;

        m_CurGuage -= m_BombGuageCount * guageAmount;
        m_GuageBar.fillAmount = m_CurGuage / m_MaxGuage;
        if (m_CurGuage <= 0.0f)
            m_CurGuage = 0.0f;
        m_BombGuageCount = 0;
	}

    void ExplosionEffFunc(GameObject expTile)
	{
        GameObject expObj = Instantiate(ExpEffect) as GameObject;
        expObj.transform.SetParent(EffectGroup.transform, false);
        expObj.transform.position = expTile.transform.position;
        ParticleSystem[] a_expPS = expObj.GetComponentsInChildren<ParticleSystem>();
        for (int ii = 0; ii < a_expPS.Length; ii++)
            a_expPS[ii].Play();
        Destroy(expObj, 1.0f);
	}

    void HammerSelFunc(string a_SelString)
	{
        if (!isHammerOn) return;
        if (m_OnlySel) m_OnlySel = false;
        for (int ii = 0; ii < 3; ii++)  //삭제할 리스트 초기화
            DelTileListArr[ii].Clear();
        if (a_SelString == "Left")
        {
            //선택한 곳에 타일이 하나도 없다면 못집게
            if (TileListArr[0].Count == 0) return;
            durHammerObj.transform.position = 
                TileListArr[0][TileListArr[0].Count - 1].transform.position;
            DelTileListArr[0].Add(TileListArr[0].Count - 1);
        }
        else if (a_SelString == "Down")
        {
            if (TileListArr[1].Count == 0) return;
            durHammerObj.transform.position = 
                TileListArr[1][TileListArr[1].Count - 1].transform.position;
            DelTileListArr[1].Add(TileListArr[1].Count - 1);
        }
        else if (a_SelString == "Right")
        {
            if (TileListArr[2].Count == 0) return;
            durHammerObj.transform.position = 
                TileListArr[2][TileListArr[2].Count - 1].transform.position;
            DelTileListArr[2].Add(TileListArr[2].Count - 1);
        }
        m_Combo++;
        ComboText(m_Combo);
        AddScore();
        DestroyTile(null, TileListArr, DelTileListArr);
    }

    void TileSelOnOff(string a_SelString)
	{
        if (m_isGameOver || m_isTimeUp) return;
        if (m_PausePanelObj.activeSelf) return;
        if (isHammerOn)return;

        if (!m_OnlySel) 
		{
            if (a_SelString == "Left")
            {
                if (TileListArr[0].Count == 0)  //선택한 곳에 타일이 하나도 없다면 못집게
                    return;

                TileNode a_TileND = TileListArr[0][TileListArr[0].Count - 1].GetComponent<TileNode>();
                TileType a_TType = a_TileND.m_TType;
                if (a_TType == TileType.Bad1 || a_TType == TileType.Bad2 || a_TType == TileType.GameOver)
                    return;
                a_TileND.m_isSelected = true;
                m_SelectedTile = TileListArr[0][TileListArr[0].Count - 1];
                m_OnlySel = true;
            }
            else if (a_SelString == "Down")
            {
                if (TileListArr[1].Count == 0)  //선택한 곳에 타일이 하나도 없다면 못집게
                    return;

                TileNode a_TileND = TileListArr[1][TileListArr[1].Count - 1].GetComponent<TileNode>();
                TileType a_TType = a_TileND.m_TType;
                if (a_TType == TileType.Bad1 || a_TType == TileType.Bad2 || a_TType == TileType.GameOver)
                    return;
                a_TileND.m_isSelected = true;
                m_SelectedTile = TileListArr[1][TileListArr[1].Count - 1];
                m_OnlySel = true;
            }
            else if (a_SelString == "Right")
            {
                if (TileListArr[2].Count == 0)  //선택한 곳에 타일이 하나도 없다면 못집게
                    return;

                TileNode a_TileND = TileListArr[2][TileListArr[2].Count - 1].GetComponent<TileNode>();
                TileType a_TType = a_TileND.m_TType;
                if (a_TType == TileType.Bad1 || a_TType == TileType.Bad2 || a_TType == TileType.GameOver)
                    return;
                a_TileND.m_isSelected = true;
                m_SelectedTile = TileListArr[2][TileListArr[2].Count - 1];
                m_OnlySel = true;
            }
        }
        else    //if (m_OnlySel == true)
        {
            if (m_SelString != a_SelString) 
            {
                if (a_SelString == "Left")
                    MoveTile(m_SelectedTile, a_SelString);
                else if (a_SelString == "Down")
                    MoveTile(m_SelectedTile, a_SelString);
                else if (a_SelString == "Right")
                    MoveTile(m_SelectedTile, a_SelString);
                m_JudgeCombo--;
                if (m_JudgeCombo < 0)
                {
                    m_Combo = 0;
                    m_JudgeCombo = m_Level + 2;
                    m_FireCombo = 13;
                    isSFOn = false;
                }
            }
            else
                m_SelectedTile.GetComponent<TileNode>().m_isSPmoved = false;
            m_OnlySel = false;
            m_SelectedTile.GetComponent<TileNode>().m_isSelected = false;
        }
        m_SelString = a_SelString;
	}

    void AddScore(bool a_IsBonus = false)
	{
        float a_Value = 0;
        if (0.0f < m_SFTimer)
            a_Value = (372.2f * (100.0f + m_TempLv * 5) * 0.01f *
                (100.0f + m_Combo * 3) * 0.01f * m_FeverRate);
        else
            a_Value = (372.2f * (100.0f + m_Level) * 0.01f *
                (100.0f + m_Combo * 3) * 0.01f * m_FeverRate);

        if (a_IsBonus)
            a_Value += m_BonusScore;
        m_CurScore += (int)a_Value;
        m_ScoreText.text = m_CurScore.ToString("N0");

        if (isHammerOn) guageAmount = 2.0f;
        else guageAmount = 3.2f;

        m_CurGuage += guageAmount;
        if (m_MaxGuage <= m_CurGuage)
        {
            m_CurGuage = 0.0f;

            //폭탄 타일 생성
            m_isSpawn[6] = true;
        }
        m_GuageBar.fillAmount = m_CurGuage / m_MaxGuage;
    }

    void GameOverFunc()
	{
        if (!m_isGameOver) return;

        //타일을 모두 게임오버 타일로 바꾸기
        GameObject[] Tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in Tiles)
        {
            TileNode a_TileND = tile.GetComponent<TileNode>();
            a_TileND.m_TileImg.sprite = a_TileND.m_TileSprite[14];
            if (a_TileND.m_BonusText.gameObject.activeSelf)
                a_TileND.m_BonusText.gameObject.SetActive(false);
        }

        m_GOPanelObj.SetActive(true);
        m_TUPanelImg.gameObject.SetActive(false);
        m_GOPanelImg.gameObject.SetActive(true);

        Time.timeScale = 0.0f;

        m_GOImgShowTimer -= Time.unscaledDeltaTime;
        if (m_GOImgShowTimer <= 0.0f)
        {
            m_GOImgShowTimer = 3.0f;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("GameOverScene");
        }
    }

    void MoveTile(GameObject a_Selectedtile, string a_SelString)
	{
        m_MovedTileNum[0] = -1;
        m_MovedTileNum[1] = -1;
        int a_SelInt = SelStringtoInt(a_SelString); //a_SelInt번째 세로줄 (0, 1, 2)
        int m_SelInt = SelStringtoInt(m_SelString); //m_SelInt번째 세로줄 (0, 1, 2)
        TileListArr[m_SelInt].RemoveAt(TileListArr[m_SelInt].Count - 1);
        TileListArr[a_SelInt].Add(a_Selectedtile);
        m_MovedTileNum[0] = a_SelInt;   //m_SelectedTile의 열
        m_MovedTileNum[1] = TileListArr[a_SelInt].Count - 1; //m_SelectedTile의 행

        if (a_Selectedtile.GetComponent<TileNode>().m_TType != TileType.Normal)
            a_Selectedtile.GetComponent<TileNode>().m_isSPmoved = true;

        for (int ii = 0; ii < 3; ii++)
            UpdateTileList(TileListArr[ii], ii);
    }

    int SelStringtoInt(string a_SelString)
	{
        if (a_SelString == "Left")
            return 0;
        else if (a_SelString == "Down")
            return 1;
        else
            return 2;
	}

    public void ComboText(int a_Combo)
    {
        if (m_ComboObj == null) return;

        GameObject[] ComboTxtObj = GameObject.FindGameObjectsWithTag("ComboTxt");
        foreach (GameObject ComboTxt in ComboTxtObj)
            Destroy(ComboTxt);

        a_ComboClone = (GameObject)Instantiate(m_ComboObj);
        a_ComboClone.transform.SetParent(TxtPrefabParent.transform);
        a_ComboTx = a_ComboClone.GetComponent<ComboText>();
        if (a_ComboTx != null)
            a_ComboTx.InitCombo(a_Combo);

        a_ComboClone.transform.localPosition = new Vector3(0.0f, 250.0f, 0.0f);
        a_ComboClone.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);

        LevelUpdate(m_Combo);

    }

    void ComboUpdate()
	{
        if (GlobalValue.g_SDJRUGLv[2] < 1) return;

        if (m_Combo == m_FireCombo && !isSFOn)
		{
            m_TempLv = m_Level;
            m_Level = 1;
            for (int ii = 0; ii < 3; ii++)
                DelTileListArr[ii].Clear();
            m_BombGuageCount = 0;
            for (int ii = 0; ii < 3; ii++)
            {
                if (TileListArr[ii].Count == 0)
                    continue;

                for (int jj = 0; jj < TileListArr[ii].Count; jj++)
                {
                    DelTileListArr[ii].Add(jj);
                    m_BombGuageCount++;
				}
			}
            m_BombGuageCount /= 2;

			DestroyTile(null, TileListArr, DelTileListArr);
            for (int ii = 0; ii < 3; ii++)
                UpdateTileList(TileListArr[ii], ii);

            m_SFTimer = m_SuperRate;
            isSFOn = true;
            isSFLineSpawn = true;
		}
        else if (m_Combo == m_FireCombo + 1)
            isSFOn = false;

        if (isSFLineSpawn)
        {
            for (int ii = 0; ii < 3; ii++)
                TileListArr[ii].Clear();

            for (int ii = 0; ii < 3; ii++)
                UpdateTileList(TileListArr[ii], ii);
            InitLineSpawn();

            if (m_Combo < 50)
            {
                a_SFImgClone = Instantiate(m_SuperFeverImgObj);
                a_SFImgClone.transform.SetParent(TxtPrefabParent.transform);

                a_SFImgClone.transform.localPosition = Vector3.zero;
                a_SFImgClone.transform.localScale = Vector3.one;
            }
            else
            {
                a_FyahClone = Instantiate(m_FyahImgObj);
                a_FyahClone.transform.SetParent(TxtPrefabParent.transform);

                a_FyahClone.transform.localPosition = Vector3.zero;
                a_FyahClone.transform.localScale = Vector3.one;
            }
            isSFLineSpawn = false;
        }
    }

    void LevelUpdate(int a_Combo)
	{
        if (6 <= m_Level) return;
        if (m_SFTimer > 0.0f) return;

        if (m_TileCount > 550) m_Level = 6;
        else if (m_TileCount > 450) m_Level = 5;
        else if (m_TileCount > 300) m_Level = 4;
        else if (m_TileCount > 130) m_Level = 3;
        else if (m_TileCount > 30) m_Level = 2;
    }

    public void CountTime(int a_ShowTime, bool a_isTimeShow)//, Color a_Color
    {
        if (!a_isTimeShow) return;
        if (m_TimerObj == null) return;

        a_TimeTxtClone = (GameObject)Instantiate(m_TimerObj);
        a_TimeTxtClone.transform.SetParent(TxtPrefabParent.transform);
        a_TimerTx = a_TimeTxtClone.GetComponent<TimerText>();
        if (a_TimerTx != null)
            a_TimerTx.InitTime(a_ShowTime);

        a_TimeTxtClone.transform.localPosition = new Vector3(130.0f, -100.0f, 0.0f);
        a_TimeTxtClone.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
    }

    void CheckTimeState(float a_CurTime)
    {
        int a_ShowTime = (int)a_CurTime;
        if (1 <= a_ShowTime && a_ShowTime <= 5)
        {
            CountTime(a_ShowTime, m_isTimeShow[a_ShowTime - 1]);
            m_isTimeShow[a_ShowTime - 1] = false;
        }
        else if (a_ShowTime == 0)
        {
            m_GameTime = 0.0f;
            m_isTimeUp = true;
            TimeUpFunc();
            for (int ii = 0; ii < m_isTimeShow.Length; ii++)
                m_isTimeShow[ii] = false;
        }
        else
        {
            for (int ii = 0; ii < m_isTimeShow.Length; ii++)
                m_isTimeShow[ii] = true;
        }
    }

    void TimeUpFunc()
	{
        if (!m_isTimeUp) return;

        //타일을 모두 게임오버 타일로 바꾸기
        GameObject[] Tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in Tiles)
        {
            TileNode a_TileND = tile.GetComponent<TileNode>();
            a_TileND.m_TileImg.sprite = a_TileND.m_TileSprite[14];
            if (a_TileND.m_BonusText.gameObject.activeSelf)
                a_TileND.m_BonusText.gameObject.SetActive(false);
        }

        m_GOPanelObj.SetActive(true);
        m_GOPanelImg.gameObject.SetActive(false);
        m_TUPanelImg.gameObject.SetActive(true);

        Time.timeScale = 0.0f;
        m_GOImgShowTimer -= Time.unscaledDeltaTime;

        if (m_GOImgShowTimer <= 0.0f)
        {
            m_GOImgShowTimer = 3.0f;
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("GameOverScene");
        }
    }

    #region ---------------Pause Panel

    void PauseBtnFunc(bool isPause)
	{
        if (!m_IsGameStart) return;
        
        m_OnlySel = false;

        if (m_PausePanelObj != null)
            m_PausePanelObj.SetActive(isPause);

        if (m_SelectedTile != null)
            m_SelectedTile.GetComponent<TileNode>().m_isSelected = false;

        GameObject[] ComboTxtObj = GameObject.FindGameObjectsWithTag("ComboTxt");
        foreach (GameObject ComboTxt in ComboTxtObj)
            ComboTxt.SetActive(!isPause);

        if (isPause) Time.timeScale = 0.0f;
        else Time.timeScale = 1.0f;
    }

    void PPRestartBtnFunc()
    {
        m_PPStr = "SDJRReady";
        MessageBoxFunc("★ 경고 알림 ★", "게임을 나가면 기록이 저장되지 않아요 ㅠ0ㅠ\n" +
            "그래도 다시 하시겠어요?", MessageState.YesNo);
    }

    void PPGotoLobbyBtnFunc()
    {
        m_PPStr = "Lobby";
        MessageBoxFunc("★ 경고 알림 ★",
            "게임을 나가면 기록이 저장되지 않아요 ㅠ0ㅠ\n" + "그래도 나가시겠어요?", MessageState.YesNo);
    }

    void MessageBoxFunc(string a_MessLabel, string a_Mess, MessageState a_MsState)
    {
        if (m_MessageBoxObj != null)
            m_MessageBoxObj = Resources.Load("MessageBox") as GameObject;

        GameObject a_MsgBoxObj = Instantiate(m_MessageBoxObj);
        a_MsgBoxObj.transform.SetParent(Msg_Canvas.transform, false);

        MessageBox a_MsgBox = a_MsgBoxObj.GetComponent<MessageBox>();
        a_MsgBox.m_MessState = a_MsState;
        if (a_MsgBox != null)
            a_MsgBox.ShowMessage(a_MessLabel, a_Mess, a_MsState);
    }

    #endregion //---------------Pause Panel
}
