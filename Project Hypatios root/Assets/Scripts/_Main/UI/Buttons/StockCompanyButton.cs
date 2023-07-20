using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StockCompanyButton : MonoBehaviour
{

    public MobiusNetUI_StockExchange StockExchange;
    public Text label_CompanyName;
    public Text label_Percentage;

    public string IndexID = "";

    private Color positiveChangeColor;
    private Color negativeChangeColor;

    private void Start()
    {

    }

    public void UpdateUI()
    {
        var profile = Hypatios.Assets.GetCompanyProfile(IndexID);
        positiveChangeColor = StockExchange.color_lastChange_Up;
        negativeChangeColor = StockExchange.color_lastChange_Down;

        bool isUp = Mathf.Sign(profile.GetNetChange()) > 0f ? true : false;
        label_CompanyName.text = profile.companyDisplayName;

        if (isUp)
        {
            label_Percentage.text = $"<color=#{ColorUtility.ToHtmlStringRGB(positiveChangeColor)}>{profile.GetString_NetChange()}</color>";
        }
        else
        {
            label_Percentage.text = $"<color=#{ColorUtility.ToHtmlStringRGB(negativeChangeColor)}>{profile.GetString_NetChange()}</color>";
        }

    }

    public void ChangeProfile()
    {
        var profile = Hypatios.Assets.GetCompanyProfile(IndexID);

        StockExchange.CurrentProfile = profile;
        StockExchange.RefreshUI();
    }

}
