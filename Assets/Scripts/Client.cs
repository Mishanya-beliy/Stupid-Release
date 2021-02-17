using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class Client : MonoBehaviour
{
    private NetworkClient client;
    private int id;
    public void RegisterAllHandlers(NetworkClient client)
    {
        this.client = client;
        client.RegisterHandler(TypesMessage.syncPosition, OnSyncPositionReciveMessageClient);
        client.RegisterHandler(TypesMessage.shuffleDeck, OnShuffleDeckReciveMessageClient);
        client.RegisterHandler(TypesMessage.playersDeck, OnPlayerDeckReciveMessageClient);
        client.RegisterHandler(TypesMessage.setPlayerId, OnSetPlayerIdReciveMessageClient);
        client.RegisterHandler(TypesMessage.startNewGame, OnStartNewGameReciveMessageClient);
        client.RegisterHandler(TypesMessage.addPlayer, OnAddPlayerInGameReciveMessageClient);
    }
    //================================================SendMessage====================================================\\
    public void SendSetGameTable(bool isSet)
    {
        if (client != null)
            client.Send(TypesMessage.tablePositionSet, new SetPositionTable(isSet, id));
    }
    public void SendAddBot(int count)
    {
        if (client != null)
            client.Send(TypesMessage.addBots, new IntegerMessage(count));
    }

    public void SendPosition(Vector3 positionPlayer, Vector3 forrward, Vector3 up)
    {
        if (client != null)
            client.Send(TypesMessage.syncPosition, new SyncPositionMessage(positionPlayer, forrward, up));
    }
    //==============================================MessageHandlers==================================================\\
    public void OnActivePlayerReciveMessageClient(NetworkMessage receiverMsg)
    {
        IntegerMessage msg = receiverMsg.ReadMessage<IntegerMessage>();
        GameObject.Find("SceneController").GetComponent<OnlineGameProcesClient>().ActivePlayer(msg.value);
    }
    public void OnAddPlayerInGameReciveMessageClient(NetworkMessage receiverMsg)
    {
        AddPlayer msg = receiverMsg.ReadMessage<AddPlayer>();
        GameObject.Find("SceneController").GetComponent<OnlineGameProcesClient>().AddPlayer(msg.type, (sbyte)msg.point);
    }
    public void OnStartNewGameReciveMessageClient(NetworkMessage receiverMsg)
    {
        GameObject.Find("SceneController").GetComponent<OnlineGameProcesClient>().StartNewGame();
    }
    public void OnSetPlayerIdReciveMessageClient(NetworkMessage receiverMsg)
    {
        SetPlayerId msg = receiverMsg.ReadMessage<SetPlayerId>();
        id = msg.id;
        GameObject.Find("SceneController").GetComponent<OnlineGameProcesClient>().SetPlayerId(msg.id);
    }
    public void OnSyncPositionReciveMessageClient(NetworkMessage receiverMsg)
    {
        SyncPositionMessage msg = receiverMsg.ReadMessage<SyncPositionMessage>();
        GameObject.Find("SceneController").GetComponent<CardsMonitoring>().SyncPlayerPositionRecive(msg.positionPlayer,
            msg.forrward, msg.up);
    }
    public void OnShuffleDeckReciveMessageClient(NetworkMessage receiverMsg)
    {
        ShuffleDeck msg = receiverMsg.ReadMessage<ShuffleDeck>();
        GameObject.Find("SceneController").GetComponent<OnlineGameProcesClient>().ReciveDeck(msg.deck, msg.index);
    }
    public void OnPlayerDeckReciveMessageClient(NetworkMessage receiverMsg)
    {
        SendPlayerDeckToClient msg = receiverMsg.ReadMessage<SendPlayerDeckToClient>();

        GameObject.Find("SceneController").GetComponent<OnlineGameProcesClient>().RecivePlayersDeck(msg.id, msg.playerDeck.ToList());
    }

    //===================================================STAFF=======================================================\\

}
public class TypesMessage : MessageBase
{
    public const short tablePositionSet = 44;
    public const short needId = 45;
    public const short setPlayerId = 46; 
    public const short startNewGame = 47;
    public const short distributionCards = 48;
    public const short activePlayer = 49;
    public const short syncPosition = 1488;
    public const short shuffleDeck = 50;
    public const short playersDeck = 51;
    public const short addBots = 100;
    public const short addPlayer = 99; 
}
