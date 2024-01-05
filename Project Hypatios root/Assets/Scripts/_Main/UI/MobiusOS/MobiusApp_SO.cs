using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Stock Exchange", menuName = "Hypatios/Mobius App", order = 1)]
public class MobiusApp_SO : ScriptableObject
{

    public enum DefaultApp
    {
        Generic,
        Settings,
        StockExchange
    }

    public Sprite appLogo;
    public bool useCustomMinSize = false;
    [ShowIf("useCustomMinSize", true)] public Vector2 minimumWindowSize = new Vector2(1100f, 600f);
    public string appID = "StockExchange";
    public string appName = "Stock Exchange";
    public MobiusApp mobiusAppPrefab;
    public DefaultApp defaultAppType;

    public static MobiusApp_SO GetMobiusApp(DefaultApp _appType)
    {
        return Hypatios.Assets.GetMobiusApp(_appType);
    }


}
