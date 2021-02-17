using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class OnlineGameProcesServer : CardsMonitoring
{
    public GameObject addBots, onlinePlayerPrefab;
    private List<byte> typePlayers = new List<byte>(),//0 - this player 1 - bot 2 - another online player
        indexPlayers = new List<byte>();
    private List<GameObject> onlinePlayers = new List<GameObject>(), bots = new List<GameObject>(),
        pointPlayersIndicatorTurn = new List<GameObject>(), deckObj = new List<GameObject>();
    private bool firstGameInServer = true;
    private sbyte[] deck = new sbyte[36];
    private sbyte index = 0, trump = 0, thisPlayerId;
    private List<List<sbyte>> playersDeck = new List<List<sbyte>>();
    private List<sbyte> idPlayers = new List<sbyte>();

    private sbyte countPlayers = 0;

    private const int maxCountPlayers = 5;
    private void Reset()
    {
        /*incomingPlayers = null;
        attacker = -1;
        fightingOffPlayer = -1;
        activePlayer = -1;
        table = null;
        playersDeck = null;

        deckObj = null;

        cumToPlayerDeck = false;
        countCumToPlayerDeck = 0;
        take = false;

        nowTake = false;
        countTakeCards = 0;
        countIncomingCard = 0;

        firstDefense = true;
        firstCardInGame = true;

        pointsIncomingCard = null;
        pointsPlayers = null;

        numberOfPoint = 0;

        playersWantThrowInCard = null;
        fastEnd = false;

        deck = new sbyte[36];
        trump = 0;*/
        index = 0;
        //playersDeck = new List<List<sbyte>>();

        //Спрятать табличку дурака
        GameObject.FindGameObjectWithTag("GameTable").GetComponent<PlaceTable>().
            GetWictory().GetComponent<WictoryText>().SetVisibleAndText(false, "");

        /*
        //Удаление предыдущих ботов
        if (players != null)
            for (int i = 1; i < players.Length; i++)
                Destroy(players[i]);

        //Moze peredelat nado dlya online tak tochno
        players = new GameObject[countPlayers];
        players[0] = GameObject.FindGameObjectWithTag("MainCamera");
        players[0].GetComponent<Player>().SetID(0);*/

        //Точки индикаторов хода
        pointPlayersIndicatorTurn = new List<GameObject>();

        typePlayers = new List<byte>();

        //Create players and zeroed array
        /*
        for (int i = 0; i < countPlayers; i++)
        {
            playersDeck.Add(new List<sbyte>());
        }
        for (int i = 0; i < 36; i++)
        {
            deck[i] = -30;
        }*/
    }

    public int HowManyPlayersNowPlay(bool withBots)
    {
        if (withBots)
            return countPlayers;
        else
            return onlinePlayers.Count;
    }
    public void SetIdHost(sbyte id)
    {
        thisPlayerId = id;
    }
    //OnNewPlayerConnect
    internal sbyte GetIdNewPlayer()
    {
        sbyte id = -1;
        if (countPlayers == 0)
        {//send всем поставить на игровое место префаб онлайн игрока
            id = 0;
            countPlayers++;
            idPlayers.Add(id);
            return id;
        }
        return id;
    }
    //OnPlaceTable
    public void JoinToGame(int id)
    {
        if(countPlayers == 1)
            GameObject.Find("Netrwork").GetComponent<Server>().SendStartNewGame();

        if (HowManyPlayersNowPlay(true) == 1)
        {
            firstGameInServer = true;
        }
        else if (HowManyPlayersNowPlay(true) == maxCountPlayers && HowManyPlayersNowPlay(true) > onlinePlayers.Count)
        {
            GameObject.Find("Recomendation").GetComponent<Recomendation>().Show
                ("В данной игре максимум игроков, по окончанию будет удален бот и вы сможете присоеденится к игре");
        }
    }
    public void OnAddBots(sbyte countAddBots)
    {
        countPlayers += countAddBots;
        List<byte> pointsPlayers = PlacePointForPlayer(countPlayers);
        for(int i = 0; i < countAddBots; i++)
        {
            GameObject.Find("Netrwork").GetComponent<Server>().SendCreatePlayer(1, pointsPlayers[i]);
            idPlayers.Add((sbyte)pointsPlayers[i]);
        }
        //================         ТУТ ЗАКОНЧИЛ ======================== АААААААААААААААААААААААА============\\
        if (firstGameInServer)
        {
            firstGameInServer = false;
            DistributionCards();
            byte activePlayer = WhoFirstGo((byte)HowManyPlayersNowPlay(true), playersDeck, trump);
            GameObject.Find("Netrwork").GetComponent<Server>().SendActivePlayer(activePlayer);
        }
    }
    private void ChangePlayersPlace(GameObject point)
    {
        pointPlayersIndicatorTurn[1] = point;
        if (onlinePlayers.Count == 2)
        {//If second player online player
            //Уведомить о переселении игрока
        }
        else
        {//If second player bot
            bots[0].transform.SetPositionAndRotation(point.transform.parent.position, point.transform.parent.rotation);
            bots[0].transform.SetParent(point.transform.parent);
        }
    }
    internal void DistributionCards()
    {
        sbyte countPlayers = (sbyte)HowManyPlayersNowPlay(true);
        GameObject network = GameObject.Find("Netrwork");
        deck = ShuffleDeck(1,12).ToArray();

        //Раздача карт игрокам (allgood)
        playersDeck = new List<List<sbyte>>();// new List<sbyte>());
        for (int i = 0; i < countPlayers; i++)
            playersDeck.Add(new List<sbyte>());
        for (int i = 0; i < countPlayers; i++)
            for (int j = 0; j < 6; j++)
            {
                playersDeck[i].Add(deck[index]);
                index++;
            }
        network.GetComponent<Server>().SendShuffleDeck(deck, index);
        for(int i = 0; i < countPlayers; i++)
            network.GetComponent<Server>().SendPlayerDeck(idPlayers[i], playersDeck[i]);

        //Выбор козыря
        trump = deck[35];
    }

}
