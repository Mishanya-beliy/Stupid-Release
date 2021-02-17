using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public sbyte BotIncoming(List<sbyte> deck, bool type)
    {
        //Если первая карта
        if (type)
        {
            //Если первая карта
            //Поиск минимальной карты
            sbyte min = deck[0];

            for (sbyte i = 1; i < deck.Count; i++)
                if (deck[i] < min)
                {
                    min = deck[i];
                }
            
            //Бот кладет карту
            return min;

        }
        else
        {
            //Если на столе уже есть карты
            sbyte addedCard = GameObject.Find("SceneController").GetComponent<CardsMonitoring>().HaveWhatsAddOfCards(deck);

            //Есть что поросить выдает номер карты если нету возвращает -1            
            return addedCard;
        }
    }
    public sbyte BotFightingOff(List<sbyte> deck, sbyte numberCard, sbyte trump)//number а не numberCard
    {
        sbyte index = -1;
        //Перебирвем карты
        GameObject sceneController = GameObject.Find("SceneController");
        for (sbyte i = 0; i < deck.Count; i++)
        {
            //Если козырь бьет ли карту
            if (sceneController.GetComponent<CardsMonitoring>().ComparisonSuit(deck[i], trump) &&
                (!sceneController.GetComponent<CardsMonitoring>().ComparisonSuit(trump, numberCard) || deck[i] > numberCard))
            {
                index = i;
                break;
            }
            else
            {
                //Если не козырь бьет ли карту
                if (sceneController.GetComponent<CardsMonitoring>().ComparisonSuit(deck[i], numberCard) && deck[i] > numberCard)
                {
                    index = i;
                    break;
                }
            }
        }

        //Нету чем отбится 
        if (index == -1)
            return -1;
        else
        {
            //Есть чем ложит карту
            return deck[index];
        }
    }
}
