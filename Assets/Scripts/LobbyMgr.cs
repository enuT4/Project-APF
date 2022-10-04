using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LobbyMgr : MonoBehaviour
{
    public Button m_BackBtn = null;

    public Text m_TotalScore_Txt = null;
    public Button YSMS_GameBtn = null;
    public Text YSMS_BSTxt = null;

    public Button SDJR_GameBtn = null;
    public Text SDJR_BSTxt = null;

    // Start is called before the first frame update
    void Start()
	{
        CheckGM();

        m_TotalScore_Txt.text = GlobalValue.g_TotalScore.ToString("N0");
		YSMS_BSTxt.text = GlobalValue.g_YSMSBestScore.ToString("N0");
        SDJR_BSTxt.text = GlobalValue.g_SDJRBestScore.ToString("N0");

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("TitleScene");
            });

        if (YSMS_GameBtn != null)
            YSMS_GameBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_GKind = GameKind.APF_YSMS;
                SceneManager.LoadScene("YSMSReadyScene");
            });

        if (SDJR_GameBtn != null)
            SDJR_GameBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_GKind = GameKind.APF_SDJR;
                SceneManager.LoadScene("SDJRReadyScene");
            });

        if (TitleMgr.m_isFBLoggedIn)
		{
            if (string.IsNullOrEmpty(GlobalValue.g_Nickname))
            {
                GlobalValue.g_Nickname = GlobalValue.g_FacebookName;
                NetworkMgr a_NetMgr = GameObject.FindObjectOfType<NetworkMgr>();
                if (a_NetMgr != null)
                {
                    a_NetMgr.m_TempStrBuff = GlobalValue.g_Nickname;
                    a_NetMgr.PushPacket(PacketType.NickUpdate);
                }
            }
            TitleMgr.m_isFBLoggedIn = false;
        }

	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
		{
            GlobalValue.g_SDJRBestScore = 1000;
            GlobalValue.g_YSMSBestScore = 1000;
            GlobalValue.g_TotalScore = 2000;
            NetworkMgr.Inst.PushPacket(PacketType.SDJRBestScore);
            NetworkMgr.Inst.PushPacket(PacketType.YSMSBestScore);
            NetworkMgr.Inst.PushPacket(PacketType.TotalBestScore);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GlobalValue.g_SDJRRegScore = 1000;
            GlobalValue.g_YSMSRegScore = 1000;
            NetworkMgr.Inst.PushPacket(PacketType.SDJRRegScore);
            NetworkMgr.Inst.PushPacket(PacketType.YSMSRegScore);
        }
    }

    void CheckGM()
	{
        if (GlobalValue.g_Nickname == "운영자" || GlobalValue.g_Nickname == "운영자1")
		{
            if (GlobalValue.g_GMGEM > 0)
                GlobalValue.g_UserGem = GlobalValue.g_GMGEM;
            if (GlobalValue.g_GMGOLD > 0)
                GlobalValue.g_UserGold = GlobalValue.g_GMGOLD;
            if (GlobalValue.g_GMRICE > 0)
                GlobalValue.g_RiceCount = GlobalValue.g_GMRICE;
        }
        
        if (GlobalValue.g_isFirstLogin)
		{
            GlobalValue.g_UserGem = 10;
            GlobalValue.g_UserGold = 20000;
            GlobalValue.g_isFirstLogin = false;
		}
            
        
	}
}
