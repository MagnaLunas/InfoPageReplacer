using System;
using UnityEngine;
using MelonLoader;
using Il2CppSystem.IO;
using VRC.Core;

namespace InfoPageReplacer
{
    public static class BuildInfo
    {
        public const string Name = "InfoPageReplacer";
        public const string Author = "MagnaLuna";
        public const string Company = null;
        public const string Version = "0.1.1";
        public const string DownloadLink = "https://github.com/MagnaLunas/InfoPageReplacer";
    }

    public class InfoPageReplacer : MelonMod
    {
        public override void VRChat_OnUiManagerInit()
        {
            var gameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (var obj in gameObjects)
            {
                if (obj.name == "LoadingPopup")
                {
                    MelonCoroutines.Start(ChangeLoadingPanel(obj));
                }
            }
        }

        public System.Collections.IEnumerator ChangeLoadingPanel(GameObject loadingPanel)
        {
            bool needToReplaceInfo = false;

            string userDataPath = Path.Combine("UserData", "InfoPageReplacer");

            if (!Directory.Exists(userDataPath))
            {
                Directory.CreateDirectory(userDataPath);
            }

            string[] files = Directory.GetFiles(userDataPath, "*.*");

            foreach(string s in files)
            {
                if (s.EndsWith(".jpg", true, null) || s.EndsWith(".png", true, null))
                {
                    needToReplaceInfo = true;
                    break;
                }
            }
            
            if (needToReplaceInfo)
            {
                Il2CppSystem.Collections.Generic.List<LoadingInfoData> loadingInfos = loadingPanel.GetComponent<VRCUiPageLoading>().loadingInfoSet.loadingInfos;
                //Time.timeScale = 3; //For fast debug
                loadingInfos.Clear();
                MelonModLogger.Log("Loading files...");
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].EndsWith(".jpg", true, null) || files[i].EndsWith(".png", true, null))
                    {

                        WWW www = new WWW("file:///" + Path.Combine(Environment.CurrentDirectory, files[i]));
                        yield return www;
                        LoadingInfoData loadingInfoData = new LoadingInfoData();
                        loadingInfoData.texture = www.texture;
                        loadingInfoData.weight = 1;
                        loadingInfoData.name = i.ToString();
                        loadingInfos.Add(loadingInfoData);
                        MelonModLogger.Log("Loaded " + files[i].Remove(0, 26));
                    }
                    else
                    {
                        MelonModLogger.LogError("File " + files[i].Remove(0, 26) + " is not supported! Delete it!");
                    }
                }
                MelonModLogger.Log("loadingInfos done!");
            }
            else
            {
                MelonModLogger.LogError("No supported files found! Skipping replacement!");
            }
        }
    }
}
