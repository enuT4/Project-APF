using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Facebook.Unity;



public class RankingPanel : MonoBehaviour
{
    public Text m_GameText = null;
    public Button m_ClosePanelBtn = null;
    public Sprite[] m_ButtonActiveImg = null;
    public GameObject RankingNodeObj = null;

    string m_GameName = "";

    [Header("-------- GameRankingPanel --------")]
    public ScrollRect GameRankingScrlView = null;
    public GameObject GameRankingContent = null;
    public Button m_GameRankBtn = null;

    [Header("-------- TotalRankingPanel --------")]
    public ScrollRect TotalRankingScrlView = null;
    public Button m_TotalRankBtn = null;
    public GameObject TotalRankingContent = null;

    public Image a_OldImage;

    float RkUpdateTimer = 0.0f;

    bool isFBImageLoaded = false;

    // Start is called before the first frame update
    void Start()
    {
#if FacebookOn
        FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
#else

#endif

        if (m_ClosePanelBtn != null)
            m_ClosePanelBtn.onClick.AddListener(() =>
            {
                Destroy(this.gameObject);
            });

        if (m_GameRankBtn != null)
            m_GameRankBtn.onClick.AddListener(() =>
            {
                GameRankingScrlView.gameObject.SetActive(true);
                m_GameRankBtn.image.sprite = m_ButtonActiveImg[0];
                TotalRankingScrlView.gameObject.SetActive(false);
                m_TotalRankBtn.image.sprite = m_ButtonActiveImg[1];
            });

        if (m_TotalRankBtn != null)
            m_TotalRankBtn.onClick.AddListener(() =>
            {
                GameRankingScrlView.gameObject.SetActive(false);
                m_GameRankBtn.image.sprite = m_ButtonActiveImg[1];
                TotalRankingScrlView.gameObject.SetActive(true);
                m_TotalRankBtn.image.sprite = m_ButtonActiveImg[0];
            });

        if (GlobalValue.g_FacebookName == "") isFBImageLoaded = true;
	}

	// Update is called once per frame
	void Update()
    {
        if (isFBImageLoaded)
		{
            RefreshRankingList();
            isFBImageLoaded = false;
		}

        if (0.0f < RkUpdateTimer) 
		{
            RkUpdateTimer -= Time.deltaTime;
            if (RkUpdateTimer <= 0.0f) RefreshRankingList();
		}
    }

    void RefreshRankingList()
	{
        ClearRank();
        CheckGameName();
        GetTotalRankingList();
        RkUpdateTimer = 30.0f;
    }

    void GetMyRanking(GameKind a_GKind)
	{
        if (GlobalValue.g_Unique_ID == "") return;
        if (m_GameName == "") return;

        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = m_GameName + "BestScore",
            MaxResultsCount = 1,
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            request,
            (result) =>
            {
                if (0 < result.Leaderboard.Count)
                {
                    var curBoard = result.Leaderboard[0];
                    if (a_GKind == GameKind.APF_YSMS)
					{
                        GlobalValue.g_MyYSMSRank = curBoard.Position + 1;
                        GlobalValue.g_YSMSBestScore = curBoard.StatValue;
                    }
                    else if (a_GKind == GameKind.APF_SDJR)
					{
                        GlobalValue.g_MySDJRRank = curBoard.Position + 1;
                        GlobalValue.g_SDJRBestScore = curBoard.StatValue;
                    }
                }
            },

            (error) =>
            {
                Debug.Log("등수 가져오기 실패");
            }

            );
    }

    void GetRankingList(GameKind a_GKind)
	{
        if (GlobalValue.g_Unique_ID == "") return;
        if (m_GameName == "") return;

        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0,
            StatisticName = m_GameName + "BestScore",
            MaxResultsCount = 20,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            (result) =>
            {
                if (RankingNodeObj == null) return;

                for (int ii = 0; ii < result.Leaderboard.Count; ii++)
                {
                    var curboard = result.Leaderboard[ii];
                    GameObject a_UserObj = Instantiate(RankingNodeObj);
                    a_UserObj.transform.SetParent(GameRankingContent.transform, false);
                    RankingImgNode a_RkNode = a_UserObj.GetComponent<RankingImgNode>();

                    a_RkNode.RankText.text = (ii + 1).ToString();
                    a_RkNode.BestScoreText.text = curboard.StatValue.ToString("N0");
                    a_RkNode.NicknameText.text = curboard.DisplayName;
                    if (curboard.PlayFabId == GlobalValue.g_Unique_ID)
                    {
                        a_RkNode.ButtonSprite.color = new Color32(120, 212, 242, 255);
                        if (GlobalValue.g_FacebookName != "")
                            a_RkNode.FacebookImg.sprite = a_OldImage.sprite;
                    }
                }
                GetMyRanking(a_GKind);
            },
            (error) =>
            {
                Debug.Log("리더보드 불러오기 실패");
            }
            );
    }

    void GetMyTotalRanking()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new GetLeaderboardAroundPlayerRequest()
        {
            StatisticName = "TotalScore",
            MaxResultsCount = 1,
		};

        PlayFabClientAPI.GetLeaderboardAroundPlayer(
            request,
            (result) =>
            {
                if (0 < result.Leaderboard.Count)
                {
                    var curBoard = result.Leaderboard[0];
                    GlobalValue.g_MyTotalRank = curBoard.Position + 1;
                    GlobalValue.g_TotalScore = curBoard.StatValue;
                }
            },
            (error) =>
            {
                Debug.Log("등수 가져오기 실패");
            }

            );
	}

    void GetTotalRankingList()
	{
        if (GlobalValue.g_Unique_ID == "") return;

        var request = new GetLeaderboardRequest()
        {
            StartPosition = 0,
            StatisticName = "TotalScore",
            MaxResultsCount = 20,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            (result) =>
            {
                if (RankingNodeObj == null) return;

                for (int ii = 0; ii < result.Leaderboard.Count; ii++)
                {
                    var curboard = result.Leaderboard[ii];
                    GameObject a_UserObj = Instantiate(RankingNodeObj);
                    a_UserObj.transform.SetParent(TotalRankingContent.transform, false);
                    RankingImgNode a_RkNode = a_UserObj.GetComponent<RankingImgNode>();
                    a_RkNode.RankText.text = (ii + 1).ToString();
                    a_RkNode.BestScoreText.text = curboard.StatValue.ToString("N0");
                    a_RkNode.NicknameText.text = curboard.DisplayName;
                    if (curboard.PlayFabId == GlobalValue.g_Unique_ID)
                    {
                        a_RkNode.ButtonSprite.color = new Color32(120, 212, 242, 255);
                        if (GlobalValue.g_FacebookName != "")
                            a_RkNode.FacebookImg.sprite = a_OldImage.sprite;
                    }
                }

                GetMyTotalRanking();
            },
            (error) =>
            {
                Debug.Log("리더보드 불러오기 실패");
            }
            );
    }

    void CheckGameName()
	{
        if (GlobalValue.g_GKind == GameKind.APF_YSMS)
		{
            m_GameName = "YSMS";
            m_GameText.text = "삼촌의 니편내편";
		}
        else if (GlobalValue.g_GKind == GameKind.APF_SDJR)
		{
            m_GameName = "SDJR";
            m_GameText.text = "엄마의 삼단정리";
		}
        GetRankingList(GlobalValue.g_GKind);
	}

    void ClearRank()
	{
        RankingImgNode[] RankingNodes = GameRankingContent.GetComponentsInChildren<RankingImgNode>();
        foreach (RankingImgNode a_Node in RankingNodes)
            Destroy(a_Node.gameObject);
    }

    private void DisplayProfilePic(IGraphResult ret)
    {
        if (ret.Error == null && a_OldImage != null)
        {
            a_OldImage.sprite = Sprite.Create(ret.Texture, new Rect(0, 0, ret.Texture.width, ret.Texture.height), new Vector2());
            isFBImageLoaded = true;
        }
    }

    

}
