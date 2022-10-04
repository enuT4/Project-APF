using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TestMgr : MonoBehaviour
{
    public Text m_TimeText;
    public Text m_Timer;
    public Text m_YMDTxt;
    public Button m_StartBtn;
    public Button m_StopBtn;

    DateTime m_TempTime;
    int m_TempTimeSec = 0;
    int m_CurTimeSec = 0;
    int m_TempSec = 0;
    int m_TimerSec = 0;
    int m_TimerMin = 0;

    bool isTimerStart = false;

    // Start is called before the first frame update
    void Start()
    {
        if (m_StartBtn != null)
            m_StartBtn.onClick.AddListener(StartTimerFunc);

        if (m_StopBtn != null)
            m_StopBtn.onClick.AddListener(StopTimerFunc);

    }

    // Update is called once per frame
    void Update()
    {
        m_TimeText.text = GetCurrentDate();
        CalcTimeFunc();
        m_YMDTxt.text = YMDtoNum(DateTime.Now).ToString();
    }

    public string GetCurrentDate()
    {
        return DateTime.Now.ToString(("yyyy-MM-dd HH:mm:ss tt"));
    }

    public static string GetTime()
    {
        return DateTime.Now.ToString(("dd HH:mm:ss"));

    }

    void StartTimerFunc()
	{
        m_TempTime = DateTime.Now;
        m_TempTimeSec = CalcDatetoSec(m_TempTime);
        m_Timer.text = m_TempTimeSec.ToString();
        isTimerStart = true;
	}

    void StopTimerFunc()
	{
        isTimerStart = false;
        m_TempSec = m_CurTimeSec - m_TempTimeSec;
        m_TimerMin = m_TempSec / 60;
        m_TimerSec = m_TempSec % 60;
        m_Timer.text = m_TimerMin.ToString() + ":" + m_TimerSec.ToString("D2"); ;
	}

	int CalcDatetoSec(DateTime a_Date)
	{
       int a_Sec = a_Date.Day * 24 * 3600 + a_Date.Hour * 3600 + a_Date.Minute * 60 + a_Date.Second;
        return a_Sec;
	}

    void CalcTimeFunc()
	{
        if (isTimerStart)
		{
            m_CurTimeSec = CalcDatetoSec(DateTime.Now);
            m_TempSec = m_CurTimeSec - m_TempTimeSec;
            m_TimerMin = m_TempSec / 60;
            m_TimerSec = m_TempSec % 60;
            m_Timer.text = m_TimerMin.ToString() + ":" + m_TimerSec.ToString("D2"); ;
        }
    }

    int YMDtoNum(DateTime a_Date)
	{
        int a_YMD = a_Date.Year * 10000 + a_Date.Month * 100 + a_Date.Day;
        return a_YMD;


	}

}
