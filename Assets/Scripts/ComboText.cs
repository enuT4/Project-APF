using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComboText : MonoBehaviour
{
    float m_EffTime = 0.0f;

    Text m_ThisText = null;

    float m_GrowSpeed = 300.0f;
    Canvas SortLayerCanvas;
    // Start is called before the first frame update
    void Start()
    {
        m_ThisText = this.GetComponent<Text>();
        m_ThisText.fontSize = 20;

        SortLayerCanvas = this.GetComponent<Canvas>();
        SortLayerCanvas.overrideSorting = true;
    }

    // Update is called once per frame
    void Update()
    {
        m_EffTime += Time.deltaTime;
        if (m_EffTime < 0.2f)
        {
            m_ThisText.fontSize += (int)(Time.deltaTime * m_GrowSpeed);
            if (50 <= m_ThisText.fontSize)
                m_ThisText.fontSize = 50;
            SortLayerCanvas.sortingOrder = 7;
        }
        else if (m_EffTime < 0.5f)
        {
            m_ThisText.fontSize -= (int)(Time.deltaTime * m_GrowSpeed * 0.2f);
            if (m_ThisText.fontSize <= 40)
                m_ThisText.fontSize = 40;
            SortLayerCanvas.sortingOrder = 6;
        }
        else if (m_EffTime < 0.8f) 
		{
            m_ThisText.fontSize = 40;
            SortLayerCanvas.sortingOrder = 5;
        }
        else
            Destroy(this.gameObject);
    }


    public void InitCombo(int a_Combo)
	{
        m_ThisText = this.GetComponent<Text>();
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
            m_ThisText.text = YSMSIngameMgr.Inst.m_Combo.ToString() + " Combo";
        else if(GlobalValue.g_GKind == GameKind.APF_SDJR)
            m_ThisText.text = SDJRIngameMgr.Inst.m_Combo.ToString() + " Combo";
    }
}
