using TMPro;
using UnityEngine;

public class AddBots : MonoBehaviour
{
    private const int maxCountPlayers = 5;
    private const int minCountPlayers = 2;
    private void Start()
    {
        if(GameObject.Find("SceneController").GetComponent<OnlineGameProcesServer>().HowManyPlayersNowPlay(true)
            == maxCountPlayers)
        {
            GameObject.Find("Recomendation").GetComponent<Recomendation>().Show
                ("Больше ботов добавлять нельзя");
            Destroy(gameObject);
        }
    }
    public void Plus()
    {
        int countPlayers = GameObject.Find("SceneController").GetComponent<OnlineGameProcesServer>().HowManyPlayersNowPlay(true);
        TextMeshProUGUI countBot = gameObject.transform.Find("Count").GetComponent<TextMeshProUGUI>();
        if (int.Parse(countBot.text) + 1 <= maxCountPlayers - countPlayers)
            countBot.text = (int.Parse(countBot.text) + 1).ToString();
        else
            GameObject.Find("Recomendation").GetComponent<Recomendation>().Show
                ("Максимально возможное число ботов");
    }
    public void Minus()
    {
        int countPlayers = GameObject.Find("SceneController").GetComponent<OnlineGameProcesServer>().HowManyPlayersNowPlay(true);
        TextMeshProUGUI countBot = gameObject.transform.Find("Count").GetComponent<TextMeshProUGUI>();
        if (countPlayers + int.Parse(countBot.text) - 1 >= minCountPlayers)
            countBot.text = (int.Parse(countBot.text) - 1).ToString();
        else
            GameObject.Find("Recomendation").GetComponent<Recomendation>().Show
                ("Минимально возможное число ботов");
    }
    public void AddBotsAction()
    {
        GameObject.Find("Netrwork").GetComponent<Client>().SendAddBot(
            int.Parse(gameObject.transform.Find("Count").GetComponent<TextMeshProUGUI>().text));
        Destroy(gameObject);
    }
}
