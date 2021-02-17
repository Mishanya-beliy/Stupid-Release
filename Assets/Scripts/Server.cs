using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Server : MonoBehaviour
{
    private List<int> playersOnline = new List<int>();
    public void AddPlayers(int id)
    {
        playersOnline.Add(id);
    }
    public void RegisterAllHandlers()
    {
        NetworkServer.RegisterHandler(TypesMessage.syncPosition, OnSyncPositionReciveMessageServer);
        NetworkServer.RegisterHandler(TypesMessage.tablePositionSet, OnSetPositionTableReciveMessageServer);
        NetworkServer.RegisterHandler(TypesMessage.addBots, OnAddBotsReciveMessageServer);
        NetworkServer.RegisterHandler(TypesMessage.distributionCards, OnDistributionCardsReciveMessageServer);
    }
    //================================================SendMessage====================================================\\
    internal void SendActivePlayer(byte activePlayer)
    {
        NetworkServer.SendToAll(TypesMessage.activePlayer, new IntegerMessage(activePlayer));
    }
    internal void SendCreatePlayer(byte type, byte point)//0- this player 1 - bot 2 - online player
    {
        NetworkServer.SendToAll(TypesMessage.addPlayer, new AddPlayer(type, point));
    }
    public void SendStartNewGame()
    {
        NetworkServer.SendToClient(NetworkServer.localConnections[0].connectionId, TypesMessage.startNewGame, new EmptyMessage());
    }
    public void SendId(int networkId, sbyte idPlayer)
    {
        NetworkServer.SendToClient(networkId, TypesMessage.setPlayerId, new SetPlayerId(idPlayer));
    }
    public void SendShuffleDeck(sbyte[] deck, sbyte index)
    {
        NetworkServer.SendToAll(TypesMessage.shuffleDeck, new ShuffleDeck(deck, index));
    }
    internal void SendPlayerDeck(sbyte id, List<sbyte> playerDeck)
    {
        NetworkServer.SendToAll(TypesMessage.playersDeck, new SendPlayerDeckToClient(id, playerDeck));
    }
    //===============================================MessageHandlers=====================================================\\
    public void OnDistributionCardsReciveMessageServer(NetworkMessage receiverMsg)
    {
        GameObject.Find("SceneController").GetComponent<OnlineGameProcesServer>().DistributionCards();
    }
    public void OnAddBotsReciveMessageServer(NetworkMessage receiverMsg)
    {
        IntegerMessage msg = receiverMsg.ReadMessage<IntegerMessage>();
        GameObject.Find("SceneController").GetComponent<OnlineGameProcesServer>().OnAddBots((sbyte)msg.value);
    }
    public void OnSyncPositionReciveMessageServer(NetworkMessage receiverMsg)
    {
        SyncPositionMessage msg = receiverMsg.ReadMessage<SyncPositionMessage>();
        NetworkServer.SendToAll(TypesMessage.syncPosition, msg);
        Debug.Log("OnSyncPosition " + msg.positionPlayer);
    }
    public void OnSetPositionTableReciveMessageServer(NetworkMessage receiverMsg)
    {
        SetPositionTable msg = receiverMsg.ReadMessage<SetPositionTable>();

        if (msg.isSet)
        {
            AddPlayers(msg.id);
            GameObject.Find("SceneController").GetComponent<OnlineGameProcesServer>().JoinToGame(msg.id);
        }
        Debug.Log("OnSetPositionTableReciveMessageServer " + msg.isSet);
    }
}
public class AddPlayer : MessageBase
{
    public byte type;
    public byte point;
    public AddPlayer()
    {
    }
    public AddPlayer(byte type, byte point)
    {
        this.type = type;
        this.point = point;
    }
}
public class SyncPositionMessage : MessageBase
{
    public Vector3 positionPlayer;
    public Vector3 forrward;
    public Vector3 up;
    public SyncPositionMessage()
    {
    }
    public SyncPositionMessage(Vector3 positionPlayer, Vector3 forrward, Vector3 up)
    {
        this.positionPlayer = positionPlayer;
        this.forrward = forrward;
        this.up = up;
    }
}
public class NeedId : MessageBase
{
    public int networkId;
    public NeedId()
    {
    }
    public NeedId(int networkId)
    {
        this.networkId = networkId;
    }
}
public class SetPlayerId : MessageBase
{
    public sbyte id;
    public SetPlayerId()
    {
    }
    public SetPlayerId(sbyte id)
    {
        this.id = id;
    }
}
public class ShuffleDeck : MessageBase
{
    public sbyte[] deck;
    public sbyte index;
    public ShuffleDeck()
    {
    }
    public ShuffleDeck(sbyte[] deck, sbyte index)
    {
        this.deck = deck;
        this.index = index;
    }
}
public class SendPlayerDeckToClient : MessageBase
{
    public sbyte[] playerDeck;
    public sbyte id;
    public SendPlayerDeckToClient()
    {
    }
    public SendPlayerDeckToClient(sbyte id, List<sbyte> playerDeck)
    {
        this.id = id;
        this.playerDeck = playerDeck.ToArray();
    }
}
public class SetPositionTable : MessageBase
{
    public bool isSet;
    public int id;
    public SetPositionTable()
    {
    }
    public SetPositionTable(bool isSet, int id)
    {
        this.isSet = isSet;
        this.id = id;
    }
}
