using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TabControl : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public GameObject TabCanvas;

    public GameObject PanelTemplate1;
    public GameObject PanelTemplate2;
    public GameObject Parent;

    private float oldPlayersCount;
    public bool CanOutputTab = false;
    // Start is called before the first frame update
    void Start()
    {
        oldPlayersCount = PhotonNetwork.PlayerList.Length;
        PanelTemplate1.SetActive(false);
        PanelTemplate2.SetActive(false);
        TabCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void TabOutput()
    {

        for (int i = 0; i < Parent.transform.childCount; i++) {
            Destroy (Parent.transform.GetChild(i).gameObject);
        }

        List<Photon.Realtime.Player> playersTeam1 = new List<Photon.Realtime.Player>();
        List<Photon.Realtime.Player> playersTeam2 = new List<Photon.Realtime.Player>();
        if(PhotonNetwork.PlayerList.ToList().All(p => p.CustomProperties["Team"] != null))
        {
            playersTeam1 = PhotonNetwork.PlayerList.Where(p => (int)p.CustomProperties["Team"] == 1).ToList();  
            playersTeam2 = PhotonNetwork.PlayerList.Where(p => (int)p.CustomProperties["Team"] == 2).ToList();
        }
        // Debug.Log(playersTeam2.Count);
        GameObject g;
        GameObject planeTemplate = TabCanvas.transform.GetChild(0).gameObject;
        
        GameObject PT1 = Instantiate(PanelTemplate1, Parent.transform);
        GameObject PT2 = Instantiate(PanelTemplate2, Parent.transform);
        
        foreach (var player in playersTeam1)
        {
            int i = playersTeam1.IndexOf(player);
            g = Instantiate(PT1, Parent.transform);
            g.SetActive(true);
            g.transform.position = PT1.transform.position + new Vector3(0, (330-60 * i - (i)), 0);
            g.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.NickName;
            g.transform.Find("Kills").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["K"].ToString();
            g.transform.Find("Deaths").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["D"].ToString();
        }
        Destroy(PT1);
        foreach (var player in playersTeam2)
        {
            int i = playersTeam2.IndexOf(player);
            g = Instantiate(PT2, Parent.transform);
            g.SetActive(true);
            g.transform.position = PT2.transform.position + new Vector3(0, (330-60 * i - (i)), 0);
            g.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = player.NickName;
            g.transform.Find("Kills").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["K"].ToString();
            g.transform.Find("Deaths").GetComponent<TextMeshProUGUI>().text = player.CustomProperties["D"].ToString();
        }
        Destroy(PT2);
        FindObjectsOfType<Player>().ToList().ForEach(p => p.TabOutline());
    }
    // Update is called once per frame
    void Update()
    {
        
        if (Math.Abs(oldPlayersCount - PhotonNetwork.PlayerList.Length) > 0)
        {
            TabOutput();
            
            oldPlayersCount = PhotonNetwork.CountOfPlayers;
        }
        // Debug.Log(PhotonNetwork.room);
        if (Input.GetKey(KeyCode.Tab))
        {
            TabCanvas.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            TabCanvas.GetComponent<Canvas>().enabled = false;
        }
    }

    
    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        // foreach (var lobbyStatistic in lobbyStatistics)
        // {
        //     Debug.Log(lobbyStatistic);
        // }
        photonView.RPC(nameof(CallTab), RpcTarget.All);
        // Debug.Log("stat");
        // TabOutput();
    }

    
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        TabOutput();
        CanOutputTab = true;
        // Debug.Log(changedProps);
        // photonView.RPC(nameof(CallTab), RpcTarget.All);
    }
    
    // public override void OnRoomListUpdate(List<RoomInfo> roomList)
    // {
    //     Debug.Log("room");
    //     TabOutput();
    // }

    public override void OnJoinedLobby()
    {
        
        photonView.RPC(nameof(CallTab), RpcTarget.All);
    }

    public override void OnLeftRoom()
    {
        photonView.RPC(nameof(CallTab), RpcTarget.All);
    }

    public override void OnLeftLobby()
    {
        photonView.RPC(nameof(CallTab), RpcTarget.All);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        photonView.RPC(nameof(CallTab), RpcTarget.All);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        photonView.RPC(nameof(CallTab), RpcTarget.All);
    }

    public override void OnConnected()
    {
        photonView.RPC(nameof(CallTab), RpcTarget.All);
    }

    [PunRPC]
    public void CallTab()
    {
        TabOutput();
    }
}
