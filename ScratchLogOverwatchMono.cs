﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ScratchLogOverwatchMono : MonoBehaviour
{

    // TO DO: Check very n seconds for new input
    // if one detected go it play mode 
    // But if all are true, go in 50 every 0.25 seconds
    // If 50 are all new  got for 300 every 0.1 seconds
    // Use a quite mode > active mode > players are playing mode.
    // I means do something to not check to much on the server. Only when a game is in process and the player active.
    public string m_projectId = "966336670";
    public string m_limite = "100";
    public float m_waitTimeBetweenDownload = 3;
    public string m_logUrl = "https://clouddata.scratch.mit.edu/logs?projectid={0}&limit={1}&offset=0";
    public string m_logUrlGenerated = "https://clouddata.scratch.mit.edu/logs?projectid={0}&limit={1}&offset=0";

    [TextArea(0, 5)]
    public string m_currentJson;
    [TextArea(0, 5)]
    public string m_previousJson;

    public JsonScratchJsonCloudVarList jsonDataArrayCurrent;
    public JsonScratchJsonCloudVarList jsonDataArrayPrevious;
    public List<JsonScratchJsonCloudVarItem> newValues;
    public UnityEvent<ScratchCloudVarItemChanged> m_onNewUserCloudVarChanged;
  
    private bool m_firstDl=true;
    public void Start()
    {
        StartCoroutine(CheckLog());

        UnityEngine.Debug.Log(JsonUtility.ToJson(new JsonScratchJsonCloudVarList() { items = new JsonScratchJsonCloudVarItem[] { new JsonScratchJsonCloudVarItem() } }));
    }

    public float m_downloadInMilliseconds;
    private IEnumerator CheckLog()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (m_waitTimeBetweenDownload < 0.1)
                m_waitTimeBetweenDownload = 0.1f;
            Stopwatch track = new Stopwatch();
            track.Start();
            yield return new WaitForSeconds(m_waitTimeBetweenDownload);
            string url = string.Format(m_logUrl, m_projectId, m_limite);
            m_logUrlGenerated = url;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Send the request and wait for a response
                yield return webRequest.SendWebRequest();

                // Check for errors
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    m_previousJson = m_currentJson;
                    m_currentJson = "";
                    jsonDataArrayPrevious = jsonDataArrayCurrent;
                    jsonDataArrayCurrent = new JsonScratchJsonCloudVarList();
                }
                else
                {
                    string webpageContent = webRequest.downloadHandler.text;
                    m_previousJson = m_currentJson;
                    m_currentJson = webpageContent;

                    jsonDataArrayPrevious = jsonDataArrayCurrent;
                    jsonDataArrayCurrent = JsonUtility.FromJson<JsonScratchJsonCloudVarList>("{\"items\":" + m_currentJson + "}");

                    if (m_firstDl) { m_firstDl = false; }
                    else { 
                        newValues = 
                            jsonDataArrayCurrent.items.Except(
                                jsonDataArrayPrevious.items, 
                                new JsonScratchJsonCloudVarItemComparer()).ToList();
                        foreach (var item in newValues)
                        {
                            ScratchCloudVarItemChanged value = new ScratchCloudVarItemChanged();
                            value.m_cloudVarName =item.name.Replace("☁", "");
                            value.m_nowUtc = DateTime.UtcNow;
                            value.m_scratchTimestamp = item.timestamp;
                            value.m_username = item.user;
                            value.m_value = item.value;
                            value.m_projectId = m_projectId;
                            m_onNewUserCloudVarChanged.Invoke(value);
                        }
                    }
                }
            }track.Stop();
            m_downloadInMilliseconds = track.ElapsedMilliseconds;

        }
    }
}
    public class JsonScratchJsonCloudVarItemComparer : IEqualityComparer<JsonScratchJsonCloudVarItem>
    {
        public bool Equals(JsonScratchJsonCloudVarItem x, JsonScratchJsonCloudVarItem y)
        {
            // Compare based on the unique identifier generated by GetUnique
            return x.GetUnique() == y.GetUnique();
        }

        public int GetHashCode(JsonScratchJsonCloudVarItem obj)
        {
            // Implement GetHashCode if needed
            return obj.GetUnique().GetHashCode();
        }
    }
    [Serializable]
public class JsonScratchJsonCloudVarList
{
    public JsonScratchJsonCloudVarItem[] items= new JsonScratchJsonCloudVarItem[0];
}

[Serializable]
public class JsonScratchJsonCloudVarItem
{
    public string user;
    public string verb;
    public string name;
    public int value;
    public long timestamp;
    public string GetUnique() { return string.Format("{0}{1}{2}", timestamp, user, name);}
}

