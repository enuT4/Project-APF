using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelUpImage : MonoBehaviour
{
    float m_EffTime = 0.0f;
    float m_SchVelocity = 5.0f / 0.3f;
    float m_ScvVelocity = 0.3f / 0.1f;
    float m_ApVelocity = 1.0f / 0.2f;
    float m_Scalex = 0.1f;
    float m_Scaley = 1.0f;

    Image m_ThisImg = null;

    Color m_Color;
    Color m_OL1Color;
    Color m_OL2Color;
    Outline[] m_OutLine;

    // Start is called before the first frame update
    void Start()
    {
        m_ThisImg = this.GetComponent<Image>();
        m_ThisImg.transform.localScale = new Vector2(m_Scalex, m_Scaley);
        m_Color = m_ThisImg.color;
        m_Color.a = 1.0f;

        m_OutLine = GetComponentsInChildren<Outline>();
        if (m_OutLine[0] != null && m_OutLine[1] != null)
		{
			m_OL1Color = m_OutLine[0].effectColor;
            m_OL2Color = m_OutLine[1].effectColor;
            m_OL1Color.a = 0.5f;
            m_OL2Color.a = 0.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_EffTime += Time.deltaTime;

        if (m_EffTime < 0.3f)
		{
            m_Scalex = m_ThisImg.transform.localScale.x;
            m_Scalex += Time.deltaTime * m_SchVelocity;
            if (1.2f <= m_Scalex) m_Scalex = 1.2f;
        }

        if (0.1f < m_EffTime)
        {
            m_Scaley = m_ThisImg.transform.localScale.y;
            m_Scaley -= Time.deltaTime * m_ScvVelocity;
            if (m_Scaley <= 0.8f) m_Scaley = 0.8f;
        }
        if (0.2f < m_EffTime)
		{
            m_Scalex = 1.0f;
            m_Scaley = 1.0f;
		}
        if (0.3f < m_EffTime)
		{
            m_Color.a -= Time.deltaTime * m_ApVelocity;
            m_OL1Color.a -= Time.deltaTime * m_ApVelocity;
            m_OL2Color.a -= Time.deltaTime * m_ApVelocity;
            
            if (m_Color.a < 0.0f) m_Color.a = 0.0f;
            if (m_OL1Color.a < 0.0f) m_OL1Color.a = 0.0f;
            if (m_OL2Color.a < 0.0f) m_OL2Color.a = 0.0f;
		}
        if (0.5f < m_EffTime)
            Destroy(this.gameObject);

        m_ThisImg.transform.localScale = new Vector2(m_Scalex, m_Scaley);
        m_ThisImg.color = m_Color;
        m_OutLine[0].effectColor = m_OL1Color;
        m_OutLine[1].effectColor = m_OL2Color;
    }
}
