using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using i5.VIAProMa.UI;

public class PlayerListItem : MonoBehaviour
{
    public static string playerName = "";

    public void GetPlayerList()
    {
        var dropdown = transform.GetComponent<Dropdown>();

        dropdown.options.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = player.NickName });
        }
        DropDownPlayerSelected(dropdown);
        dropdown.onValueChanged.AddListener(delegate { DropDownPlayerSelected(dropdown); });
    }

    public void DropDownPlayerSelected(Dropdown dropdown)
    {
        int index = dropdown.value;
        playerName = dropdown.options[index].text;
    }

}
