using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public enum PacketType
{
    UserMoney = 0,
    UserRice,
    TotalBestScore,
    YSMSBestScore,
    YSMSUGLv,
    YSMSRegScore,
    SDJRBestScore,
    SDJRUGLv,
    SDJRRegScore,
    NickUpdate,
    SkipOnOff,
}

public class NetworkMgr : MonoBehaviour
{
    bool isNetworkLock = false;
    List<PacketType> m_PacketBuff = new List<PacketType>();

    //서버에 전송할 패킷 처리용 큐 변수
    [HideInInspector] public string m_TempStrBuff = "";

    public static NetworkMgr Inst = null;

	void Awake()
	{
        Inst = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNetworkLock)
		{
            if (0 < m_PacketBuff.Count)
                Req_Network();
        }
    }

    void Req_Network()
	{
        if (m_PacketBuff[0] == PacketType.UserMoney)
            UpdateUserMoneyCo();
        else if (m_PacketBuff[0] == PacketType.UserRice)
            UpdateUserRiceCo();
        else if (m_PacketBuff[0] == PacketType.YSMSBestScore)
            UpdateYSMSScoreCo();
        else if (m_PacketBuff[0] == PacketType.SDJRBestScore)
            UpdateSDJRScoreCo();
        else if (m_PacketBuff[0] == PacketType.TotalBestScore)
            UpdateTotalScoreCo();
        else if (m_PacketBuff[0] == PacketType.YSMSUGLv)
            UpdateYSMSUGLvCo();
        else if (m_PacketBuff[0] == PacketType.SDJRUGLv)
            UpdateSDJRUGLvCo();
        else if (m_PacketBuff[0] == PacketType.YSMSRegScore)
            UpdateYSMSRegScoreCo();
        else if (m_PacketBuff[0] == PacketType.SDJRRegScore)
		{
			UpdateSDJRRegScoreCo();
		}
		else if (m_PacketBuff[0] == PacketType.NickUpdate)
			UpdateNickCo(m_TempStrBuff);
		else if (m_PacketBuff[0] == PacketType.SkipOnOff)
			UpdateSkipCo();

        m_PacketBuff.RemoveAt(0);
	}


    void UpdateUserMoneyCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"UserGold", GlobalValue.g_UserGold.ToString() },
                {"UserGem", GlobalValue.g_UserGem.ToString() }
			}
		};
        isNetworkLock = true;
        PlayFabClientAPI.UpdateUserData(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
	}

    void UpdateUserRiceCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"UserRice", GlobalValue.g_RiceCount.ToString() },
                {"IsRiceTimer", GlobalValue.g_IsRiceTimerStart.ToString() },
                {"RiceCheckTime", GlobalValue.g_RiceCheckTime.ToString() },
                {"RiceCheckDate", GlobalValue.g_RiceCheckDate.ToString() },
            }
        };
        isNetworkLock = true;
        PlayFabClientAPI.UpdateUserData(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    void UpdateYSMSScoreCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdatePlayerStatisticsRequest()
		{
            Statistics = new List<StatisticUpdate>()
			{
                new StatisticUpdate
				{
                    StatisticName = "YSMSBestScore",
                    Value = GlobalValue.g_YSMSBestScore,
				},
			}
		};
        isNetworkLock = true;
        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
	}

    void UpdateSDJRScoreCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate
                {
                    StatisticName = "SDJRBestScore",
                    Value = GlobalValue.g_SDJRBestScore,
                },
            }
        };
        isNetworkLock = true;
        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    void UpdateYSMSRegScoreCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate
                {
                    StatisticName = "YSMSRegScore",
                    Value = GlobalValue.g_YSMSRegScore,
                },
            }
        };
        isNetworkLock = true;
        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    void UpdateSDJRRegScoreCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>()
            {
                new StatisticUpdate
                {
                    StatisticName = "SDJRRegScore",
                    Value = GlobalValue.g_SDJRRegScore,
                },
            }
        };
        isNetworkLock = true;
        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            (result) =>
            {
                isNetworkLock = false;
                Debug.Log("저장 성공, 스코어 : " + GlobalValue.g_SDJRRegScore);
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    void UpdateTotalScoreCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdatePlayerStatisticsRequest()
		{
            Statistics = new List<StatisticUpdate>()
			{
                new StatisticUpdate
				{
                    StatisticName = "TotalScore",
                    Value = GlobalValue.g_TotalScore,
				},
			}
		};
        isNetworkLock = true;
        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
			(error) =>
			{
                isNetworkLock = false;
			}
            );
	}

    void UpdateYSMSUGLvCo()
    {
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"YSMSBonusUGLv", GlobalValue.g_YSMSUGLv[0].ToString() },
                {"YSMSFeverUGLv", GlobalValue.g_YSMSUGLv[1].ToString() },
                {"YSMSSuperUGLv", GlobalValue.g_YSMSUGLv[2].ToString() }
            }
        };
        isNetworkLock = true;
        PlayFabClientAPI.UpdateUserData(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    void UpdateSDJRUGLvCo()
    {
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"SDJRBonusUGLv", GlobalValue.g_SDJRUGLv[0].ToString() },
                {"SDJRFeverUGLv", GlobalValue.g_SDJRUGLv[1].ToString() },
                {"SDJRSuperUGLv", GlobalValue.g_SDJRUGLv[2].ToString() }
            }
        };
        isNetworkLock = true;
        PlayFabClientAPI.UpdateUserData(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    void UpdateNickCo(string a_NickName)
	{
        if (GlobalValue.g_Unique_ID == "") return;
        if (a_NickName == "") return;

        isNetworkLock = true;
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = a_NickName
            },
			(result) =>
			{
                GlobalValue.g_Nickname = result.DisplayName;
                isNetworkLock = false;
			},
			(error) =>
			{
                Debug.Log(error.GenerateErrorReport());
                isNetworkLock = false;
			}
            );
	}

    void UpdateSkipCo()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"YSMSTutSkipOnOff", GlobalValue.g_YSMSTutSkipYN.ToString() },
                {"SDJRTutSkipOnOff", GlobalValue.g_SDJRTutSkipYN.ToString() },
            }
        };
        isNetworkLock = true;
        PlayFabClientAPI.UpdateUserData(
            request,
            (result) =>
            {
                isNetworkLock = false;
            },
            (error) =>
            {
                isNetworkLock = false;
            }
            );
    }

    public void PushPacket(PacketType a_PType)
	{
        bool a_isExist = false;
        for (int ii = 0; ii < m_PacketBuff.Count; ii++)
		{
            if (m_PacketBuff[ii] == a_PType)
                a_isExist = true;
		}

        if (!a_isExist)
            m_PacketBuff.Add(a_PType);
	}
}
