using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class MobiusNetUI_StockExchange : MonoBehaviour
{

    public StockExchange_ProfileObject CurrentProfile;
    public ModularUI_LineGraph lineGraph;
    public InputField input_ShareBuySell;
    public int dotTotal = 18;
    public float RefreshRate = 1f; //every second updates

    [FoldoutGroup("Stock Indexes")] public StockCompanyButton buttonPrefab;
    [FoldoutGroup("Stock Indexes")] public RectTransform parentCompanyList;


    [FoldoutGroup("Info")] public Text label_title;
    [FoldoutGroup("Info")] public Text label_Description;
    [FoldoutGroup("Info")] public Text label_CurrentPrice;
    [FoldoutGroup("Info")] public Text labelInfo_GrossSum;
    [FoldoutGroup("Info")] public Text labelInfo_TotalInvestment;
    [FoldoutGroup("Info")] public Text labelInfo_ShareOwned;
    [FoldoutGroup("Info")] public Text labelInfo_LastChange;
    [FoldoutGroup("Info")] public Color color_lastChange_Down;
    [FoldoutGroup("Info")] public Color color_lastChange_Up;

    private float _refreshTime = 1f;
    [ReadOnly] public float[] _DEBUG_DataProfiles;
    [ReadOnly] public string[] _DEBUG_GraphHorizontalLabels;

    private int sharePrice_Soul = 0;
    private int _shareToBuy = 0;
    private List<StockCompanyButton> allStockCompanyButtons = new List<StockCompanyButton>();

    private void Start()
    {
        buttonPrefab.gameObject.SetActive(false);
    }

    void Update()
    {
        _refreshTime -= Time.deltaTime;

        if (_refreshTime < 0f)
        {
            RefreshUI();
            _refreshTime = RefreshRate;
        }
    }

    [Button("Refresh UI")]
    public void RefreshUI()
    {

        DEBUG_GetStockDatas(dotTotal);
        sharePrice_Soul = Mathf.RoundToInt(_DEBUG_DataProfiles.Last() / 2f);
        lineGraph.ShowLineGraph(_DEBUG_DataProfiles, _DEBUG_GraphHorizontalLabels);

        Refresh_CompanyProfile();
        Refresh_Indexes();
    }

    private void Refresh_CompanyProfile()
    {
        var portfolioSave = Hypatios.Game.PortfolioShares.Find(x => x.ID == CurrentProfile.indexID);

        label_CurrentPrice.text = $"{sharePrice_Soul}";


        if (int.TryParse(input_ShareBuySell.text, out _shareToBuy))
        {
            labelInfo_GrossSum.text = $"{sharePrice_Soul * _shareToBuy} Souls";
        }
        else
        {
            labelInfo_GrossSum.text = "0 Souls";
        }

        if (portfolioSave == null)
        {
            labelInfo_ShareOwned.text = "0";
            labelInfo_TotalInvestment.text = "0";
        }
        else
        {
            labelInfo_ShareOwned.text = $"{portfolioSave.GetTotalShares()}";
            labelInfo_TotalInvestment.text = $"{portfolioSave.GetTotalInvestment()}";
        }

        {
            string str1 = CurrentProfile.GetString_NetChange();
            bool isUp = Mathf.Sign(CurrentProfile.GetNetChange()) > 0f ? true : false;

            if (isUp)
            {
                labelInfo_LastChange.text = $"<color=#{ColorUtility.ToHtmlStringRGB(color_lastChange_Up)}>{str1}</color>";
            }
            else
            {
                labelInfo_LastChange.text = $"<color=#{ColorUtility.ToHtmlStringRGB(color_lastChange_Down)}>{str1}</color>";
            }

            label_title.text = $"{CurrentProfile.companyDisplayName} [{CurrentProfile.indexID}]";
            label_Description.text = $"{CurrentProfile.companyDescription}\n\nMarket Cap: €{string.Format("{0:0,0}", CurrentProfile.marketCap)} billion";
        }
    }

    private void Refresh_Indexes()
    {
        foreach(var button in allStockCompanyButtons)
        {
            Destroy(button.gameObject);
        }

        allStockCompanyButtons.Clear();

        foreach(var company in Hypatios.Assets.AllStockCompanies)
        {
            var button1 = Instantiate(buttonPrefab, parentCompanyList);
            button1.IndexID = company.indexID;
            button1.gameObject.SetActive(true);
            button1.UpdateUI();

            allStockCompanyButtons.Add(button1);
        }
    }

    public void BuyStock()
    {
        var portfolioSave = Hypatios.Game.PortfolioShares.Find(x => x.ID == CurrentProfile.indexID);

        if (_shareToBuy * sharePrice_Soul > Hypatios.Game.SoulPoint && _shareToBuy > 0)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Insufficient souls! Require {_shareToBuy * sharePrice_Soul} Souls to buy x{_shareToBuy} {CurrentProfile.indexID} shares", 7f);
            return;
        }
        if (_shareToBuy == 0)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"Input the amount: 10 to buy 10 {CurrentProfile.indexID} stocks/-5 to sell 5 of your {CurrentProfile.indexID} stocks.", 6f);
            return;
        }
        if (_shareToBuy < 0 && portfolioSave == null)
        {
            DeadDialogue.PromptNotifyMessage_Mod($"You have zero {CurrentProfile.indexID} stocks to sell.", 4f);
            return;
        }

        if (portfolioSave != null)
        {
            if (portfolioSave.GetTotalShares() == 0)
            {
                DeadDialogue.PromptNotifyMessage_Mod($"You have zero {CurrentProfile.indexID} stocks to sell.", 4f);
                return;
            }
            if (portfolioSave.GetTotalShares() < Mathf.Abs(_shareToBuy))
            {
                DeadDialogue.PromptNotifyMessage_Mod($"You only have {portfolioSave.GetTotalShares()} stocks, when you want to sell x{Mathf.Abs(_shareToBuy)}.", 6f);
                return;
            }
        }
        else
        {
            portfolioSave = new HypatiosSave.ShareCompanySave();
            portfolioSave.ID = CurrentProfile.indexID;
            Hypatios.Game.PortfolioShares.Add(portfolioSave);
        }

        if (_shareToBuy > 0)
        {
            Hypatios.Game.SoulPoint -= _shareToBuy * sharePrice_Soul;
            portfolioSave.BuyShare(sharePrice_Soul, Mathf.Abs(_shareToBuy));
            DeadDialogue.PromptNotifyMessage_Mod($"Bought x{Mathf.Abs(_shareToBuy)} {CurrentProfile.indexID} stocks for {_shareToBuy * sharePrice_Soul} Souls.", 6f);
            soundManagerScript.instance.Play("reward");
        }
        else
        {
            int sellAmount = Mathf.Abs(_shareToBuy);
            Hypatios.Game.SoulPoint += sellAmount * sharePrice_Soul;
            portfolioSave.RemoveShare(sellAmount);
            DeadDialogue.PromptNotifyMessage_Mod($"Sold x{Mathf.Abs(_shareToBuy)} {CurrentProfile.indexID} stocks for {sellAmount * sharePrice_Soul} Souls.", 6f);
            soundManagerScript.instance.Play("reward");

        }

        RefreshUI();
    }

    [Button("Get Stock Datas")]
    public void DEBUG_GetStockDatas(int length)
    {
        _DEBUG_DataProfiles = CurrentProfile.GetCurrentSharePrice(length); 
        var list1 = _DEBUG_DataProfiles.ToList(); list1.Reverse();
        _DEBUG_GraphHorizontalLabels = CurrentProfile.GetHorizontalLabels(length); 
        var list2 = _DEBUG_GraphHorizontalLabels.ToList(); list2.Reverse();


        _DEBUG_DataProfiles = list1.ToArray();
        _DEBUG_GraphHorizontalLabels = list2.ToArray();
    }
}
