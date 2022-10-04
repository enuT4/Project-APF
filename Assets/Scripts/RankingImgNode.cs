using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Facebook.Unity;


public class RankingImgNode : MonoBehaviour
{
    public Image ButtonSprite;
    public Text RankText;
    public Text NicknameText;
    public Image FacebookImg;
    public Text BestScoreText;

    [HideInInspector] public int m_Unique_ID = -1;
    [HideInInspector] public string m_NickName = "";
    [HideInInspector] public int m_BestScore = -1;
    [HideInInspector] public int m_Rank = -1;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}

    public void InitInfo(int a_Rank, string a_Nick, int a_Score)
	{
        m_Rank = a_Rank;
        m_NickName = a_Nick;
        m_BestScore = a_Score;
        RankText.text = a_Rank.ToString();
        NicknameText.text = a_Nick;
        BestScoreText.text = a_Score.ToString("N0");
	}


    //private void DisplayProfilePic(IGraphResult ret)
    //{
    //    if (ret.Error == null && FacebookImg != null)
    //    {
    //        FacebookImg.sprite =
    //            Sprite.Create(ret.Texture,
    //                          new Rect(0, 0,
    //                          ret.Texture.width, ret.Texture.height),
    //                          new Vector2());
    //
    //        //Debug.Log(ret.Texture.width);
    //        //Debug.Log(ret.Texture.height);
    //    }
    //}
}
