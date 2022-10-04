using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharNode : MonoBehaviour
{
    public int m_ChImgIdx = 0;     //캐릭터번호
    public Texture[] m_CharImg = null;
    public bool m_isMove = false;
    public bool m_isLeft = false;
    public static bool m_isClear = true;
    
    public void SetCharRsc(int a_Idx)
	{
        if (a_Idx < (int)CharType.YSMS_Bomb || (int)CharType.YSMS_Char8 < a_Idx)
            return;

        m_ChImgIdx = a_Idx;
        Transform a_FindObj = this.gameObject.transform.Find("CharImg");
        if (a_FindObj != null)
            a_FindObj.GetComponent<RawImage>().texture = m_CharImg[m_ChImgIdx];
    }

    void Update()
    {
        IsMove();
        ClearColor();
    }

    public void IsMove()
    {
        if (!m_isMove) return;
        
        int key = 0;
        float a_Speed = 30.0f;

        if (m_isLeft) key = -1;
        else key = 1;
        YSMSIngameMgr.Inst.UpdateCharArr();
        this.transform.Translate(new Vector3(2 * key, -1, 0) * a_Speed);
        if (this.transform.position.y < -100.0f)
        {
            Destroy(this.gameObject);
            m_isMove = false;
            YSMSIngameMgr.m_SpawnList.RemoveAt(0);
        }
    }

    public void ClearColor()
	{
        if (m_isClear) return;

        Transform a_FindObj = YSMSIngameMgr.m_SpawnList[0].gameObject.transform.Find("CharImg");
        if (a_FindObj != null)
            a_FindObj.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);

        m_isClear = true;   
    }
}
