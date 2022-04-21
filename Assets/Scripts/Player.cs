using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public int playerHp;
    public int k, d;
    
    float hpadd = 0;
    bool dead = false;

    public PlayerLocal PlayerLocal;
    public GameObject Canvas;

    public TMP_Text hpCount;

    public Camera Camera;
    public GameObject plane;

    public string lastDamagePlayer;

    void Start()
    {
        transform.name = photonView.Owner.NickName;
    }

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            PlayerLocal.enabled = false;
            Destroy(PlayerLocal.camera.gameObject);
            Canvas.SetActive(false);
            gameObject.layer = 0;
            gameObject.GetComponent<MeshCollider>().isTrigger = false;
        }
    }

    public void Update()
    {
        hpCount.text = $"hp: {playerHp}";
        if (playerHp > 500)
        {
            playerHp = 500;
        }
        if (photonView.IsMine)
        {
            if (photonView.Owner.CustomProperties["K"] != null)
            {
                k = (int)photonView.Owner.CustomProperties["K"];
                d = (int)photonView.Owner.CustomProperties["D"];
            }
            if (playerHp <= 0 )
            {
                // Dead();
                playerHp = 499;
            }
        }
        else
        {
            
            if (playerHp <= 0)
            {
                dead = true;
            }
        }
        plane.GetComponent<Renderer>().material.color = Color.cyan;
        if (Camera == null)
            Camera = FindObjectOfType<Camera>();
        if (Camera != null)
        {
            plane.transform.LookAt(Camera.transform);
        }
    }
    public void Dead()
    {
        if (photonView.IsMine)
        {
            if (dead == false)
            {
                if (lastDamagePlayer != "" && lastDamagePlayer != photonView.Owner.NickName)
                {
                    var ldp = PhotonNetwork.PlayerList.ToList().Find(x => x.NickName == lastDamagePlayer);
                    if (ldp != null)
                    {
                        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                        h.Add("K", (int)(((int)ldp.CustomProperties["K"]) + 1));
                        h.Add("D", (int)ldp.CustomProperties["D"]);
                        ldp.SetCustomProperties(h);
                    }
                }
                d++;
                SaveKD();
                PhotonNetwork.Destroy(gameObject);
                dead = true;
                // FindObjectOfType<GameManager>().StartCoroutine(FindObjectOfType<GameManager>().Respawn());
            }
        }
    }
    public void SaveKD()
    {
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h.Add("K", k);
        h.Add("D", d);
        photonView.Owner.SetCustomProperties(h);
    }
    [PunRPC]
    public void TakeDamage(int dmg, string actorName)
    {
        if (photonView.IsMine)
        {
            lastDamagePlayer = actorName;
            playerHp -= dmg;
            // timeLastDamage = 0; 
            hpadd = 0;
        }
    } 
}
