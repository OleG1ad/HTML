using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
//using System.Globalization;
//using UnityEngine.EventSystems;
using UnityEngine.Networking;
using SimpleJSON;
//using System.Linq;
//using Newtonsoft.Json;
//using QuickTypeEndpoints;
//using QuickTypeApiResponse;

public class ContentAPI : MonoBehaviour
{
    [SerializeField] private RawImage _targetCurrencyIconRawImage;
    [SerializeField] private RawImage _toCurrencyIconRawImage;

    [SerializeField] private string _urlApiList;
    [SerializeField] private string _urlApiLive;
    [SerializeField] private string _urlApiConvert;

    [SerializeField] private string _targetCurrencyIconUrl;

    [SerializeField] private bool _isApiListRequest;
    [SerializeField] private bool _isApiLiveRequest;
    [SerializeField] private bool _isApiConvertRequest;

    [SerializeField] private bool _isEeventTracking;

    [SerializeField] private string _targetCryptoSymbol;
    [SerializeField] private long _targetCurrencyMaxSupply;

    [SerializeField] private long _infoTimestamp;
    [SerializeField] private double _rate;
    [SerializeField] private double _result;
    [SerializeField] private bool _isReverse; //обмен в обратную сторону

    [SerializeField] private bool _isCryptoToCurrency;
    [SerializeField] private bool _isFiatToCurrency;

    [SerializeField] private bool _successApi;
    [SerializeField] private string _targetCurrencyApi;
    //[SerializeField] private bool _isErrorGetRequestApi;
    [SerializeField] private string _access_key;

    [SerializeField] private TMP_Text dateText;

    public TMP_InputField targetCurrencyNumberInputField;
    public TMP_Dropdown targetCurrencyShortNameDropdown;
    public TMP_Text targetCurrencyText;

    public TMP_Text currencyRateText;

    public TMP_InputField toCurrencyNumberInputField;
    public TMP_Dropdown toCurrencyShortNameDropdown;
    public TMP_Text toCurrencyText;

    private string localDate; //время

    [SerializeField] private string _targetCurrencyShortName;

    [SerializeField] private string _queryFromCurrency;
    [SerializeField] private string _queryToCurrency;
    [SerializeField] private string _queryAmountCurrencyValue;

    [SerializeField] private string[] _targetCryptoShortNames;
    [SerializeField] private string[] _targetFiatShortNames;

    //public List<string> currencyNameList = new List<string>();


    [SerializeField] private string _targetCurrencyLongName;
    [SerializeField] private string _targetCurrencyFullName;

    [SerializeField] private string _toCurrencyShortName;
    [SerializeField] private string _currencyPairName;

    [SerializeField] private float _currencyPairRate;

    [SerializeField] private float _targetCurrencyRate;
    [SerializeField] private float _targetCurrencyValuePlaceholder;

    [SerializeField] private string _targetCurrencyValueText;

    [SerializeField] private float _targetCurrencyValueInputField;

    [SerializeField] private float _amountCurrencyValue;
    //public InputField enterValue;

    //public TextMeshProUGUI targetCurrencyValueText;
    [SerializeField] private string targetCurrencyValueText;
    //public TextMeshProUGUI toCurrencyValueText;
    [SerializeField]
    private string toCurrencyValueText;

    //[SerializeField] private float _toCurrencyRate;
    //[SerializeField] private float _toCurrencyValuePlaceholder;
    [SerializeField] private float _toCurrencyValueInputField;

    public string[] currencyNames;
    public List<string> currencyName;
    public List<string> currencyName2;
    [SerializeField] private string _toCryptoSymbol;
    [SerializeField] private string _toCurrencyLongName;
    [SerializeField] private string _toCurrencyFullName;
    [SerializeField] private string _toCurrencyMaxSupply;
    [SerializeField] private string _toCurrencyIconUrl;

    private JSONNode JsonParseApiList;
    private JSONNode JsonParseApiLive;
    private JSONNode JsonParseApiConvert;
    //private bool _isTargetInputFieldCanChange;
    //private bool _isToInputFieldCanChange;
    private float _reverseResult;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ListRequest());

        // Записываем новый список в Dropdown1
        targetCurrencyShortNameDropdown.ClearOptions(); // better approach
        currencyName = new List<string>() { "USD", "RUB" };
        targetCurrencyShortNameDropdown.AddOptions(currencyName); // this is your required solution

        // Берем из Dropdown значение для короткого названия валюты1
        _targetCurrencyShortName = targetCurrencyShortNameDropdown.options[targetCurrencyShortNameDropdown.value].text;
        //Debug.Log("_targetCurrencyShortName: " + _targetCurrencyShortName);

        // Записываем новый список в Dropdown2
        toCurrencyShortNameDropdown.ClearOptions(); // better approach        
        //currencyName2 = JsonParseApiLive.Value<>("").Value<JArray>("").ToObject<List<>>();
        currencyName2 = new List<string>() { "ADA", "BCH","BNB", "BTC", "BTCS", "BTCZ", "BTG", "BURST", "DASH", "DOGE", "ENJ", "EOS",
            "ETC", "ETH", "LINK", "LTC", "MANA", "MIOTA", "QTUM", "STORJ", "THETA", "TRX", "USDT", "WAVES", "WINGS", "XLM", "XMR",
            "XRP", "ZEC", "ZEN", "ZIL" };
        /*currencyName2 = new List<string>() { "ABC", "ACP", "ACT", "ACT*", "ADA", "ADCN", "ADL", "ADX", "ADZ", "AE", "AGI", "AIB", 
            "AIDOC", "AION", "AIR", "ALT", "AMB", "AMM", "ANT", "APC", "APPC", "ARC", "ARCT", "ARDR", "ARK", "ARN", "ASAFE2", 
            "AST", "ATB", "ATM", "AURS", "BAR", "BASH", "BAT", "BAY", "BBP", "BCD", "BCH", "BCN", "BCPT", "BEE", "BIO", 
            "BLC", "BLOCK", "BLU", "BLZ", "BMC", "BNB", "BNT", "BOST", "BQ", "BQX", "BRD", "BRIT", "BTC", "BTCA", "BTCS", "BTCZ", 
            "BTG", "BTLC", "BTM", "BTM*", "BTQ", "BTS", "BTX", "BURST", "CALC", "CAS", "CAT", "CCRB", "CDT", "CESC", "CHAT", "CJ", 
            "CL", "CLD", "CLOAK", "CMT*", "CND", "CNX", "CPC", "CRAVE", "CRC", "CRE", "CRW", "CTO", "CTR", "CVC", "DAS", "DASH", 
            "DAT", "DATA", "DBC", "DBET", "DCN", "DCR", "DCT", "DEEP", "DENT", "DGB", "DGD", "DIM", "DIME", "DMD", "DNT", "DOGE", 
            "DRGN", "DRZ", "DSH", "DTA", "EC", "EDG", "EDO", "EKO", "ELA", "ELF", "EMC", "EMGO", "ENG", "ENJ", "EOS", "ERT", "ETC", 
            "ETH", "ETN", "ETP", "ETT", "EVR", "EVX", "FCT", "FLP", "FOTA", "FRST", "FUEL", "FUN", "FUNC", "FUTC", "GAME", "GAS", 
            "GBYTE", "GMX", "GNO", "GNT", "GNX", "GRC", "GRS", "GRWI", "GTC", "GTO", "GUP", "GVT", "GXS", "HAC", "HSR", "HST", "HVN", 
            "ICN", "ICOS", "ICX", "IGNIS", "ILC", "INK", "INS", "INSN", "INT", "IOP", "IOST", "ITC", "KCS", "KICK", "KIN", "KLC", 
            "KMD", "KNC", "KRB", "LA", "LEND", "LEO", "LINDA", "LINK", "LOC", "LOG", "LRC", "LSK", "LTC", "LUN", "LUX", "MAID", "MANA", 
            "MCAP", "MCO", "MDA", "MDS", "MIOTA", "MKR", "MLN", "MNX", "MOD", "MOIN", "MONA", "MTL", "MTN*", "MTX", "NAS", "NAV", 
            "NBT", "NDC", "NEBL", "NEO", "NEU", "NEWB", "NGC", "NKC", "NLC2", "NMC", "NMR", "NULS", "NVC", "NXT", "OAX", "OBITS", "OC", 
            "OCN", "ODN", "OK", "OMG", "OMNI", "ORME", "OST", "OTX", "OXY", "PART", "PAY", "PBT", "PCS", "PIVX", "PIZZA", "PLBT", "PLR", 
            "POE", "POLY", "POSW", "POWR", "PPC", "PPT", "PPY", "PRC", "PRES", "PRG", "PRL", "PRO", "PURA", "QASH", "QSP", "QTUM", 
            "QUN", "RCN", "RDD", "RDN*", "REBL", "REE", "REP", "REQ", "REV", "RGC", "RHOC", "RIYA", "RKC", "RLC", "RPX", "RUFF", "SALT", 
            "SAN", "SBC", "SC", "SENT", "SIB", "SMART", "SMLY", "SMT*", "SNC", "SNGLS", "SNM", "SNT", "SPK", "SRN", "STEEM", "STK", 
            "STORJ", "STRAT", "STU", "STX", "SUB", "SUR", "SWFTC", "SYS", "TAAS", "TESLA", "THC", "THETA", "THS", "TIO", "TKY", "TNB", 
            "TNT", "TOA", "TRC", "TRIG", "TRST", "TRUMP", "TRX", "UBQ", "UNO", "UNRC", "UQC", "USDT", "UTK", "UTT", "VEE", "VEN", "VERI", 
            "VIA", "VIB", "VIBE", "VOISE", "VOX", "VRS", "VTC", "VUC", "WABI", "WAVES", "WAX", "WC", "WGR", "WINGS", "WLK", "WOP", "WPR", 
            "WRC", "WTC", "XAUR", "XBP", "XBY", "XCP", "XCXT", "XDN", "XEM", "XGB", "XHI", "XID", "XLM", "XMR", "XNC", "XRB", "XRP", "XTO", 
            "XTZ", "XUC", "XVG", "XZC", "YEE", "YOC", "YOYOW", "ZCL", "ZEC", "ZEN", "ZIL", "ZNY", "ZRX", "ZSC", "611" };*/
        toCurrencyShortNameDropdown.AddOptions(currencyName2); // this is your required solution
        toCurrencyShortNameDropdown.value = 3;

        // Берем из Dropdown значение для короткого названия валюты2
        _toCurrencyShortName = toCurrencyShortNameDropdown.options[toCurrencyShortNameDropdown.value].text;
        //Debug.Log("_toCurrencyShortName: " + _toCurrencyShortName);

        _currencyPairName = _targetCurrencyShortName + "_" + _toCurrencyShortName;
        Debug.Log("_currencyPairName: " + _currencyPairName);

        // ограничитель колличества символов в InputField
        targetCurrencyNumberInputField.characterLimit = 9;
        toCurrencyNumberInputField.characterLimit = 9;

        // Событие если в InputField1 сменилось значение
        targetCurrencyNumberInputField.onValueChanged.AddListener(delegate { TargetValueChangeCheck(targetCurrencyNumberInputField); });
        // Событие если в InputField2 сменилось значение
        toCurrencyNumberInputField.onValueChanged.AddListener(delegate { ToValueChangeCheck(toCurrencyNumberInputField); });

        // Событие если в InputField1 завершили ввод
        targetCurrencyNumberInputField.onEndEdit.AddListener(delegate { TargetLockInput(targetCurrencyNumberInputField); });
        // Событие если в InputField2 завершили ввод
        toCurrencyNumberInputField.onEndEdit.AddListener(delegate { ToLockInput(toCurrencyNumberInputField); });

        //Событие если значение Dropdown1 изменился
        targetCurrencyShortNameDropdown.onValueChanged.AddListener(delegate { TargetCurrencyShortNameDropdownValueChanged(); });
        //Событие если значение Dropdown2 изменился
        toCurrencyShortNameDropdown.onValueChanged.AddListener(delegate { ToCurrencyShortNameDropdownValueChanged(); });        
    }

    void Recalculation()
    {
        _targetCurrencyRate = JsonParseApiLive["rates"][_toCurrencyShortName];
        //Debug.Log("The generated item is _targetCurrencyRate: \n" + _toCurrencyShortName + " to " + _targetCurrencyShortName + " is " + _targetCurrencyRate);

        //текст на тобло криптовалюты
        currencyRateText.text = "Currency Rate: " + _toCurrencyShortName + " to " + _targetCurrencyShortName + " is " + _targetCurrencyRate;

        if (_isReverse)
        {
            _reverseResult = _amountCurrencyValue * _targetCurrencyRate;  // конвертация
            if (_targetCurrencyLongName == null)
            {
                _targetCurrencyShortName = targetCurrencyShortNameDropdown.options[targetCurrencyShortNameDropdown.value].text;
                targetCurrencyText.text = _reverseResult.ToString("0.########") + " " + _targetCurrencyShortName;
            }
            else
            {
                targetCurrencyText.text = _reverseResult.ToString("0.########") + " " + _targetCurrencyLongName;
            }

            if (_toCurrencyLongName == null)
            {
                _toCurrencyShortName = toCurrencyShortNameDropdown.options[toCurrencyShortNameDropdown.value].text;
                toCurrencyText.text = _amountCurrencyValue.ToString("0.########") + " " + _toCurrencyShortName;
            }
            else
            {
                toCurrencyText.text = _amountCurrencyValue.ToString("0.########") + " " + _toCurrencyLongName;
            }
            targetCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text = _reverseResult.ToString("0.########");

            //замена надписи колличества и длинного названия основной валюты
            targetCurrencyValueText = _reverseResult.ToString("0.########");
            //замена надписи колличества и длинного названия второй валюты
            toCurrencyValueText = _amountCurrencyValue.ToString("0.########");
        }
        else
        {
            _result = _amountCurrencyValue / _targetCurrencyRate;  // конвертация
            if (_targetCurrencyLongName == null)
            {
                _targetCurrencyShortName = targetCurrencyShortNameDropdown.options[targetCurrencyShortNameDropdown.value].text;
                targetCurrencyText.text = _amountCurrencyValue.ToString("0.########") + " " + _targetCurrencyShortName;
            }
            else
            {
                targetCurrencyText.text = _amountCurrencyValue.ToString("0.########") + " " + _targetCurrencyLongName;
            }

            if (_toCurrencyLongName == null)
            {
                _toCurrencyShortName = toCurrencyShortNameDropdown.options[toCurrencyShortNameDropdown.value].text;
                toCurrencyText.text = _result.ToString("0.########") + " " + _toCurrencyShortName;
            }
            else
            {
                toCurrencyText.text = _result.ToString("0.########") + " " + _toCurrencyLongName;
            }
            //замена надписи колличества и длинного названия основной валюты
            targetCurrencyValueText = _amountCurrencyValue.ToString("0.########");
            //замена надписи колличества и длинного названия второй валюты
            toCurrencyValueText = _result.ToString("0.########");
            toCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text = _result.ToString("0.########");

        }
        _isReverse = false;
    }

    void TargetCurrencyShortNameDropdownValueChanged() //Событие если значение Dropdown1 изменился
    {
        //берем значение из списка Dropdown1
        _targetCurrencyShortName = targetCurrencyShortNameDropdown.options[targetCurrencyShortNameDropdown.value].text;
        //Debug.Log("_targetCurrencyShortName ParseWebRequest2: " + _targetCurrencyShortName);

        if (JsonParseApiList["crypto"][_targetCurrencyShortName]["symbol"] != null)
        {
            _targetCryptoSymbol = JsonParseApiList["crypto"][_targetCurrencyShortName]["symbol"];
            //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " symbol: " + _targetCryptoSymbol);

            _targetCurrencyLongName = JsonParseApiList["crypto"][_targetCurrencyShortName]["name"];
            //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " name: " + _targetCurrencyLongName);

            _targetCurrencyFullName = JsonParseApiList["crypto"][_targetCurrencyShortName]["name_full"];
            //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " name_full: " + _targetCurrencyFullName);

            _targetCurrencyMaxSupply = JsonParseApiList["crypto"][_targetCurrencyShortName]["max_supply"];
            //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " max_supply: " + _targetCurrencyMaxSupply);

            _targetCurrencyIconUrl = JsonParseApiList["crypto"][_targetCurrencyShortName]["icon_url"];
            //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " icon_url: " + _targetCurrencyIconUrl);

            StartCoroutine(TargetWebRequestTexture(_targetCurrencyIconUrl));
        }
        else
        {
            _targetCurrencyLongName = JsonParseApiList["fiat"][_targetCurrencyShortName];
            Debug.Log("The generated item is: \nfiat :" + _targetCurrencyShortName + " symbol: " + _targetCurrencyLongName);

            _targetFiatShortNames = JsonParseApiList["fiat"];
        }

        if (_isApiConvertRequest)
        {
            StartCoroutine(ConvertRequest());
        }
        else
        {
            _isApiLiveRequest = true;
            StartCoroutine(BetweenRequests());
        }
        //замена надписи колличества и длинного названия основной валюты
        targetCurrencyText.text = targetCurrencyValueText + " " + _targetCurrencyLongName;
        //стирание данных InputField
        toCurrencyNumberInputField.text = "";        
        targetCurrencyNumberInputField.text = "";
    }

    void ToCurrencyShortNameDropdownValueChanged() //Событие если значение Dropdown2 изменился
    {
        _toCurrencyShortName = toCurrencyShortNameDropdown.options[toCurrencyShortNameDropdown.value].text;
        //Debug.Log("_toCurrencyShortName ParseWebRequest2: " + _toCurrencyShortName);

        if (JsonParseApiList["crypto"][_toCurrencyShortName]["symbol"] != null)
        {
            _toCryptoSymbol = JsonParseApiList["crypto"][_toCurrencyShortName]["symbol"];
            //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " symbol: " + _toCryptoSymbol);

            _toCurrencyLongName = JsonParseApiList["crypto"][_toCurrencyShortName]["name"];
            //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " name: " + _toCurrencyLongName);

            _toCurrencyFullName = JsonParseApiList["crypto"][_toCurrencyShortName]["name_full"];
            //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " name_full: " + _toCurrencyFullName);

            _toCurrencyMaxSupply = JsonParseApiList["crypto"][_toCurrencyShortName]["max_supply"];
            //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " max_supply: " + _toCurrencyMaxSupply);

            _toCurrencyIconUrl = JsonParseApiList["crypto"][_toCurrencyShortName]["icon_url"];
            //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " icon_url: " + _toCurrencyIconUrl);

            StartCoroutine(ToWebRequestTexture(_toCurrencyIconUrl));
        }
        else
        {
            _toCurrencyLongName = JsonParseApiList["fiat"][_toCurrencyShortName];
            //Debug.Log("The generated item is: \nfiat :" + _toCurrencyShortName + " symbol: " + _toCurrencyLongName);
        }

        if (_isApiConvertRequest)
        {
            StartCoroutine(ConvertRequest());
        }
        else
        {
            Recalculation();
        }

        //замена надписи колличества и длинного названия второй валюты
        toCurrencyText.text = toCurrencyValueText + " " + _toCurrencyLongName;
        //стирание данных InputField
        toCurrencyNumberInputField.text = "";
        targetCurrencyNumberInputField.text = "";
    }


    // Checks if there is anything entered into the input field.
    void TargetLockInput(TMP_InputField input) // Изминения в InputField при подтвержденном завершении ввода
    {
        toCurrencyNumberInputField.text = "";

        //если появилось не пустое поле
        if (input.text.Length > 0)
        {
            //Debug.Log("Text has been entered");
            // основная валюта
            if (float.TryParse(targetCurrencyNumberInputField.text, out _targetCurrencyValueInputField))
            {
                //замена количества валют для обмена
                _amountCurrencyValue = _targetCurrencyValueInputField;
                Debug.Log("InputField.onValueChanged _targetCurrencyValueInputField: " + _amountCurrencyValue);

                if (_isApiConvertRequest)
                {
                    StartCoroutine(ConvertRequest());
                }
                else
                {
                    Recalculation();
                }
                //замена надписи колличества 
                targetCurrencyValueText = _targetCurrencyValueInputField.ToString("0.########");
                //Debug.Log("targetCurrencyValueText.text" + targetCurrencyValueText);

                //замена длинного названия основной валюты
                targetCurrencyText.text = targetCurrencyValueText + " " + _targetCurrencyLongName;
            }

        }
        else if (input.text.Length == 0)
        {
            //Debug.Log("Main Input Empty (при подтвержденном завершении ввода)");
            Recalculation();
        }
    }

    void ToLockInput(TMP_InputField input) // Изминения в To InputField при подтвержденном завершении ввода
    {
        targetCurrencyNumberInputField.text = "";// основная валюта

        //если появилось не пустое поле
        if (input.text.Length > 0)
        {
            //Debug.Log("Text has been entered");            

            if (float.TryParse(toCurrencyNumberInputField.text, out _toCurrencyValueInputField))
            {

                _isReverse = true;
                //замена количества валют для обмена
                _amountCurrencyValue = _toCurrencyValueInputField;
                //Debug.Log("InputField.onValueChanged _targetCurrencyValueInputField: " + _amountCurrencyValue);

                if (_isApiConvertRequest)
                {
                    StartCoroutine(ConvertRequest());
                }
                else
                {
                    Recalculation();
                }
                //замена надписи колличества 
                toCurrencyValueText = _toCurrencyValueInputField.ToString("0.########");
                //Debug.Log("toCurrencyValueText.text" + toCurrencyValueText);

                //замена длинного названия второй валюты
                toCurrencyText.text = toCurrencyValueText + " " + _toCurrencyLongName;
            }
        }
        else if (input.text.Length == 0)
        {
            //Debug.Log("Main Input Empty (при подтвержденном завершении ввода)");
            Recalculation();
        }
    }

    private void TargetValueChangeCheck(TMP_InputField input) // Изминения в InputField на ходу без подтверждения завершения ввода
    {
        toCurrencyNumberInputField.text = "";

        if (input.text.Length > 0)
        {
            //Debug.Log("Text has been entered");
            // основная валюта
            if (float.TryParse(targetCurrencyNumberInputField.text, out _targetCurrencyValueInputField))
            {
                //замена количества валют для обмена
                _amountCurrencyValue = _targetCurrencyValueInputField;
                //Debug.Log("InputField.onValueChanged _targetCurrencyValueInputField: " + _amountCurrencyValue);

                if (_isApiConvertRequest)
                {
                    StartCoroutine(ConvertRequest());
                }
                else
                {
                    //StartCoroutine(BetweenRequests());
                    Recalculation();
                    //toCurrencyNumberInputField.text = toCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text;
                }
                //замена надписи колличества 
                targetCurrencyValueText = _targetCurrencyValueInputField.ToString("0.########");
                //Debug.Log("targetCurrencyValueText.text" + targetCurrencyValueText);

                //замена длинного названия основной валюты
                targetCurrencyText.text = targetCurrencyValueText + " " + _targetCurrencyLongName;
            }
        }
        else if (input.text.Length == 0)
        {
            //Debug.Log("Main Input Empty (на ходу без подтверждения завершения ввода)");
            Recalculation();            
        }
    }

    private void ToValueChangeCheck(TMP_InputField input) // Изминения в InputField на ходу без подтверждения завершения ввода
    {
        targetCurrencyNumberInputField.text = "";

        if (input.text.Length > 0)
        {
            Debug.Log("Text has been entered");
            // основная валюта

            if (float.TryParse(toCurrencyNumberInputField.text, out _toCurrencyValueInputField))
            {

                _isReverse = true;
                //замена количества валют для обмена
                _amountCurrencyValue = _toCurrencyValueInputField;
                //Debug.Log("InputField.onValueChanged _targetCurrencyValueInputField: " + _amountCurrencyValue);

                if (_isApiConvertRequest)
                {
                    StartCoroutine(ConvertRequest());
                }
                else
                {
                    Recalculation();                    
                }
                //замена надписи колличества 
                toCurrencyValueText = _toCurrencyValueInputField.ToString("0.########");
                //Debug.Log("toCurrencyValueText.text" + toCurrencyValueText);

                //замена длинного названия второй валюты
                toCurrencyText.text = toCurrencyValueText + " " + _toCurrencyLongName;
            }
        }
        else if (input.text.Length == 0)
        {
            Debug.Log("Main Input Empty (на ходу без подтверждения завершения ввода)");
            Recalculation();            
        }
    }


    // Update is called once per frame
    void Update()
    {
        localDate = DateTime.Now.ToString();
        dateText.text = localDate;

        // !!!!!!!!!!!!!!!!!! Не стирать!!!!!!!!!, функция замены InputField из InputField.placeholder !!!!!!!!!!!!!!!!!!
        /*if (targetCurrencyNumberInputField.GetComponent<TMP_InputField>().isFocused && _isTargetInputFieldCanChange)
        {
            Debug.Log("1targetCurrencyNumberInputField.GetComponent<TMP_InputField>().isFocused");
            targetCurrencyNumberInputField.text = targetCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text;
            _isTargetInputFieldCanChange = false;
        }

        if (toCurrencyNumberInputField.GetComponent<TMP_InputField>().isFocused && _isToInputFieldCanChange)
        {
            Debug.Log("2targetCurrencyNumberInputField.GetComponent<TMP_InputField>().isFocused");
            toCurrencyNumberInputField.text = toCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text;
            _isToInputFieldCanChange = false;
        }*/
        // !!!!!!!!!!!!!!!!!! Не стирать!!!!!!!!!, функция замены InputField из InputField.placeholder !!!!!!!!!!!!!!!!!!
    }

    public IEnumerator ListRequest()
    {
        _isApiListRequest = true;
        _isApiLiveRequest = false;
        _isApiConvertRequest = false;        
        _urlApiList = "https://api.coinlayer.com/list?access_key=" + _access_key;
        yield return StartCoroutine(GetRequest(_urlApiList));
        _isApiListRequest = false;
        _isApiLiveRequest = true;
        StartCoroutine(BetweenRequests());
    }

    private IEnumerator BetweenRequests()
    {
        if (_isApiLiveRequest)
        {
            yield return StartCoroutine(LiveRequest());
            // Если в InputField.placeholder есть текст принимаем это значение для конвертации
            if (float.TryParse(targetCurrencyNumberInputField.text, out _targetCurrencyValueInputField))
            {
                _amountCurrencyValue = _targetCurrencyValueInputField;
                //Debug.Log("_targetCurrencyValuePlaceholder: " + _amountCurrencyValue);
                //Debug.Log("_targetCurrencyLongName: " + _targetCurrencyLongName);

                Recalculation();
            }
            else if (float.TryParse(targetCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text, out _targetCurrencyValuePlaceholder))
            {
                _amountCurrencyValue = _targetCurrencyValuePlaceholder;
                //Debug.Log("_targetCurrencyValuePlaceholder: " + _amountCurrencyValue);
                //Debug.Log("_targetCurrencyLongName: " + _targetCurrencyLongName);

                Recalculation();
            }
            //Debug.Log("_targetCurrencyValuePlaceholder: " + targetCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text);
        }
        else if (_isApiConvertRequest)
        {
            StartCoroutine(ConvertRequest());
        }
    }

    public void ResetButton()
    {
        targetCurrencyNumberInputField.text = "";
        _amountCurrencyValue = 1;
        targetCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text = _amountCurrencyValue.ToString("0.########");
        TargetLockInput(targetCurrencyNumberInputField);
    }

    public IEnumerator LiveRequest()
    {
        _isApiLiveRequest = true;
        _isApiListRequest = false;
        _isApiConvertRequest = false;

        _urlApiLive = "http://api.coinlayer.com/api/live?access_key=" + _access_key + "&target=" + _targetCurrencyShortName;

        yield return StartCoroutine(GetRequest(_urlApiLive));
        _isApiListRequest = false;
        _isApiLiveRequest = false;
        //_isApiConvertRequest = true; //Выключение прямой конвертации!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        StartCoroutine(BetweenRequests());
    }

    public IEnumerator ConvertRequest()
    {
        _isApiConvertRequest = true;
        _isApiListRequest = false;
        _isApiLiveRequest = false;

        string from;
        string to;

        if (_isReverse)
        {
            from = _toCurrencyShortName;
            to = _targetCurrencyShortName;
        }
        else
        {
            from = _targetCurrencyShortName;
            to = _toCurrencyShortName;
        }
        _urlApiConvert = "https://api.coinlayer.com/convert?access_key="
                         + _access_key
                         + "&from="
                         + from
                         + "& to="
                         + to
                         + "&amount="
                         + _amountCurrencyValue;
        Debug.Log("_urlApiConvert: " + _urlApiConvert);
        yield return StartCoroutine(GetRequest(_urlApiConvert));
        _isReverse = false;
        _isApiConvertRequest = false;   
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);

                    //_isErrorGetRequestApi = true;
                    Debug.Log("errorGetRequest1 = true;");

                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);

                    //_isErrorGetRequestApi = true;
                    Debug.Log("errorGetRequest1 = true;");

                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    yield return StartCoroutine(ParseWebRequest(webRequest.downloadHandler.text));
                    Debug.Log("ParseWebRequest(webRequest.downloadHandler.text);");

                    break;
            }
        }
    }

    IEnumerator ParseWebRequest(string downloadHandler)
    {
        //Debug.Log("void ParseWebRequest(string downloadHandler)" + downloadHandler);

        if (_isApiListRequest)
        {
            string textApiListRequest = downloadHandler;

            JsonParseApiList = JSON.Parse(textApiListRequest);
            Debug.Log("The generated item is: textApiListRequest " + JsonParseApiList);

            //Debug.Log("The generated item is: \nJSON.Parse(textApiListRequest).AsStringList; :: " + x);

            _successApi = JsonParseApiList["success"];
            //Debug.Log("The generated item is: success " + _successApi);

            if (_successApi)
            {
                if (JsonParseApiList["crypto"][_targetCurrencyShortName]["symbol"] != null)
                {
                    _targetCryptoSymbol = JsonParseApiList["crypto"][_targetCurrencyShortName]["symbol"];
                    //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " symbol: " + _targetCryptoSymbol);

                    _targetCurrencyLongName = JsonParseApiList["crypto"][_targetCurrencyShortName]["name"];
                    //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " name: " + _targetCurrencyLongName);

                    _targetCurrencyFullName = JsonParseApiList["crypto"][_targetCurrencyShortName]["name_full"];
                    //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " name_full: " + _targetCurrencyFullName);

                    _targetCurrencyMaxSupply = JsonParseApiList["crypto"][_targetCurrencyShortName]["max_supply"];
                    //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " max_supply: " + _targetCurrencyMaxSupply);

                    _targetCurrencyIconUrl = JsonParseApiList["crypto"][_targetCurrencyShortName]["icon_url"];
                    //Debug.Log("The generated item is: \ncrypto :" + _targetCurrencyShortName + " icon_url: " + _targetCurrencyIconUrl);

                    StartCoroutine(TargetWebRequestTexture(_targetCurrencyIconUrl));
                }
                else
                {
                    _targetCurrencyLongName = JsonParseApiList["fiat"][_targetCurrencyShortName];
                    //Debug.Log("The generated item is: \nfiat :" + _targetCurrencyShortName + " symbol: " + _targetCurrencyLongName);
                }

                if (JsonParseApiList["crypto"][_toCurrencyShortName]["symbol"] != null)
                {
                    _toCryptoSymbol = JsonParseApiList["crypto"][_toCurrencyShortName]["symbol"];
                    //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " symbol: " + _toCryptoSymbol);

                    _toCurrencyLongName = JsonParseApiList["crypto"][_toCurrencyShortName]["name"];
                    //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " name: " + _toCurrencyLongName);

                    _toCurrencyFullName = JsonParseApiList["crypto"][_toCurrencyShortName]["name_full"];
                    //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " name_full: " + _toCurrencyFullName);

                    _toCurrencyMaxSupply = JsonParseApiList["crypto"][_toCurrencyShortName]["max_supply"];
                    //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " max_supply: " + _toCurrencyMaxSupply);

                    _toCurrencyIconUrl = JsonParseApiList["crypto"][_toCurrencyShortName]["icon_url"];
                    //Debug.Log("The generated item is: \ncrypto :" + _toCurrencyShortName + " icon_url: " + _toCurrencyIconUrl);

                    StartCoroutine(ToWebRequestTexture(_toCurrencyIconUrl));
                }
                else
                {
                    _toCurrencyLongName = JsonParseApiList["fiat"][_toCurrencyShortName];
                    //Debug.Log("The generated item is: \nfiat :" + _toCurrencyShortName + " symbol: " + _toCurrencyLongName);
                }
            }
            else
            {
                Debug.LogError("The generated item is _isApiListRequest: success " + _successApi);
            }

        }
        else if (_isApiLiveRequest)
        {
            string textApiLiveRequest = downloadHandler;
            JsonParseApiLive = JSON.Parse(textApiLiveRequest);
            Debug.Log("The generated item is: textApiLiveRequest " + JsonParseApiLive);

            _successApi = JsonParseApiLive["success"];
            //Debug.Log("The generated item is: success " + _successApi);

            if (_successApi)
            {
                _targetCurrencyApi = JsonParseApiLive["target"];
                //Debug.Log("The generated item is: target " + JsonParseApiLive["target"]);

                _targetCurrencyRate = JsonParseApiLive["rates"][_toCurrencyShortName];
                //Debug.Log("The generated item is _targetCurrencyRate: \n" + _toCurrencyShortName + " to " + _targetCurrencyShortName + " is " + _targetCurrencyRate);

                currencyRateText.text = "Currency Rate: " + _toCurrencyShortName + " to " + _targetCurrencyShortName + " is " + _targetCurrencyRate;
            }
            else
            {
                Debug.LogError("The generated item is _isApiLiveRequest: success " + _successApi);
            }

        }
        else if (_isApiConvertRequest)
        {
            string textApiConvertRequest = downloadHandler;
            JsonParseApiConvert = JSON.Parse(textApiConvertRequest);
            Debug.Log("The generated item is: textApiConvertRequest " + JsonParseApiConvert);

            _successApi = JsonParseApiConvert["success"];
            //Debug.Log("The generated item is: success " + _successApi);

            if (_successApi)
            {
                _queryFromCurrency = JsonParseApiConvert["query"]["from"];
                //Debug.Log("The generated item is: \nquery : from: " + _queryFromCurrency);

                _queryToCurrency = JsonParseApiConvert["query"]["to"];
                //Debug.Log("The generated item is: \nquery : to: " + _queryToCurrency);

                _queryAmountCurrencyValue = JsonParseApiConvert["query"]["amount"];
                //Debug.Log("The generated item is: \nquery : amount: " + _queryAmountCurrencyValue);

                _infoTimestamp = JsonParseApiConvert["info"]["timestamp"];
                //Debug.Log("The generated item is: \ninfo : timestamp: " + _infoTimestamp);

                _rate = JsonParseApiConvert["info"]["rate"];
                //Debug.Log("The generated item is: \ninfo : rate: " + _rate);

                _result = JsonParseApiConvert["result"];
                //Debug.Log("The generated item is: \nresult: " + _result);

                //Debug.Log("_result.ToString(): " + _result.ToString());
                if (_isReverse)
                {
                    targetCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text = _result.ToString();
                }
                else
                {
                    toCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text = _result.ToString();
                }
            }
            else
            {
                toCurrencyNumberInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Нет соединения";
                //Debug.LogError("The generated item is _isApiConvertRequest: success " + _successApi);
            }

        }
        yield return null;
    }
    //замена картинок криптовалют
    IEnumerator TargetWebRequestTexture(string targetCurrencyIconUrl)
    {
        UnityWebRequest itemImageRequest = UnityWebRequestTexture.GetTexture(targetCurrencyIconUrl);

        yield return itemImageRequest.SendWebRequest();

        if (itemImageRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(itemImageRequest.error);
        }
        else
        {
            // Get downloaded asset bundle
            var texture = DownloadHandlerTexture.GetContent(itemImageRequest);

            _targetCurrencyIconRawImage.texture = texture;
        }
    }
    //замена картинок криптовалют2
    IEnumerator ToWebRequestTexture(string toCurrencyIconUrl)
    {
        UnityWebRequest itemImageRequest = UnityWebRequestTexture.GetTexture(toCurrencyIconUrl);
        yield return itemImageRequest.SendWebRequest();
        if (itemImageRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(itemImageRequest.error);
        }
        else
        {
            // Get downloaded asset bundle
            var texture = DownloadHandlerTexture.GetContent(itemImageRequest);

            _toCurrencyIconRawImage.texture = texture;
        }
    }
}