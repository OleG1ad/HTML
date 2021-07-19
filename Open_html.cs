using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Firebase;
using Firebase.Analytics;
using Google.Play.Review;
//using UnityEngine.iOS;
using System.Collections.Generic;

public class Open_html : MonoBehaviour
{
    public GameObject WindowShowNavigationEnable;
    public GameObject WindowWebViewDisable;
    public GameObject WindowFullWebView;
    public GameObject CanvasWebView;

    public UniWebView webView;
    public SiteProgressLoading siteProgressLoading;
    private CustomSlider customSlider;

    [SerializeField] private int _urlInt = 0;
    private int _requestСounter = 0;

    [SerializeField] private double sekPauseStartJS2; // на какое время приостанавливается выполнение JS2 в Update()
    [SerializeField] private double sekPausePlayReview; // пауза на открытие окна оценки качества приложения в IEnumerator CheckRatingKeywords(string url)

    [SerializeField] private string oneSignalAppID; //"YOUR_ONESIGNAL_APP_ID"
    [SerializeField] private string[] _urlRest;
    [SerializeField] private string _html;
    [SerializeField] private string _offlineMode;
    [SerializeField] private string _showContentApi;
    [SerializeField] private string _siteLink;
    [SerializeField] private string _yandexAPI_KEY;
    [SerializeField] private string _showNavigation;
    [SerializeField] private string _uid;
    [SerializeField] private string _showUid;
    [SerializeField] private string _url;
    [SerializeField] private string _resetLock;
    private string _clearHtml;
    [SerializeField] private string[] ClearHtml;
    [SerializeField] private string _enableExecuteJavascript;
    private string _keywordsBlockJavascript;
    [SerializeField] private string[] KeywordsBlockJavascript;
    private string _ratingKeywords;
    [SerializeField] private string[] RatingKeywords;
    private string _clickHtml;
    [SerializeField] private string[] ClickHtml;
    private string _blockUrlKeywords;
    [SerializeField] private string[] BlockUrlKeywords;
    [SerializeField] private string _redirectLink;
    private string _redirectKeywords;
    [SerializeField] private string[] RedirectKeywords;
    [SerializeField] private string _checkUrlFixedUpdate;
    [SerializeField] private string _checkUrlUpdate;

    public Text uidText;
    public GameObject uidBlackRawImage;

    [SerializeField] private bool isJS;
    [SerializeField] private bool isClearHtml;
    [SerializeField] private bool isClickHtml;
    [SerializeField] private bool isFlagUrlJS;
    [SerializeField] private bool isBlackLockConnect;
    [SerializeField] private bool isOnOfflineMode;
    [SerializeField] private bool isErrorGetRequest;
    [SerializeField] private bool isCheckRatingKeywords;

    public AppMetrica appMetrica;
    public GameObject AppMetricaScript;
    public GameObject menuPanel;
    public GameObject tumblerImage;

    // Create instance of ReviewManager
    private ReviewManager _reviewManager;

    // Start is called before the first frame update
    private void Awake()
    {
        isOnOfflineMode = true;
    }
    void Start()
    {
        LaunchFareBase();

        // Прогресс запуск
        menuPanel.SetActive(true);
        tumblerImage.SetActive(true);

        siteProgressLoading = GetComponent<SiteProgressLoading>();
        siteProgressLoading.ActiveProgressMenu();
        
        tumblerImage = GameObject.FindGameObjectWithTag("TumblerImage");
        customSlider = tumblerImage.GetComponent<CustomSlider>();  

#if UNITY_ANDROID
        _reviewManager = new ReviewManager();
#endif
#if UNITY_ANDROID || UNITY_IOS
        _html = UniWebViewHelper.StreamingAssetURLForPath("offline_content/index.html");
#endif
        LoadUserData();
        StartCoroutine(GetRequest(_urlRest[_urlInt]));

#if UNITY_ANDROID || UNITY_IOS
        LaunchOneSignal();
#endif
    }
    private void LoadUserData()
    {
        //SaveUserData(); //если требуется обновить для теста userUid

        string key = "userData";
        if (PlayerPrefs.HasKey(key))
        {
            string value = PlayerPrefs.GetString(key);

            SaveData data = JsonUtility.FromJson<SaveData>(value);

            if (data.uid != null)
            {
                _uid = data.uid;
                //Debug.Log("_uid = data.uid Load()" + _uid);
                uidText.text = _uid;
            }
            else
            {
                SaveUserDataUID();
            }

            if (data.blacklockConnect != null)
            {
                if (data.blacklockConnect == "Enable")
                {
                    isBlackLockConnect = true;
                    //Debug.Log("LoadUserData() if (data.blacklockConnect == Enable) isBlacklockConnect = true;" + isBlackLockConnect);                    
                }
                else
                {
                    isBlackLockConnect = false;
                    //Debug.Log("LoadUserData() if (data.blacklockConnect == Disable) isBlacklockConnect = false;" + isBlackLockConnect);
                }
            }
        }
        else
        {
            SaveUserDataUID();
        }
    }
    public void BlackMode()
    {
        isBlackLockConnect = true;
        webView.Load(_html);
        //Debug.Log("BlackMode() webView.Load(_html);");
    }

    void SaveUserDataBlackMode()
    {
        string key = "userData";

        SaveData data = new SaveData();
        if (isBlackLockConnect)
        {
            data.blacklockConnect = "Enable";
        }
        else
        {
            data.blacklockConnect = "Disable";
        }
        //Debug.Log(data.blacklockConnect);
        string value = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, value);

        PlayerPrefs.Save();
    }

    public void SaveUserDataUID()
    {
        string key = "userData";

        SaveData data = new SaveData();
        Guid guidID = Guid.NewGuid();
        Guid userGUID = Guid.Parse(guidID.ToString());
        //Debug.Log(userGUID);
        string strGUID = userGUID.ToString();
        string thirteenGUID = strGUID.Substring(0, 13);
        //Debug.Log("thirteenGUID :" + thirteenGUID);
        string firstDay = DateTime.Now.ToString("dd-MM-yyyy");
        //Debug.Log(firstDay);
        string userGuidDay = thirteenGUID + "-" + firstDay;
        //string userGuidDay = userGUID + "-" + firstDay;
        uidText.text = userGuidDay;
        //Debug.Log(uidText.text);
        data.uid = userGuidDay;
        //Debug.Log(data.uid);

        string value = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, value);

        PlayerPrefs.Save();
        _uid = data.uid;
        Debug.Log("_uid = data.uid Save()" + _uid);
    }
    private IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;


            // switch проверка работает только в версии Unity 2020, для 2019 заменить на if
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);

                    isErrorGetRequest = true;
                    Debug.Log("errorGetRequest1 = true;");
                    NextGetRequest(); // если все запросы к хосту вызывают ошибки                    
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);

                    isErrorGetRequest = true;
                    Debug.Log("errorGetRequest2 = true;");
                    NextGetRequest();  // если все запросы к хосту вызывают ошибки                    
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                    ParseWebRequest(webRequest.downloadHandler.text);
                    break;
            }
        }

        if (!isErrorGetRequest)
        {
            ShowContentAPI(); //проверка на запуск обмена валют

            UrlFromUid();

            var webViewGameObject = new GameObject("UniWebView");
            webView = webViewGameObject.AddComponent<UniWebView>();

            PointerUid();

            ShowNawigation();

            if (isBlackLockConnect)
            {
                BlackMode();
                //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest) if (isBlackLockConnect) BlackMode();");
            }
            else
            {
                webView.Load(_url);
                //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest) webView.Load(_url);" + _url);
            }
            isFlagUrlJS = true;
            //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest)  stopFlagUrlJS = true;" + stopFlagUrlJS);

            webView.OnPageProgressChanged += OnPageProgressChanged;

            //isCheckUrlUpdate = true;

            // Устанавливает, должна ли быть активирована кнопка возврата устройства для выполнения операции «назад» или «закрытия» на Android
            webView.SetBackButtonEnabled(false);
                        
            //webView.Show(true, UniWebViewTransitionEdge.Right);
            //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest) webView.Show(_url);" + _url);

            isCheckRatingKeywords = true;
            //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest) isCheckRatingKeywords: " + isCheckRatingKeywords);

            webView.OnPageStarted += OnPageStarted;
            //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest)  webView.OnPageStarted += OnPageStarted;" + _url);

            webView.OnPageFinished += OnPageFinished;
            //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest)  webView.OnPageStarted += OnPageFinished;" + _url);

            webView.OnPageErrorReceived += OnPageErrorReceived;
            //Debug.Log("IEnumerator GetRequest if (!_errorGetRequest)  webView.OnPageStarted += OnPageErrorReceived;" + _url);
        }
    }
    void ShowContentAPI()
    {
        if (_showContentApi == "Enable")
        {
            SceneManager.LoadScene("ContentApi", LoadSceneMode.Single);
        }
    }
    void LaunchOneSignal()
    {
        // Uncomment this method to enable OneSignal Debugging log output 
        OneSignal.SetLogLevel(OneSignal.LOG_LEVEL.VERBOSE, OneSignal.LOG_LEVEL.NONE);

        // Replace 'YOUR_ONESIGNAL_APP_ID' with your OneSignal App ID.
        OneSignal.StartInit(/*"YOUR_ONESIGNAL_APP_ID"*/oneSignalAppID)
          .HandleNotificationOpened(OneSignalHandleNotificationOpened)
          .Settings(new Dictionary<string, bool>() {
      { OneSignal.kOSSettingsAutoPrompt, false },
      { OneSignal.kOSSettingsInAppLaunchURL, false } })
          .EndInit();

        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
        Debug.Log("OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;");

        // iOS - Shows the iOS native notification permission prompt.
        //   - Instead we recomemnd using an In-App Message to prompt for notification 
        //     permission to explain how notifications are helpful to your users.
        OneSignal.PromptForPushNotificationsWithUserResponse(OneSignalPromptForPushNotificationsReponse);
        Debug.Log("OneSignal.PromptForPushNotificationsWithUserResponse(OneSignalPromptForPushNotificationsReponse);");
    }
    // Gets called when the player opens a OneSignal notification.
    private static void OneSignalHandleNotificationOpened(OSNotificationOpenedResult result)
    {
        // Place your app specific notification opened logic here.
        Debug.Log("private static void OneSignalHandleNotificationOpened(OSNotificationOpenedResult result) " + result);
    }

    // iOS - Fires when the user anwser the notification permission prompt.
    private void OneSignalPromptForPushNotificationsReponse(bool accepted)
    {
        // Optional callback if you need to know when the user accepts or declines notification permissions.
        Debug.Log("private void OneSignalPromptForPushNotificationsReponse(bool accepted) " + accepted);
    }
    void LaunchFareBase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                var app = FirebaseApp.DefaultInstance;
                Debug.Log(message: "var app = FirebaseApp.DefaultInstance;");

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                //_readyFirebase = true;
            }
            else
            {
                Debug.LogError(String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }
    void LaunchAppMetrica()
    {
        var appMetricaGameObject = new GameObject("AppMetrica");
        appMetrica = appMetricaGameObject.AddComponent<AppMetrica>();
        appMetrica.tag = "AppMetrica";

        // значение для запуска AppMetrica!
        string appKey = _yandexAPI_KEY;
        AppMetricaScript = GameObject.FindGameObjectWithTag("AppMetrica");

        if (appKey != AppMetricaScript.GetComponent<AppMetrica>().ApiKey)
        {
            AppMetricaScript.GetComponent<AppMetrica>().ApiKey = appKey;
            AppMetricaScript.GetComponent<AppMetrica>().AlternativeStart();
        }
    }
    void PointerUid()
    {
        if (_showUid == "Enable")
        {
            uidBlackRawImage.SetActive(true);
        }
        else
        {
            uidBlackRawImage.SetActive(false);
        }
    }
    void NextGetRequest()
    {
        //StopAllCoroutines();
        Debug.Log("NextGetRequest() StopAllCoroutines();" + _urlRest[_urlInt]);

        if (_urlInt <= _urlRest.Length - 1 && _requestСounter < _urlRest.Length - 1)
        {
            while (_urlInt < _urlRest.Length - 1)
            {
                _urlInt++;
                Debug.Log("if  _urlInt++: " + _urlInt);
                break;
            }
            _requestСounter++;
            Debug.Log("if  requestСounter++: " + _requestСounter);
            StartCoroutine(GetRequest(_urlRest[_urlInt]));
            Debug.Log("NextGetRequest() if  StartCoroutine(GetRequest(" + _urlRest[_urlInt]);
        }
        else
        {
            OfflineMode();
            Debug.Log("NextGetRequest() else OfflineMode();");
        }
    }
    void ParseWebRequest(string text)
    {
        JSONNode JsonParse = JSON.Parse(text);

        Debug.Log("The generated item is: OfflineMode " + JsonParse["OfflineMode"]);
        Debug.Log("The generated item is: ShowNavigation " + JsonParse["ShowNavigation"]);
        Debug.Log("The generated item is: ShowUid " + JsonParse["ShowUid"]);
        Debug.Log("The generated item is: EnableExecuteJavascript " + JsonParse["EnableExecuteJavascript"]);

        _offlineMode = JsonParse["OfflineMode"];

        _showContentApi = JsonParse["ShowContentAPI"];
        //_showContentApi = "Disable";

        _siteLink = JsonParse["SiteLink"];
        Debug.Log("_siteLink : " + _siteLink);
        if (String.IsNullOrEmpty(_siteLink) || String.IsNullOrWhiteSpace(_siteLink))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_siteLink)) : " + String.IsNullOrWhiteSpace(_siteLink));
            NextGetRequest();
        }
        else
        {
            if (!_siteLink.Contains("https") || !_siteLink.Contains("http"))
            {
                _siteLink = "https://" + _siteLink; // https://
                Debug.Log("_redirectLink is " + _siteLink);
            }
        }

        _yandexAPI_KEY = JsonParse["yandexAPI_KEY"];
        Debug.Log("_yandexAPI_KEY : " + _yandexAPI_KEY);
        if (String.IsNullOrEmpty(_yandexAPI_KEY) || String.IsNullOrWhiteSpace(_yandexAPI_KEY))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_yandexAPI_KEY)) : " + String.IsNullOrWhiteSpace(_yandexAPI_KEY));
        }
        else
        {
            LaunchAppMetrica();
        }

        _showNavigation = JsonParse["ShowNavigation"];
        _showUid = JsonParse["ShowUid"];
        _enableExecuteJavascript = JsonParse["EnableExecuteJavascript"];
        Debug.Log("_enableExecuteJavascript " + _enableExecuteJavascript);

        _clearHtml = JsonParse["ClearHtml"];
        Debug.Log("_clearHtml : " + _clearHtml);
        if (String.IsNullOrEmpty(_clearHtml) || String.IsNullOrWhiteSpace(_clearHtml))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_clearHtml)) : " + String.IsNullOrWhiteSpace(_clearHtml));
            isClearHtml = false;
        }
        else
        {
            ClearHtml = _clearHtml.Split('|');
            //Debug.Log("ClearHtml[0] : " + ClearHtml[0]);
            isClearHtml = true;
        }

        _keywordsBlockJavascript = JsonParse["KeywordsBlockJavascript"];
        Debug.Log("_keywordsBlockJavascript : " + _keywordsBlockJavascript);
        if (String.IsNullOrEmpty(_keywordsBlockJavascript) || String.IsNullOrWhiteSpace(_keywordsBlockJavascript))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_keywordsBlockJavascript)) : " + String.IsNullOrWhiteSpace(_keywordsBlockJavascript));
        }
        else
        {
            KeywordsBlockJavascript = _keywordsBlockJavascript.Split('|');
            //Debug.Log("KeywordsBlockJavascript[0] : " + KeywordsBlockJavascript[0]);
        }

        _ratingKeywords = JsonParse["RatingKeywords"];
        Debug.Log("_ratingKeywords : " + _ratingKeywords);
        if (String.IsNullOrEmpty(_ratingKeywords) || String.IsNullOrWhiteSpace(_ratingKeywords))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_ratingKeywords)) : " + String.IsNullOrWhiteSpace(_ratingKeywords));
        }
        else
        {
            RatingKeywords = _ratingKeywords.Split('|');
            //Debug.Log("RatingKeywords[0] : " + RatingKeywords[0]);
        }

        _clickHtml = JsonParse["ClickHtml"];
        Debug.Log("_clickHtml : " + _clickHtml);
        if (String.IsNullOrEmpty(_clickHtml) || String.IsNullOrWhiteSpace(_clickHtml))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_clickHtml)) : " + String.IsNullOrWhiteSpace(_clickHtml));
            isClickHtml = false;
        }
        else
        {
            ClickHtml = _clickHtml.Split('|');
            //Debug.Log("ClickHtml[0] : " + ClickHtml[0]);
            isClickHtml = true;
        }

        _resetLock = JsonParse["ResetLock"];
        Debug.Log("_resetLock = JsonParse[ResetLock]; " + _resetLock);
        if (String.IsNullOrEmpty(_resetLock) || String.IsNullOrWhiteSpace(_resetLock))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_resetLock)) : " + String.IsNullOrWhiteSpace(_resetLock));
        }
        else
        {
            if (_resetLock == "Enable")
            {
                if (isBlackLockConnect)
                {
                    isBlackLockConnect = false;
                    Debug.Log("_resetLock = JsonParse[ResetLock]; if (_resetLock == Enable) if (isBlackLockConnect) isBlackLockConnect = false; " + isBlackLockConnect);
                    SaveUserDataBlackMode();
                    //ReloadCurrentScene();
                    //Debug.Log("_resetLock = JsonParse[ResetLock]; if (_resetLock == Enable) if (isBlackLockConnect) SaveUserDataBlackMode(); ReloadCurrentScene();");
                }
            }
        }

        _blockUrlKeywords = JsonParse["BlockUrlKeywords"];
        Debug.Log("_blockUrlKeywords : " + _blockUrlKeywords);
        if (String.IsNullOrEmpty(_blockUrlKeywords) || String.IsNullOrWhiteSpace(_blockUrlKeywords))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_blockUrlKeywords)) : " + String.IsNullOrWhiteSpace(_blockUrlKeywords));
            //isCheckBlockUrlKeywords = false;
        }
        else
        {
            BlockUrlKeywords = _blockUrlKeywords.Split('|');
            //Debug.Log("BlockUrlKeywords[0] : " + BlockUrlKeywords[0]);
            //isCheckBlockUrlKeywords = true;
        }

        _redirectKeywords = JsonParse["RedirectKeywords"];
        Debug.Log("_redirectKeywords : " + _redirectKeywords);
        if (String.IsNullOrEmpty(_redirectKeywords) || String.IsNullOrWhiteSpace(_redirectKeywords))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_redirectKeywords)) : " + String.IsNullOrWhiteSpace(_redirectKeywords));
            //isCheckRedirectKeywords = false;
        }
        else
        {
            RedirectKeywords = _redirectKeywords.Split('|');
            //Debug.Log("RedirectKeywords[0] : " + RedirectKeywords[0]);
            //isCheckRedirectKeywords = true;
        }

        _redirectLink = JsonParse["RedirectLink"];
        Debug.Log("_redirectLink = JsonParse[RedirectLink]; " + _redirectLink);
        if (String.IsNullOrEmpty(_redirectLink) || String.IsNullOrWhiteSpace(_redirectLink))
        {
            Debug.Log("String.IsNullOrWhiteSpace(_redirectLink)) : " + String.IsNullOrWhiteSpace(_redirectLink));
        }
        else
        {
            bool redirectLinkAny = RedirectKeywords.Any(p => _redirectLink.Contains(p)); //Проверка: есль ли хоть одно слово для блока RedirectKeywords страницы в _redirectLink
            Debug.Log("bool redirectLinkAny: " + redirectLinkAny);

            if (!_redirectLink.Contains("https") || !_redirectLink.Contains("http"))
            {
                _redirectLink = "https://" + _redirectLink; // https://
                Debug.Log("_redirectLink is " + _redirectLink);
            }

            if (redirectLinkAny)
            {
                Debug.Log("Проверка: есль ли хоть одно слово для блока RedirectKeywords страницы в _redirectLink");
            }
        }

        isErrorGetRequest = false;
        Debug.Log("ParseWebRequest()  errorGetRequest = false;");
        isOnOfflineMode = false;
        Debug.Log("ParseWebRequest()  errorGetRequest = false;");
    }
    void ShowNawigation()
    {
        if (_showNavigation == "Enable")
        {
            WindowShowNavigationEnable.SetActive(true);
            WindowWebViewDisable.SetActive(false);
            WindowFullWebView.SetActive(false);
            RectTransform panel = WindowShowNavigationEnable.GetComponent<RectTransform>();
            webView.ReferenceRectTransform = panel;
        }
        else if (_showNavigation != "Enable" && _showUid == "Enable")
        {
            WindowShowNavigationEnable.SetActive(false);
            WindowWebViewDisable.SetActive(true);
            WindowFullWebView.SetActive(false);
            RectTransform panel = WindowWebViewDisable.GetComponent<RectTransform>();
            webView.ReferenceRectTransform = panel;
        }
        else
        {
            WindowShowNavigationEnable.SetActive(false);
            WindowWebViewDisable.SetActive(false);
            WindowFullWebView.SetActive(true);
            RectTransform panel = WindowFullWebView.GetComponent<RectTransform>();
            webView.ReferenceRectTransform = panel;
        }
    }
    void OfflineMode() //офлайн режим
    {
        isOnOfflineMode = true;
        Debug.Log("OfflineMode() onOfflineMode = true;");

        StopAllCoroutines();
        Debug.Log("void OfflineMode() StopAllCoroutines();");

        var webViewGameObject = new GameObject("UniWebView"); // Запускаем вью в офлайн режиме
        webView = webViewGameObject.AddComponent<UniWebView>();

        _url = _html; //офлайн режим
        Debug.Log("void OfflineMode() _url is " + _url);
        _showNavigation = "Disable";
        _showUid = "Enable";
        ShowNawigation();
        UrlFromUid();
        PointerUid();
        webView.Load(_url);
        webView.Show(true, UniWebViewTransitionEdge.Right);
    }
    void UrlFromUid()
    {
        Debug.Log("UrlFromUid()");

        if (Application.internetReachability != NetworkReachability.NotReachable) // Проверяем, может ли устройство подключиться к Интернету 
        {
            Debug.Log("_offlineMode is " + _offlineMode);

            if (_offlineMode == "Enable")
            {
                _url = _html; //офлайн режим
                Debug.Log("_url is " + _url);
                isOnOfflineMode = true;
                Debug.Log("onOfflineMode = true;");
                //webView.Show();
            }
            else
            {
                Debug.Log(_siteLink);
                if (_siteLink.Contains("{uid}"))
                {
                    string siteLinkUid = _siteLink.Replace("{uid}", _uid);
                    _url = siteLinkUid;
                    Debug.Log("string siteLinkUid = _siteLink.Replace(); " + _url);
                }
                else
                {
                    _url = _siteLink;
                }
                Debug.Log("_url is " + _url);
            }
        }
        else
        {
            _url = _html; //офлайн режим
            Debug.Log("_url is " + _url);
            isOnOfflineMode = true;
            Debug.Log("onOfflineMode = true;");
            //webView.Show();
        }
    }
    public void BackButton()
    {
        if (webView.CanGoBack)
        {
            if (isBlackLockConnect)
            {
                BlackMode();
                Debug.Log("Update() if (Input.GetKeyUp(KeyCode.Escape)) if (isBlackLockConnect) BlackMode(); ReloadCurrentScene()");
                //ReloadCurrentScene();
                //Debug.Log("if (isBlackLockConnect) ReloadCurrentScene()");
            }
            else
            {
                webView.GoBack();
            }
        }
    }
    public void ForwardButton()
    {
        if (webView.CanGoForward)
        {
            if (isBlackLockConnect)
            {
                BlackMode();
                Debug.Log("Update() if (Input.GetKeyUp(KeyCode.Escape)) if (isBlackLockConnect) BlackMode(); ReloadCurrentScene()");
                //ReloadCurrentScene();
                //Debug.Log("if (isBlackLockConnect) ReloadCurrentScene()");
            }
            else
            {
                webView.GoForward();
            }
        }
    }
    IEnumerator JSkeywordsBlock(string url)
    {
        //Debug.Log("JSkeywordsBlock(string url);" + url);

        if (_enableExecuteJavascript == "Enable")
        {
            //Debug.Log("JSkeywordsBlock() _enableExecuteJavascript == Enable");
            bool hasBlockElements = KeywordsBlockJavascript.Any(); //Проверка: Если нет ни одного слова в списке
            //Debug.Log("JSkeywordsBlock() bool hasBlockElements: " + hasBlockElements);

            if (hasBlockElements && (!String.IsNullOrEmpty(_keywordsBlockJavascript) || !String.IsNullOrWhiteSpace(_keywordsBlockJavascript)))
            {
                bool keywordsBlockAny = KeywordsBlockJavascript.Any(p => url.Contains(p)); //Проверка: есль ли хоть одно стоп-слово в url                
                //Debug.Log("JSkeywordsBlock() bool keywordsBlockAny: " + keywordsBlockAny);

                //Debug.Log("JSkeywordsBlock() Loading started for url: " + url);

                if (keywordsBlockAny)
                {
                    isJS = false;
                    //print("JSkeywordsBlock() Loading started for url: " + url);
                    //Debug.Log("JSkeywordsBlock() javascript = false_3 " + javascript);

                    isFlagUrlJS = true;
                    //Debug.Log("JSkeywordsBlock() stopFlagUrl = true;" + stopFlagUrlJS);
                }
                else
                {
                    isJS = true;
                    //Debug.Log("JSkeywordsBlock() javascript = true" + javascript);
                }
            }
            else
            {
                isJS = true;
                //Debug.Log("JSkeywordsBlock() javascript = " + javascript);
            }
        }
        else
        {
            isJS = false;
            //Debug.Log("JSkeywordsBlock() javascript = false_1 " + javascript);

            isFlagUrlJS = true;
            //Debug.Log("JSkeywordsBlock() stopFlagUrl = true;" + stopFlagUrlJS);
        }
        JavaScript1();
        //Debug.Log("JSkeywordsBlock() JavaScript();");
        yield return new WaitForSeconds((float)sekPauseStartJS2);
    }
    void JavaScript1() //запуск javascript во вновь открытом вью сразу после проверки стоп-слов
    {
        //Debug.Log("JavaScript();");
        if (isJS)
        {
            if (isClearHtml)
            {
                //Debug.Log("JavaScript1() if (isJS) if (isClearHtml)");
                for (int i = 0; i < ClearHtml.Length; i++)
                {
                    webView.AddJavaScript("javascript:document.getElementById(''); x=document.getElementsByClassName('" +
                        ClearHtml[i] +
                        "'); for(i=0;i<x.length;i++){x[i].innerHTML='';} void(0);");
                    //Debug.Log("JavaScript1() ClearHtml[i] adding finished " + ClearHtml[i]);
                }
                //Debug.Log("JavaScript1() if (isJS) if (isClearHtml) adding finished");
            }

            if (isClickHtml)
            {
                //Debug.Log("JavaScript1() if (isJS) if (isClickHtml)");
                for (int i = 0; i < ClickHtml.Length; i++)
                {
                    webView.AddJavaScript("javascript:document.getElementById(''); x=document.getElementsByClassName('" +
                        ClickHtml[i] + "'); for(i=0;i<x.length;i++){x[i].click();} void(0);");
                    //Debug.Log("JavaScript1() ClickHtml[i] adding finished " + ClickHtml[i]);
                }
                //Debug.Log("JavaScript1() if (isJS) if (isClickHtml) adding finished");
            }

            isFlagUrlJS = true;
            //Debug.Log("JavaScript1() 1 adding finished  stopFlagUrl = true;");
        }
    }
    IEnumerator JavaScript2() //Сопрограмма запускающая javascript в открытом вью каждую 1 сек
    {
        //Debug.Log("JavaScript2()");
        if (isJS)
        {
            if (isClearHtml)
            {
                //Debug.Log("JavaScript2() if (isJS) if (isClearHtml)");
                for (int i = 0; i < ClearHtml.Length; i++)
                {
                    webView.AddJavaScript("javascript:document.getElementById(''); x=document.getElementsByClassName('" +
                        ClearHtml[i] +
                        "'); for(i=0;i<x.length;i++){x[i].innerHTML='';} void(0);");
                    //Debug.Log("JavaScript2() ClearHtml[i] adding finished " + ClearHtml[i]);
                }
                //Debug.Log("JavaScript2() if (isJS) if (isClearHtml) adding finished");
            }

            if (isClickHtml)
            {
                //Debug.Log("JavaScript2() if (isJS) if (isClickHtml)");
                for (int i = 0; i < ClickHtml.Length; i++)
                {
                    webView.AddJavaScript("javascript:document.getElementById(''); x=document.getElementsByClassName('" +
                        ClickHtml[i] + "'); for(i=0;i<x.length;i++){x[i].click();} void(0);");
                    //Debug.Log("JavaScript2() ClickHtml[i] adding finished " + ClickHtml[i]);
                }
                //Debug.Log("JavaScript2() if (isJS) if (isClickHtml) adding finished");
            }
        }
        yield return new WaitForSeconds((float)sekPauseStartJS2);
        isFlagUrlJS = true;
        //Debug.Log("JavaScript 2 finished  stopFlagUrl = true;");
    }
    public void ReloadCurrentScene()
    {
        //StopAllCoroutines();
        // Получаем имя текущей сцены
        string sceneName = SceneManager.GetActiveScene().name;
        // Загружаем её
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    // RatingKeywords
    IEnumerator CheckRatingKeywords(string url)
    {
        //Debug.Log("CheckRatingKeywords(string url);" + url);

        bool hasRatingKeywords = RatingKeywords.Any(); // Есть ли хоть одно слово в списке
        //Debug.Log("IEnumerator CheckRatingKeywords(url) bool hasRatingKeywords: " + hasRatingKeywords);

        if (hasRatingKeywords && (!String.IsNullOrEmpty(_ratingKeywords) || !String.IsNullOrWhiteSpace(_ratingKeywords)))
        {
            bool ratingKeywordsAny = RatingKeywords.Any(p => url.Contains(p)); //Проверка: есль ли хоть одно слово для отзыва в url
            //Debug.Log("IEnumerator CheckRatingKeywords(url) Loading started for url: " + url);

            if (ratingKeywordsAny)
            {
                //Debug.Log("IEnumerator CheckRatingKeywords(url) if (ratingKeywordsAny): " + ratingKeywordsAny);

#if UNITY_IOS || UNITY_EDITOR
                yield return new WaitForSeconds((float)sekPausePlayReview);
                //Debug.Log("yield return new WaitForSeconds((float)sekPausePlayReview); iOS sek: " + sekPausePlayReview);
                UnityEngine.iOS.Device.RequestStoreReview(); // iOS
                //Debug.Log("IEnumerator CheckRatingKeywords(url) Device.RequestStoreReview(); // iOS");
#endif

#if UNITY_ANDROID
                var requestFlowOperation = _reviewManager.RequestReviewFlow();
                //Debug.Log("var requestFlowOperation = _reviewManager.RequestReviewFlow();");
                yield return requestFlowOperation;
                //Debug.Log("yield return requestFlowOperation;");
                if (requestFlowOperation.Error != ReviewErrorCode.NoError)
                {
                    Debug.Log("requestFlowOperation.Error.ToString(): " + requestFlowOperation.Error.ToString());
                    // Log error. For example, using requestFlowOperation.Error.ToString().
                    yield break;
                }
                var _playReviewInfo = requestFlowOperation.GetResult();
                //Debug.Log("var _playReviewInfo = requestFlowOperation.GetResult();");

                yield return new WaitForSeconds((float)sekPausePlayReview);
                //Debug.Log("yield return new WaitForSeconds((float)sekPausePlayReview); Android sek: " + sekPausePlayReview);

                var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
                //Debug.Log("var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);");
                yield return launchFlowOperation;
                //Debug.Log("yield return launchFlowOperation;");
                _playReviewInfo = null; // Reset the object
                //Debug.Log("_playReviewInfo = null; // Reset the object");
                if (launchFlowOperation.Error != ReviewErrorCode.NoError)
                {
                    Debug.Log("requestFlowOperation.Error.ToString(): " + requestFlowOperation.Error.ToString());
                    // Log error. For example, using requestFlowOperation.Error.ToString().
                    yield break;
                }
                // The flow has finished. The API does not indicate whether the user
                // reviewed or not, or even whether the review dialog was shown. Thus, no
                // matter the result, we continue our app flow.
#endif
                //yield return new WaitForSeconds(1.0f);
                //Debug.Log("yield return new WaitForSeconds(1.0f);");

                isCheckRatingKeywords = false;
                //Debug.Log("IEnumerator CheckRatingKeywords(url) if (ratingKeywordsAny) isCheckRatingKeywords: " + isCheckRatingKeywords);
            }
            else
            {
                yield return new WaitForSeconds(2.0f);
                //Debug.Log("yield return new WaitForSeconds(2.0f);");

                isCheckRatingKeywords = true;
                //Debug.Log("IEnumerator CheckRatingKeywords(url) if (ratingKeywordsAny) isCheckRatingKeywords: " + isCheckRatingKeywords);                
            }
        }
        else
        {
            isCheckRatingKeywords = false;
            //Debug.Log("IEnumerator CheckRatingKeywords(url) if (hasRatingKeywords && (!String.IsNullOrEmpty(_ratingKeywords) || !String.IsNullOrWhiteSpace(_ratingKeywords))): " + isCheckRatingKeywords);
        }
    }
    void CheckBlacklockСonnect(string url)
    {
        if (url.Contains("blacklock-connect"))
        {
            isBlackLockConnect = true;
            SaveUserDataBlackMode();
            BlackMode();
            Debug.Log("void CheckBlacklockСonnect(string url) if(url.Contains(blacklock - connect)) SaveUserDataBlackMode(); BlackMode();");
        }
    }
    void CheckBlockUrlKeywords(string url)
    {
        //Debug.Log("CheckBlockUrlKeywords(string url);" + url);

        bool hasBlockUrlKeywords = BlockUrlKeywords.Any(); // Есть ли хоть одно слово в списке
        //Debug.Log("IEnumerator CheckBlockUrlKeywords(url) bool hasBlockUrlKeywords: " + hasBlockUrlKeywords);

        if (hasBlockUrlKeywords && (!String.IsNullOrEmpty(_blockUrlKeywords) || !String.IsNullOrWhiteSpace(_blockUrlKeywords)))
        {
            bool blockUrlKeywordsAny = BlockUrlKeywords.Any(p => url.Contains(p)); //Проверка: есль ли хоть одно слово для блока страницы в url
            //Debug.Log("IEnumerator CheckBlockUrlKeywords(url) Loading started for url: " + url);

            if (blockUrlKeywordsAny)
            {
                Debug.Log("IEnumerator CheckBlockUrlKeywords(url) if (blockUrlKeywordsAny): " + blockUrlKeywordsAny);
                webView.Stop();
                //Debug.Log("void CheckBlockUrlKeywords(string url) if (blockUrlKeywordsAny) webView.Stop();");
                if (webView.CanGoBack)
                {
                    webView.GoBack();
                    Debug.Log("void CheckBlockUrlKeywords(string url) if (blockUrlKeywordsAny) if (webView.CanGoBack) webView.GoBack();");
                }
                else
                {
                    webView.Reload();
                    Debug.Log("void CheckBlockUrlKeywords(string url) if (blockUrlKeywordsAny) if (!webView.CanGoBack) webView.Reload();");

                }
            }
        }
    }
    void CheckRedirectKeywords(string url)
    {
        //Debug.Log("CheckRedirectKeywords(string url);" + url);

        bool hasRedirectKeywords = RedirectKeywords.Any(); // Есть ли хоть одно слово в списке RedirectKeywords
        //Debug.Log("IEnumerator CheckRedirectKeywords(url) bool hasRedirectKeywords: " + hasRedirectKeywords);

        if (hasRedirectKeywords && (!String.IsNullOrEmpty(_redirectKeywords) || !String.IsNullOrWhiteSpace(_redirectKeywords)))
        {
            bool redirectKeywordsAny = RedirectKeywords.Any(p => url.Contains(p)); //Проверка: есль ли хоть одно слово для блока RedirectKeywords страницы в url
            //Debug.Log("IEnumerator CheckRedirectKeywords(url) bool redirectKeywordsAny: " + redirectKeywordsAny);

            if (redirectKeywordsAny)
            {
                Debug.Log("IEnumerator CheckRedirectKeywords(url) if (redirectKeywordsAny): " + redirectKeywordsAny);
                webView.Stop();
                //Debug.Log("void CheckRedirectKeywords(string url) if (redirectKeywordsAny) webView.Stop();");

                webView.Load(_redirectLink);
                Debug.Log("void CheckRedirectKeywords(string url) if (redirectKeywordsAny) if (_redirectLink != url) iwebView.Load(_redirectLink); " + _redirectLink);
            }
        }
    }
    void OnPageStarted(object view, string url)
    {
        this.webView = (UniWebView)view;
        webView.Hide(true, UniWebViewTransitionEdge.Left);

        // Прогресс запуск
        menuPanel.SetActive(true);
        tumblerImage.SetActive(true);
        
        siteProgressLoading.ActiveProgressMenu();

        Debug.Log("Работает void OnPageStarted(object view, string url) : " + view + url);

        if (!isOnOfflineMode && !isBlackLockConnect && !isErrorGetRequest)
        {
            // Проверка линка на blacklock-connect
            CheckBlacklockСonnect(url);
            Debug.Log("CheckBlacklockСonnect(url);" + url);
        }

        if (!isOnOfflineMode && !isBlackLockConnect && !isErrorGetRequest && (_redirectLink != url))
        {
            CheckRedirectKeywords(url);
            Debug.Log("CheckRedirectKeywords(url);" + url);
        }

        if (!isOnOfflineMode && !isBlackLockConnect && !isErrorGetRequest)
        {
            CheckBlockUrlKeywords(url);
            Debug.Log("CheckBlockUrlKeywords(url);" + url);
        }
    }

    void OnPageProgressChanged(object view, float progress) //Не работает!!!!!!!!!!!!!!!!!!!!!! Отсутствует передача прогресса по событию!
    {
        this.webView = (UniWebView)view;
        
        Debug.Log("Progress void OnPageProgressChanged : " + view + progress);
    }

    void OnPageFinished(object view, int statusCode, string url)
    {
        this.webView = (UniWebView)view;
       
        StartCoroutine(WebViewShow(view));        

        Debug.Log("OnPageFinished(object view, int statusCode, string url) : " + view + statusCode + url);
        // Android не предоставлял способ получения кода состояния HTTP до уровня API 23 (Android 6).
        // Это status Code не заслуживает доверия и всегда будет 200 на устройствах Android с системой до Android 6.
        if (!isOnOfflineMode && !isBlackLockConnect && !isErrorGetRequest)
        {
            if (isFlagUrlJS) // флаг готовности принимать новый url JS
            {
                //call a function here
                StartCoroutine(JSkeywordsBlock(url));
                //Debug.Log("JSkeywordsBlock(url);" + url);

                isFlagUrlJS = false;
                //Debug.Log("Update() webView.OnPageStarted stopFlagUrl = false;");
            }

            if (isCheckRatingKeywords)
            {
                StartCoroutine(CheckRatingKeywords(url));
                //Debug.Log("CheckRatingKeywords(url);" + url);

                isCheckRatingKeywords = false;
                //Debug.Log("Update() webView.OnPageStarted isCheckRatingKeywords = false;");
            }
        }
    }

    private IEnumerator WebViewShow(object view)
    {
        this.webView = (UniWebView)view;

        yield return new WaitForSeconds(5f);
        //siteProgressLoading.SkipProgressMenuPanel();
        webView.Show(true, UniWebViewTransitionEdge.Right);
    }

    void OnPageErrorReceived(object view, int error, string message)
    {
        this.webView = (UniWebView)view;

        Debug.Log("void OnPageErrorReceived(object view, int error, string message) : " + view + error + message);

        /*if (error == 999)
        {
            Debug.Log("наше вмешательство ошибка: " + error);
        }
        else
        {
            if (webView.CanGoBack)
            {
                if (isBlackLockConnect)
                {
                    BlackMode();
                    Debug.Log("void OnPageErrorReceived if (webView.CanGoBack)  if (isBlackLockConnect) BlackMode(); ReloadCurrentScene()");
                }
                else
                {
                    webView.GoBack();
                    Debug.Log("void OnPageErrorReceived if (webView.CanGoBack) webView.GoBack();");

                    //webView.Load("about:blank");
                    //Debug.Log("webView.Load(about: blank);");
                }
            }
        }*/
    }
    void Update()
    {
        // Check Input in Update():
#if UNITY_ANDROID
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Debug.Log("Back Android Button was clicked.");

            if (isBlackLockConnect)
            {
                BlackMode();
                Debug.Log("Update() if (Input.GetKeyUp(KeyCode.Escape)) if (isBlackLockConnect) BlackMode(); ReloadCurrentScene()");
            }
            else
            {
                ReloadCurrentScene();
                Debug.Log("Update() if (Input.GetKeyUp(KeyCode.Escape)) ReloadCurrentScene()");
            }
        }
#endif
        if (!isOnOfflineMode && !isBlackLockConnect)
        {
            if (!isErrorGetRequest)
            {
                StartCoroutine(JavaScript2());
                //Debug.Log("Update() StartCoroutine(JavaScript2());");                
            }
        }        
    }
}

//Debug.Log("");