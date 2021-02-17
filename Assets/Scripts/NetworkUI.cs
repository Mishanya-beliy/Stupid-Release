using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{
    public GameObject listMatches, createMatch;

    public GameObject list, messageSearch;
    public GameObject nameServer, maxCount, isPrivate, passwordToggle, password, messageCreate;

    private int countServersList = 0;
    public void CloseUI()
    {
        try
        {
            Transform menu = GameObject.Find("StartMenu").transform;
            for (int i = 0; i < menu.childCount; i++)
            {
                GameObject buttonInMenu = menu.GetChild(i).gameObject;
                if (buttonInMenu.name == "PlayOnlineButton")
                {
                    buttonInMenu.SetActive(true);
                    break;
                }
            }
        }
        finally
        {
            gameObject.SetActive(false);
        }        
    }

    public void ShowPanelListMatchesAndStartFind()
    {
        RectTransform search = gameObject.transform.Find("Search").GetComponent<RectTransform>();
        RectTransform create = gameObject.transform.Find("Create").GetComponent<RectTransform>();

        search.sizeDelta = new Vector2(400, 100);
        create.sizeDelta = new Vector2(400, 90);

        search.anchoredPosition = new Vector2(210, -55);
        create.anchoredPosition = new Vector2(615, -60);

        messageCreate.SetActive(false);
        createMatch.SetActive(false);
        listMatches.SetActive(true);

        transform.parent.GetComponent<NetworkMultiplayer>().FindInternetMatch("", false);
    }

    public void ShowListMatches(int count, InformationOfMatches[] matches)
    {
        messageSearch.SetActive(false);
        Transform parent = gameObject.transform.Find("List Matches");

        DeleteBeforeListMatches(parent);
        countServersList = count;

        //Отображение серверов
        for (int i = 0; i < count; i++)
        {
            GameObject list = Instantiate(this.list, parent);
            list.name = i.ToString();

            float y = list.GetComponent<RectTransform>().anchoredPosition.y - i * 105;
            list.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, y);

            list.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = matches[i].name;
            list.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                matches[i].currentSize.ToString() + " / " + matches[i].maxSize.ToString();
        }
    }
    public void NoMatches()
    {
        DeleteBeforeListMatches(gameObject.transform.Find("List Matches"));
        messageSearch.SetActive(true);
        messageSearch.GetComponent<TextMeshProUGUI>().text = "Не найдено серверов😆\nМожешь создать свой";
    }
    private void DeleteBeforeListMatches(Transform parent)
    {
        for (int i = 0; i < countServersList; i++)
            Destroy(parent.Find(i.ToString()).gameObject);

        countServersList = 0;
    }

    public void ShowPanelCreateMatch()
    {
        RectTransform search = gameObject.transform.Find("Search").GetComponent<RectTransform>();
        RectTransform create = gameObject.transform.Find("Create").GetComponent<RectTransform>();

        search.sizeDelta = new Vector2(400, 90);
        create.sizeDelta = new Vector2(400, 100);

        search.anchoredPosition = new Vector2(210, -60);
        create.anchoredPosition = new Vector2(615, -55);

        messageSearch.SetActive(false);
        listMatches.SetActive(false);
        createMatch.SetActive(true);
    }
    public void IsPrivate(bool enable)
    {
        passwordToggle.SetActive(enable);
        if (!enable)
            password.SetActive(enable);
        else if (passwordToggle.GetComponent<Toggle>().isOn)
            password.SetActive(enable);
    }
    public void WithPassword(bool enable)
    {
        password.SetActive(enable);
    }
    public void CreateMatch()
    {
        string name = nameServer.GetComponent<TMP_InputField>().text;
        if(name == "")
        {
            messageCreate.SetActive(true);
            messageCreate.GetComponent<TextMeshProUGUI>().text = "Введите название сервера";
            return;
        }
        else
        {
            int max = -100;
            int.TryParse(maxCount.GetComponent<TMP_InputField>().text, out max);
            if (max == -100)
            {
                messageCreate.SetActive(true);
                messageCreate.GetComponent<TextMeshProUGUI>().text = "Введите максимальное число игроков от 2 до 5";
                return;
            }
            else if (max > 5)
            {
                messageCreate.SetActive(true);
                messageCreate.GetComponent<TextMeshProUGUI>().text = "Введите максимальное число игроков меньше 5";
                return;
            }
            else if (max < 2)
            {
                messageCreate.SetActive(true);
                messageCreate.GetComponent<TextMeshProUGUI>().text = "Введите максимальное число игроков больше 2";
                return;
            }

            bool isPublic = true;//!isPrivate.GetComponent<Toggle>().isOn;

            bool withPassword = false;
            if (isPrivate)
                withPassword = passwordToggle.GetComponent<Toggle>().isOn;

            string password = "";
            if (withPassword)
                password = this.password.GetComponent<TMP_InputField>().text;
            if (password == "")
                passwordToggle.GetComponent<Toggle>().isOn = false;

            messageCreate.SetActive(false);

            transform.parent.GetComponent<NetworkMultiplayer>().CreateInternetMatch(name, uint.Parse(max.ToString()), isPublic, password);
        }        
    }
    public void SuccessfulCreateServer()
    {
        messageCreate.SetActive(true);
        messageCreate.GetComponent<TextMeshProUGUI>().text = "Сервер успешно создан";
    }
    public void FailedCreateServer()
    {
        messageCreate.SetActive(true);
        messageCreate.GetComponent<TextMeshProUGUI>().text = "Сервер создать не удалось";
    }
    public void ReciveMessage(string message)
    {
        messageSearch.GetComponent<TextMeshProUGUI>().text = message;
    }

}

public class InformationOfMatches
{
    public string name { get; set; }
    public int maxSize { get; set; }
    public int currentSize { get; set; }

    public InformationOfMatches(string name, int maxSize, int currentSize)
    {
        this.name = name;
        this.maxSize = maxSize;
        this.currentSize = currentSize;
    }
}
