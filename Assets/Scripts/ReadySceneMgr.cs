using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class ReadySceneMgr : MonoBehaviour
{
    public Button m_BackBtn = null;
    public Button m_SeeRankingBtn = null;
    public Text m_RankBtnTxt = null;
    public int m_MyRank = 0;

    [Header("-------- Upgrade Btn --------")]
    public Button m_UpBonusBtn = null;
    public Text m_BonusLvText = null;
    public Text m_BonusAmText = null;
    public Button m_UpFeverBtn = null;
    public Text m_FeverLvText = null;
    public Text m_FeverAmText = null;
    public Button m_UpSuperBtn = null;
    public Text m_SuperLvText = null;
    public Text m_SuperAmText = null;

    [Header("-------- Item Btn --------")]
    public Button m_ItemNo1Btn = null;
    public Button m_ItemNo2Btn = null;
    public Button m_ItemNo3Btn = null;
    public Button m_ItemNo4Btn = null;
    string[] m_ItemInfo = new string[4];
    string[] m_YSMSItemInfo = new string[4];
    string[] m_SDJRItemInfo = new string[4];
    public Button m_CharShopBtn = null;
    public Button m_GameStartBtn = null;
    public Sprite[] m_ButtonSprite;
    
    public Image[] m_CheckImg;
    public static bool[] m_isChecked = new bool[4];
    int[] m_ItemCost = new int[4];
    public Text[] m_ItemCostTxt;

    [Header("-------- Info Text --------")]
    public Text m_GameNameText = null;
    public Text m_UserGoldTxt = null;
    public Text m_UserGemTxt = null;
    public Text m_ItemInfoText = null;
    public Text m_RiceText = null;
    public Text m_UserNickTxt = null;

    int m_GoldValue = 0;
    int m_GemValue = 0;

    public GameObject m_UpgradePanelObj;
    public GameObject Canvas_Upgrade = null;
    public GameObject m_RankingPanelObj;
    public GameObject m_MessageBoxObj = null;

    bool isTutSkipOnOff = false;
    string m_GameName = "";

    [Header("-------- Rice Count --------")]
    //밥 사용 시간 기록
    DateTime m_CheckTime;
    int[] m_FillArr = new int[5];
    [SerializeField] int m_CurTimeSec = 0;
    int m_CurYMD = 0;
    int m_TempYMD = 0;
    int m_TimerSec = 0;
    int m_TimerMin = 0;
    int m_RiceFillTimeSec = 300;
    bool isRiceTimerStart = false;
    bool isDayChange = false;

    // Start is called before the first frame update
    void Start()
    {
        CheckRiceCount();
        CalcTimeFunc();
        CheckGameName();

        for (int ii = 0; ii < m_isChecked.Length; ii++)
		{
            m_isChecked[ii] = false;
            m_CheckImg[ii].gameObject.SetActive(m_isChecked[ii]);
		}

        for (int ii = 0; ii < 4; ii++)
            m_ItemCostTxt[ii].text = m_ItemCost[ii].ToString();

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        if (m_SeeRankingBtn != null)
            m_SeeRankingBtn.onClick.AddListener(SeeRankingFunc);

        if (m_UpBonusBtn != null)
            m_UpBonusBtn.onClick.AddListener(() =>
            {
                UpgradePanel.m_UGKind = UGKind.UGBonus;
                UpgradeFunc();
            });

        if (m_UpFeverBtn != null)
            m_UpFeverBtn.onClick.AddListener(() =>
            {
                UpgradePanel.m_UGKind = UGKind.UGFever;
                UpgradeFunc();
            });

        if (m_UpSuperBtn != null)
            m_UpSuperBtn.onClick.AddListener(() =>
            {
                UpgradePanel.m_UGKind = UGKind.UGSuper;
                UpgradeFunc();
            });

        if (m_ItemNo1Btn != null)
            m_ItemNo1Btn.onClick.AddListener(() => { ItemBtnFunc(1); });

        if (m_ItemNo2Btn != null)
            m_ItemNo2Btn.onClick.AddListener(() => { ItemBtnFunc(2); });

        if (m_ItemNo3Btn != null)
            m_ItemNo3Btn.onClick.AddListener(() => { ItemBtnFunc(3); });

        if (m_ItemNo4Btn != null)
            m_ItemNo4Btn.onClick.AddListener(() => { ItemBtnFunc(4); });

        if (m_CharShopBtn != null)
            m_CharShopBtn.onClick.AddListener(() =>
            {
                //SceneManager.LoadScene("CharShop");
                MessageBoxFunc(MessageState.OK, "★ 미구현 알림 ★", "다음 업데이트를 기다려주세요 ㅠ0ㅠ");
            });

        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(CheckGameStart);

        m_UserNickTxt.text = GlobalValue.g_Nickname;

        if (GlobalValue.g_IsRiceTimerStart == 1) isRiceTimerStart = true;

        m_GoldValue = GlobalValue.g_UserGold;
        m_GemValue = GlobalValue.g_UserGem;
        m_TempYMD = GlobalValue.g_RiceCheckDate;
    }

    // Update is called once per frame
    void Update()
    {
        CalcItemCost();
        UpgradeUpdate();
        if (m_RankBtnTxt.text == "-위\n" + "<color=#bb3e01>순위보기</color>")
            GetMyRanking(GlobalValue.g_GKind);

        CalcTimeFunc();

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(GlobalValue.g_RiceCheckDate);
            int hh = GlobalValue.g_RiceCheckTime / 3600;
            int mm = (GlobalValue.g_RiceCheckTime % 3600) / 60;
            int ss = (GlobalValue.g_RiceCheckTime % 3600) % 60;
            Debug.Log(hh.ToString() + " : " + mm.ToString() + " : " + ss.ToString());
            Debug.Log(DateTime.Now);
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (GlobalValue.g_GKind == GameKind.APF_YSMS)
			{
                GlobalValue.g_YSMSRegScore = 0;
				NetworkMgr.Inst.PushPacket(PacketType.YSMSRegScore);
			}
			else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
			{
				GlobalValue.g_SDJRRegScore = 0;
				NetworkMgr.Inst.PushPacket(PacketType.SDJRRegScore);
			}
        }
    }

    void SeeRankingFunc()
	{
        if (m_RankingPanelObj != null)
            m_RankingPanelObj = Resources.Load("RankingPanel") as GameObject;
        GameObject a_RkPanel = Instantiate(m_RankingPanelObj);
        a_RkPanel.transform.SetParent(Canvas_Upgrade.transform, false);
	}

    void UpgradeFunc()
	{
        if (m_UpgradePanelObj != null)
            m_UpgradePanelObj = Resources.Load("UpgradePanel") as GameObject;
        GameObject a_UGPanel = Instantiate(m_UpgradePanelObj);
        a_UGPanel.transform.SetParent(Canvas_Upgrade.transform, false);
    }

    void CalcItemCost()
	{
        int a_TotalGold = 0;
        int a_TotalGem = 0;
        for (int ii = 0; ii < 3; ii++)
		{
            //골드 소모 아이템
            if (m_isChecked[ii]) a_TotalGold += m_ItemCost[ii];
		}
        //다이아 소모 아이템
        if (m_isChecked[3]) a_TotalGem = m_ItemCost[3];

        m_GoldValue = GlobalValue.g_UserGold - a_TotalGold;
        m_GemValue = GlobalValue.g_UserGem - a_TotalGem;
        m_UserGoldTxt.text = m_GoldValue.ToString();
        m_UserGemTxt.text = m_GemValue.ToString();

        for (int ii = 0; ii < 4; ii++)
            m_CheckImg[ii].gameObject.SetActive(m_isChecked[ii]);

    }

    void SetItemInfo(GameKind a_GKind)
	{
        if (a_GKind == GameKind.APF_YSMS)
            m_ItemInfo = new string[4] { "[추가시간]타이머 5초 추가!", "[변신]일정 콤보마다 한 종류로 통일!",
            "[스피드]틀렸을 때 더 빨리 회복돼요~", "[슈퍼피버시작]100콤보부터 시작하는 슈퍼피버~" };
        else if(a_GKind == GameKind.APF_SDJR)
            m_ItemInfo = new string[4] { "[지우개]모든 블록을 전부 다 지워줘요~!", "[뿅망치]누르는 모든 블록을 뿅뿅 없애줘요~",
            "[한줄뿅]블록 한줄을 몽땅 없애줘요~", "[슈퍼피버시작]100콤보부터 시작하는 슈퍼피버~" };
    }

    void ItemBtnFunc(int a_ItemNum)
	{
        a_ItemNum--;
        m_isChecked[a_ItemNum] = !m_isChecked[a_ItemNum];

        if (m_isChecked[a_ItemNum])
            m_ItemInfoText.text = m_ItemInfo[a_ItemNum];
        else
            m_ItemInfoText.text = "아이템을 선택해서 플레이 할 수 있어요~";
    }

    void CheckGameStart()
	{
        if (m_GoldValue < 0 || m_GemValue < 0)
            MessageBoxFunc(MessageState.OK, "★ 재화 부족 알림 ★", "재화가 부족해요 ㅠ0ㅠ");
        else if (GlobalValue.g_RiceCount <= 0)
            MessageBoxFunc(MessageState.OK, "★ 밥 부족 알림 ★", "밥이 부족해요 ㅠ0ㅠ");
        else
		{
            GlobalValue.g_RiceCount--;

            if (GlobalValue.g_RiceCount == 4)
            {
                //밥을 쓴 순간의 시간을 체크
                m_CheckTime = DateTime.Now;
                //시간을 초로 변환
                GlobalValue.g_RiceCheckTime = CalcDatetoSec(m_CheckTime);
                //날짜가 변경될 수 있으므로 날짜도 함께 저장
                GlobalValue.g_RiceCheckDate = CalcYMDtoNum(m_CheckTime);
                GlobalValue.g_IsRiceTimerStart = 1;    //타이머 스타트
                isRiceTimerStart = true;
                CheckRiceCount();
                Debug.Log(GlobalValue.g_RiceCheckDate + " : " + GlobalValue.g_RiceCheckTime);
            }
            
            NetworkMgr.Inst.PushPacket(PacketType.UserRice);

            m_RiceText.text = GlobalValue.g_RiceCount.ToString();
            GlobalValue.g_UserGem = m_GemValue;
            GlobalValue.g_UserGold = m_GoldValue;

            NetworkMgr.Inst.PushPacket(PacketType.UserMoney);

            if (!isTutSkipOnOff)
                SceneManager.LoadScene("TutorialScene");
            else
                SceneManager.LoadScene(m_GameName + "InGame");
        }
	}

    void CheckRiceCount()
	{
        for (int ii = 0; ii < 5; ii++)
            m_FillArr[ii] = 0;

        if (GlobalValue.g_RiceCount < 5) 
            for (int ii = 0; ii < 5; ii++)
                m_FillArr[ii] = GlobalValue.g_RiceCheckTime + m_RiceFillTimeSec * (ii + 1);
    }

    void CheckGameName()
	{
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
		{
            if (GlobalValue.g_YSMSTutSkipYN == 1) isTutSkipOnOff = true;
            m_GameName = "YSMS";
            m_GameNameText.text = "삼촌의 니편내편";
            m_ItemCost = new int[4] { 700, 900, 1100, 1 };
        }
        else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
		{
            if (GlobalValue.g_SDJRTutSkipYN == 1) isTutSkipOnOff = true;
            m_GameName = "SDJR";
            m_GameNameText.text = "엄마의 삼단정리";
            m_ItemCost = new int[4] { 1100, 900, 800, 1 };
        }
        SetItemInfo(GlobalValue.g_GKind);
    }

    void UpgradeUpdate()
	{
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
		{
            GlobalValue.YSMSUGAm();
            UGAmUpdate(GlobalValue.g_YSMSUGLv[0], GlobalValue.g_YSMSUGLv[1], GlobalValue.g_YSMSUGLv[2]);
            UGLvUpdate(GlobalValue.g_YSMSUGLv[0], GlobalValue.g_YSMSUGLv[1], GlobalValue.g_YSMSUGLv[2]);
        }
        else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
		{
            GlobalValue.SDJRUGAm();
            UGAmUpdate(GlobalValue.g_SDJRUGLv[0], GlobalValue.g_SDJRUGLv[1], GlobalValue.g_SDJRUGLv[2]);
            UGLvUpdate(GlobalValue.g_SDJRUGLv[0], GlobalValue.g_SDJRUGLv[1], GlobalValue.g_SDJRUGLv[2]);
		}
	}

    void UGLvUpdate(int a_BonusLv, int a_FeverLv, int a_SuperLv)
	{
        //레벨 UI 업데이트
        if (a_BonusLv == 15) m_BonusLvText.text = "Lv Max";
        else m_BonusLvText.text = "Lv " + a_BonusLv.ToString();

        if (a_FeverLv == 15) m_FeverLvText.text = "Lv Max";
        else m_FeverLvText.text = "Lv " + a_FeverLv.ToString();

        if (a_SuperLv == 15) m_SuperLvText.text = "Lv Max";
        else m_SuperLvText.text = "Lv " + a_SuperLv.ToString();
    }

    void UGAmUpdate(int a_BonusLv, int a_FeverLv, int a_SuperLv)
    {
        if (a_BonusLv == 0)
        {
            m_BonusAmText.gameObject.SetActive(false);
            m_UpBonusBtn.image.sprite = m_ButtonSprite[0];
        }
        else
        {
            m_UpBonusBtn.image.sprite = m_ButtonSprite[1];
            m_BonusAmText.text = "+" + GlobalValue.BonusAmount[a_BonusLv].ToString();
            m_BonusAmText.gameObject.SetActive(true);
        }

        if (a_FeverLv == 0)
            m_UpFeverBtn.image.sprite = m_ButtonSprite[0];
        else
            m_UpFeverBtn.image.sprite = m_ButtonSprite[1];
        m_FeverAmText.text = GlobalValue.FeverAmount[a_FeverLv].ToString("F1");
        m_FeverAmText.gameObject.SetActive(true);

        if (a_SuperLv == 0)
        {
            m_SuperAmText.gameObject.SetActive(false);
            m_UpSuperBtn.image.sprite = m_ButtonSprite[0];
        }
        else
        {
            m_UpSuperBtn.image.sprite = m_ButtonSprite[1];
            m_SuperAmText.text = GlobalValue.SuperAmount[a_SuperLv].ToString("F1") + "초";
            m_SuperAmText.gameObject.SetActive(true);
        }
    }

    void MessageBoxFunc(MessageState a_MessState, string a_MessLabel, string a_Mess)
	{
        if (m_MessageBoxObj != null)
            m_MessageBoxObj = Resources.Load("MessageBox") as GameObject;
        
        GameObject a_MsgBoxObj = Instantiate(m_MessageBoxObj);
        a_MsgBoxObj.transform.SetParent(Canvas_Upgrade.transform, false);
        
        MessageBox a_MsgBox = a_MsgBoxObj.GetComponent<MessageBox>();
        //a_MsgBox.m_MessState = a_MessState;
        if (a_MsgBox != null)
            a_MsgBox.ShowMessage(a_MessLabel, a_Mess, a_MessState);
    }


    void GetMyRanking(GameKind a_GKind)
    {
        if (GlobalValue.g_Unique_ID == "") return;
        if (m_GameName == "") return;

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = m_GameName + "BestScore",
            MaxResultsCount = 1,
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            request,
            (result) =>
            {
                if (0 < result.Leaderboard.Count)
                {
                    var curBoard = result.Leaderboard[0];

                    if (a_GKind == GameKind.APF_YSMS)
					{
                        GlobalValue.g_MyYSMSRank = curBoard.Position + 1;
                        GlobalValue.g_YSMSBestScore = curBoard.StatValue;
                        m_MyRank = GlobalValue.g_MyYSMSRank;
                    }
                    else if (a_GKind == GameKind.APF_SDJR)
					{
                        GlobalValue.g_MySDJRRank = curBoard.Position + 1;
                        GlobalValue.g_SDJRBestScore = curBoard.StatValue;
                        m_MyRank = GlobalValue.g_MySDJRRank;
                    }
                }

                if (m_MyRank != 0 && m_RankBtnTxt!= null)
                    m_RankBtnTxt.text = m_MyRank + "위\n" + "<color=#bb3e01>순위보기</color>";
            },

            (error) =>
            {
                Debug.Log("등수 가져오기 실패" + error.GenerateErrorReport());
            }

            );
    }

    #region 밥 시간 계산

    int CalcYMDtoNum(DateTime a_Date, int a_TempYMD = 0)
	{
        int a_Num = (a_Date.Year % 100) * 10000 + a_Date.Month * 100 + a_Date.Day;
        if (a_TempYMD != a_Num && a_TempYMD != 0)
            isDayChange = true;
        return a_Num;
	}

    int CalcDatetoSec(DateTime a_Date)
    {
        int a_Sec = a_Date.Hour * 3600 + a_Date.Minute * 60 + a_Date.Second;
        //밥 충전 완료 시간이 자정을 넘어간다면
        if (isDayChange)
		{
            a_Sec += (CalcYMDtoNum(a_Date) - GlobalValue.g_RiceCheckDate) * 86400;
            isDayChange = false;
		}

        return a_Sec;
    }

    void CalcTimeFunc()
    {
        if (!isRiceTimerStart)
		{
            m_RiceText.text = GlobalValue.g_RiceCount.ToString();
            return;
		}

        m_CurYMD = CalcYMDtoNum(DateTime.Now, m_TempYMD);
        m_CurTimeSec = CalcDatetoSec(DateTime.Now);

        //시간이 충분히 지났다면 밥 충전
        if (m_FillArr[0] <= m_CurTimeSec)
        {
            for (int ii = 0; ii < 4; ii++)
            {
                if (m_FillArr[ii] <= m_CurTimeSec && m_CurTimeSec < m_FillArr[ii + 1])
                {
                    GlobalValue.g_RiceCount += ii + 1;
                    GlobalValue.g_RiceCheckTime = m_FillArr[ii];
                }
            }
            if (m_FillArr[4] <= m_CurTimeSec)
                GlobalValue.g_RiceCount += 5;

            if (GlobalValue.g_RiceCount >= 5)
            {
                GlobalValue.g_RiceCount = 5;
                GlobalValue.g_IsRiceTimerStart = 0;
                GlobalValue.g_RiceCheckTime = -1;
                GlobalValue.g_RiceCheckDate = 0;
                isRiceTimerStart = false;
            }
            CheckRiceCount();
            NetworkMgr.Inst.PushPacket(PacketType.UserRice);
        }

        if (GlobalValue.g_RiceCount == 0)
        {
            m_TimerMin = (m_FillArr[0] - m_CurTimeSec) / 60;
            m_TimerSec = (m_FillArr[0] - m_CurTimeSec) % 60;
            m_RiceText.text = m_TimerMin.ToString() + ":" + m_TimerSec.ToString("D2");
        }
        else
            m_RiceText.text = GlobalValue.g_RiceCount.ToString();

        m_TempYMD = m_CurYMD;
    }

    #endregion 밥 시간 계산

}
