using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace Ade_Framework
{
    public class AdShieldRuntime : MonoBehaviour
    {
        const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        const string DateFormat = "yyyy-MM-dd";
        const string AreaRequestUrl = "https://api.live.bilibili.com/client/v1/Ip/getInfoNew";
        const int AreaRequestTimeoutSeconds = 5;

        static readonly string[] ShieldAreas =
        {
            "广州", "厦门", "北京", "深圳", "长沙", "南京", "东莞", "杭州", "重庆", "成都", "武汉", "上海"
        };

        static AdShieldRuntime instance;

        AdShieldData config;
        bool areaChecking;
        bool areaReady;
        bool areaBlocked;
        string currentArea = string.Empty;

        public static AdShieldRuntime Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("AdeAdShieldRuntime");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<AdShieldRuntime>();
                }

                return instance;
            }
        }

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Init(AdShieldData shieldData)
        {
            StopAllCoroutines();
            config = shieldData;
            areaChecking = false;
            areaReady = !IsAreaShieldEnabled();
            areaBlocked = false;
            currentArea = string.Empty;

            if (IsAreaShieldEnabled())
            {
                StartCoroutine(RequestArea());
            }
        }

        public bool CanShowAd(out string reason)
        {
            if (config == null)
            {
                reason = string.Empty;
                return true;
            }

            if (IsTimeBlocked(out reason))
            {
                return false;
            }

            if (!IsAreaShieldEnabled())
            {
                reason = string.Empty;
                return true;
            }

            if (!areaReady)
            {
                reason = areaChecking ? "地区屏蔽检测中" : "地区屏蔽未完成";
                return false;
            }

            if (areaBlocked)
            {
                reason = string.IsNullOrEmpty(currentArea) ? "地区命中屏蔽列表" : $"地区命中屏蔽列表:{currentArea}";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        bool IsTimeBlocked(out string reason)
        {
            reason = string.Empty;
            if (config == null || !config.EnableTimeShield)
            {
                return false;
            }

            if (!TryParseTime(config.ShieldStartTime, false, out DateTime shieldStartTime))
            {
                reason = $"时间屏蔽开始时间格式错误:{config.ShieldStartTime}";
                return true;
            }

            if (!TryParseTime(config.ShieldEndTime, true, out DateTime shieldEndTime))
            {
                reason = $"时间屏蔽结束时间格式错误:{config.ShieldEndTime}";
                return true;
            }

            DateTime now = DateTime.Now;
            if (now >= shieldStartTime && now <= shieldEndTime)
            {
                reason = $"时间屏蔽:{shieldStartTime.ToString(DateTimeFormat)}-{shieldEndTime.ToString(DateTimeFormat)}";
                return true;
            }

            return false;
        }

        static bool TryParseTime(string value, bool isEndTime, out DateTime time)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                time = default;
                return false;
            }

            string trimmedValue = value.Trim();
            if (DateTime.TryParseExact(trimmedValue, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            {
                return true;
            }

            if (DateTime.TryParseExact(trimmedValue, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            {
                time = isEndTime ? time.Date.AddDays(1).AddTicks(-1) : time.Date;
                return true;
            }

            if (DateTime.TryParse(trimmedValue, out time))
            {
                if (isEndTime && !trimmedValue.Contains(":"))
                {
                    time = time.Date.AddDays(1).AddTicks(-1);
                }

                return true;
            }

            return false;
        }

        bool IsAreaShieldEnabled()
        {
            return config != null && config.EnableAreaShield && ShieldAreas.Length > 0;
        }

        IEnumerator RequestArea()
        {
            areaChecking = true;
            areaReady = false;
            areaBlocked = false;

            using (UnityWebRequest request = UnityWebRequest.Get(AreaRequestUrl))
            {
                request.timeout = AreaRequestTimeoutSeconds;
                yield return request.SendWebRequest();

                areaChecking = false;
                areaReady = true;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    LogManager.LogWarning($"地区屏蔽请求失败，默认放行:{request.error}");
                    yield break;
                }

                HandleAreaResponse(request.downloadHandler.text);
            }
        }

        void HandleAreaResponse(string json)
        {
            try
            {
                AreaResponseRoot response = JsonUtility.FromJson<AreaResponseRoot>(json);
                if (response == null || response.data == null)
                {
                    LogManager.LogWarning("地区屏蔽返回为空，默认放行");
                    return;
                }

                string province = CleanAreaName(response.data.province);
                string city = CleanAreaName(response.data.city);
                currentArea = string.IsNullOrEmpty(province) ? city : $"{province}/{city}";
                areaBlocked = IsAreaBlocked(province, city);
                LogManager.Log($"地区屏蔽检测:{currentArea}, blocked={areaBlocked}");
            }
            catch (Exception exception)
            {
                areaBlocked = false;
                LogManager.LogWarning($"地区屏蔽解析失败，默认放行:{exception.Message}");
            }
        }

        bool IsAreaBlocked(string province, string city)
        {
            for (int i = 0; i < ShieldAreas.Length; i++)
            {
                string area = CleanAreaName(ShieldAreas[i]);
                if (string.IsNullOrEmpty(area))
                {
                    continue;
                }

                if (MatchesArea(area, province) || MatchesArea(area, city))
                {
                    return true;
                }
            }

            return false;
        }

        static bool MatchesArea(string configArea, string userArea)
        {
            if (string.IsNullOrEmpty(configArea) || string.IsNullOrEmpty(userArea))
            {
                return false;
            }

            return configArea.Contains(userArea) || userArea.Contains(configArea);
        }

        static string CleanAreaName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Trim()
                .Replace("省", string.Empty)
                .Replace("市", string.Empty)
                .Replace("自治区", string.Empty)
                .Replace("特别行政区", string.Empty)
                .Replace("壮族", string.Empty)
                .Replace("回族", string.Empty)
                .Replace("维吾尔", string.Empty);
        }

        [Serializable]
        class AreaResponseRoot
        {
            public int code;
            public AreaResponseData data;
        }

        [Serializable]
        class AreaResponseData
        {
            public string country;
            public string province;
            public string city;
            public string ip;
            public string server_time;
        }
    }
}
