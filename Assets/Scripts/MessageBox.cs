using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum MessageState
{
    OK,
    YesNo,
    MessageKind
}

public class MessageBox : MonoBehaviour
{
    public Button m_OKBtn = null;
    public Button m_YesBtn = null;
    public Button m_NoBtn = null;
    public Text m_MessLabelTxt = null;
    public Text m_MessTxt = null;
    public MessageState m_MessState = MessageState.MessageKind;

    // Start is called before the first frame update
    void Start()
    {
        if (m_OKBtn != null)
            m_OKBtn.onClick.AddListener(() =>
            {
                Destroy(this.gameObject);
            });

        if (m_NoBtn != null)
            m_NoBtn.onClick.AddListener(NoBtnFunc);

        if (m_YesBtn != null)
            m_YesBtn.onClick.AddListener(YesBtnFunc);
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    
    //}

    public void ShowMessage(string a_MessLabel, string a_Mess, MessageState a_MessState)
	{
        if(a_MessState == MessageState.OK)
		{
            m_OKBtn.gameObject.SetActive(true);
            m_YesBtn.gameObject.SetActive(false);
            m_NoBtn.gameObject.SetActive(false);
		}
        else if(a_MessState == MessageState.YesNo)
		{
            m_OKBtn.gameObject.SetActive(false);
            m_YesBtn.gameObject.SetActive(true);
            m_NoBtn.gameObject.SetActive(true);
        }
        m_MessLabelTxt.text = a_MessLabel;
        m_MessTxt.text = a_Mess;
	}

    void NoBtnFunc()
	{
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
		{
            if (YSMSIngameMgr.Inst.m_PPStr != "")
                YSMSIngameMgr.Inst.m_PPStr = "";
		}
        else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
		{
			if (SDJRIngameMgr.Inst.m_PPStr != "")
                SDJRIngameMgr.Inst.m_PPStr = "";
		}
        Destroy(this.gameObject);
	}

    void YesBtnFunc()
	{
        Time.timeScale = 1.0f;
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
		{
            YSMSIngameMgr.m_SpawnList.Clear();
            SceneManager.LoadScene(YSMSIngameMgr.Inst.m_PPStr + "Scene");
            YSMSIngameMgr.Inst.m_PPStr = "";
        }
        else if(GlobalValue.g_GKind == GameKind.APF_SDJR)
		{
            SceneManager.LoadScene(SDJRIngameMgr.Inst.m_PPStr + "Scene");
            SDJRIngameMgr.Inst.m_PPStr = "";
        }
        Destroy(this.gameObject);
	}
}
