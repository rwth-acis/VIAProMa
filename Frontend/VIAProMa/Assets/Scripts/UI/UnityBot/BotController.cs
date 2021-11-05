using Microsoft.MixedReality.Toolkit.Input;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
//using UnityEditor.VersionControl;
using UnityEngine;
using i5.Toolkit.Core.OpenIDConnectClient;
using HoloToolkit.Unity;
using UnityEngine.UI;
using i5.VIAProMa.UI.Chat;
using TMPro;
using i5.VIAProMa.Multiplayer.Chat;

public class BotController : MonoBehaviour, IMixedRealityFocusHandler
{

    DictationHandler dictation;
    [SerializeField] private TextMesh textDisplay;
    private TextToSpeech textToSpeech;
    private AudioSource[] audioSources;
    private AudioSource audioSrcPing;

    [Obsolete]
    void Start()
    {
        dictation = GetComponent<DictationHandler>();
        textToSpeech = gameObject.AddComponent<TextToSpeech>();
        // Play a ping sound when speech recording starts
        audioSources = this.GetComponents<AudioSource>();
        foreach (AudioSource a in audioSources)
        {
            if ((a.clip != null) && (a.clip.name == "Ping"))
            {
                audioSrcPing = a; 
            }
        }

    }

    public void DictationHypothesis(string text)
    {
        Debug.Log($"Dictation Hypothesis: {text}");
        //textDisplay.text = text;
    }

    public void DictationResult(string text)
    {
        Debug.Log($"Dictation Result: {text}");
        textDisplay.text = text;
    }

    public void DictationCompleted(string text)
    {
        Debug.Log($"Dictation Completed: {text}");
        //textDisplay.text = text;
    }

    public void DictationError(string text)
    {
        Debug.Log($"Dictation Error: {text}");
        //textDisplay.text = text; 
    }


    public void OnFocusEnter(FocusEventData eventData)
    {
        if (dictation != null)
        {
           /* if (audioSrcPing != null)
            {
                audioSrcPing.Play();
            }*/
            dictation.StartRecording();
        }
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        if (dictation != null)
        {
            dictation.StopRecording();
            if (!string.IsNullOrEmpty(textDisplay.text))
            {
                Debug.Log("OnFocusExit: " + textDisplay.text);
                UserSendMessage.SendMessageToSlack(textDisplay.text);
                ChatManager.Instance.SendChatMessage("<b>Me</b>: " + textDisplay.text);
                textDisplay.text = string.Empty;
                string timeStamp = GetTimestamp();
                GetMessageFromBot(timeStamp);
            }
             
        }
    }

    public MesgCont msgb = new MesgCont() { text = "" };
    public async void GetMessageFromBot(string tmstp)
    {
        int cnt = 0;
        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(2000)).ConfigureAwait(true);
        string jsonString = UserSendMessage.GetMessageFromSlack(tmstp);
        var objArr = JObject.Parse(jsonString);
        JArray msgcont = JArray.Parse(objArr["messages"].ToString());
        IList<MesgCont> mesgConts = msgcont.Select(p => new MesgCont
        {
            username = (string)p["username"],
            ts = (double)p["ts"],
            text = (string)p["text"]
        }).ToList();

        foreach (var item in mesgConts)
        {
            if ((String.Compare(item.username, "unibot", true) == 0) & (item.ts >= Convert.ToDouble(tmstp)))
            {
                msgb.text = item.text;
                cnt += 1;
            }
        }
        if (cnt != 0)
        {
            textDisplay.text = msgb.text;           
            ChatManager.Instance.SendChatMessage("<b>UniBot</b>: " + msgb.text);
            textToSpeech.Voice = TextToSpeechVoice.Mark;
            textToSpeech.StartSpeaking(textDisplay.text);
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(1000)).ConfigureAwait(true);
            Debug.Log(textDisplay.text);
            textDisplay.text = string.Empty;
        }
        else
        {
            GetMessageFromBot(tmstp);
            Debug.Log("waiting for unibot responses...");
        }
    }
   
    public static string GetTimestamp()
    {
        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        return unixTimestamp.ToString();
    } 

}

public class MesgCont
{
    public string username
    {
        get;
        set;
    }
    public string text
    {
        get;
        set;
    }
    public double ts
    {
        get;
        set;
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public TextMesh textObject;
} 