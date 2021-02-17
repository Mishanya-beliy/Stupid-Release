using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OnlineGameProcesClient : MonoBehaviour
{
    public GameObject addBots, botPrefab, onlinePlayerPrefab;

    private sbyte[] deck;
    private List<List<sbyte>> playersDeck = new List<List<sbyte>>();
    private List<GameObject> players = new List<GameObject>(), pointPlayersIndicatorTurn = new List<GameObject>();
    private sbyte thisPlayerId, index;
    private List<sbyte> placeId = new List<sbyte>();
    private List<sbyte> idDeck = new List<sbyte>();

    public void ReciveDeck(sbyte[] deck, sbyte  index)
    {
        this.deck = deck;
        //Trump
        GetComponent<CreateCard>().Create((int)deck[35]);

        //Создание колоды
        GetComponent<CreateCard>().Reset();
        for (int i = index; i < deck.Length - 1; i++)
            GetComponent<CreateCard>().CreateDeck(deck[i]);
    }

    internal void RecivePlayersDeck(sbyte id, List<sbyte> playerDeck)
    {
        playersDeck.Add(playerDeck);
        idDeck.Add(id);
        //Создание карт
        foreach (sbyte card in playerDeck)
            if (id == thisPlayerId)
                GetComponent<CreateCard>().Create(card);
            else
                GetComponent<CreateCard>().Create(id, card, players[placeId.IndexOf(id)].transform);
    }

    internal void SetPlayerId(sbyte id)
    {
        thisPlayerId = id;
    }

    internal void StartNewGame()
    {
        Instantiate(addBots, GameObject.Find("Screen").transform);
        GameObject.Find("Recomendation").GetComponent<Recomendation>().Show
            ("В данной игре нету онлайн игроков добавьте ботов для начала игры");
    }
    internal void AddPlayer(byte type, sbyte point)
    {
        List<GameObject> points = GameObject.FindGameObjectsWithTag("Point Player").OfType<GameObject>().ToList();
        CardsMonitoring cm = GameObject.Find("SceneController").GetComponent<CardsMonitoring>();
        points.Sort(cm.CompareObNames);
        
        if(type == 1)
        {
            placeId.Add(point);
            players.Add(Instantiate(botPrefab, points[point].transform));
            players[players.Count - 1].name = "Bot" + point;
            pointPlayersIndicatorTurn.Add(points[point].transform.GetChild(0).gameObject);
            players[players.Count - 1].transform.GetChild(0).GetComponent<TextMeshPro>().text = point.ToString();
            players[players.Count - 1].GetComponent<CardPlacing>().nameCards = "CardsPlayer" + point;
        }else if(type == 2)
        {
            placeId.Add(point);
            players.Add(Instantiate(onlinePlayerPrefab));
            players[players.Count - 1].GetComponent<Player>().SetID(point);
            pointPlayersIndicatorTurn.Add(GameObject.Find("PointIndicatorTurnFirstPlayer"));
            players[players.Count - 1].GetComponent<CardPlacing>().nameCards = "CardsPlayer" + point;
            players[players.Count - 1].name = "OnlinePlayerId:" + point;
        }
        /*
        byte index = (byte)(HowManyPlayersNowPlay(true) - 1);
        typePlayers.Add(1);
        indexPlayers.Add((byte)(bots.Count - 1));
        //==================================
        List<byte> pointsPlayers = PlacePointForPlayer((sbyte)(countAddBots + HowManyPlayersNowPlay(true)));

        if (HowManyPlayersNowPlay(true) == 2)
            ChangePlayersPlace(points[pointsPlayers[1]].transform.GetChild(0).gameObject);*/
    }

    internal void ActivePlayer(int value)
    {

    }
}
