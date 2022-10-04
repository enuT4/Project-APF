using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TileType
{
    Bomb = 0,
    Normal,
    Bonus,
    Special1,   //지우개
    Special2,   //해머
    Special3,   //한줄
    Bad1,       //좌우반전
    Bad2,       //블록 가리기
    GameOver,   //게임오버
    TTypeCount
}

public class TileNode : MonoBehaviour
{
    public Image m_TileImg;
    public Sprite[] m_TileSprite;
    public Text m_BonusText;
    public Image m_SelImg;

    public Image m_BadCountImg;
    public Text m_BadCountText;
    float m_BadCount = 0.0f;
    public Image m_Bad2TileImg;

    public int m_TileIdx = -1;
    [HideInInspector] public bool m_isSelected = false;
    [HideInInspector] public TileType m_TType = TileType.TTypeCount;
    public bool m_isSPmoved = false;
    
    //// Start is called before the first frame update
    //void Start()
    //{
    //    
    //}

    // Update is called once per frame
    void Update()
    {
        m_SelImg.gameObject.SetActive(m_isSelected);

        if (0.0f < m_BadCount)
		{
            m_BadCount -= Time.deltaTime;
            m_BadCountText.text = ((int)m_BadCount).ToString();
            if (m_BadCount <= 0.0f)
			{
                m_BadCount = 0.0f;
                //페널티 발동
                //본인은 도움이 안되는 타일로 변신
                m_BadCountImg.gameObject.SetActive(false);
                m_TileImg.sprite = m_TileSprite[14];
                if (m_TType == TileType.Bad1)
                {   //페널티 1 : 좌우 반전
                    SDJRIngameMgr.Inst.m_isReverse = true;
                    SDJRIngameMgr.m_Bad1EffTimer = 5.0f;
                }
				else if (m_TType == TileType.Bad2)
                {   //페널티 2 : 타일 가리기
                    SDJRIngameMgr.Inst.m_isTileHide = true;
					SDJRIngameMgr.Inst.m_Switchbool = true;
                    SDJRIngameMgr.m_Bad2EffTimer = 5.0f;
                }
            }
        }
    }

    public void SetTile(TileType a_TType, int a_Level)
	{
        if ((int)a_TType < 0 || (int)TileType.TTypeCount < (int)a_TType)
            return;

        if (a_TType == TileType.Normal)
            m_TileIdx = Random.Range(1, a_Level + 2);
        else
            CheckTileType(a_TType);

        SetTileIdx(m_TileIdx);
	}

    public void SetTileIdx(int a_TileIdx)
	{
        if (a_TileIdx == -1) return;

        if (m_TileImg != null)
            m_TileImg.sprite = m_TileSprite[a_TileIdx];
    }

	void CheckTileType(TileType a_TType)
	{
        if (a_TType == TileType.Bonus)
        {
            m_BonusText.gameObject.SetActive(true);
            m_TileIdx = 8;
        }
        else if (a_TType == TileType.Bomb) m_TileIdx = 0;
        else if (a_TType == TileType.Special1) m_TileIdx = 9;
        else if (a_TType == TileType.Special2) m_TileIdx = 10;
        else if (a_TType == TileType.Special3) m_TileIdx = 11;
        else if (a_TType == TileType.Bad1)
        {
            m_BadCountImg.gameObject.SetActive(true);
            m_TileIdx = 12;
            m_BadCount = 10.0f;
        }
        else if (a_TType == TileType.Bad2)
        {
            m_BadCountImg.gameObject.SetActive(true);
            m_TileIdx = 13;
            m_BadCount = 10.0f;
        }
        else if (a_TType == TileType.GameOver) m_TileIdx = 14;
    }
}
