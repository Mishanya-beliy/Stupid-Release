using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.Networking.Types;

public class NetworkMultiplayer : MonoBehaviour
{
    [System.NonSerialized]
    private NetworkID[] networkId;
    private NetworkClient thisClient;
    void Start()
    {
        ConnectionsManager.singleton.StartMatchMaker();
    }

    //call this method to request a match to be created on the server
    public void CreateInternetMatch(string matchName, uint maxSize, bool isPublic, string pass)
    {
        ConnectionsManager.singleton.matchMaker.CreateMatch(matchName, maxSize, isPublic, pass, "", "", 0, 0, OnInternetMatchCreate);
    }
    //this method is called when your request for creating a match is returned
    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            GetComponentInChildren<NetworkUI>().SuccessfulCreateServer();
            Debug.Log("Create match succeeded");
            NetworkServer.Listen(matchInfo, matchInfo.port);
            
            GetComponent<Client>().RegisterAllHandlers(ConnectionsManager.singleton.StartHost(matchInfo));
            GameObject sceneController = GameObject.Find("SceneController");
            if (sceneController)
            {
                sceneController.AddComponent<OnlineGameProcesServer>();
            }

            GetComponent<Server>().RegisterAllHandlers();
        }
        else
        {
            if(extendedInfo == "Failed; CCU exceeded for appId=9063102")
            {
            }
            GetComponentInChildren<NetworkUI>().FailedCreateServer();
            Debug.LogError("Create match failed " + extendedInfo);
        }
    }

    //call this method to find a match through the matchmaker
    public void FindInternetMatch(string matchName, bool withPassword)
    {
        ConnectionsManager.singleton.matchMaker.ListMatches(0, 10, matchName, withPassword, 0, 0, OnInternetMatchList);
        
    }

    //this method is called when a list of matches is returned
    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                InformationOfMatches[] info = new InformationOfMatches[matches.Count];
                networkId =  new NetworkID[matches.Count];
                for (int i = 0; i < matches.Count; i++)
                {
                    networkId[i] = matches[i].networkId;
                    info[i] = new InformationOfMatches(matches[i].name, matches[i].maxSize, matches[i].currentSize);
                }

                GetComponentInChildren<NetworkUI>().ShowListMatches(matches.Count, info);
            }
            else
            {
                GetComponentInChildren<NetworkUI>().NoMatches();
                Debug.Log("No matches in requested room!");
            }
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker " + extendedInfo);
        }
    }
    public void JoinToMatch(int numberOfArrray)
    {
        ConnectionsManager.singleton.matchMaker.JoinMatch(networkId[numberOfArrray], "", "", "", 0, 0, OnJoinInternetMatch);
    }

    //this method is called when your request to join a match is returned
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (success)
        {
            Debug.Log("Able to join a match");
            //Client
            GetComponent<Client>().RegisterAllHandlers(ConnectionsManager.singleton.StartClient(matchInfo));
        }
        else
        {
            Debug.LogError("Join match failed");
        }
    }
    private void KickPlayer()
    {
        //NetworkManager.singleton.matchMaker.DropConnection(networkId[0], 123, 0, OnKickPlayer);
    }
    private void OnKickPlayer(bool success, string extendedInfo)
    {

    }
    private void EndOfMatch()
    {
        ConnectionsManager.singleton.matchMaker.DestroyMatch(networkId[0], 0, OnEndOfMatch);
    }
    private void OnEndOfMatch(bool success, string extendedInfo)
    {
    }
}
