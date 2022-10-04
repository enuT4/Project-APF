using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerDur : MonoBehaviour
{
    Quaternion m_HammerRot;
    float m_RotSpeed = 3.0f;
    bool isUp = false;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    
    //}

    // Update is called once per frame
    void Update()
    {
        m_HammerRot = this.transform.rotation;
        if (!isUp)
		{
            m_HammerRot.z += Time.deltaTime * m_RotSpeed;
            if (0.5f <= m_HammerRot.z)
                isUp = true;
        }
        else
		{
            m_HammerRot.z -= Time.deltaTime * m_RotSpeed;
            if (m_HammerRot.z <= -0.13f)
                isUp = false;
        }
        this.transform.rotation = m_HammerRot;
    }
}
