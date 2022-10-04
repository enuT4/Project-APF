using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;



public enum CharType
{
    YSMS_Bomb = 0,
    YSMS_Char1,
    YSMS_Char2,
    YSMS_Char3,
    YSMS_Char4,
    YSMS_Char5,
    YSMS_Char6,
    YSMS_Char7,
    YSMS_Char8,
    CharCount
}

public class YSMSIngameMgr : MonoBehaviour
{
    public Transform CharSpawnTr;
    public Transform ShowLChGroupTr;
    public Transform ShowRChGroupTr;
    public GameObject m_CharPrefab = null;

    float[] PosArray = new float[10];
    float[] ScaleArray = new float[10];
    float[] SpeedArray = new float[10];
    public static int[] randArray = new int[8];
    int[] selArray = new int[8];
    int[] LeftArray = new int[3];
    int[] RightArray = new int[3];
    public int m_Level = 1;

    public static List<GameObject> m_SpawnList = new List<GameObject>();

    float m_MoveSpeed = 4.0f;

    [Header("-------- GameUI --------")]
    public Button m_LeftBtn = null;
    public Button m_RightBtn = null;
    public Text m_ScoreText = null;
    public Image m_GuageBarBack = null;
    public Image m_GuageBar = null;
    public Image m_TimeBar = null;
    Vector3 m_TimeBarPos;
    public Text m_TimerText = null;

    public float m_GameTime = 60.0f;
    public float m_MaxTime = 60.0f;
    public int m_CurScore = 0;

    float m_MaxGuage = 100.0f;
    float m_CurGuage = 0.0f;
    public int m_BombCount = 0;
    public Image m_PlusSecImg = null;
    float m_PlusShowTimer = 0.0f;

    float m_FalseTimer = 0.0f;
    float m_SetFalse = 1.0f;
    float m_scale = 1.073366f;
    int m_MonsterCount = 0;
    
    bool m_InitSpawnOnOff = false;
    bool[] m_isLvLeft = new bool[2];

    int m_LevelLIdx = 0;
    int m_LevelRIdx = 0;

    public Image m_TimeUpImg = null;
    float m_GOImgShowTimer = 3.0f;

    //콤보 텍스트
    [Header("-------- Combo Text --------")]
    public GameObject m_ComboObj = null;
    public Transform m_Canvas = null;
    GameObject a_ComboClone;
    ComboText a_ComboTx;
    //public Transform m_HUD_Canvas = null;
    public int m_Combo = 0;
    public float m_CheckComboTimer = 0.0f;

    GameObject a_TimeTxtClone;
    TimerText a_TimerTx;
    public GameObject m_TimerObj = null;
    bool[] m_isTimeShow = new bool[5];

    //아이템
    bool isSFOn = false;
    bool isTFTimerOn = false;
    int m_RanTFCount = 0;
    int m_TFCount = 0;
    float m_TFItemTime = 0.0f;
    float m_TFTimer = 0.0f;
    public Image m_TFIconImg;

    //업그레이드
    int m_BonusScore = 0;
    float m_FeverRate = 1.0f;
    float m_SuperRate = 1.0f;
    bool m_isBonusHit = false;
    [HideInInspector] public Transform m_BonusTxtPos = null;
    public GameObject m_BonusTxtObj = null;
    GameObject BonusTxtClone;
    BonusText BonusTx;

    public static YSMSIngameMgr Inst = null;

    [Header("-------- Game Ready --------")]
    public GameObject m_ReadyPanelObj = null;
    public Image m_ReadyImg = null;
    public Image m_GoImg = null;
    float m_ReadyTimer = 1.0f;
    float m_GoTimer = 0.0f;
    bool m_IsGameStart = false;
    Color m_ReadyImgColor;

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
        InitSpawn();
        m_TimeBarPos = m_TimeBar.transform.position;
        m_TimerText.text = ((int)m_GameTime).ToString();

        if (m_LeftBtn != null)
            m_LeftBtn.onClick.AddListener(LeftBtnFunc);

        if (m_RightBtn != null)
            m_RightBtn.onClick.AddListener(RightBtnFunc);

		m_ScoreText.text = m_CurScore.ToString("N0");

        for (int ii = 0; ii < SpeedArray.Length; ii++)
            SpeedArray[ii] = m_MoveSpeed / (Mathf.Pow(m_scale, ii));

        if (m_TimeUpImg != null)
            m_TimeUpImg.gameObject.SetActive(false);

        if (m_PlusSecImg != null)
            m_PlusSecImg.gameObject.SetActive(false);

        m_Level = 1;
        m_CurScore = 0;
        m_Combo = 0;
        m_MonsterCount = 0;
        
        //5초 세기
        for (int ii = 0; ii < m_isTimeShow.Length; ii++)
            m_isTimeShow[ii] = true;

        if (m_ReadyPanelObj != null && !m_IsGameStart)
            m_ReadyPanelObj.SetActive(true);

        if (m_PauseBtn != null)
            m_PauseBtn.onClick.AddListener(() =>
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

        m_RanTFCount = Random.Range(130, 200);
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
                if (m_ReadyImg.gameObject.activeSelf ||
                    !m_GoImg.gameObject.activeSelf) 
                    return;

                m_GoTimer -= Time.deltaTime;
                if (m_GoTimer <= 0.0f)
				{
                    m_ReadyPanelObj.SetActive(false);
                    m_IsGameStart = true;
                    if (isSFOn)
                        m_Combo = 100;
				}
			}
		}
        else
		{
            m_GameTime -= Time.deltaTime;
            m_TimerText.text = ((int)m_GameTime).ToString();
            if(Time.timeScale != 0.0f && (0.0f < m_GameTime && m_GameTime <= 60.0f))
                m_TimeBar.transform.Translate(-0.055f, 0, 0);

            CheckTimeState(m_GameTime);

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.F)) 
                LeftBtnFunc();
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K)) 
                RightBtnFunc();

            if (0.0f < m_FalseTimer)
            {
                m_FalseTimer -= Time.deltaTime;
                if (m_FalseTimer <= 0.0f)
                {
                    m_FalseTimer = 0.0f;
                    CharNode.m_isClear = false;
                }
            }

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

            if (0.0f < m_CheckComboTimer)
            {
                m_CheckComboTimer -= Time.deltaTime;
                if (m_CheckComboTimer <= 0.0f)
                {
                    m_CheckComboTimer = 0.0f;
                    m_Combo = 0;
                }
            }
        }
        
        if (0.0f < m_TFTimer)
		{
            m_TFTimer -= Time.deltaTime;
            if (m_TFTimer <= 0.0f)
			{
                m_TFTimer = 0.0f;
                m_RanTFCount = Random.Range(130, 200);
                isTFTimerOn = false;
                m_TFIconImg.gameObject.SetActive(isTFTimerOn);
            }
		}
    }

    void SpawnChar(int a_Idx, float a_PosRate, float a_ScaleRate)
	{
        GameObject go = Instantiate(m_CharPrefab) as GameObject;
        CharNode a_Node = go.GetComponent<CharNode>();
        a_Node.SetCharRsc(a_Idx);
        
        if (CharSpawnTr != null)
            go.transform.SetParent(CharSpawnTr);
        go.transform.position = new Vector3(CharSpawnTr.position.x,
            CharSpawnTr.position.y + a_PosRate, CharSpawnTr.position.z);
        go.transform.localScale = new Vector2(a_ScaleRate, a_ScaleRate);

        if (m_InitSpawnOnOff) m_SpawnList.Insert(0, go);
        else m_SpawnList.Add(go);
    }

    void CheckItem()
	{
        if (ReadySceneMgr.m_isChecked[0]) m_GameTime = 65.0f;
        if (ReadySceneMgr.m_isChecked[1]) m_TFItemTime = 3.0f;
        if (ReadySceneMgr.m_isChecked[2]) m_SetFalse = 0.5f;
        if (ReadySceneMgr.m_isChecked[3]) isSFOn = true;
    }

    void CheckUGRate()
	{
        GlobalValue.YSMSUGAm();
        m_BonusScore = GlobalValue.BonusAmount[GlobalValue.g_YSMSUGLv[0]];
        m_FeverRate = GlobalValue.FeverAmount[GlobalValue.g_YSMSUGLv[1]];
        m_SuperRate = GlobalValue.SuperAmount[GlobalValue.g_YSMSUGLv[2]];
	}

    void InitSpawn()
	{
        InitChar();
        m_InitSpawnOnOff = true;
        ScaleArray[9] = 0.7f / m_scale;
        PosArray[9] = PosArray[8] + 1.0f;

        for (int ii = 0; ii < 9; ii++)
        {
            ScaleArray[ii] = 0.7f + 0.6f / 9.0f * (8 - ii);
            PosArray[ii] = (-240.0f / 9.0f * (8 - ii)) * ScaleArray[ii] * 1.2f;
        }

        for (int ii = 0; ii < 9; ii++)
		{
            int LeftorRight = Random.Range(0, 2);
            if (LeftorRight == 0)
                SpawnChar(LeftArray[0], PosArray[8 - ii], ScaleArray[8 - ii]);
            else
                SpawnChar(RightArray[0], PosArray[8 - ii], ScaleArray[8 - ii]);
        }
        ShowChar(LeftArray[0], true, 1);
        ShowChar(RightArray[0], false, 1);

        m_LevelLIdx = 1;
        m_InitSpawnOnOff = false;
    }

    //void InitChar2()
    //{//왼쪽 3개, 오른쪽 3개, 제외 1개 + 보너스블록 해당하는 랜덤 숫자 생성
    //    int CharCount = 7;
    //    bool isSameNum;
    //
    //    for (int ii = 0; ii < CharCount; ii++)
	//	{
    //        while(true)
	//		{
    //            randArray[ii] = Random.Range(1, CharCount + 1);
    //            isSameNum = false;
    //            for (int jj = 0; jj < ii; jj++)
	//			{
    //                if (randArray[jj] == randArray[ii])
	//				{
    //                    isSameNum = true;
    //                    break;
	//				}
	//			}
    //            if (!isSameNum) break;
	//		}
	//	}
    //
    //    for (int ii = 0; ii < 2; ii++)
    //    {
    //        int a_LvLorR = Random.Range(0, 2);
    //        if (a_LvLorR == 0)
    //            m_isLvLeft[ii] = true;
    //        else
    //            m_isLvLeft[ii] = false;
    //    }
    //
    //    if (GlobalValue.g_YSMSUGLv[0] == 0)
	//	{
    //        for (int ii = 0; ii < 3; ii++)
    //        {
    //            //randarray의 0, 1, 2는 왼쪽, 3, 4, 5는 오른쪽, 6은 제외
    //            LeftArray[ii] = randArray[ii];
    //            RightArray[ii] = randArray[3 + ii];
    //        }
    //    }
    //    else if (1 <= GlobalValue.g_YSMSUGLv[0] && GlobalValue.g_YSMSUGLv[0] <= 6)
	//	{
    //        //보너스 블록이 4번째에 등장
    //        if (m_isLvLeft[0])
	//		{
    //            for (int ii = 0; ii < 3; ii++)
    //                LeftArray[ii] = randArray[ii];
    //            RightArray[0] = randArray[3];
    //            RightArray[1] = 8;
    //            RightArray[2] = randArray[4];
    //        }
    //        else
	//		{
    //            LeftArray[0] = randArray[0];
    //            LeftArray[1] = 8;
    //            LeftArray[2] = randArray[1];
    //            for (int ii = 0; ii < 3; ii++)
    //                RightArray[ii] = randArray[2 + ii];
    //        }
	//	}
    //    else if (7 <= GlobalValue.g_YSMSUGLv[0] && GlobalValue.g_YSMSUGLv[0] <= 10)
	//	{
    //        //보너스 블록이 3번째에 등장
    //        if (m_isLvLeft[0])
	//		{
    //            LeftArray[0] = randArray[0];
    //            LeftArray[1] = 8;
    //            LeftArray[2] = randArray[1];
    //            for (int ii = 0; ii < 3; ii++)
    //                RightArray[ii] = randArray[2 + ii];
    //        }
    //        else
	//		{
    //            for (int ii = 0; ii < 3; ii++)
    //                LeftArray[ii] = randArray[ii];
    //            RightArray[0] = randArray[3];
    //            RightArray[1] = 8;
    //            RightArray[2] = randArray[4];
    //        }
    //    }
    //    else if (11 <= GlobalValue.g_YSMSUGLv[0] && GlobalValue.g_YSMSUGLv[0] <= 13)
	//	{
    //        //보너스 블록이 2번째에 등장
    //        for (int ii = 0; ii < 3; ii++)
    //            LeftArray[ii] = randArray[ii];
    //        RightArray[0] = 8;
    //        RightArray[1] = randArray[3];
    //        RightArray[2] = randArray[4];
    //    }
    //    else //if(14 <= GlobalValue.g_YSMSUGLv[0] && GlobalValue.g_YSMSUGLv[0] <= 15)
	//	{
    //        //보너스 블록이 1번째에 등장
    //        LeftArray[0] = 8;
    //        LeftArray[1] = randArray[0];
    //        LeftArray[2] = randArray[1];
    //        for (int ii = 0; ii < 3; ii++)
    //            RightArray[ii] = randArray[2 + ii];
    //    }
    //}

    void InitChar()
	{
        for (int ii = 0; ii < 2; ii++)
        { //3, 5번째 캐릭터가 어느쪽에서 나올 것인지 정해주는 코드 
            int a_LvLorR = Random.Range(0, 2);
            if (a_LvLorR == 0) m_isLvLeft[ii] = true;
            else m_isLvLeft[ii] = false;
        }

        int a_BonusLv = GlobalValue.g_YSMSUGLv[0];
        if (a_BonusLv == 0)
            selArray[7] = 8;
        else if (1 <= a_BonusLv && a_BonusLv <= 6)
        {   //보너스 캐릭터가 4번째에 등장
            if (m_isLvLeft[0]) selArray[4] = 8;
            else selArray[1] = 8;
        }
		else if (7 <= a_BonusLv && a_BonusLv <= 10)
        {   //보너스 캐릭터가 3번째에 등장
            if (m_isLvLeft[0]) selArray[1] = 8;
            else selArray[4] = 8;
		}
		else if (11 <= a_BonusLv && a_BonusLv <= 13)
			selArray[3] = 8;
		else if (14 <= a_BonusLv && a_BonusLv <= 15)
			selArray[0] = 8;

        int CharCount = 7;
        bool isSameNum;
        for (int ii = 0; ii < CharCount + 1; ii++)
        {
            if (selArray[ii] == 8) continue; //보너스 블럭
            while (true)
            {
                selArray[ii] = Random.Range(1, CharCount + 1);
                isSameNum = false;
                for (int jj = 0; jj < ii; jj++)
                {
                    if (selArray[jj] == selArray[ii])
                    {
                        isSameNum = true;
                        break;
                    }
                }
                if (!isSameNum) break;
            }
        }

        for (int ii = 0; ii < 3; ii++)
        {
            //randarray의 0, 1, 2는 왼쪽, 3, 4, 5는 오른쪽, 6과 7은 제외
            LeftArray[ii] = selArray[ii];
            RightArray[ii] = selArray[3 + ii];
        }
    }

    void ShowChar(int a_Idx, bool a_IsLeft, int a_Level)
	{
        a_Level = a_Level - 2;

        GameObject go = Instantiate(m_CharPrefab) as GameObject;
        CharNode a_Node = go.GetComponent<CharNode>();
        a_Node.SetCharRsc(a_Idx);

        if (a_IsLeft) SetGroup(go, ShowLChGroupTr, a_Level);
        else SetGroup(go, ShowRChGroupTr, a_Level);

        go.transform.localScale = new Vector3(1, 1, 1);
        if (a_Idx == 8)
            m_BonusTxtPos = go.transform;
    }

    void SetGroup(GameObject a_go, Transform tr, int a_Level)
	{
        if (tr != null)
            a_go.transform.SetParent(tr);
        a_go.transform.position = new Vector3(tr.position.x,
        tr.position.y + a_Level * 100, tr.position.z);
    }

    void LeftBtnFunc()
	{
        if (m_PausePanelObj.activeSelf) return;
        if (m_GameTime <= 0.0f) return;
        if (0.0f < m_FalseTimer) return;

        CharNode a_FirstNode = m_SpawnList[0].GetComponent<CharNode>();
        if (a_FirstNode.m_isMove) return;

        if (a_FirstNode.m_ChImgIdx == LeftArray[0] || a_FirstNode.m_ChImgIdx == LeftArray[1] ||
            a_FirstNode.m_ChImgIdx == LeftArray[2] || a_FirstNode.m_ChImgIdx == (int)CharType.YSMS_Bomb) 
		{
            if (a_FirstNode.m_ChImgIdx == (int)CharType.YSMS_Bomb)
            {
                m_GameTime += 10.0f;
                m_BombCount++;
                if (m_MaxTime <= m_GameTime)
                    m_GameTime = m_MaxTime;

                float a_TimeBarX = m_TimeBar.transform.position.x + (400.0f / 6.0f);
                if (a_TimeBarX >= m_TimeBarPos.x)
                    a_TimeBarX = m_TimeBarPos.x;
                m_PlusShowTimer = 1.0f;
                m_TimeBar.transform.position = new Vector3(a_TimeBarX,
                    m_TimeBar.transform.position.y, m_TimeBar.transform.position.z);
            }
            m_MonsterCount++;
            m_Combo++;
            ComboText(m_Combo);
            TransformFunc(m_RanTFCount);
            m_CheckComboTimer = 1.0f;
            if (a_FirstNode.m_ChImgIdx == 8)
                m_isBonusHit = true;
            AddScore();
            MakeNewChar();
            SetLayer();
            UpdateLevel();

            a_FirstNode.m_isMove = true;
            a_FirstNode.m_isLeft = true;
        }
		else //틀렸을 때
		{
            m_CurGuage -= 15.0f;
            if (m_CurGuage <= 0.0f)
                m_CurGuage = 0.0f;
            m_GuageBar.fillAmount = m_CurGuage / m_MaxGuage;
            a_FirstNode.m_isMove = false;
            m_FalseTimer = m_SetFalse;
            Transform a_FindObj = a_FirstNode.transform.Find("CharImg");
            if (a_FindObj != null)
                a_FindObj.GetComponent<RawImage>().color = new Color32(30, 30, 30, 255);
        }
	}

	void RightBtnFunc()
	{
        if (m_PausePanelObj.activeSelf) return;
        if (m_GameTime <= 0.0f) return;
        if (0.0f < m_FalseTimer) return;
        CharNode a_FirstNode = m_SpawnList[0].GetComponent<CharNode>();
        if (a_FirstNode.m_isMove) return;

        if (a_FirstNode.m_ChImgIdx == RightArray[0] || a_FirstNode.m_ChImgIdx == RightArray[1] ||
            a_FirstNode.m_ChImgIdx == RightArray[2] || a_FirstNode.m_ChImgIdx == (int)CharType.YSMS_Bomb) 
        {
            if (a_FirstNode.m_ChImgIdx == (int)CharType.YSMS_Bomb)
            {
                m_GameTime += 10.0f;
                m_BombCount++;
                if (m_MaxTime <= m_GameTime)
                    m_GameTime = m_MaxTime;

                float a_TimeBarX = m_TimeBar.transform.position.x + (400.0f / 6.0f);
                if (a_TimeBarX >= m_TimeBarPos.x)
                    a_TimeBarX = m_TimeBarPos.x;
                m_PlusShowTimer = 1.0f;
                m_TimeBar.transform.position = new Vector3(a_TimeBarX,
                    m_TimeBar.transform.position.y, m_TimeBar.transform.position.z);
            }

            m_MonsterCount++;
            m_Combo++;
            ComboText(m_Combo);
            TransformFunc(m_RanTFCount);
            m_CheckComboTimer = 1.0f;
            if (a_FirstNode.m_ChImgIdx == 8)
                m_isBonusHit = true;
            AddScore();
            MakeNewChar();
            SetLayer();
            UpdateLevel();

            a_FirstNode.m_isMove = true;
            a_FirstNode.m_isLeft = false;
        }
        else //틀렸을 때
        {
            m_CurGuage -= 15.0f;
            if (m_CurGuage <= 0.0f)
                m_CurGuage = 0.0f;
            m_GuageBar.fillAmount = m_CurGuage / m_MaxGuage;
            a_FirstNode.m_isMove = false;
            m_FalseTimer = m_SetFalse;
			Transform a_FindObj = a_FirstNode.transform.Find("CharImg");
            if (a_FindObj != null)
                a_FindObj.GetComponent<RawImage>().color = new Color32(30, 30, 30, 255);
		}
	}

    void MakeNewChar()
	{
        if (m_MaxGuage <= m_CurGuage) return;

        if (0.0f < m_TFTimer)
            SpawnChar(LeftArray[0], PosArray[9], ScaleArray[9]);
        else
		{
            int LorR = Random.Range(0, 2);
            if (LorR == 1)
            {
                int RandChar = Random.Range(0, m_LevelLIdx);
                SpawnChar(LeftArray[RandChar], PosArray[9], ScaleArray[9]);
            }
            else
            {
                int RandChar = Random.Range(0, m_LevelRIdx);
                SpawnChar(RightArray[RandChar], PosArray[9], ScaleArray[9]);
            }
        }
    }

    public void UpdateCharArr()
    {
        for (int ii = 1; ii < 10; ii++) 
		{
            Vector3 Destination = new Vector3(m_SpawnList[ii].transform.position.x,
                PosArray[ii - 1], m_SpawnList[ii].transform.position.z);
            m_SpawnList[ii].transform.position =
                Vector3.MoveTowards(m_SpawnList[ii].transform.position, Destination, SpeedArray[ii - 1]);
            if (m_SpawnList[ii].transform.position.y < Destination.y)
                m_SpawnList[ii].transform.position = Destination;
            float m_NewScale = ScaleArray[ii - 1];
            m_SpawnList[ii].transform.localScale = new Vector3(m_NewScale, m_NewScale, 1.0f);
		}
    }

    void TransformFunc(int a_Count)
	{
        if (isTFTimerOn) return;
        if (GlobalValue.g_YSMSUGLv[2] == 0 && !ReadySceneMgr.m_isChecked[1]) return;

        m_TFCount++;
        if (a_Count < m_TFCount)
		{
            m_TFTimer = m_SuperRate + m_TFItemTime;
            m_TFCount = 0;
            isTFTimerOn = true;
            m_TFIconImg.gameObject.SetActive(isTFTimerOn);
		}
    }

    public void ComboText(int a_Combo)
	{
        if (m_ComboObj == null) return;

        GameObject[] ComboTxtObj = GameObject.FindGameObjectsWithTag("ComboTxt");
        foreach (GameObject ComboTxt in ComboTxtObj)
            Destroy(ComboTxt);

        a_ComboClone = Instantiate(m_ComboObj);
        a_ComboClone.transform.SetParent(m_Canvas);
        a_ComboTx = a_ComboClone.GetComponent<ComboText>();
        if (a_ComboTx != null)
            a_ComboTx.InitCombo(a_Combo);

        a_ComboClone.transform.localPosition = new Vector3(0.0f, 135.0f, 0.0f);
    }

    public void CountTime(int a_ShowTime, bool a_isTimeShow)//, Color a_Color
    {
        if (!a_isTimeShow) return;
        if (m_TimerObj == null) return;

        a_TimeTxtClone = (GameObject)Instantiate(m_TimerObj);
        a_TimeTxtClone.transform.SetParent(m_Canvas);
        a_TimerTx = a_TimeTxtClone.GetComponent<TimerText>();
        if (a_TimerTx != null)
            a_TimerTx.InitTime(a_ShowTime);

        a_TimeTxtClone.transform.localPosition = new Vector3(93.0f, -110.0f, 0.0f);
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
            GameOverFunc();
            m_SpawnList.Clear();
            for (int ii = 0; ii < m_isTimeShow.Length; ii++)
                m_isTimeShow[ii] = false;
        }
        else
		{
            for (int ii = 0; ii < m_isTimeShow.Length; ii++)
                m_isTimeShow[ii] = true;
        }
    }

    void GameOverFunc()
	{
        m_TimeUpImg.gameObject.SetActive(true);
        
        m_GOImgShowTimer -= Time.deltaTime;
        if (m_GOImgShowTimer <= 0.0f)
		{
            m_GOImgShowTimer = 3.0f;
            //여기서 세이브 한번 해줘야 할듯
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
		}
	}

	void AddScore()
	{
        float a_Value = 0;
        a_Value = 372.2f * (100.0f + m_Level) * 0.01f *
            (100.0f + m_Combo) * 0.01f * m_FeverRate;
        if (m_isBonusHit)
		{
            a_Value += GlobalValue.BonusAmount[GlobalValue.g_YSMSUGLv[0]];
            BonusText();
		}
        m_CurScore += (int)a_Value;
        m_ScoreText.text = m_CurScore.ToString("N0");
        m_CurGuage += 1.5f;
        if (m_MaxGuage <= m_CurGuage)
		{
            m_CurGuage = 0.0f;
            SpawnChar((int)CharType.YSMS_Bomb, PosArray[9], ScaleArray[9]);
		}
        m_GuageBar.fillAmount = m_CurGuage / m_MaxGuage;
	}

    void BonusText()
	{
        if (!m_isBonusHit)return;
        if (m_BonusTxtObj == null) return;

        BonusTxtClone = (GameObject)Instantiate(m_BonusTxtObj);
        BonusTxtClone.transform.SetParent(m_BonusTxtPos);
        BonusTxtClone.transform.position = new Vector3(m_BonusTxtPos.position.x, m_BonusTxtPos.position.y + 60.0f, m_BonusTxtPos.position.z);
        BonusTx = BonusTxtClone.GetComponent<BonusText>();
        if (BonusTx != null)
            BonusTx.InitScore(GlobalValue.g_YSMSUGLv[0]);

        m_isBonusHit = false;
    }

    void UpdateLevel()
	{
        if (m_Level == 4 && 600 < m_MonsterCount)   
		{
            m_Level = 5;
            if (m_isLvLeft[1])
                ShowChar(RightArray[2], !m_isLvLeft[1], 3);
            else
                ShowChar(LeftArray[2], !m_isLvLeft[1], 3);
            m_LevelLIdx = (m_Level + 1) / 2;
        }
		else if (m_Level == 3 && 500 < m_MonsterCount)
		{
			m_Level = 4;
            if (m_isLvLeft[1])
			{
                ShowChar(LeftArray[2], m_isLvLeft[1], 3);
                m_LevelLIdx = 3;
            }
            else
			{
                ShowChar(RightArray[2], m_isLvLeft[1], 3);
                m_LevelLIdx = 2;
            }
            
        }
		else if (m_Level == 2 && 250 < m_MonsterCount)
		{
            m_Level = 3;
            
            if (m_isLvLeft[0])
                ShowChar(RightArray[1], !m_isLvLeft[0], 2);
            else
                ShowChar(LeftArray[1], !m_isLvLeft[0], 2);
            m_LevelLIdx = (m_Level + 1) / 2;
        }
		else if (m_Level == 1 && 100 < m_MonsterCount)
		{
			m_Level = 2;
            if (m_isLvLeft[0])
			{
                ShowChar(LeftArray[1], m_isLvLeft[0], 2);
                m_LevelLIdx = 2;
            }
            else //m_isLvLeft[0] == false
            {
                ShowChar(RightArray[1], m_isLvLeft[0], 2);
                m_LevelLIdx = 1;
            }
        }
        m_LevelRIdx = m_Level + 1 - m_LevelLIdx;
	}

    void SetLayer()
	{
        for (int ii = 0; ii < 10; ii++)
		{
            if (m_SpawnList[ii] == null) continue;
        
            Canvas SortLayerCanvas = m_SpawnList[ii].GetComponent<Canvas>();
            SortLayerCanvas.overrideSorting = true;
            SortLayerCanvas.sortingOrder = 9 - ii;
        }
	}

    #region ---------------Pause Panel

    void PauseBtnFunc(bool isPause)
	{
        if(m_PausePanelObj != null)
            m_PausePanelObj.SetActive(isPause);

        foreach (GameObject Char in m_SpawnList)
            Char.SetActive(!isPause);
        GameObject[] ComboTxtObj = GameObject.FindGameObjectsWithTag("ComboTxt");
        foreach (GameObject ComboTxt in ComboTxtObj)
            ComboTxt.SetActive(!isPause);

        if (isPause) Time.timeScale = 0.0f;
        else Time.timeScale = 1.0f;
    }

    void PPRestartBtnFunc()
    {
        m_PPStr = "YSMSReady";
        MessageBoxFunc("★ 경고 알림 ★", "게임을 나가면 기록이 저장되지 않아요 ㅠ0ㅠ\n" +
            "그래도 다시 하시겠어요?", MessageState.YesNo);
    }

    void PPGotoLobbyBtnFunc()
    {
        m_PPStr = "Lobby";
        MessageBoxFunc("★ 경고 알림 ★", "게임을 나가면 기록이 저장되지 않아요 ㅠ0ㅠ\n" +
            "그래도 나가시겠어요?", MessageState.YesNo);
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
