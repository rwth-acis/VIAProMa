using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Multiplayer.Chat;

public class ChatSoundTester : MonoBehaviour
{
    public void Update()
    {
        if (PhotonNetwork.IsConnected && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Connecting to room", gameObject);
            PhotonNetwork.JoinOrCreateRoom("ChatSoundTest", null, null);
            StartCoroutine("SendMessages");
        }
    }

    private IEnumerator SendMessages()
    {
        while(true)
        {
            ChatManager.Instance.SendChatMessage("This is a test message!");
            yield return new WaitForSeconds(3);
        }
        
    }
}
