using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerText : MonoBehaviour
{
    float m_EffTime = 0.0f;
    float MvVelocity = 250.0f;
    float ApVelocity = 3.5f;

    Text m_ThisText = null;
    Color m_TextColor;

    Vector3 m_CurPos = Vector3.zero;
    Outline[] m_TxtOutline;
    Color m_OL1Color;
    Color m_OL2Color;
    Canvas SortLayerCanvas;

    // Start is called before the first frame update
    void Start()
    {
        m_ThisText = this.GetComponent<Text>();
        m_TextColor = m_ThisText.color;
        m_TextColor.a = 0.0f;

        m_TxtOutline = m_ThisText.GetComponentsInChildren<Outline>();
        m_OL1Color = m_TxtOutline[0].effectColor;
        m_OL2Color = m_TxtOutline[1].effectColor;
        m_OL1Color.a = 0.0f;
        m_OL2Color.a = 0.0f;

        if(GlobalValue.g_GKind == GameKind.APF_YSMS)
            MvVelocity = 250.0f;
        else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
            MvVelocity = 5.0f;

        SortLayerCanvas = this.GetComponent<Canvas>();
        SortLayerCanvas.overrideSorting = true;
        SortLayerCanvas.sortingOrder = 10;
    }

    // Update is called once per frame
    void Update()
    {
        m_EffTime += Time.deltaTime;

        if (m_EffTime < 0.3f)
		{
            m_CurPos = transform.position;
            m_CurPos.x -= Time.deltaTime * MvVelocity;
            transform.position = m_CurPos;

            m_TextColor.a += Time.deltaTime * ApVelocity;

            if (1.0f <= m_TextColor.a)
                m_TextColor.a = 1.0f;
            m_OL1Color.a = m_TextColor.a * 0.5f;
            m_OL2Color.a = m_TextColor.a * 0.5f;

            m_ThisText.color = m_TextColor;
            m_TxtOutline[0].effectColor = m_OL1Color;
            m_TxtOutline[1].effectColor = m_OL2Color;
		}

        if (0.5f < m_EffTime)
		{
            m_TextColor.a += Time.deltaTime * ApVelocity * 0.15f;
            if (1.0f <= m_TextColor.a)
                m_TextColor.a = 1.0f;
            m_OL1Color.a = m_TextColor.a * 0.5f;
            m_OL2Color.a = m_TextColor.a * 0.5f;

            m_ThisText.color = m_TextColor;
            m_TxtOutline[0].effectColor = m_OL1Color;
            m_TxtOutline[1].effectColor = m_OL2Color;

            m_CurPos = transform.position;
            m_CurPos.x -= Time.deltaTime * MvVelocity * 0.1f;
            transform.position = m_CurPos;
        }

        if (1.0f < m_EffTime)
            Destroy(this.gameObject);
    }

    public void InitTime(int a_ShowTime)
	{
        m_ThisText = this.GetComponent<Text>();
        m_ThisText.text = a_ShowTime.ToString();
	}
}
