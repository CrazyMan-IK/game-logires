using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using CrazyGames.Logires.Utils;

namespace CrazyGames
{
    public class AdsPool : MonoBehaviour, IUnityAdsListener
    {
        private const string _GameId = "3942697";
        private const string _DefaultId = "video";
        private const string _RewardedId = "rewardedVideo";

        public event Action<string> AdReady;
        public event Action<string, ShowResult> AdFinished;

        private void Start()
        {
            Advertisement.AddListener(this);
            Advertisement.Initialize(_GameId, true);
        }

        public void ShowVideo()
        {
            Advertisement.Show(_DefaultId);
        }

        public void ShowRewardedVideo()
        {
            Advertisement.Show(_RewardedId);
        }

        public void OnUnityAdsDidError(string message)
        {
            Debug.LogError("AD ERROR: " + message);
        }

        public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
        {
            if (showResult == ShowResult.Finished)
            {
                var currentTime = DateTime.Now;
                var endTime = EncryptedGlobalPreferences.GetPrimitive("advanced_set_end_time", currentTime);
                if (endTime < currentTime)
                {
                    endTime = currentTime;
                }

                EncryptedGlobalPreferences.SetPrimitive("advanced_set_end_time", endTime.Value.AddMinutes(30));

                Debug.Log("FINISH: FINISHED");
            }
            else if (showResult == ShowResult.Skipped)
            {
                Debug.Log("FINISH: SKIPPED");
            }
            else if (showResult == ShowResult.Failed)
            {
                Debug.LogWarning("FINISH: FAILED");
            }

            AdFinished?.Invoke(placementId, showResult);
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            Debug.Log("AD STARTED: " + placementId);
        }

        public void OnUnityAdsReady(string placementId)
        {
            AdReady?.Invoke(placementId);
        }
    }
}
