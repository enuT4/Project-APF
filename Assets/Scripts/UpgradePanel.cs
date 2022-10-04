using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UGKind
{
    UGBonus,
    UGFever,
    UGSuper,
    UpgradeCount
}

public class UpgradePanel : MonoBehaviour
{
    public Image m_UGIconImg = null;
    public Text m_UGLevelTxt = null;
    public Image m_UGGuageBar = null;
    public Text m_UGAmountText = null;
    public Text m_UGInfoText = null;
    public Button m_UGLvUpBtn = null;
    public Image m_LvUpCostImg = null;
    public Text m_LVUpCostText = null;
    public Text m_MaxLvText = null;
    public Button m_PanelCloseBtn = null;

    public Sprite[] m_SpriteGroup;
    public Text[] m_UGAmGroup;

    public static UGKind m_UGKind = UGKind.UpgradeCount;
    int UGCost = 0;

    float m_UpgradeDelay = 0.0f;

    public GameObject m_MessageBoxObj = null;
    public GameObject m_LvUpImgObj = null;
    public GameObject Canvas_Message = null;


    // Start is called before the first frame update
    void Start()
    {
        UpdateUGKind();
        if (m_UGLvUpBtn != null)
            m_UGLvUpBtn.onClick.AddListener(ItemUpgrade);

        if (m_PanelCloseBtn != null)
            m_PanelCloseBtn.onClick.AddListener(() =>
            {
                m_UGKind = UGKind.UpgradeCount;
                Destroy(this.gameObject);
            });
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_UpgradeDelay)
        {
            m_UpgradeDelay -= Time.deltaTime;
            if (m_UpgradeDelay <= 0.0f) m_UpgradeDelay = 0.0f;
        }
    }


    public void ItemUpgrade()
	{
        if (GlobalValue.g_UserGold - UGCost < 0)
		{
            MessageBoxFunc(MessageState.OK, "★ 재화 부족 알림 ★", "재화가 부족해요 ㅠ0ㅠ");
            return;
		}
        if (0.0f < m_UpgradeDelay) return;
        
        //변화된 골드값 서버에 저장
        GlobalValue.g_UserGold -= UGCost;
        NetworkMgr.Inst.PushPacket(PacketType.UserMoney);

        //변화된 레벨값 UI에 적용 및 서버에 저장
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
		{
            if (m_UGKind == UGKind.UGBonus && GlobalValue.g_YSMSUGLv[0] < 15) 
                GlobalValue.g_YSMSUGLv[0]++;
            else if (m_UGKind == UGKind.UGFever && GlobalValue.g_YSMSUGLv[1] < 15)
                GlobalValue.g_YSMSUGLv[1]++;
            else if (m_UGKind == UGKind.UGSuper && GlobalValue.g_YSMSUGLv[2] < 15)
                GlobalValue.g_YSMSUGLv[2]++;

            NetworkMgr.Inst.PushPacket(PacketType.YSMSUGLv);
        }
        else if(GlobalValue.g_GKind == GameKind.APF_SDJR)
		{
            if (m_UGKind == UGKind.UGBonus && GlobalValue.g_SDJRUGLv[0] < 15)
                GlobalValue.g_SDJRUGLv[0]++;
            else if (m_UGKind == UGKind.UGFever && GlobalValue.g_SDJRUGLv[1] < 15)
                GlobalValue.g_SDJRUGLv[1]++;
            else if (m_UGKind == UGKind.UGSuper && GlobalValue.g_SDJRUGLv[2] < 15)
                GlobalValue.g_SDJRUGLv[2]++;

            NetworkMgr.Inst.PushPacket(PacketType.SDJRUGLv);
        }
        UpdateUGKind();

        //레벨업 이미지 보여주기
        if (m_LvUpImgObj != null)
            m_LvUpImgObj = Resources.Load("LevelUpImg") as GameObject;

        GameObject a_LvUpObj = Instantiate(m_LvUpImgObj);
        a_LvUpObj.transform.SetParent(Canvas_Message.transform, false);

        m_UpgradeDelay = 0.5f;
	}

    void UpdateUGKind()
	{
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
            UpdateYSMSUGKind();
        else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
            UpdateSDJRUGKind();
	}

    void UpdateYSMSUGKind()
	{
        GlobalValue.YSMSUGAm();

        if (m_UGKind == UGKind.UGBonus)
		{
            if (m_UGIconImg != null) 
                m_UGIconImg.sprite = m_SpriteGroup[0];

            m_UGAmGroup[0].gameObject.SetActive(true);
            m_UGAmGroup[1].gameObject.SetActive(false);
            m_UGAmGroup[2].gameObject.SetActive(false);

            if (GlobalValue.g_YSMSUGLv[0] == 0)
                m_UGAmGroup[0].gameObject.SetActive(false);
            else
                m_UGAmGroup[0].text = "+" + GlobalValue.BonusAmount[GlobalValue.g_YSMSUGLv[0]].ToString();
            m_UGLevelTxt.text = GlobalValue.g_YSMSUGLv[0].ToString();
            m_UGGuageBar.fillAmount = (float)GlobalValue.g_YSMSUGLv[0] / 15.0f;

            m_UGAmountText.text = "";
            if (GlobalValue.g_YSMSUGLv[0] == 15)
			{
                m_UGAmountText.text = "보너스 캐릭터가 첫번째에 등장해요~\n" + "보너스 점수 " + GlobalValue.BonusAmount[GlobalValue.g_YSMSUGLv[0]] + "점";
                m_UGLvUpBtn.interactable = false;
                m_LvUpCostImg.gameObject.SetActive(false);
                m_LVUpCostText.gameObject.SetActive(false);
                m_MaxLvText.gameObject.SetActive(true);
            }
            else
			{
                if (GlobalValue.g_YSMSUGLv[0] == 0)
                    m_UGAmountText.text = "보너스 블록이 등장해요~\n";
                else if (1 <= GlobalValue.g_YSMSUGLv[0] && GlobalValue.g_YSMSUGLv[0] <= 5)
                    m_UGAmountText.text = "보너스 캐릭터 네번째에 등장 > <color=#ff0000>네번째에 등장</color>\n";
                else if (GlobalValue.g_YSMSUGLv[0] == 6)
                    m_UGAmountText.text = "보너스 캐릭터 네번째에 등장 > <color=#ff0000>세번째에 등장</color>\n";
                else if (7 <= GlobalValue.g_YSMSUGLv[0] && GlobalValue.g_YSMSUGLv[0] <= 9)
                    m_UGAmountText.text = "보너스 캐릭터 세번째에 등장 > <color=#ff0000>세번째에 등장</color>\n";
                else if(GlobalValue.g_YSMSUGLv[0] == 10)
                    m_UGAmountText.text = "보너스 캐릭터 세번째에 등장 > <color=#ff0000>두번째에 등장</color>\n";
                else if(GlobalValue.g_YSMSUGLv[0] == 11 || GlobalValue.g_YSMSUGLv[0] == 12)
                    m_UGAmountText.text = "보너스 캐릭터 두번째에 등장 > <color=#ff0000>두번째에 등장</color>\n";
                else if(GlobalValue.g_YSMSUGLv[0] == 13)
                    m_UGAmountText.text = "보너스 캐릭터 두번째에 등장 > <color=#ff0000>첫번째에 등장</color>\n";
                else if(GlobalValue.g_YSMSUGLv[0] == 14)
                    m_UGAmountText.text = "보너스 캐릭터가 첫번째에 등장해요~\n";

                m_UGAmountText.text += "보너스 점수 " + GlobalValue.BonusAmount[GlobalValue.g_YSMSUGLv[0]] +
                "점 > <color=#ff0000>" + GlobalValue.BonusAmount[GlobalValue.g_YSMSUGLv[0] + 1] + "점</color>";
                UGCost = GlobalValue.BonusUGCost[GlobalValue.g_YSMSUGLv[0]];
                m_LVUpCostText.text = UGCost.ToString();
            }
            m_UGInfoText.text = "점수를 듬~뿍 주는 귀요미 친구가 나와요.\n" + "레벨업할수록 더 자주! 점수도 높~게!";
        }
        else if (m_UGKind == UGKind.UGFever)
		{
            if (m_UGIconImg != null)
                m_UGIconImg.sprite = m_SpriteGroup[1];

            m_UGAmGroup[0].gameObject.SetActive(false);
            m_UGAmGroup[1].gameObject.SetActive(true);
            m_UGAmGroup[2].gameObject.SetActive(false);

            m_UGAmGroup[1].text = GlobalValue.FeverAmount[GlobalValue.g_YSMSUGLv[1]].ToString("F1");
            m_UGLevelTxt.text = GlobalValue.g_YSMSUGLv[1].ToString();
            m_UGGuageBar.fillAmount = (float)GlobalValue.g_YSMSUGLv[1] / 15.0f;
            
            if (GlobalValue.g_YSMSUGLv[1] == 15)
			{
                m_UGAmountText.text = "피버 점수 " + GlobalValue.FeverAmount[GlobalValue.g_YSMSUGLv[1]] + "배";
                m_UGLvUpBtn.interactable = false;
                m_LvUpCostImg.gameObject.SetActive(false);
                m_LVUpCostText.gameObject.SetActive(false);
                m_MaxLvText.gameObject.SetActive(true);
            }
            else
			{
                m_UGAmountText.text = "피버 점수 " + GlobalValue.FeverAmount[GlobalValue.g_YSMSUGLv[1]] +
                "배 > <color=#ff0000>" + GlobalValue.FeverAmount[GlobalValue.g_YSMSUGLv[1] + 1] + "배</color>";
                UGCost = GlobalValue.FeverUGCost[GlobalValue.g_YSMSUGLv[1]];
                m_LVUpCostText.text = UGCost.ToString();
            }
            m_UGInfoText.text = "피버 상태가 되면 점수가 많~이 올라요!\n" + "레벨업할수록 몇 배로 쭉! 쭉! 쭉!";
        }
        else if (m_UGKind == UGKind.UGSuper)
		{
            if (m_UGIconImg != null)
                m_UGIconImg.sprite = m_SpriteGroup[2];

            m_UGAmGroup[0].gameObject.SetActive(false);
            m_UGAmGroup[1].gameObject.SetActive(false);
            m_UGAmGroup[2].gameObject.SetActive(true);

            if (GlobalValue.g_YSMSUGLv[2] == 0)
                m_UGAmGroup[2].gameObject.SetActive(false);
            else
                m_UGAmGroup[2].text = GlobalValue.SuperAmount[GlobalValue.g_YSMSUGLv[2]].ToString("F1") + "초";
            m_UGLevelTxt.text = GlobalValue.g_YSMSUGLv[2].ToString();
            m_UGGuageBar.fillAmount = (float)GlobalValue.g_YSMSUGLv[2] / 15.0f;

            if (GlobalValue.g_YSMSUGLv[2] == 15)
			{
                m_UGAmountText.text = "이벤트 시간 " + GlobalValue.SuperAmount[GlobalValue.g_YSMSUGLv[2]].ToString("F1") + "초";
                m_UGLvUpBtn.interactable = false;
                m_LvUpCostImg.gameObject.SetActive(false);
                m_LVUpCostText.gameObject.SetActive(false);
                m_MaxLvText.gameObject.SetActive(true);
            }
            else
			{
                m_UGAmountText.text = "이벤트 시간 " + GlobalValue.SuperAmount[GlobalValue.g_YSMSUGLv[2]].ToString("F1") +
                "초 > <color=#ff0000>" + GlobalValue.SuperAmount[GlobalValue.g_YSMSUGLv[2] + 1].ToString("F1") + "초</color>";
                UGCost = GlobalValue.SuperUGCost[GlobalValue.g_YSMSUGLv[2]];
                m_LVUpCostText.text = UGCost.ToString();
            }
            m_UGInfoText.text = "한 캐릭터로 모두 통일돼요!\n" + "레벨업할수록 오~래 지속되어 짱 쉬워요!";
        }
	}

    void UpdateSDJRUGKind()
	{
        GlobalValue.SDJRUGAm();

        if (m_UGKind == UGKind.UGBonus)
		{
            if (m_UGIconImg != null)
                m_UGIconImg.sprite = m_SpriteGroup[0];

            m_UGAmGroup[0].gameObject.SetActive(true);
            m_UGAmGroup[1].gameObject.SetActive(false);
            m_UGAmGroup[2].gameObject.SetActive(false);

            if (GlobalValue.g_SDJRUGLv[0] == 0)
                m_UGAmGroup[0].gameObject.SetActive(false);
            else
                m_UGAmGroup[0].text = "+" + GlobalValue.BonusAmount[GlobalValue.g_SDJRUGLv[0]].ToString();

            m_UGLevelTxt.text = GlobalValue.g_SDJRUGLv[0].ToString();
            m_UGGuageBar.fillAmount = (float)GlobalValue.g_SDJRUGLv[0] / 15.0f;

            m_UGAmountText.text = "";
            if (GlobalValue.g_SDJRUGLv[0] == 15)
            {
                m_UGAmountText.text = "보너스 블럭이 등장해요~\n" + "보너스 점수 " +
                    GlobalValue.BonusAmount[GlobalValue.g_SDJRUGLv[0]] + "점";
                m_UGLvUpBtn.interactable = false;
                m_LvUpCostImg.gameObject.SetActive(false);
                m_LVUpCostText.gameObject.SetActive(false);
                m_MaxLvText.gameObject.SetActive(true);
            }
            else
            {
                m_UGAmountText.text = "보너스 블록이 등장해요~\n" + "보너스 점수 " + 
                    GlobalValue.BonusAmount[GlobalValue.g_SDJRUGLv[0]] + "점 > <color=#ff0000>" + 
                    GlobalValue.BonusAmount[GlobalValue.g_SDJRUGLv[0] + 1] + "점</color>";
                UGCost = GlobalValue.BonusUGCost[GlobalValue.g_SDJRUGLv[0]];
                m_LVUpCostText.text = UGCost.ToString();
            }

            m_UGInfoText.text = "한가지 색을 모두 지워주는 블록이 나와요.\n" + "레벨업할수록 더 자주! 점수도 높~게!";
        }
        else if (m_UGKind == UGKind.UGFever)
		{
            if (m_UGIconImg != null)
                m_UGIconImg.sprite = m_SpriteGroup[1];

            m_UGAmGroup[0].gameObject.SetActive(false);
            m_UGAmGroup[1].gameObject.SetActive(true);
            m_UGAmGroup[2].gameObject.SetActive(false);

            m_UGAmGroup[1].text = GlobalValue.FeverAmount[GlobalValue.g_SDJRUGLv[1]].ToString("F1");
            m_UGLevelTxt.text = GlobalValue.g_SDJRUGLv[1].ToString();
            m_UGGuageBar.fillAmount = (float)GlobalValue.g_SDJRUGLv[1] / 15.0f;

            if (GlobalValue.g_SDJRUGLv[1] == 15)
            {
                m_UGAmountText.text = "피버 점수 " + GlobalValue.FeverAmount[GlobalValue.g_SDJRUGLv[1]] + "배";
                m_UGLvUpBtn.interactable = false;
                m_LvUpCostImg.gameObject.SetActive(false);
                m_LVUpCostText.gameObject.SetActive(false);
                m_MaxLvText.gameObject.SetActive(true);
            }
            else
            {
                m_UGAmountText.text = "피버 점수 " + GlobalValue.FeverAmount[GlobalValue.g_SDJRUGLv[1]] +
                "배 > <color=#ff0000>" + GlobalValue.FeverAmount[GlobalValue.g_SDJRUGLv[1] + 1] + "배</color>";
                UGCost = GlobalValue.FeverUGCost[GlobalValue.g_SDJRUGLv[1]];
                m_LVUpCostText.text = UGCost.ToString();
            }

            m_UGInfoText.text = "피버 상태가 되면 점수가 많~이 올라요!\n" + "레벨업할수록 몇 배로 쭉! 쭉! 쭉!";
        }
        else if (m_UGKind == UGKind.UGSuper)
		{
            if (m_UGIconImg != null)
                m_UGIconImg.sprite = m_SpriteGroup[2];

            m_UGAmGroup[0].gameObject.SetActive(false);
            m_UGAmGroup[1].gameObject.SetActive(false);
            m_UGAmGroup[2].gameObject.SetActive(true);

            if (GlobalValue.g_SDJRUGLv[2] == 0)
                m_UGAmGroup[2].gameObject.SetActive(false);
            else
                m_UGAmGroup[2].text = GlobalValue.SuperAmount[GlobalValue.g_SDJRUGLv[2]].ToString("F1") + "초";
            m_UGLevelTxt.text = GlobalValue.g_SDJRUGLv[2].ToString();
            m_UGGuageBar.fillAmount = (float)GlobalValue.g_SDJRUGLv[2] / 15.0f;

            if (GlobalValue.g_SDJRUGLv[2] == 15)
            {
                m_UGAmountText.text = "이벤트 시간 " + GlobalValue.SuperAmount[GlobalValue.g_SDJRUGLv[2]].ToString("F1") + "초";
                m_UGLvUpBtn.interactable = false;
                m_LvUpCostImg.gameObject.SetActive(false);
                m_LVUpCostText.gameObject.SetActive(false);
                m_MaxLvText.gameObject.SetActive(true);
            }
            else
            {
                m_UGAmountText.text = "이벤트 시간 " + GlobalValue.SuperAmount[GlobalValue.g_SDJRUGLv[2]].ToString("F1") +
                "초 > <color=#ff0000>" + GlobalValue.SuperAmount[GlobalValue.g_SDJRUGLv[2] + 1].ToString("F1") + "초</color>";
                UGCost = GlobalValue.SuperUGCost[GlobalValue.g_SDJRUGLv[2]];
                m_LVUpCostText.text = UGCost.ToString();
            }
            m_UGInfoText.text = "블럭이 두가지 색으로 통일돼요!\n" + "레벨업할수록 오~래 지속되어 짱 쉬워요!";
        }

	}

    void MessageBoxFunc(MessageState a_MessState, string a_MessLabel, string a_Mess)
    {
        if (m_MessageBoxObj != null)
            m_MessageBoxObj = Resources.Load("MessageBox") as GameObject;

        GameObject a_MsgBoxObj = Instantiate(m_MessageBoxObj);
        a_MsgBoxObj.transform.SetParent(Canvas_Message.transform, false);

        MessageBox a_MsgBox = a_MsgBoxObj.GetComponent<MessageBox>();
        //a_MsgBox.m_MessState = a_MessState;
        if (a_MsgBox != null)
            a_MsgBox.ShowMessage(a_MessLabel, a_Mess, a_MessState);
    }
}
