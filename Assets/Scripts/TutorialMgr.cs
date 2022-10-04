using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TutorialMgr : MonoBehaviour
{
    public Image m_InfoImg;
    Sprite[] m_SpriteArray;
    public Sprite[] m_YSMSSprite;
    public Sprite[] m_SDJRSprite;
    int m_CurSpr = 0;
    
    public GameObject m_YSMSTutGroup;
    public Text m_YSMSTutText;
    public Image m_YSMSArrowImg;

    public GameObject m_SDJRTutGroup;
    public Text m_SDJRTutText;
    public Image m_SDJRArrowImg;
    Vector2 m_ArrowPos;
    Vector2 m_ArrowSizeDel;
    Vector3 m_ArrowAngle;
    public Toggle m_tutToggle;
    public Text m_ReadyTxt;
    public Image m_CrossImage;
    string m_GameStr = "";

    string[] m_SDJRTextArr = new string[11] { 
        "엄마의 삼단정리는\n같은 블럭 세개를 세로로 정리해서\n블럭을 터트리는 게임이에요.",
        "왼쪽 화살표(A)키, 아래 화살표(S)키,\n오른쪽 화살표(D)키를 이용해서\n블럭을 선택해요.",
        "왼쪽에 있는 파란 블럭을 선택해서\n이렇게 오른쪽으로 샥~!",
        "같은 색깔의 블럭을 3개 이상 모으면\n블럭을 터트릴 수 있어요.",
        "가운데에 있는 빨간 블럭을 선택해서\n이렇게 왼쪽으로 펑~!",
        "아래에 있는 시간 게이지를 누르거나\n스페이스바 키를 누르면\n새로운 블럭이 등장해요.",
        "타일을 내리면 2000점이 꽁짜~!\n하지만 너무 많이 누르다간\n순식간에 게임오버 해버리고 말거에요!",
        "폭탄게이지를 다 채우면\n폭탄 타일이 등장해요!",
        "폭탄 타일은 폭발 범위가 넓고\n남은 시간을 10초 추가해줘요!",
        "자신의 양옆줄을 포함해\n위아래 두칸을 다 터뜨려버려요~!",
        "제한시간 내에 최대한\n 많은 블록을 터트리세요~!\n준비 되셨나요~?"
    };

    string[] m_YSMSTextArr = new string[10] {
        "삼촌의 니편내편은\n끝없이 등장하는 캐릭터들의\n편을 나누는 게임이에요.",
        "왼쪽 화살표(D,F)키나\n오른쪽 화살표(J,K)키를 이용해서\n편을 나누시면 돼요.",
        "일정 시간 내에\n다음 편을 가르지 않으면\n콤보가 끊겨버려요!",
        "틀리지 않도록 왼쪽 편은 왼쪽으로,\n오른쪽 편은 오른쪽 편으로 갈라주세요.",
        "틀리면 1초동안 편을 가르지 못하고\n폭탄 게이지도 감소해요 ㅠ0ㅠ",
        "최대한 틀리지 않으면서\n콤보를 이어 나가보세요!",
        "폭탄게이지를 다 채우면\n폭탄 친구가 줄에 등장해요!",
        "폭탄 친구는 모두의 친구에요.\n어느쪽으로 편을 갈라도 정답이에요~!",
        "폭탄 친구는 남은 시간을\n10초 추가해줘요!",
        "제한시간 내에 최대한 많은\n친구의 편을 가르세요~!\n준비 되셨나요~?",
    };



    // Start is called before the first frame update
    void Start()
    {
        InitGameName();
    }
    
	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
            m_CurSpr++;
            if (m_SpriteArray.Length <= m_CurSpr)
                GameStart();
            else
				ShowTutImage(GlobalValue.g_GKind, m_CurSpr);
		}
    }

    void InitGameName()
    {
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
		{
			m_SpriteArray = m_YSMSSprite;
            m_YSMSTutGroup.SetActive(true);
            m_SDJRTutGroup.SetActive(false);
            if (m_YSMSTutText != null)
                m_YSMSTutText.text = m_YSMSTextArr[m_CurSpr];
            m_YSMSArrowImg.gameObject.SetActive(true);
            m_GameStr = "YSMS";
        }
		else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
		{
			m_SpriteArray = m_SDJRSprite;
            m_YSMSTutGroup.SetActive(false);
            m_SDJRTutGroup.SetActive(true);
            if (m_SDJRTutText != null)
                m_SDJRTutText.text = m_SDJRTextArr[m_CurSpr];
            m_GameStr = "SDJR";
        }

        if (m_InfoImg != null)
            m_InfoImg.sprite = m_SpriteArray[0];

        if (m_tutToggle != null)
        {
            m_tutToggle.gameObject.SetActive(false);
            m_tutToggle.isOn = false;
        }

        if (m_ReadyTxt != null) 
            m_ReadyTxt.text = "(스페이스바를 눌러 튜토리얼 진행)";
    }

    void ShowTutImage(GameKind a_GKind, int a_CurSpr)
	{
        if (m_SpriteArray.Length <= m_CurSpr) return;

        if (m_InfoImg != null)
            m_InfoImg.sprite = m_SpriteArray[a_CurSpr];

        if (a_GKind == GameKind.APF_YSMS)
		{
            if (m_YSMSTutText != null)
                m_YSMSTutText.text = m_YSMSTextArr[a_CurSpr];

            if (a_CurSpr == 6)
                m_YSMSTutText.gameObject.transform.localPosition = new Vector2(200.0f, 50.0f);
            else
                m_YSMSTutText.gameObject.transform.localPosition = new Vector2(0.0f, 50.0f);

            //화살표 좌표 및 크기
            m_YSMSArrowImg.gameObject.SetActive(true);
            if (a_CurSpr == 1)
            {
                m_ArrowPos = new Vector2(30.0f, -230.0f);
                m_ArrowSizeDel = new Vector2(80.0f, 50.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, -30.0f);
            }
            else if (a_CurSpr == 2)
            {
                m_ArrowPos = new Vector2(-200.0f, -140.0f);
                m_ArrowSizeDel = new Vector2(100.0f, 100.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, -30.0f);
            }
            else if (a_CurSpr == 3)
            {
                m_ArrowPos = new Vector2(-30.0f, -230.0f);
                m_ArrowSizeDel = new Vector2(80.0f, 50.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 210.0f);
            }
            else if (a_CurSpr == 4)
            {
                m_ArrowPos = new Vector2(200.0f, -140.0f);
                m_ArrowSizeDel = new Vector2(100.0f, 100.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 210.0f);
            }
            else if (a_CurSpr == 5) 
			{
                m_YSMSArrowImg.gameObject.SetActive(false);
                m_CrossImage.gameObject.SetActive(true);
            }
            else if (a_CurSpr == 6)
            {
                if (m_CrossImage.gameObject.activeSelf == true)
                    m_CrossImage.gameObject.SetActive(false);
                m_ArrowPos = new Vector2(-150.0f, 70.0f);
                m_ArrowSizeDel = new Vector2(100.0f, 100.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else if (a_CurSpr == 7)
            {
                m_ArrowPos = new Vector2(-200.0f, -140.0f);
                m_ArrowSizeDel = new Vector2(100.0f, 100.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, -30.0f);
            }
            else if (a_CurSpr == 8)
            {
                m_ArrowPos = new Vector2(-80.0f, -220.0f);
                m_ArrowSizeDel = new Vector2(80.0f, 50.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 210.0f);
            }
            else if (a_CurSpr == 9)
            {
                m_ArrowPos = new Vector2(-130.0f, -250.0f);
                m_ArrowSizeDel = new Vector2(100.0f, 100.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 0.0f);
            }
            SetArrow(m_YSMSArrowImg, m_ArrowPos, m_ArrowSizeDel, m_ArrowAngle);

            //토글, 게임 시작 문구
            if (a_CurSpr == 9)
            {
                m_tutToggle.gameObject.SetActive(true);
                m_ReadyTxt.text = "(스페이스바를 눌러 게임 시작)";
                m_ReadyTxt.gameObject.SetActive(true);
            }
            else
                m_ReadyTxt.gameObject.SetActive(false);
        }
        else if (a_GKind == GameKind.APF_SDJR)
		{
            if (m_SDJRTutText != null)
                m_SDJRTutText.text = m_SDJRTextArr[a_CurSpr];

            //화살표 좌표 및 크기
            m_SDJRArrowImg.gameObject.SetActive(true);
            if (a_CurSpr == 2)
			{
                m_ArrowPos = new Vector2(-20.0f, -55.0f);
                m_ArrowSizeDel = new Vector2(200.0f, 80.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, -12.0f);
			}
            else if(a_CurSpr == 4)
			{
                m_ArrowPos = new Vector2(-70.0f, -35.0f);
                m_ArrowSizeDel = new Vector2(100.0f, 60.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 180.0f);
            }
            else if(a_CurSpr == 5)
			{
                m_ArrowPos = new Vector2(270.0f, -280.0f);
                m_ArrowSizeDel = new Vector2(200.0f, 150.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 200.0f);
            }
            else if(a_CurSpr == 6)
			{
                m_ArrowPos = new Vector2(270.0f, -280.0f);
                m_ArrowSizeDel = new Vector2(200.0f, 150.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 200.0f);
            }
            else if(a_CurSpr == 7)
			{
                m_ArrowPos = new Vector2(-280.0f, 175.0f);
                m_ArrowSizeDel = new Vector2(120.0f, 100.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 30.0f);
            }
            else if(a_CurSpr == 9)
			{
                m_ArrowPos = new Vector2(-75.0f, 35.0f);
                m_ArrowSizeDel = new Vector2(80.0f, 55.0f);
                m_ArrowAngle = new Vector3(0.0f, 0.0f, 210.0f);
            }
            else
                m_SDJRArrowImg.gameObject.SetActive(false);
            SetArrow(m_SDJRArrowImg, m_ArrowPos, m_ArrowSizeDel, m_ArrowAngle);

            //토글, 게임 시작 문구
			if (a_CurSpr == 10)
			{
				m_tutToggle.gameObject.SetActive(true);
				m_ReadyTxt.text = "(스페이스바를 눌러 게임 시작)";
				m_ReadyTxt.gameObject.SetActive(true);
			}
            else
                m_ReadyTxt.gameObject.SetActive(false);
        }
	}

    void GameStart()
	{
        if (m_tutToggle.isOn)
        {
            if (GlobalValue.g_GKind == GameKind.APF_YSMS)
                GlobalValue.g_YSMSTutSkipYN = 1;
            else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
                GlobalValue.g_SDJRTutSkipYN = 1;

            NetworkMgr.Inst.PushPacket(PacketType.SkipOnOff);
        }
        SceneManager.LoadScene(m_GameStr + "InGame");
    }

    void SetArrow(Image a_ArrowImg, Vector2 a_Pos, Vector2 a_SizDel, Vector3 a_Angle)
	{
        a_ArrowImg.gameObject.transform.localPosition = a_Pos;
        a_ArrowImg.rectTransform.sizeDelta = a_SizDel;
        a_ArrowImg.gameObject.transform.localEulerAngles = a_Angle;
    }
}
