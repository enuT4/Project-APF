using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LineSpawnText : MonoBehaviour
{
    float m_EffTime = 0.0f;
    float MvVelocity = 1.0f / 1.05f;
    float ApVelocity = 1.0f / (1.0f - 0.5f);

    Vector3 m_CurPos = Vector3.zero;

    Canvas SortLayerCanvas;

    Text m_ThisText = null;
    Color m_Color;
    Outline m_OutLine;
    Color m_OLColor;

    // Start is called before the first frame update
    void Start()
    {
        m_ThisText = this.GetComponent<Text>();
        m_Color = m_ThisText.color;
        m_Color.a = 1.0f;

        m_OutLine = m_ThisText.GetComponentInChildren<Outline>();
        m_OLColor = m_OutLine.effectColor;

        SortLayerCanvas = this.GetComponent<Canvas>();
        SortLayerCanvas.overrideSorting = true;
        SortLayerCanvas.sortingOrder = 6;

        if (GlobalValue.g_GKind == GameKind.APF_SDJR)
            MvVelocity /= 10.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_EffTime += Time.deltaTime;
        if (m_EffTime < 1.05f)
        {
            m_CurPos = transform.position;
            m_CurPos.y += Time.deltaTime * MvVelocity;
            transform.position = m_CurPos;
        }
        if (0.4f < m_EffTime)
        {
            m_Color.a -= Time.deltaTime * ApVelocity;
            if (m_Color.a < 0.0f)
                m_Color.a = 0.0f;
            m_OLColor.a -= Time.deltaTime * ApVelocity;
            m_OutLine.effectColor = m_OLColor;
            m_ThisText.color = m_Color;

        }
        if (1.05f <= m_EffTime)
            Destroy(this.gameObject);
    }


    public void LineSpawnTxt()
    {
        m_ThisText = this.GetComponent<Text>();
        m_ThisText.text = "200";
    }
}
