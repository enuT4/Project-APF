using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;

public class GameOverMgr : MonoBehaviour
{
    public Text m_UserExp_Txt = null;
    public Text m_UserGold_Txt = null;
    public Text m_UserGem_Txt = null;
    public Text m_BestScore_Txt = null;
    public Text m_ResultScore_Txt = null;
    public Text m_ScBonus_Txt = null;
    public Text m_RewardGold_Txt = null;
    public Text m_RewardExp_Txt = null;
    public Text m_CurTotalScore_Txt = null;
    public Text m_InSchComp_Txt = null;
    public Text m_School_Txt = null;
    public Text m_RegionComp_Txt = null;

    public Button m_SendFriendBtn = null;
    public Button m_OKBtn = null;

    public Image m_NewRecordImg = null;
    public Image m_UpImg = null;

    public GameObject m_MessageBoxObj;
    public GameObject m_CanvasObj;
    public bool isMsgBox = false;

    bool isSendFriend = false;
    int m_ResultScore = 0;
    int m_LevelBonus = 0;
    int m_TimeBonus = 0;
    int m_RwGold = 0;
    int m_RwExp = 200;
    int m_MyRank = 0;

    float m_DelayTimer = 1.5f;
    float m_DelayTimer2 = 0.0f;

    //점수 오름 연출을 위한 현재값 저장
    int m_CurScore = 0;
    int m_CurGold = 0;
    int m_ScoreDiff = 0;
    int m_CurTotalScore = 0;
    int m_CurBestScore = 0;
    int m_RegScore = 0;

    //점수 오름 연출 관련 변수
    float m_GoldUpSpeed = 0.0f;
    float m_CurScoreUpSpeed = 0.0f;
    float m_DiffUpSpeed = 0.0f;

    string m_GameName = "";
    bool[] isGetReg = new bool[2];

    // Start is called before the first frame update
    void Start()
    {
        CheckGameName();

        if (m_SendFriendBtn != null)
            m_SendFriendBtn.onClick.AddListener(SendFriendFunc);

        if (m_OKBtn != null)
            m_OKBtn.onClick.AddListener(() =>
            {
                isSendFriend = false;
				SceneManager.LoadScene("LobbyScene");
			});

        m_SendFriendBtn.gameObject.SetActive(false);
        m_OKBtn.gameObject.SetActive(false);
        m_UpImg.gameObject.SetActive(false);
        m_NewRecordImg.gameObject.SetActive(false);

        m_UserExp_Txt.text = GlobalValue.g_ExpPercent.ToString() + "%";
        m_UserGold_Txt.text = m_CurGold.ToString("N0");
        m_UserGem_Txt.text = GlobalValue.g_UserGem.ToString();

        m_ResultScore_Txt.text = m_CurScore.ToString("N0");

        m_ScBonus_Txt.text = "<color=#00ffff>레벨 보너스 : " + m_LevelBonus + "%</color>\n" +
            "<color=#ff00ff>시간 보너스 : " + m_TimeBonus + "%</color>";
        m_RewardExp_Txt.text = m_RwExp.ToString();
        m_CurTotalScore_Txt.text = m_CurTotalScore.ToString("N0");
        m_School_Txt.text = GlobalValue.g_SchoolName;

        isSendFriend = false;
    }

	// Update is called once per frame
	void Update()
    {
        ShowScore();
        if (0.0f < m_DelayTimer)
		{
            m_DelayTimer -= Time.deltaTime;
            if (m_DelayTimer <= 0.0f)
                m_DelayTimer2 = 1.5f;
        }
        else
		{
            ShowRegionScore();
            if (0.0f < m_DelayTimer2)
            {
                m_DelayTimer2 -= Time.deltaTime;
                if (m_DelayTimer2 <= 0.0f)
                {
                    m_SendFriendBtn.gameObject.SetActive(true);
                    m_OKBtn.gameObject.SetActive(true);
                }
            }
        }

        if (isGetReg[0] && isGetReg[1])
		{
            if (m_GameName == "YSMS")
			{
                GlobalValue.g_YSMSRegScore = m_RegScore + m_ScoreDiff;
                NetworkMgr.Inst.PushPacket(PacketType.YSMSRegScore);
            }
			else if (m_GameName == "SDJR")
			{
                GlobalValue.g_SDJRRegScore = m_RegScore + m_ScoreDiff;
				NetworkMgr.Inst.PushPacket(PacketType.SDJRRegScore);
			}
            isGetReg[1] = false;
        }
    }

    void CheckGameName()
	{
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
        {
            m_GameName = "YSMS";
            m_LevelBonus = YSMSIngameMgr.Inst.m_Level * 5;
            m_TimeBonus = YSMSIngameMgr.Inst.m_BombCount * 3;
            m_InSchComp_Txt.text = "<color=#ffff00>-위</color>\n" + 
                GlobalValue.g_YSMSBestScore.ToString();
            m_RegionComp_Txt.text = "<color=#ffff00>-위</color>   " +
                GlobalValue.g_YSMSRegScore.ToString("N0");

            //보너스를 포함한 최종 스코어 계산
            if (int.MaxValue - 10 <= (YSMSIngameMgr.Inst.m_CurScore *
                (1 + (float)((m_LevelBonus + m_TimeBonus) / 100.0f))))
                m_ResultScore = int.MaxValue - 10;
            else
                m_ResultScore = (int)(YSMSIngameMgr.Inst.m_CurScore *
                    (1 + (float)((m_LevelBonus + m_TimeBonus) / 100.0f)));

            //최종 골드 계산
            m_RwGold = (int)(500 * (1 + (float)((m_LevelBonus + m_TimeBonus) / 100.0f)));
            m_RewardGold_Txt.text = m_RwGold.ToString();

            //기록 경신
            if (m_ResultScore <= GlobalValue.g_YSMSBestScore)
                m_BestScore_Txt.text = GlobalValue.g_YSMSBestScore.ToString("N0");
            else
                m_BestScore_Txt.text = m_ResultScore.ToString("N0");

            m_CurBestScore = GlobalValue.g_YSMSBestScore;
            m_MyRank = GlobalValue.g_MyYSMSRank;
        }
        else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
        {
            m_GameName = "SDJR";
            m_LevelBonus = SDJRIngameMgr.Inst.m_Level * 4;
            m_TimeBonus = SDJRIngameMgr.Inst.m_BombCount * 3;
            m_InSchComp_Txt.text = "<color=#ffff00>-위</color>\n" + 
                GlobalValue.g_SDJRBestScore.ToString();
            m_RegionComp_Txt.text = "<color=#ffff00>-위</color>   " +
                GlobalValue.g_SDJRRegScore.ToString("N0");

            //보너스를 포함한 현재 스코어 계산
            if (int.MaxValue - 10 <= (SDJRIngameMgr.Inst.m_CurScore *
                (1 + (float)((m_LevelBonus + m_TimeBonus) / 100.0f))))
                m_ResultScore = int.MaxValue - 10;
            else
                m_ResultScore = (int)(SDJRIngameMgr.Inst.m_CurScore *
                    (1 + (float)((m_LevelBonus + m_TimeBonus) / 100.0f)));

            //최종 골드 계산
            m_RwGold = (int)(500 * (1 + (float)(3 * ((m_LevelBonus - 1) + m_TimeBonus) / 100.0f)));
            m_RewardGold_Txt.text = m_RwGold.ToString();

            //기록 경신
            if (m_ResultScore <= GlobalValue.g_SDJRBestScore)
                m_BestScore_Txt.text = GlobalValue.g_SDJRBestScore.ToString("N0");
            else
                m_BestScore_Txt.text = m_ResultScore.ToString("N0");

            m_CurBestScore = GlobalValue.g_SDJRBestScore;
            m_MyRank = GlobalValue.g_MySDJRRank;
        }

        //골드 값 저장
        m_CurGold = GlobalValue.g_UserGold;
        GlobalValue.g_UserGold += m_RwGold; 
        NetworkMgr.Inst.PushPacket(PacketType.UserMoney); 
        m_GoldUpSpeed = (float)m_RwGold / 1.5f;

        //현재 점수 값 표시
        m_CurScore = 0;
        m_CurScoreUpSpeed = (float)(m_ResultScore) / 1.5f;

        //기록 경신 여부 
        m_ScoreDiff = m_ResultScore - m_CurBestScore;
        m_CurTotalScore = GlobalValue.g_TotalScore;

        //지역 점수 저장
        GetRegionScore(m_GameName);

        if (m_ScoreDiff > 0)
        {
            if (m_GameName == "YSMS")
			{
                GlobalValue.g_YSMSBestScore += m_ScoreDiff;
                NetworkMgr.Inst.PushPacket(PacketType.YSMSBestScore);
			}
            else if(m_GameName == "SDJR")
			{
                GlobalValue.g_SDJRBestScore += m_ScoreDiff;
                NetworkMgr.Inst.PushPacket(PacketType.SDJRBestScore);
            }
            m_MyRank = 0;
            GlobalValue.g_TotalScore += m_ScoreDiff;
            NetworkMgr.Inst.PushPacket(PacketType.TotalBestScore);
            m_DiffUpSpeed = m_ScoreDiff / 1.5f;
            isGetReg[1] = true;
        }
    }

    void ShowScore()
	{
        //골드 올라가는 연출
        if (m_CurGold < GlobalValue.g_UserGold)
		{
            m_CurGold += (int)(Time.deltaTime * m_GoldUpSpeed);
            if (GlobalValue.g_UserGold <= m_CurGold)
                m_CurGold = GlobalValue.g_UserGold;
            m_UserGold_Txt.text = m_CurGold.ToString("N0");
        }

        //현재 점수 올라가는 연출
        m_CurScore += (int)(Time.deltaTime * m_CurScoreUpSpeed);
        if (m_ResultScore <= m_CurScore)
            m_CurScore = m_ResultScore;
        m_ResultScore_Txt.text = m_CurScore.ToString("N0");

        if (m_ScoreDiff > 0) //기록 경신
		{
            m_UpImg.gameObject.SetActive(true);

            if (m_CurTotalScore < GlobalValue.g_TotalScore)
			{
                m_CurTotalScore += (int)(Time.deltaTime * m_DiffUpSpeed);     //추가된 점수 만큼 올라가기
                if (GlobalValue.g_TotalScore <= m_CurTotalScore)
				{
                    m_CurTotalScore = GlobalValue.g_TotalScore;
                    m_NewRecordImg.gameObject.SetActive(true);
                }
            }

            if (m_GameName == "YSMS")
            {
                if (m_CurBestScore < GlobalValue.g_YSMSBestScore)
                {
                    m_CurBestScore += (int)(Time.deltaTime * m_DiffUpSpeed);
                    if (GlobalValue.g_YSMSBestScore <= m_CurBestScore)
                        m_CurBestScore = GlobalValue.g_YSMSBestScore;
                }
            }
            else if (m_GameName == "SDJR") 
			{
                if (m_CurBestScore < GlobalValue.g_SDJRBestScore)
                {
                    m_CurBestScore += (int)(Time.deltaTime * m_DiffUpSpeed);
                    if (GlobalValue.g_SDJRBestScore <= m_CurBestScore)
                        m_CurBestScore = GlobalValue.g_SDJRBestScore;
                }
            }
		}
        m_CurTotalScore_Txt.text = m_CurTotalScore.ToString("N0");
        if (m_MyRank == 0)
		{
            m_InSchComp_Txt.text = "<color=#ffff00>-위</color>\n" +
                m_CurBestScore.ToString("N0");
            RefreshMyRank(m_GameName);
        }
        else
		{
            m_InSchComp_Txt.text = "<color=#ffff00>" + m_MyRank + "위</color>\n" +
                m_CurBestScore.ToString("N0");
        }
        
    }

    void ShowRegionScore()
	{
        if (!isGetReg[0]) return;

        if (m_ScoreDiff > 0)
		{
            if (m_GameName == "YSMS")
            {
                m_RegScore += (int)(Time.deltaTime * m_DiffUpSpeed);
                if (GlobalValue.g_YSMSRegScore <= m_RegScore)
                {
                    m_RegScore = GlobalValue.g_YSMSRegScore;
                    isGetReg[0] = false;
                }
            }
            else if (m_GameName == "SDJR")
            {
                m_RegScore += (int)(Time.deltaTime * m_DiffUpSpeed);
                if (GlobalValue.g_SDJRRegScore <= m_RegScore)
                {
                    m_RegScore = GlobalValue.g_SDJRRegScore;
                    isGetReg[0] = false;
                }
			}
        }
        else
            m_SendFriendBtn.interactable = false;

        m_RegionComp_Txt.text = "<color=#ffff00>" + GlobalValue.g_YSMSRegionRank +
            "위</color>   " + m_RegScore.ToString("N0");
    }

    void SendFriendFunc()
	{
        if (isSendFriend) 
            MessageBoxFunc("★ 중복 알림 ★", "이미 한 번 자랑하셨어요 ㅠ0ㅠ");
        else
		{
            MessageBoxFunc("★ 자랑하기 ★", "친구에게 자랑했어요 ^0^");
            isSendFriend = true;
        }
	}

    void MessageBoxFunc(string a_MessLabel, string a_Mess)
	{
        if (m_MessageBoxObj != null)
            m_MessageBoxObj = Resources.Load("MessageBox") as GameObject;

        GameObject a_MsgBoxObj = Instantiate(m_MessageBoxObj);
        a_MsgBoxObj.transform.SetParent(m_CanvasObj.transform, false);

        MessageBox a_MsgBox = a_MsgBoxObj.GetComponent<MessageBox>();
        a_MsgBox.m_MessState = MessageState.OK;
        if (a_MsgBox != null)
            a_MsgBox.ShowMessage(a_MessLabel, a_Mess, MessageState.OK);
    }

    void RefreshMyRank(string a_GameName)
	{
        if (GlobalValue.g_Unique_ID == "") return;
        if (a_GameName == "") return;

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = a_GameName + "BestScore",
            MaxResultsCount = 1,
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            request,
            (result) =>
            {
                if (0 < result.Leaderboard.Count)
                {
                    var curBoard = result.Leaderboard[0];
                    if (a_GameName == "YSMS") 
					{
                        GlobalValue.g_MyYSMSRank = curBoard.Position + 1;
                        m_MyRank = GlobalValue.g_MyYSMSRank;
                    }
                    else if (a_GameName == "SDJR")
					{
                        GlobalValue.g_MySDJRRank = curBoard.Position + 1;
                        m_MyRank = GlobalValue.g_MySDJRRank;
                    }
                }
            },
            (error) =>
            {
                Debug.Log("등수 가져오기 실패");
            }
            );
    }

    void GetRegionScore(string a_GameName)
	{
        if (GlobalValue.g_Unique_ID == "") return;
        if (a_GameName == "") return;

        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0,
            StatisticName = a_GameName + "RegScore",
            MaxResultsCount = 20,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            (result) =>
            {
                m_RegScore = result.Leaderboard[0].StatValue;
                isGetReg[0] = true;
            },
            (error) =>
            {
                Debug.Log("리더보드 불러오기 실패");
            }
            );
    }
}
