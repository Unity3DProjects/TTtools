using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AdeDataInfo", menuName = "MyTools/AdeDataInfo")]
public class AdeDataInfo : ScriptableObject
{
    [Header("分享ID")]
    public string ShareId;

    [Header("订阅ID")]
    [Tooltip("最多3个")]
    public List<string> SubscribeTmplIds = new List<string>(3);

    [Header("推荐流复访 content_id")]
    public List<FeedRepeatContentData> FeedRepeatContents = new List<FeedRepeatContentData>();

    // Retained for existing assets and migrated by the editor tooling.
    [HideInInspector]
    public List<string> FeedRepeatContentIDs = new List<string>();

    [Header("推荐流获客ID")]
    public List<string> FeedAcquisitionContentIDs = new List<string>();

    [Header("更多游戏")]
    public MoreGamesData MoreGames = new MoreGamesData();

    [Header("游戏圈")]
    public GameClubData GameClub = new GameClubData();

    [HideInInspector]
    public List<string> FeedContentIDs = new List<string>();

    [HideInInspector]
    public string FeedRepeatContentID;
}

[System.Serializable]
public enum FeedRepeatSceneType
{
    [InspectorName("离线收益")]
    OfflineReward = 1,

    [InspectorName("体力恢复")]
    EnergyRecovery = 2,

    [InspectorName("重要事件提醒")]
    ImportantEventReminder = 3
}

[System.Serializable]
public class FeedRepeatContentData
{
    public string ContentId;
    public FeedRepeatSceneType SceneType = FeedRepeatSceneType.ImportantEventReminder;
}
