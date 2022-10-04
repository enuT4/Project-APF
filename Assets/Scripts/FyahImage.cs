using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FyahImage : MonoBehaviour
{
    float m_EffTime = 0.0f;
    float m_SchVelocity = 5.0f / 0.3f;
    float m_ApVelocity = 1.0f / 0.2f;
    float m_Scalex = 0.1f;
    float m_Scaley = 1.0f;

    Image m_ThisImg = null;

    Color m_Color;
    Canvas SortLayerCanvas;

    // Start is called before the first frame update
    void Start()
    {
        m_ThisImg = this.GetComponent<Image>();
        m_ThisImg.transform.localScale = new Vector2(m_Scalex, m_Scaley);
        m_Color = m_ThisImg.color;
        m_Color.a = 1.0f;

        SortLayerCanvas = this.GetComponent<Canvas>();
        SortLayerCanvas.overrideSorting = true;
        SortLayerCanvas.sortingOrder = 10;
    }

    // Update is called once per frame
    void Update()
    {
        m_EffTime += Time.deltaTime;
        if (m_EffTime < 0.5f)
        {
            m_Scalex = m_ThisImg.transform.localScale.x;
            m_Scaley = m_ThisImg.transform.localScale.y;
            m_Scalex += Time.deltaTime * m_SchVelocity;
            m_Scaley += Time.deltaTime * m_SchVelocity;
            if (1.0f <= m_Scalex)
            {
                m_Scalex = 1.0f;
                m_Scaley = 1.0f;
            }
        }
        if (0.7f < m_EffTime)
        {
            m_Color.a -= Time.deltaTime * m_ApVelocity;
            if (m_Color.a < 0.0f)
                m_Color.a = 0.0f;
        }
        if (1.0f < m_EffTime)
            Destroy(this.gameObject);

        m_ThisImg.transform.localScale = new Vector2(m_Scalex, m_Scaley);
        m_ThisImg.color = m_Color;
    }
}
