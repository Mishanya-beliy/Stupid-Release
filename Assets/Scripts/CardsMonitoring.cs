using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;
using System.Xml.Linq;
using System.Linq;

public class CardsMonitoring : MonoBehaviour
{
    public GameObject indicatorTurn;
    public Material attack, defense;
    public GameObject wictoryText;

    private sbyte[] incomingPlayers = null;
    private sbyte attacker;
    private sbyte fightingOffPlayer = -1;
    private sbyte activePlayer = 0;
    private List<sbyte> table = null;
    private List<List<sbyte>> playersDeck;

    private sbyte countPlayers = 0;
    private sbyte index = 0;
    private sbyte[] deck = new sbyte[36];
    private List<GameObject> deckObj;
    private sbyte trump = 0;

    public GameObject bot;
    private GameObject[] players;
    private sbyte[] typePlayers; // 0 player 1 bot

    private bool cumToPlayerDeck = false;
    private byte countCumToPlayerDeck = 0;
    private bool take = false;
    private byte countDiscardCard = 0;

    private bool nowTake = false;
    private sbyte countTakeCards = 0;
    private sbyte countIncomingCard = 0;

    private bool firstDefense = true;
    private bool firstCardInGame = true;

    private List<List<byte>> pointsIncomingCard;
    private List<byte> pointsPlayers;
    private GameObject[] pointPlayersIndicatorTurn;

    sbyte numberOfPoint = 0;

    private bool[] playersWantThrowInCard;

    private bool fastEnd = false;

    private sbyte stupidPlayer;

    private bool player_see_explanation_u_dont_have_card_for_defense;
    private bool player_see_explanation_u_dont_have_card_for_throwIn;
    private bool player_see_recommendation_u_can_defense_any_card;
    private bool player_see_recommendation_u_can_throwIn_any_card;
    private bool player_see_recommendation_u_can_attack_any_card;
    private bool player_see_recommendation_can_dont_wait_bots_play;

    public void CountPlayers(sbyte count)
    {
        countPlayers = count;
    }
    public void Reset()
    {
        incomingPlayers = null;
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
        index = 0;
        trump = 0;

        //Спрятать табличку дурака
        GameObject.FindGameObjectWithTag("GameTable").GetComponent<PlaceTable>().
            GetWictory().GetComponent<WictoryText>().SetVisibleAndText(false, "");

        //Удаление предыдущих ботов
        if (players != null)
            for (int i = 1; i < players.Length; i++)
                Destroy(players[i]);

        //Moze peredelat nado dlya online tak tochno
        players = new GameObject[countPlayers];
        players[0] = GameObject.FindGameObjectWithTag("MainCamera");
        players[0].GetComponent<Player>().SetID(0);

        //Точки индикаторов хода
        pointPlayersIndicatorTurn = new GameObject[countPlayers];
        pointPlayersIndicatorTurn[0] = GameObject.Find("PointIndicatorTurnFirstPlayer");

        typePlayers = new sbyte[countPlayers];

        //Create players and zeroed array
        playersDeck = new List<List<sbyte>>();
        for (int i = 0; i < countPlayers; i++)
        {
            playersDeck.Add(new List<sbyte>());
        }
        for (int i = 0; i < 36; i++)
        {
            deck[i] = -30;
        }
    }
    public void DistributionOfCards(sbyte newGame)//<--- -1 new game 0 next play 2 online game
    {
        Reset();

        //Создание и расстановка ботов
        GameObject[] points = GameObject.FindGameObjectsWithTag("Point Player");
        Array.Sort(points, CompareObNames);
        pointsPlayers = PlacePointForPlayer(countPlayers);

        for (int i = 1; i < countPlayers; i++)
        {
            GameObject bots = Instantiate(bot);
            bots.name = "Bot" + (i - 1);
            pointPlayersIndicatorTurn[i] = points[pointsPlayers[i - 1]].transform.GetChild(0).gameObject;

            bots.transform.GetChild(0).GetComponent<TextMeshPro>().text = i.ToString();
            players[i] = bots;
            if (bots.name.IndexOf("Bot") >= 0)
                typePlayers[i] = 1;
            bots.transform.SetPositionAndRotation(points[pointsPlayers[i - 1]].transform.position,
                points[pointsPlayers[i - 1]].transform.rotation);
            bots.transform.SetParent(points[pointsPlayers[i - 1]].transform);
            bots.GetComponent<CardPlacing>().nameCards = "CardsBot" + (i - 1);
        }

        //Перетасовка карт (allgood)
        for (int i = 0; i < 36; i++)
        {
            bool correct = true;
            while (correct)
            {
                int card = UnityEngine.Random.Range(0, 36);

                if (!AlreadySuchNumbersInArray(card, i, deck))
                {
                    deck[i] = (sbyte)card;
                    correct = false;
                }
            }
        }

        //Раздача карт игрокам (allgood)
        for (int i = 0; i < countPlayers; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                playersDeck[i].Add(deck[index]);
                index++;
            }
        }

        //Выбор козыря (allgood)
        trump = deck[35];
        GetComponent<CreateCard>().Create((int)deck[35]);

        //Колода
        GetComponent<CreateCard>().Reset();
        for (int i = index; i < deck.Length - 1; i++)
            GetComponent<CreateCard>().CreateDeck(deck[i]);
        //deckObj = GameObject.FindGameObjectsWithTag("Deck").ToList();
        //deckObj = SortDeck(deck, index, deckObj);


        //Создание карт игрока
        foreach (sbyte card in playersDeck[0])
            GetComponent<CreateCard>().Create(card);
        //Создание карт bots
        for (int i = 1; i < countPlayers; i++)
            for (int j = 0; j < playersDeck[i].Count; j++)
                GetComponent<CreateCard>().Create((sbyte)(i - 1), playersDeck[i][j]);


        //Кто первый начинает игру
        if (newGame == -1)
            activePlayer = (sbyte)WhoFirstGo((byte)countPlayers, playersDeck, trump);
        else
        {
            //Если не первая игра к дураку ходят
            stupidPlayer--;
            if (stupidPlayer == -1)
                activePlayer = (sbyte)(countPlayers - 1);
            else
                activePlayer = stupidPlayer;
        }
        //Для каждого игрока разные точки для ходящих карт
        PlacePointForIncomingCard();

        //Начало Игры
        if (activePlayer == 0)
            WhoIncomingWhoFightOff(Convert.ToSByte(countPlayers));
        else
            WhoIncomingWhoFightOff(Convert.ToSByte(activePlayer));
    }
    private void Update()
    {
        //SyncPlayerPositionSend();
    }
    //Начало хода
    void DealCards(sbyte incomingPlayer)
    {
        //Чтобы не добавлять везде единицу
        incomingPlayer++;
        table = null;
        take = false;
        countTakeCards = 125;
        activePlayer = incomingPlayer;

        //Раздача карт если нужно если есть
        if (!AllPlayersHaveSixCard(playersDeck) && index != deck.Length)
        {
            List<PositionAndRotation[]> positionRotationNewCards = new List<PositionAndRotation[]>();
            for (int i = 0; i < countPlayers; i++)
                if (playersDeck[i].Count < 6)
                    positionRotationNewCards.Add(players[i].GetComponent<CardPlacing>().PointsForNewCards((byte)(6 - playersDeck[i].Count)));
                else
                    positionRotationNewCards.Add(new PositionAndRotation[0]);

            cumToPlayerDeck = true;

            for (int j = 0; !AllPlayersHaveSixCard(playersDeck) && index != deck.Length; j++)
            {
                for (int i = 0; i < incomingPlayers.Length && index != deck.Length; i++)
                    if (playersDeck[incomingPlayers[i]].Count < 6)
                        if (index == deck.Length - 1)
                            AddCardInPlayerDeck(incomingPlayers[i], GameObject.FindGameObjectWithTag("Trump"),
                                positionRotationNewCards[incomingPlayers[i]][j]);
                        else
                        {
                            List<GameObject> deckObj = GameObject.FindGameObjectsWithTag("Deck").ToList();
                            AddCardInPlayerDeck(incomingPlayers[i], deckObj.Where(obj => obj.name == deck[index].ToString()).SingleOrDefault(),
                                positionRotationNewCards[incomingPlayers[i]][j]);
                        }

                if (playersDeck[fightingOffPlayer].Count < 6 && index != deck.Length)
                    if (index == deck.Length - 1)
                        AddCardInPlayerDeck(fightingOffPlayer, GameObject.FindGameObjectWithTag("Trump"),
                            positionRotationNewCards[fightingOffPlayer][j]);
                    else
                    {
                        List<GameObject> deckObj = GameObject.FindGameObjectsWithTag("Deck").ToList();
                        AddCardInPlayerDeck(fightingOffPlayer, deckObj.Where(obj => obj.name == deck[index].ToString()).SingleOrDefault(),
                         positionRotationNewCards[fightingOffPlayer][j]);
                    }
            }
            Debug.Log("Cards is deal");
        }
        else
        {
            Debug.Log("Cards is not need deals");
            WhoIncomingWhoFightOff(incomingPlayer);
        }
    }
    GameObject indicatorAttackTurn, indicatorDefenseTurn;
    void WhoIncomingWhoFightOff(sbyte incomingPlayer)
    {
        //Обнуляем: ходящих игроков, точка нас толе для карты, кол-во атакующих карт, игроки которые хотят подбросить
        incomingPlayers = new sbyte[countPlayers - 1];
        numberOfPoint = 0;
        countIncomingCard = -1;
        countTakeCards = 0;
        playersWantThrowInCard = new bool[countPlayers - 1];
        for (int i = 0; i < playersWantThrowInCard.Length; i++)
            playersWantThrowInCard[i] = true;

        //Если нужно начать считать игроков с начала (нуля)
        if (incomingPlayer == countPlayers)
        {
            //Первые два игрока ходящий и отбивающий(необходимый минимум)
            incomingPlayers[0] = 0;
            if (playersDeck[incomingPlayers[0]].Count == 0)
            {
                WhoIncomingWhoFightOff(1);
                return;
            }
            fightingOffPlayer = 1;

            //Остальные ходящие(которые могут подбрасывать) игроки
            for (int i = 1; i < incomingPlayers.Length; i++)
            {
                incomingPlayers[i] = Convert.ToSByte(i + 1);
            }
        }
        else
        {
            //Если ходящий не выходит за пределы кол-ва игроков
            incomingPlayers[0] = incomingPlayer;
            if (playersDeck[incomingPlayers[0]].Count == 0)
            {
                if (incomingPlayer + 1 >= countPlayers)
                    WhoIncomingWhoFightOff(0);
                else
                    WhoIncomingWhoFightOff(++incomingPlayer);
                return;
            }

            //Если отбивающийся выходит за пределы кол-ва игроков (начинаем с нуля считать)
            if (incomingPlayer + 1 >= countPlayers)
                fightingOffPlayer = 0;
            else
                fightingOffPlayer = (sbyte)(incomingPlayer + 1);

            //Остальные ходящие игроки с учетом что может стать больше чем кол-во игроков
            int inaccuracy = 0;
            for (int i = 0; i < incomingPlayers.Length - 1; i++)
            {
                if (incomingPlayer + i + 2 >= countPlayers)
                    inaccuracy = countPlayers;
                incomingPlayers[i + 1] = (sbyte)(incomingPlayer + i + 2 - inaccuracy);
            }
        }

        //Отбивающийся игрок если без карт назначается следующий пока не будет того у кого есть карты
        while (playersDeck[fightingOffPlayer].Count == 0)
        {
            fightingOffPlayer = incomingPlayers[1];
            incomingPlayers = RemovePlayerFromIncomingArray(incomingPlayers, new List<sbyte> { incomingPlayers[1] });
        }

        //Удаление предыдущих индикаторов
        if (indicatorAttackTurn != null)
            Destroy(indicatorAttackTurn);
        if (indicatorDefenseTurn != null)
            Destroy(indicatorDefenseTurn);

        //Индикатор атакующего
        indicatorAttackTurn = Instantiate(indicatorTurn);
        indicatorAttackTurn.transform.SetPositionAndRotation(pointPlayersIndicatorTurn[incomingPlayers[0]].
            transform.position, pointPlayersIndicatorTurn[incomingPlayers[0]].transform.rotation);
        indicatorAttackTurn.GetComponent<MeshRenderer>().material = attack;
        indicatorAttackTurn.transform.SetParent(GameObject.FindGameObjectWithTag("GameTable").transform);

        //Индикатор защищающегося
        indicatorDefenseTurn = Instantiate(indicatorTurn);
        indicatorDefenseTurn.transform.SetPositionAndRotation(pointPlayersIndicatorTurn[fightingOffPlayer].
            transform.position, pointPlayersIndicatorTurn[fightingOffPlayer].transform.rotation);
        indicatorDefenseTurn.GetComponent<MeshRenderer>().material = defense;
        indicatorDefenseTurn.transform.SetParent(GameObject.FindGameObjectWithTag("GameTable").transform);

        attacker = incomingPlayers[0];
        Debug.Log("Incoming fightoff players set");
        WhoActivePlayer(fightingOffPlayer, false);
    }
    public void WhoActivePlayer(sbyte nowActivePlayer, bool pass)
    {
        //Перед каждым ходои проверка игроков у которых нету карт и удаление если таковые имеются 
        List<sbyte> playerWithoutCard = HavePlayerWithoutCards(playersDeck, incomingPlayers);
        if (playerWithoutCard != null)
            incomingPlayers = RemovePlayerFromIncomingArray(incomingPlayers, playerWithoutCard);

        if (nowTake)
        {
            //Подбрасывание карт которые заберет игрок
            if (!pass)
                countTakeCards++;
            ThrowIn(nowActivePlayer, pass);
            return;
        }
        else
        {
            if (nowActivePlayer != fightingOffPlayer)
            {
                if (pass)
                {
                    //Подбрасыванaие
                    ThrowIn(nowActivePlayer, pass);
                    return;
                }
                else
                    //Oтбивание
                    activePlayer = fightingOffPlayer;
            }
            else
            {
                if (pass)
                {
                    //Забирающему начинают подбрасывать
                    nowTake = true;
                    if (table.Count % 2 != 0)
                        numberOfPoint++;
                    countTakeCards = 1;
                    ThrowIn(incomingPlayers[0], false);
                    return;
                }
                else
                {
                    //После удачного отбития ходит первый ходящий
                    activePlayer = incomingPlayers[0];
                    countIncomingCard++;
                }
            }
        }

        SetTurn();
    }
    void ThrowIn(sbyte nowActivePlayer, bool pass)
    {
        //Больше неукого нечего подкинуть
        if (AllArrayValueEqual(playersWantThrowInCard, false) ||
            //Если забирает кол-во подкинутых неотбившихся карт не больше 
            (nowTake && countTakeCards >= playersDeck[fightingOffPlayer].Count) ||
            //При первой защите максимум до 5
            (firstDefense && countTakeCards + countIncomingCard >= 5) ||
            //После первой защиты максимум до 6
            (!firstDefense && countTakeCards + countIncomingCard >= 6))
        {
            //Больше нельзя или некому подбрасывать
            if (nowTake)
            {
                //Забирает
                nowTake = false;
                activePlayer = fightingOffPlayer;
                Debug.Log("Take all cards");
                TakeCards(fightingOffPlayer);
                return;

            }
            else
            {
                //Отбой
                SuccessfulDefense();
                DealCards(attacker);
                return;
            }
        }
        else
        {
            if (pass)
            {
                //Индекс следующего ходящего
                int indexNextActivePlayer = Array.IndexOf(incomingPlayers, nowActivePlayer) + 1;

                //Предыдущему игроку нечего подкидывать
                if (indexNextActivePlayer == 0)
                    playersWantThrowInCard[playersWantThrowInCard.Length - 1] = false;
                else
                    playersWantThrowInCard[indexNextActivePlayer - 1] = false;

                //Следующий игрок подкидывает
                if (indexNextActivePlayer == incomingPlayers.Length)
                    activePlayer = incomingPlayers[0];
                else
                    activePlayer = incomingPlayers[indexNextActivePlayer];

            }
            else
                //Игроку предоставляется ещё одна возможность подкинуть карту
                activePlayer = nowActivePlayer;
        }
        SetTurn();
    }
    void SetTurn()
    {
        //Передача хода
        switch (typePlayers[activePlayer])
        {
            //bot
            case 1:
                //Бот отбиваеться или ходит
                sbyte card;
                if (activePlayer == fightingOffPlayer)
                    //Отбивается
                    card = players[activePlayer].GetComponent<Bot>().BotFightingOff(playersDeck[activePlayer], table[table.Count - 1], trump);
                else
                {
                    //Ходит
                    bool type;
                    if (table == null)
                        type = true;
                    else
                        type = false;

                    card = players[activePlayer].GetComponent<Bot>().BotIncoming(playersDeck[activePlayer], type);
                }

                if (card != -1)
                {
                    Debug.Log("Bot incoming card" + card);
                    IncomingPlayerLaysCard(card, activePlayer);
                }
                else
                {
                    Debug.Log("Bot dont have card");
                    WhoActivePlayer(activePlayer, true);
                }
                break;
            //this player
            case 0:
                Debug.Log("Player turn");
                Recomendation recomendation = GameObject.Find("Recomendation").GetComponent<Recomendation>();
                string textButtonPass;
                if (activePlayer == fightingOffPlayer)
                {
                    if (HaveWhatsDefendCards(playersDeck[activePlayer]) == -1)
                    {
                        //Выводится обьяснение
                        if (!player_see_explanation_u_dont_have_card_for_defense)
                        {
                            player_see_explanation_u_dont_have_card_for_defense = true;
                            recomendation.Show("Вам нечем побится вы забираете карты");
                            RecordInXml("/ExplanationAndRecommendation.xml", "ExplanationAndRecommendation",
                                "Explanation", "explanationDefense", "True");
                        }
                        WhoActivePlayer(activePlayer, true);
                        return;
                    }

                    //Выводится рекомендация
                    if (!player_see_recommendation_u_can_defense_any_card)
                    {
                        player_see_recommendation_u_can_defense_any_card = true;
                        recomendation.Show("Ваш ход, отбивайтесь любой возможной картой или заберите карты");
                        RecordInXml("/ExplanationAndRecommendation.xml", "ExplanationAndRecommendation",
                            "Recommendation", "recommendationDefense", "True");
                    }

                    textButtonPass = "3абрать карты";
                }
                else
                {
                    if (table != null && HaveWhatsAddOfCards(playersDeck[activePlayer]) == -1)
                    {
                        //Выводится обьяснение
                        if (!player_see_explanation_u_dont_have_card_for_throwIn)
                        {
                            player_see_explanation_u_dont_have_card_for_throwIn = true;
                            recomendation.Show("Вам нечего подбросить вы пропускаете ход");
                            RecordInXml("/ExplanationAndRecommendation.xml", "ExplanationAndRecommendation",
                                "Explanation", "explanationThrowIn", "True");
                        }
                        WhoActivePlayer(activePlayer, true);
                        return;
                    }
                    //Выводится рекомендация
                    if (table != null)
                    {
                        if (!player_see_recommendation_u_can_throwIn_any_card)
                        {
                            player_see_recommendation_u_can_throwIn_any_card = true;
                            recomendation.Show("Ваш ход, подбрасывайте карты или пропустите ход");
                            RecordInXml("/ExplanationAndRecommendation.xml", "ExplanationAndRecommendation",
                                "Recommendation", "recommendationThrowIn", "True");
                        }
                        textButtonPass = "Не подбрасывать";
                    }
                    else
                    {
                        if (!player_see_recommendation_u_can_attack_any_card)
                        {
                            player_see_recommendation_u_can_attack_any_card = true;
                            recomendation.Show("Ваш ход, атакуйте любой картой");
                            RecordInXml("/ExplanationAndRecommendation.xml", "ExplanationAndRecommendation",
                                "Recommendation", "recommendationAttack", "True");
                        }
                        textButtonPass = "";
                    }
                }
                players[activePlayer].GetComponent<Player>().YouTurn(true, textButtonPass);
                break;
        }
    }
    public void PlayerIncoming(sbyte numberCard, sbyte player)
    {
        //Убирается рекомендация или обьяснение
        Recomendation recomendation = GameObject.Find("Recomendation").GetComponent<Recomendation>();
        recomendation.Show(false);
        //Время ходить игроку
        if (activePlayer == player)
        {
            //Игрок отбивается
            if (player == fightingOffPlayer)
            {
                if (CanDefendThisCard(numberCard))
                {
                    players[player].GetComponent<Player>().YouTurn(false, "");
                    IncomingPlayerLaysCard(numberCard, player);
                }
            }
            //Игрок ходит
            else
            {
                //Первая картa
                if (table == null)
                {
                    players[player].GetComponent<Player>().YouTurn(false, "");
                    IncomingPlayerLaysCard(numberCard, player);
                }
                //Игрок подкидывает
                else
                {
                    if (CanAddThisCard(numberCard))
                    {
                        players[player].GetComponent<Player>().YouTurn(false, "");
                        IncomingPlayerLaysCard(numberCard, player);
                    }
                    //Дядя нужно както пояснить что игрок даун не ту карту выбрал
                }
            }
        }
    }
    public void IncomingPlayerLaysCard(sbyte numberCard, sbyte player)
    {
        if (activePlayer == player)
        {
            //Если на столе нету карт создаем стол
            if (table == null)
                table = new List<sbyte>() { numberCard };
            else
                //Если на столе есть карты добавляем еще одну
                table.Add(numberCard);

            //Удаляем с колоды игрока
            playersDeck[player].Remove(numberCard);

            //Определение точки куда лететь карте на столе
            bool first;
            if (table.Count - 1 == 0)
            {
                first = true;
            }
            else
            {
                if ((table.Count - 1) % 2 == 0)
                {
                    first = true;
                }
                else
                {
                    first = false;
                }
            }

            //Карта удаляется из руки
            GameObject[] deck;
            if (typePlayers[activePlayer] == 0)
            {
                deck = GameObject.FindGameObjectsWithTag("CardsPlayer");
            }
            else
            {
                deck = GameObject.FindGameObjectsWithTag("CardsBot" + (player - 1));
            }

            for (int i = 0; i < deck.Length; i++)
                if (deck[i].name == numberCard.ToString())
                {
                    Debug.Log("Lays card " + deck[i].name + " numberOfPoint " + numberOfPoint);
                    if (typePlayers[activePlayer] == 1)
                        GetComponent<CreateCard>().SetMesh(deck[i], numberCard);
                    if (nowTake)
                    {
                        Drop(deck[i], (sbyte)pointsIncomingCard[fightingOffPlayer][numberOfPoint], true);
                        numberOfPoint++;
                    }
                    else
                    {
                        Drop(deck[i], (sbyte)pointsIncomingCard[fightingOffPlayer][numberOfPoint], first);
                        if (!first)
                            numberOfPoint++;
                    }
                    return;
                }
        }
    }
    void TakeCards(sbyte playerWhoTake)
    {
        playersDeck[playerWhoTake].AddRange(table);
        table = null;

        GameObject[] tableCards = GameObject.FindGameObjectsWithTag("TableCards");
        var posRotNewCards = players[playerWhoTake].GetComponent<CardPlacing>().PointsForNewCards((byte)tableCards.Length);

        take = true;
        countTakeCards = (sbyte)tableCards.Length;

        for (int i = 0; i < tableCards.Length; i++)
        {
            var pointForCard = Instantiate(new GameObject("Lolpoints")).transform;
            pointForCard.SetPositionAndRotation(posRotNewCards[i].position, posRotNewCards[i].rotation);
            pointForCard.SetParent(players[playerWhoTake].transform);

            tableCards[i].tag = "Untagged";
            if (typePlayers[playerWhoTake] == 1)
                GetComponent<CreateCard>().SetMesh(tableCards[i], 36);

            string tag;
            if (typePlayers[playerWhoTake] == 0)
                //player
                tag = "CardsPlayer";
            else
                //bot
                tag = "CardsBot" + (playerWhoTake - 1);
            tableCards[i].transform.GetComponent<ManipulationOfCards>().GoDrop(pointForCard, tag, fastEnd);
        }
        Debug.Log("Take cards complete wait for card fly to deck");
    }
    void SuccessfulDefense()
    {
        firstDefense = false;
        table = null;
        try
        {
            Transform point = GameObject.FindGameObjectWithTag("PointDiscardPile").transform;
            GameObject[] tableCards = GameObject.FindGameObjectsWithTag("TableCards");
            foreach (GameObject card in tableCards)
            {
                card.tag = "Untagged";
                card.transform.GetComponent<ManipulationOfCards>().GoDrop(point, "Untagged", true, countDiscardCard);
                countDiscardCard++;
            }
            Debug.Log("SuccessfulDefense");
        }
        catch
        {
        }

    }
    void Drop(GameObject card, sbyte point, bool first)
    {
        card.tag = "Untagged";
        if (firstCardInGame)
        {
            firstCardInGame = false;
            var posRotNewCards = players[activePlayer].GetComponent<CardPlacing>().PointsForNewCards(1);
            card.transform.SetPositionAndRotation(posRotNewCards[0].position, posRotNewCards[0].rotation);
        }
        card.transform.GetComponent<ManipulationOfCards>().GoDrop(GetComponent<CreateCard>().GetPoint(point, first), "TableCards", fastEnd);
    }
    public void RestartAfterDropCard()
    {
        if (take)
        {
            countTakeCards--;
            if (countTakeCards == 0)
            {
                take = false;
                countTakeCards = 125;
                DealCards(activePlayer);
            }
        }
        else
        {
            if (cumToPlayerDeck)
            {
                countCumToPlayerDeck--;
                if (countCumToPlayerDeck == 0)
                {
                    cumToPlayerDeck = false;
                    countTakeCards = 0;
                    WhoIncomingWhoFightOff(activePlayer);
                }
            }
            else
            {
                //Все получают новую возможность подбрасывать
                for (int i = 0; i < playersWantThrowInCard.Length; i++)
                    playersWantThrowInCard[i] = true;

                //Конец игры
                if (Wictory())
                {
                    return;
                }

                if (playersDeck[fightingOffPlayer].Count == 0)
                {
                    //Отбой
                    SuccessfulDefense();
                    DealCards(attacker);
                    return;
                }
                else
                    //Следующий ход
                    WhoActivePlayer(activePlayer, false);
            }
        }
    }
    void AddCardInPlayerDeck(sbyte idPlayer, GameObject card, PositionAndRotation positionRotationNewCards)
    {
        playersDeck[idPlayer].Add(deck[index]);

        var pointForCard = Instantiate(new GameObject("Lolpoints")).transform;
        pointForCard.SetPositionAndRotation(positionRotationNewCards.position, positionRotationNewCards.rotation);
        pointForCard.SetParent(players[idPlayer].transform);

        card.tag = "Untagged";

        if (typePlayers[idPlayer] == 0)
        {
            GetComponent<CreateCard>().SetMesh(card, deck[index]);
            card.transform.GetComponent<ManipulationOfCards>().GoDrop(pointForCard, "CardsPlayer", fastEnd);
        }
        else
            card.transform.GetComponent<ManipulationOfCards>().GoDrop(pointForCard, "CardsBot" + (idPlayer - 1), fastEnd);

        if (index == deck.Length)
            Destroy(GameObject.FindGameObjectWithTag("Trump"));

        countCumToPlayerDeck++;
        index++;
        //После индекс++
        deckObj = GameObject.FindGameObjectsWithTag("Deck").ToList();
        deckObj = SortDeck(deck, index, deckObj);
    }
    //----------------------------------------------------------SHIT-------------------------------------------------\\
    bool Wictory()
    {
        if (index == deck.Length)
        {
            byte countPlayerWithoutCard = 0;
            sbyte stupidPlayer = 0;
            for (sbyte i = 0; i < playersDeck.Count; i++)
                if (playersDeck[i].Count == 0)
                    countPlayerWithoutCard++;
                else
                    stupidPlayer = i;

            if (countPlayerWithoutCard == countPlayers - 1)
            {
                //Карты в отбой положить
                GameObject[] cardPlayer = GameObject.FindGameObjectsWithTag("CardsPlayer");
                foreach (GameObject card in cardPlayer)
                    card.tag = "TableCards";

                for (int i = 0; i < countPlayers - 1; i++)
                {
                    cardPlayer = GameObject.FindGameObjectsWithTag("CardsBot" + i);
                    foreach (GameObject card in cardPlayer)
                        card.tag = "TableCards";
                }
                SuccessfulDefense();

                //Салют
                GameObject.Find("Fireworks").GetComponent<ParticleSystem>().Play();

                //Скрыть кнопку быстрого конца
                GameObject.Find("FastEnd").transform.GetChild(0).gameObject.SetActive(false);

                //Текст дурака
                GameObject wictory = GameObject.FindGameObjectWithTag("GameTable").GetComponent<PlaceTable>().GetWictory();
                if (wictory == null)
                    wictory = Instantiate(wictoryText);
                wictory.GetComponent<WictoryText>().SetVisibleAndText(true, stupidPlayer + " игрок, ну ты и дурак");
                this.stupidPlayer = stupidPlayer;

                return true;
            }
        }
        return false;
    }
    public List<byte> PlacePointForPlayer(sbyte countPlayers)
    {
        List<byte> pointsPlayers = new List<byte>();

        switch (countPlayers)
        {
            case 2:
                pointsPlayers.Add(0);
                break;
            case 3:
                pointsPlayers.Add(1);
                pointsPlayers.Add(2);
                break;
            case 4:
                pointsPlayers.Add(1);
                pointsPlayers.Add(2);
                pointsPlayers.Add(0);
                break;
            case 5:
                pointsPlayers.Add(1);
                pointsPlayers.Add(2);
                pointsPlayers.Add(0);
                pointsPlayers.Add(4);
                break;
            case 6:
                pointsPlayers.Add(1);
                pointsPlayers.Add(2);
                pointsPlayers.Add(0);
                pointsPlayers.Add(4);
                pointsPlayers.Add(3);
                break;
        }
        return pointsPlayers;
    }
    //На больше чем 1 колода нужно переделать
    protected sbyte[] ShuffleDeck(byte countDeck)
    {
        int countCards = countDeck * 36;
        sbyte[] deck = new sbyte[countCards];

        for (int i = 0; i < countCards; i++)
        {
            bool correct = true;
            while (correct)
            {
                int card = UnityEngine.Random.Range(1, 37);

                if (!AlreadySuchNumbersInArray(card, i, deck))
                {
                    deck[i] = (sbyte)(card - 1);
                    correct = false;
                }
            }
        }
        return deck;
    }
    protected List<sbyte> ShuffleDeck(byte countDeck, byte deleteThisKEBENYAM)
    {
        int countCards = countDeck * 36;
        List<sbyte> deck = new List<sbyte>();

        for (int i = 0; i < countCards; i++)
        {
            bool correct = true;
            while (correct)
            {
                int card = UnityEngine.Random.Range(0, 36);
                if(deck.IndexOf((sbyte)card) == -1)
                {
                    deck.Add((sbyte)card);
                    correct = false;
                }
            }
        }
        return deck;
    }
    void PlacePointForIncomingCard()
    {
        pointsIncomingCard = new List<List<byte>>();
        for (int i = 0; i < countPlayers; i++)
            pointsIncomingCard.Add(new List<byte>());

        for (byte i = 0; i < 13; i++)
        {
            pointsIncomingCard[0].Add((byte)(12 - i));
            pointsIncomingCard[1].Add(i);
        }

        if (countPlayers > 2)
        {
            pointsIncomingCard[1][0] = 10;
            pointsIncomingCard[1][1] = 8;
            pointsIncomingCard[1][2] = 11;
            pointsIncomingCard[1][3] = 5;
            pointsIncomingCard[1][4] = 9;
            pointsIncomingCard[1][5] = 12;
            pointsIncomingCard[1][6] = 6;
            pointsIncomingCard[1][7] = 7;
            pointsIncomingCard[1][8] = 3;
            pointsIncomingCard[1][9] = 4;
            pointsIncomingCard[1][10] = 0;
            pointsIncomingCard[1][11] = 1;
            pointsIncomingCard[1][12] = 2;
            pointsIncomingCard[2].Add(0);
            pointsIncomingCard[2].Add(3);
            pointsIncomingCard[2].Add(1);
            pointsIncomingCard[2].Add(5);
            pointsIncomingCard[2].Add(4);
            pointsIncomingCard[2].Add(2);
            pointsIncomingCard[2].Add(6);
            pointsIncomingCard[2].Add(7);
            pointsIncomingCard[2].Add(8);
            pointsIncomingCard[2].Add(9);
            pointsIncomingCard[2].Add(10);
            pointsIncomingCard[2].Add(11);
            pointsIncomingCard[2].Add(12);
            if (countPlayers > 3)
            {
                for (byte i = 0; i < 13; i++)
                    pointsIncomingCard[3].Add(i);
                if (countPlayers > 4)
                {
                    pointsIncomingCard[4].Add(2);
                    pointsIncomingCard[4].Add(4);
                    pointsIncomingCard[4].Add(1);
                    pointsIncomingCard[4].Add(7);
                    pointsIncomingCard[4].Add(0);
                    pointsIncomingCard[4].Add(3);
                    pointsIncomingCard[4].Add(6);
                    pointsIncomingCard[4].Add(5);
                    pointsIncomingCard[4].Add(8);
                    pointsIncomingCard[4].Add(9);
                    pointsIncomingCard[4].Add(10);
                    pointsIncomingCard[4].Add(11);
                    pointsIncomingCard[4].Add(12);
                    if (countPlayers > 5)
                    {
                        pointsIncomingCard[5].Add(12);
                        pointsIncomingCard[5].Add(9);
                        pointsIncomingCard[5].Add(11);
                        pointsIncomingCard[5].Add(7);
                        pointsIncomingCard[5].Add(10);
                        pointsIncomingCard[5].Add(8);
                        pointsIncomingCard[5].Add(6);
                        pointsIncomingCard[5].Add(5);
                        pointsIncomingCard[5].Add(4);
                        pointsIncomingCard[5].Add(3);
                        pointsIncomingCard[5].Add(2);
                        pointsIncomingCard[5].Add(1);
                        pointsIncomingCard[5].Add(0);
                    }
                }
            }
        }
    }
    bool AlreadySuchNumbersInArray(int number, int index, sbyte[] arr)
    {
        if (index == 0)
            return false;
        for (int i = index; i >= 0; i--)
        {
            if (arr[i] == number)
                return true;
        }
        return false;
    }
    public bool ComparisonSuit(int firstCard, int secondCard)
    {
        if (firstCard > secondCard)
        {
            while (firstCard > secondCard)
            {
                firstCard -= 4;
                if (firstCard == secondCard)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            if (firstCard == secondCard)
            {
                return true;
            }
            else
            {
                while (secondCard > firstCard)
                {
                    secondCard -= 4;
                    if (firstCard == secondCard)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
    bool EqualRankOfCards(sbyte firstCard, sbyte secondCard)
    {
        double firstRank = firstCard / 4;
        double secondRank = secondCard / 4;
        if (Math.Truncate(firstRank) == Math.Truncate(secondRank))
            return true;
        else
            return false;
    }
    public sbyte HaveWhatsAddOfCards(List<sbyte> playerDeck)
    {
        for (sbyte i = 0; i < playerDeck.Count; i++)
            if (CanAddThisCard(playerDeck[i]))
                return playerDeck[i];
        return -1;
    }
    bool CanAddThisCard(sbyte card)
    {
        foreach (sbyte cardOnTable in table)
            if (EqualRankOfCards(cardOnTable, card))
                return true;

        return false;
    }
    public sbyte HaveWhatsDefendCards(List<sbyte> playerDeck)
    {
        for (sbyte i = 0; i < playerDeck.Count; i++)
            if (CanDefendThisCard(playerDeck[i]))
                return i;
        return -1;
    }
    bool CanDefendThisCard(sbyte numberCard)
    {
        //Если козырь бьет ли карту
        sbyte tableCard = table[table.Count - 1];
        if (ComparisonSuit(numberCard, trump) &&
            (!ComparisonSuit(trump, tableCard) || numberCard > tableCard))
            return true;
        //Если не козырь бьет ли карту
        else
            if (ComparisonSuit(numberCard, tableCard) && numberCard > tableCard)
            return true;
        return false;
    }
    bool AllPlayersHaveSixCard(List<List<sbyte>> playersDeck)
    {
        for (int i = 0; i < countPlayers; i++)
            if (playersDeck[i].Count < 6)
                return false;
        return true;
    }
    public int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }
    int CompareOfNumbersName(GameObject x, GameObject y)
    {
        byte a = Convert.ToByte(x.name);
        byte b = Convert.ToByte(y.name);
        return a.CompareTo(b);
    }
    /*protected GameObject[] SortDeck(sbyte[] deck, sbyte startIndex, GameObject[] deckObjects)
    {
        Array.Sort(deckObjects, CompareOfNumbersName);

        List<byte> deckName = new List<byte>();
        for (int i = 0; i < deckObjects.Length; i++)
            deckName.Add(Convert.ToByte(deckObjects[i].name));

        for (int i = 0; i < deckObjects.Length; i++)
        {
            int index = deckName.IndexOf((byte)deck[i + startIndex]);
            GameObject tmp = deckObjects[index];
            deckObjects[index] = deckObjects[i];
            deckObjects[i] = tmp;

            byte name = deckName[index];
            deckName[index] = deckName[i];
            deckName[i] = name;
        }
        return deckObjects;
    }*/
    protected List<GameObject> SortDeck(sbyte[] deck, sbyte startIndex, List<GameObject> deckObjects)
    {
        deckObjects.Sort(CompareOfNumbersName);

        for (int i = 0; i < deckObjects.Count; i++)
        {
            int index = deckObjects.IndexOf(
                deckObjects.Where(obj => obj.name == deck[i + startIndex].ToString()).SingleOrDefault());

            GameObject tmp = deckObjects[index];
            deckObjects[index] = deckObjects[i];
            deckObjects[i] = tmp;
        }
        return deckObjects;
    }
    bool AllArrayValueEqual(bool[] array, bool value)
    {
        foreach (bool a in array)
            if (a != value)
                return false;
        return true;
    }
    public void GoEnd()
    {
        fastEnd = true;
        GameObject.Find("FastEnd").transform.GetChild(0).gameObject.SetActive(false);
    }
    List<sbyte> HavePlayerWithoutCards(List<List<sbyte>> playersDeck, sbyte[] players)
    {
        List<sbyte> playersWithoutCards = new List<sbyte>();
        foreach (sbyte player in players)
            if (playersDeck[player].Count == 0)
            {
                if (player == 0 && index == deck.Length && !fastEnd)
                {
                    GameObject.Find("FastEnd").transform.GetChild(0).gameObject.SetActive(true);

                    //Выводится рекомендация
                    if (!player_see_recommendation_can_dont_wait_bots_play)
                    {
                        player_see_recommendation_can_dont_wait_bots_play = true;
                        Recomendation recomendation = GameObject.Find("Recomendation").GetComponent<Recomendation>();
                        recomendation.Show("Mожете не ждать пока эти дурачки наиграются");
                        RecordInXml("/ExplanationAndRecommendation.xml", "ExplanationAndRecommendation",
                            "Recommendation", "recommendationWaitBotsPlay", "True");
                    }
                }
                playersWithoutCards.Add(player);
            }

        if (playersWithoutCards.Count == 0)
            return null;
        else
            return playersWithoutCards;
    }
    sbyte[] RemovePlayerFromIncomingArray(sbyte[] incomingPlayers, List<sbyte> whoNeedRemove)
    {
        int newCount = incomingPlayers.Length - whoNeedRemove.Count;
        sbyte[] newIncomingPlayer = new sbyte[newCount];
        bool[] newPlayersWantThrowInCard = new bool[newCount];
        byte index = 0;
        foreach (sbyte player in incomingPlayers)
            if (whoNeedRemove.IndexOf(player) == -1)
            {
                newIncomingPlayer[index] = player;
                newPlayersWantThrowInCard[index] = playersWantThrowInCard[Array.IndexOf(incomingPlayers, player)];
                index++;
            }
        playersWantThrowInCard = newPlayersWantThrowInCard;
        return newIncomingPlayer;
    }
    public void SetExplanationAndRecommendation(bool explanationDefense, bool explanationThrowIn,
        bool recommendationDefense, bool recommendationThrowIn, bool recommendationAttack, bool waitBotsPlay)
    {
        player_see_explanation_u_dont_have_card_for_defense = explanationDefense;
        player_see_explanation_u_dont_have_card_for_throwIn = explanationThrowIn;
        player_see_recommendation_u_can_defense_any_card = recommendationDefense;
        player_see_recommendation_u_can_throwIn_any_card = recommendationThrowIn;
        player_see_recommendation_u_can_attack_any_card = recommendationAttack;
        player_see_recommendation_can_dont_wait_bots_play = waitBotsPlay;
    }

    //RecordInXml("/ExplanationAndRecommendation.xml", "ExplanationAndRecommendation", "Explanation", "explanationDefense", "True");
    private void RecordInXml(string nameFile, string nameRootElement, string nameSubRootElement, string nameChangeElement, string newValue)
    {
        var doc = XDocument.Load(Application.persistentDataPath + nameFile);

        foreach (XElement explanationElement in doc.Element(nameRootElement).
            Elements(nameSubRootElement))
        {
            XElement element = explanationElement.Element(nameChangeElement);

            if (element != null)
                element.Value = newValue;
            else
                explanationElement.Add(new XElement(nameChangeElement, newValue));
        }

        doc.Save(Application.persistentDataPath + nameFile);
    }

    protected byte WhoFirstGo(byte countPlayers, List<List<sbyte>> playersDeck, sbyte trump)
    {
        //Создаем массив для каждого игрока минимальных карт
        int[] mincard = new int[countPlayers];

        //Забиваем числами больше тех что в колоде
        for (int i = 0; i < mincard.Length; i++)
            mincard[i] = 100;

        //Ищем минимальные карты у каждого игрока
        for (int i = 0; i < countPlayers; i++)
            for (int j = 0; j < 6; j++)
                if (ComparisonSuit(playersDeck[i][j], trump))
                    if (playersDeck[i][j] < mincard[i])
                        mincard[i] = playersDeck[i][j];

        //Проверяем у какого игрока карта меньше
        int min = mincard[0];
        byte activePlayer = 0;
        for (byte i = 1; i < mincard.Length; i++)
            if (mincard[i] < min)
            {
                min = mincard[i];
                activePlayer = i;
            }
        return activePlayer;
    }
    //---------------------------------------------------NETWORK-----------------------------------------------------\\
    public GameObject onlinePlayer;
    private Vector3 position;

    public void SyncPlayerPositionRecive(Vector3 positionPlayer, Vector3 forrward, Vector3 up)
    {
        if(GameObject.Find("OnlinePlayer") == null)
        {
            onlinePlayer = Instantiate(onlinePlayer);
            onlinePlayer.name = "OnlinePlayer";
        }

        Transform table = GameObject.FindGameObjectWithTag("GameTable").transform;

        Vector3 position = table.TransformPoint(positionPlayer);

        Vector3 forwardPlayer, upPlayer;
        forwardPlayer = table.TransformDirection(table.InverseTransformDirection(table.forward) - forrward);
        upPlayer = table.TransformDirection(table.InverseTransformDirection(table.up) - up);

        Quaternion rotation = Quaternion.LookRotation(forwardPlayer, upPlayer);

        onlinePlayer.transform.SetPositionAndRotation(position, rotation);
    }
    private void SyncPlayerPositionSend()
    {
        /*if (this.position == null)
        {
            GameObject[] points = GameObject.FindGameObjectsWithTag("Point Player");
            Array.Sort(points, CompareObNames);
            PlacePointForPlayer();
            this.position = points[3].transform.position;
        }*/
        Transform player = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Transform table = GameObject.FindGameObjectWithTag("GameTable").transform;

        Vector3 position = this.position;//player.position;

        //position = table.InverseTransformPoint(position);
        Vector3 forwardPlayer, upPlayer;
        forwardPlayer = table.InverseTransformDirection(table.forward - player.TransformDirection(Vector3.forward));
        upPlayer = table.InverseTransformDirection(table.up - player.TransformDirection(Vector3.up));
        
        GameObject.Find("Netrwork").GetComponent<Client>().SendPosition(position, forwardPlayer, upPlayer);
    }
}
