using System;
using UnityEngine;

public class CreateCard : MonoBehaviour
{
    public GameObject card;
    public Material[] cardsFace;
    private Transform[] pointsCard;
    private Transform[] pointsCardUp;
    private float high;

    public Transform GetPoint(sbyte number, bool downCard)
    {
        if (downCard)
        {
            if (pointsCard == null)
                pointsCard = InitializedPoints("Point Table Card");
            return pointsCard[number];
        }
        else
        {
            if (pointsCardUp == null)
                pointsCardUp = InitializedPoints("Point Table Card Up");
            return pointsCardUp[number];
        }

    }
    public void Reset()
    {
        high = 0;
    }
    public void SetMesh(GameObject card, sbyte numberCard)
    {
        card.transform.GetChild(0).GetComponent<MeshRenderer>().material = cardsFace[numberCard];
    }
    //Player
    public void Create(sbyte numberCard)
    {
        GameObject newCard = Instantiate(card);

        newCard.transform.GetChild(0).GetComponent<MeshRenderer>().material = cardsFace[numberCard];
        newCard.tag = "CardsPlayer";
        newCard.name = numberCard.ToString();
    }
    //Bot
    public void Create(sbyte idBot, sbyte numberCard)
    {
        GameObject newCard = Instantiate(card);
        newCard.tag = "CardsBot" + idBot;
        newCard.transform.SetParent(InitializedPoints("Point Player",idBot));
        newCard.name = numberCard.ToString();
    }
    //BotOnline
    public void Create(sbyte idBot, sbyte numberCard, sbyte idPlayer)
    {
        GameObject newCard = Instantiate(card);
        newCard.tag = "CardsBot" + idBot;
        newCard.transform.SetParent(InitializedPoints("Point Player", idPlayer));
        newCard.name = numberCard.ToString();
    }
    //OnlinePlayer
    public void Create(sbyte idPlayer, sbyte numberCard, Transform player)
    {
        GameObject newCard = Instantiate(card);
        newCard.tag = "CardsPlayer" + idPlayer;
        newCard.transform.SetParent(player);
        newCard.name = numberCard.ToString();
    }
    //Trump
    public void Create(int numberCard)
    {
        GameObject newCard = Instantiate(card);

        newCard.transform.GetChild(0).GetComponent<MeshRenderer>().material = cardsFace[numberCard];
        newCard.tag = "Trump";
        
        Transform pointTrump = GameObject.FindGameObjectWithTag("Point Trump").transform;

        newCard.transform.SetPositionAndRotation(pointTrump.position, pointTrump.rotation);
        newCard.transform.SetParent(pointTrump);
        newCard.name = numberCard.ToString();
    }
    //Deck
    public void CreateDeck(sbyte numberCard)
    {
        GameObject newCard = Instantiate(card);
        newCard.tag = "Deck";
        newCard.name = numberCard.ToString();

        Transform pointDeck = GameObject.FindGameObjectWithTag("PointDeck").transform;

        newCard.transform.SetPositionAndRotation(new Vector3(pointDeck.position.x, pointDeck.position.y + high,
            pointDeck.position.z), pointDeck.rotation);
        high += 0.0003f;
        newCard.transform.SetParent(pointDeck);
    }

    
    private Transform[] InitializedPoints(string tag)
    {
        GameObject[] points = GameObject.FindGameObjectsWithTag(tag);
        Array.Sort(points, CompareObNames);
        Transform[] pointsCard = new Transform[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            pointsCard[i] = points[i].transform;
        }
        return pointsCard;
    }
    private Transform InitializedPoints(string tag, sbyte number)
    {
        Transform[] arr = InitializedPoints(tag);
        return arr[number];
    }
    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }
}
