using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AdsData", menuName = "MyTools/AdsData")]
public class AdsData: ScriptableObject
{
    public AdsPlatformData AdData;
}

[System.Serializable]
public class AdItemData 
{
    public string name;
    public string ID;
}

[System.Serializable]
public enum GridAdType
{
    [InspectorName("单格子")]
    Single,
    [InspectorName("竖格子")]
    Vertical,
    [InspectorName("矩阵格子")]
    Matrix,
    [InspectorName("横格子")]
    Horizontal
}

[System.Serializable]
public enum GridAnchorType
{
    [InspectorName("左上")]
    TopLeft,
    [InspectorName("上")]
    Top,
    [InspectorName("右上")]
    TopRight,
    [InspectorName("左")]
    Left,
    [InspectorName("中")]
    Center,
    [InspectorName("右")]
    Right,
    [InspectorName("左下")]
    BottomLeft,
    [InspectorName("下")]
    Bottom,
    [InspectorName("右下")]
    BottomRight
}

[System.Serializable]
public class GridAdData
{
    [InspectorName("名称ID")]
    public string NameId;

    [InspectorName("格子类型")]
    public GridAdType Type;

    [InspectorName("原生广告宽度")]
    public float Width;

    [InspectorName("格子广告ID")]
    public string AdUnitId;

    [InspectorName("格子锚点")]
    public GridAnchorType Anchor;

    [InspectorName("格子位置")]
    public Vector2 Position;
}

[System.Serializable]
public enum MoreGamesPanelCount
{
    [InspectorName("单宫格")]
    One,
    [InspectorName("四宫格")]
    Four,
    [InspectorName("九宫格")]
    Nine
}

[System.Serializable]
public enum MoreGamesPanelSize
{
    [InspectorName("大")]
    Large,
    [InspectorName("中")]
    Medium,
    [InspectorName("小")]
    Small
}

[System.Serializable]
public class MoreGamesQueryData
{
    [InspectorName("目标AppID")]
    public string AppId;

    [InspectorName("Query")]
    public string Query;
}

[System.Serializable]
public class MoreGamesData
{
    [InspectorName("宫格数量")]
    public MoreGamesPanelCount GridCount = MoreGamesPanelCount.Nine;

    [InspectorName("宫格尺寸")]
    public MoreGamesPanelSize Size = MoreGamesPanelSize.Medium;

    [InspectorName("自定义位置")]
    public bool CustomPosition;

    [InspectorName("Top")]
    public int Top;

    [InspectorName("Left")]
    public int Left;

    [InspectorName("Query列表")]
    public List<MoreGamesQueryData> Queries = new List<MoreGamesQueryData>();
}

[System.Serializable]
public class GameClubData
{
    [InspectorName("OpenLink")]
    public string OpenLink;
}

[System.Serializable]
public class AdShieldData
{
    [InspectorName("启用时间屏蔽")]
    public bool EnableTimeShield;

    [InspectorName("屏蔽开始时间")]
    public string ShieldStartTime = "2025-05-01 00:00:00";

    [InspectorName("屏蔽结束时间")]
    public string ShieldEndTime = "2025-05-01 19:00:00";

    [InspectorName("启用地区屏蔽")]
    public bool EnableAreaShield;
}

[System.Serializable]
public class AutoInterstitialData
{
    [InspectorName("启用自弹插屏")]
    public bool EnableAutoInterstitial;

    [InspectorName("自弹间隔秒数")]
    public float IntervalSeconds = 30f;
}

[System.Serializable]
public class AdsPlatformData
{
    public string ID;
    public AdItemData InterstitialID;
    public AdItemData BannerID;
    public AdItemData[] RewardID;
    [InspectorName("格子广告列表")]
    public List<GridAdData> GridAdList = new List<GridAdData>();
    [HideInInspector]
    [InspectorName("更多游戏")]
    public MoreGamesData MoreGames = new MoreGamesData();
    [HideInInspector]
    [InspectorName("游戏圈")]
    public GameClubData GameClub = new GameClubData();
    [InspectorName("广告屏蔽")]
    public AdShieldData AdShield = new AdShieldData();
    [InspectorName("自弹插屏")]
    public AutoInterstitialData AutoInterstitial = new AutoInterstitialData();
}
