using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadTile1 : MonoBehaviour
{
    float m_RotSpeed = 3.0f;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    
    //}

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, m_RotSpeed);
    }
}
