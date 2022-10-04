using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using Facebook.Unity;
//----------------- 이메일형식이 맞는지 확인하는 방법 스크립트
using System.Globalization;
using System.Text.RegularExpressions;
using System;


public class TitleMgr : MonoBehaviour
{
    public Button m_GameStartBtn = null;
    bool invalidEmailType = false;       // 이메일 포맷이 올바른지 체크
    bool isValidFormat = false;          // 올바른 형식인지 아닌지 체크
    public Text m_MessageText = null;
    float ShowMsTimer = 0.0f;

    [Header("-------- Login Panel --------")]
    public GameObject m_LoginPanelObj = null;
    public InputField m_IDInputField = null;
    public InputField m_PWInputField = null;
    public Button m_LoginBtn = null;
    public Button m_SignUpBtn = null;
    public Button m_CloseLgPanelBtn = null;
    public Button m_FacebookLoginBtn = null;

    [Header("-------- SignUp Panel --------")]
    public GameObject m_SignUpPanelObj = null;
    public InputField m_SUIDInputField = null;
    public InputField m_SUPWInputField = null;
    public InputField m_SUPWCfmInputField = null;
    public InputField m_SUNickInputField = null;
    public Button m_CreateAccBtn = null;
    public Button m_CloseSUPanelBtn = null;


    [HideInInspector] public static bool m_isFBLoggedIn = false;


    // Start is called before the first frame update
    void Start()
    {
        if (m_GameStartBtn != null)
            m_GameStartBtn.onClick.AddListener(() =>
			{
                m_LoginPanelObj.SetActive(true);
			});

        if(m_LoginBtn != null)
			m_LoginBtn.onClick.AddListener(LoginBtnFunc);

        if (m_CloseLgPanelBtn != null)
            m_CloseLgPanelBtn.onClick.AddListener(() =>
            {
                m_LoginPanelObj.SetActive(false);
            });

        if (m_SignUpBtn != null)
            m_SignUpBtn.onClick.AddListener(() =>
            {
                m_LoginPanelObj.SetActive(false);
                m_SignUpPanelObj.SetActive(true);
            });

        if (m_CreateAccBtn != null)
            m_CreateAccBtn.onClick.AddListener(CreateAccBtnFunc);

        if (m_CloseSUPanelBtn != null)
            m_CloseSUPanelBtn.onClick.AddListener(() =>
            {
                m_SignUpPanelObj.SetActive(false);
                m_LoginPanelObj.SetActive(true);
            });

        //GlobalValue.g_FacebookName = "";
        //GlobalValue.g_Nickname = "";

#if FacebookOn
        if (m_FacebookLoginBtn != null)
            m_FacebookLoginBtn.onClick.AddListener(FaceBookLoginFunc);
		//Facebook Init
		if (!FB.IsInitialized)
		{
			FB.Init(OnFBInitSuccess, OnFBInitFailure);
		}
        else
		{
            FB.ActivateApp();
		}


#else
        if (m_FacebookLoginBtn != null)
            m_FacebookLoginBtn.interactable = false;

#endif  //FacebookOn

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
		{
            //로그인 판넬
            if (m_PWInputField.isFocused) m_IDInputField.Select();

            //회원가입 판넬
            if (m_SUNickInputField.isFocused) m_SUPWCfmInputField.Select();
            if (m_SUPWCfmInputField.isFocused) m_SUPWInputField.Select();
            if (m_SUPWInputField.isFocused) m_SUIDInputField.Select();
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            //로그인 판넬
            if (m_IDInputField.isFocused) m_PWInputField.Select();

            //회원가입 판넬
            if (m_SUIDInputField.isFocused) m_SUPWInputField.Select();
            if (m_SUPWInputField.isFocused) m_SUPWCfmInputField.Select();
            if (m_SUPWCfmInputField.isFocused) m_SUNickInputField.Select();
        }

        if (0.0f < ShowMsTimer)
        {
            ShowMsTimer -= Time.deltaTime;
            if (ShowMsTimer <= 0.0f)
                MessageOnOff("", false);        //메세지 끄기
        }

    }

    void LoginBtnFunc()
	{
        string a_IDStr = m_IDInputField.text;
        string a_PWStr = m_PWInputField.text;
        a_IDStr = a_IDStr.Trim();
        a_PWStr = a_PWStr.Trim();

        if (string.IsNullOrEmpty(a_IDStr) || string.IsNullOrEmpty(a_PWStr)) 
		{
            MessageOnOff("빈칸 없이 입력해주세요~!");
            return;
		}

        if (!(3 <= a_IDStr.Length && a_IDStr.Length < 20))
		{
			MessageOnOff("아이디는 3글자 이상 20글자 미만으로 작성해주세요~!");
            return;
		}

        if (!(6 <= a_PWStr.Length && a_PWStr.Length < 20))
		{
			MessageOnOff("비밀번호는 6글자 이상 20글자 미만으로 작성해주세요~!");
            return;
		}

        if (!CheckEmailAddress(a_IDStr))
		{
			MessageOnOff("이메일 형식이 맞지 않아요 ㅠ0ㅠ");
            return;
		}

        var option = new GetPlayerCombinedInfoRequestParams()
        {
            GetPlayerProfile = true,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true,   
			},

            GetPlayerStatistics = true,
            GetUserData = true

		};

        var request = new LoginWithEmailAddressRequest()
		{
            Email = a_IDStr,
            Password = a_PWStr,
            InfoRequestParameters = option
		};

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
	}

    void OnLoginSuccess(PlayFab.ClientModels.LoginResult result)
	{
        MessageOnOff("로그인 성공~!!");

        GlobalValue.g_Unique_ID = result.PlayFabId;

        if (result.InfoResultPayload != null)
		{
            GlobalValue.g_Nickname = result.InfoResultPayload.PlayerProfile.DisplayName;

            foreach (var eachStat in result.InfoResultPayload.PlayerStatistics)
			{
                if (eachStat.StatisticName == "YSMSBestScore")
                    GlobalValue.g_YSMSBestScore = eachStat.Value;
                else if (eachStat.StatisticName == "SDJRBestScore")
                    GlobalValue.g_SDJRBestScore = eachStat.Value;
                else if (eachStat.StatisticName == "TotalScore")
                    GlobalValue.g_TotalScore = eachStat.Value;
            }

            int a_GetValue = 0;
            foreach (var eachData in result.InfoResultPayload.UserData)
			{
                if (eachData.Key == "UserGold")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_UserGold = a_GetValue;
                }
                else if (eachData.Key == "UserGem")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_UserGem = a_GetValue;
				}
                else if (eachData.Key == "UserRice")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_RiceCount = a_GetValue;
				}
                else if (eachData.Key == "IsRiceTimer")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_IsRiceTimerStart = a_GetValue;
				}
                else if (eachData.Key == "RiceCheckTime")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_RiceCheckTime = a_GetValue;
				}
                else if(eachData.Key == "RiceCheckDate")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_RiceCheckDate = a_GetValue;
                }
                else if (eachData.Key == "YSMSBonusUGLv")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_YSMSUGLv[0] = a_GetValue;
				}
                else if (eachData.Key == "YSMSFeverUGLv")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_YSMSUGLv[1] = a_GetValue;
				}
                else if (eachData.Key == "YSMSSuperUGLv")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_YSMSUGLv[2] = a_GetValue;
				}
                else if (eachData.Key == "YSMSTutSkipOnOff")
                {
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_YSMSTutSkipYN = a_GetValue;
                }
                else if (eachData.Key == "SDJRBonusUGLv")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_SDJRUGLv[0] = a_GetValue;
				}
                else if (eachData.Key == "SDJRFeverUGLv")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_SDJRUGLv[1] = a_GetValue;
				}
                else if (eachData.Key == "SDJRSuperUGLv")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_SDJRUGLv[2] = a_GetValue;
				}
                else if (eachData.Key == "SDJRTutSkipOnOff")
				{
                    if (int.TryParse(eachData.Value.Value, out a_GetValue))
                        GlobalValue.g_SDJRTutSkipYN = a_GetValue;
				}
			}
		}

        GlobalValue.g_GMGOLD = 10000000;
        GlobalValue.g_GMGEM = 10000;
        GlobalValue.g_GMRICE = 100;
		SceneManager.LoadScene("LobbyScene");

	}

	void OnLoginFailure(PlayFabError error)
	{
        string errorReason = error.GenerateErrorReport();
        if (errorReason.Contains("User not found") || errorReason.Contains("Invalid email address")) 
            errorReason = "아이디나 비밀번호를 확인해주세요~!";
        else if (errorReason.Contains("Email address is not valid"))
            errorReason = "이메일 형식이 맞지 않아요 ㅠ0ㅠ";

        MessageOnOff("로그인 실패 : " + errorReason);
    }

    void CreateAccBtnFunc()
	{
        string a_IDStr = m_SUIDInputField.text;
        string a_PWStr = m_SUPWInputField.text;
        string a_PWCfmStr = m_SUPWCfmInputField.text;
        string a_NickStr = m_SUNickInputField.text;

        a_IDStr = a_IDStr.Trim();
        a_PWStr = a_PWStr.Trim();
        a_PWCfmStr = a_PWCfmStr.Trim();
        a_NickStr = a_NickStr.Trim();

        if (string.IsNullOrEmpty(a_IDStr) || string.IsNullOrEmpty(a_PWStr) ||
            string.IsNullOrEmpty(a_PWCfmStr) || string.IsNullOrEmpty(a_NickStr))
		{
            MessageOnOff("빈칸 없이 입력해주세요~!");
            return;
		}

        if (!(3 <= a_IDStr.Length && a_IDStr.Length < 20))
		{
			MessageOnOff("아이디는 3글자 이상 20글자 미만으로 작성해주세요~!");
            return;
		}

        if (!(6 <= a_PWStr.Length && a_PWStr.Length < 20))
		{
			MessageOnOff("비밀번호는 6글자 이상 20글자 미만으로 작성해주세요~!");
            return;
		}

        if (!(2 <= a_NickStr.Length && a_NickStr.Length < 14))
		{
			MessageOnOff("닉네임은 3글자 이상 14글자 미만으로 작성해주세요~!");
            return;
		}

        if (a_PWCfmStr != a_PWStr)
		{
            MessageOnOff("비밀번호가 일치하지 않아요 ㅠ-ㅠ");
            return;
		}

        if (!CheckEmailAddress(a_IDStr))
		{
            MessageOnOff("이메일 형식이 맞지 않아요 ㅠ0ㅠ");
            return;
		}

        var request = new RegisterPlayFabUserRequest()
        {
            Email = a_IDStr,
            Password = a_PWStr,
            DisplayName = a_NickStr,
            RequireBothUsernameAndEmail = false
		};
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
	}

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
	{
        MessageOnOff("가입 성공~!!");
        if (m_LoginPanelObj != null) m_LoginPanelObj.SetActive(true);
        if (m_SignUpPanelObj != null) m_SignUpPanelObj.SetActive(false);
        GlobalValue.g_isFirstLogin = true;
	}

    void OnRegisterFailure(PlayFabError error)
	{
        string errorReason = error.GenerateErrorReport();
        if (errorReason.Contains("already exists"))
            errorReason = "중복된 이메일이 있네요 ㅠ0ㅠ";
        else if (errorReason.Contains("address is not valid"))
            errorReason = "이메일 형식이 맞지 않아요~!";
        else if (errorReason.Contains("display name entered is not available"))
            errorReason = "이미 생성된 닉네임이에요 ㅜ0ㅜ";
        else if(errorReason.Contains("DisplayName value was 2 characters long which is outside of allowed length"))
            errorReason = "닉네임 형식이 맞지 않아요~!";

        MessageOnOff("가입 실패 : " + errorReason);
	}

    void MessageOnOff(string Mess = "", bool isOn = true)
	{
        if (isOn)
		{
            m_MessageText.text = Mess;
            m_MessageText.gameObject.SetActive(true);
            ShowMsTimer = 7.0f;
		}
        else
		{
            m_MessageText.text = "";
            m_MessageText.gameObject.SetActive(false);
		}
	}

    private bool CheckEmailAddress(string EmailStr)
    {
        if (string.IsNullOrEmpty(EmailStr)) isValidFormat = false;

        EmailStr = Regex.Replace(EmailStr, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
        if (invalidEmailType) isValidFormat = false;

        // true 로 반환할 시, 올바른 이메일 포맷임.
        isValidFormat = Regex.IsMatch(EmailStr,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase);
        return isValidFormat;
    }

    private string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            invalidEmailType = true;
        }
        return match.Groups[1].Value + domainName;
    }

#if FacebookOn
    //Facebook 관련 함수
    private void OnFBInitSuccess()
	{
        if (FB.IsInitialized == false && FB.IsLoggedIn == false)
		{
            //실패
			Debug.Log("OnInitComplete() : Facebook Init Failure");
		}
        else if (FB.IsInitialized == true && FB.IsLoggedIn == false)
		{
			//Init()만 성공
			Debug.Log("OnInitComplete() : Facebook Init Success");
		}
        else if (FB.IsInitialized == true && FB.IsLoggedIn == true)
		{
			//내부 액세스 토큰에 의해 Init()과 Login()까지 성공
			Debug.Log("OnInitComplete() : Login was Successful!");
		}
	}

    private void OnFBInitFailure(bool isGameShown)
	{
        Debug.Log("Is game showing? " + isGameShown);
        if (!isGameShown)
		{
            //facebook로그인 실패 -> 게임 종료
		}
	}

    private void FaceBookLoginFunc()
	{
        if (FB.IsLoggedIn == true)
            return;

        //FB.Login("필요한 권한들", CallBack);
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, FB_LoginCallback);
	}

    void FB_LoginCallback(ILoginResult result)
	{
        if (result.Error != null)
		{
            Debug.Log(string.Format("Auth Error : {0}", result.Error));
		}
        else
		{
            if (FB.IsLoggedIn)
			{
				Debug.Log("Facebook Login Success!!");
                Debug.Log("CurrentAccessToken string : " + 
                    AccessToken.CurrentAccessToken.TokenString);
                //facebook AccessToken을 이용한 Playfab 로그인 시도
                var option = new GetPlayerCombinedInfoRequestParams()
                {
                    GetPlayerProfile = true,
                    ProfileConstraints = new PlayerProfileViewConstraints()
                    {
                        ShowDisplayName = true,
                        ShowAvatarUrl = true
					},

                    GetPlayerStatistics = true,
                    GetUserData = true
				};

                var request = new LoginWithFacebookRequest()
				{
                    CreateAccount = true,
                    AccessToken = AccessToken.CurrentAccessToken.TokenString,
                    InfoRequestParameters = option
				};

                PlayFabClientAPI.LoginWithFacebook(request,
                    OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailure);
			}
		}
	}

    private void OnPlayfabFacebookAuthComplete(PlayFab.ClientModels.LoginResult result)
	{
        Debug.Log("Playfab Facebook Auth Complete. Session ticket : " +
            result.SessionTicket);
        GlobalValue.g_Unique_ID = result.PlayFabId;
        if (result.InfoResultPayload != null)
		{
            if (result.InfoResultPayload.PlayerProfile == null)
			{
                GlobalValue.g_Nickname = "User";
                GlobalValue.g_Exp = 0;
			}
            else
			{
                GlobalValue.g_Nickname = 
                    result.InfoResultPayload.PlayerProfile.DisplayName;
                if (GlobalValue.g_Nickname == "")
                    GlobalValue.g_Nickname = "User";
                ////경험치 가져오기
                //string a_AvatarUrl = result.InfoResultPayload.PlayerProfile.AvatarUrl;
                ////Json 파싱
                //if(string.IsNullOrEmpty(a_AvatarUrl) == false && a_AvatarUrl.Contains("{\"") == true)

                foreach (var eachStat in result.InfoResultPayload.PlayerStatistics)
				{
                    if (eachStat.StatisticName == "YSMSBestScore")
					{
                        GlobalValue.g_YSMSBestScore = eachStat.Value;
					}
                    else if (eachStat.StatisticName == "TotalScore")
					{
                        GlobalValue.g_TotalScore = eachStat.Value;
					}
				}

                int a_GetValue = 0;
                foreach (var eachData in result.InfoResultPayload.UserData)
				{
                    if (eachData.Key == "UserGold")
					{
                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                            GlobalValue.g_UserGold = a_GetValue;
					}
                    else if (eachData.Key == "UserGem")
					{
                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                            GlobalValue.g_UserGem = a_GetValue;
					}
                    else if (eachData.Key == "UserRice")
                    {
                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                            GlobalValue.g_RiceCount = a_GetValue;
                    }
                    else if (eachData.Key == "YSMSBonusUGLv")
                    {
                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                            GlobalValue.g_YSMSUGLv[0] = a_GetValue;
                    }
                    else if (eachData.Key == "YSMSFeverUGLv")
                    {
                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                            GlobalValue.g_YSMSUGLv[1] = a_GetValue;
                    }
                    else if (eachData.Key == "YSMSSuperUGLv")
                    {
                        if (int.TryParse(eachData.Value.Value, out a_GetValue) == true)
                            GlobalValue.g_YSMSUGLv[2] = a_GetValue;
                    }
                }
			}
		}
        DealWithFBMenus(FB.IsLoggedIn);
	}

    private void OnPlayfabFacebookAuthFailure(PlayFabError error)
	{
        MessageOnOff("Playfab Facebook Auth Failed : " + error.GenerateErrorReport());
	}

    private void DealWithFBMenus(bool isLoggedIn)
	{
        if (isLoggedIn)
		{
            FB.API("/me?fields=name", HttpMethod.GET, DisplayUsername);
		}
	}

    private void DisplayUsername(IResult ret)
	{
        if (ret.Error == null)
		{
            GlobalValue.g_FacebookName = ret.ResultDictionary["name"].ToString();
            m_isFBLoggedIn = true;
			SceneManager.LoadScene("LobbyScene");
		}
        else
		{
            MessageOnOff(string.Format("DisplayUsername Error {0}", ret.Error));
		}
	}
#else

#endif
}
