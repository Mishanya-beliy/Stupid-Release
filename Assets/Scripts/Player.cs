using GoogleARCore;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject pass;
    private bool turn = false;
    private sbyte id = -1;
    private sbyte networkid = -10;
    private GameObject[] cards;
    private bool firstFrame = true;

    public void SetID(sbyte id)
    {
        this.id = id;
    }
    public sbyte GetID()
    {
        return id;
    }
    public void SetNetworkID(sbyte id)
    {
        networkid = id;
    }
    public sbyte GetNetworkID()
    {
        return networkid;
    }
    public void YouTurn(bool turn, string text)
    {
        if (pass == null)
            pass = GameObject.Find("Pass").transform.GetChild(0).gameObject;

        if (text == "")
            pass.SetActive(false);
        else
        {
            pass.SetActive(turn);
            pass.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        }
        this.turn = turn;
        
        if (turn)
        {
            cards = GameObject.FindGameObjectsWithTag("CardsPlayer");
            for (int i = 0; i < cards.Length; i++)
                cards[i] = cards[i].transform.GetChild(0).gameObject;
        }
    }

    private void Update()
    {
        if (turn)
            if (Session.Status == SessionStatus.Tracking)
            {
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    Touch touch = Input.GetTouch(0);
                    var ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        var hitGO = hit.transform.gameObject;
                        if (hitGO != null)
                        {
                            for (sbyte i = 0; i < cards.Length; i++)
                            {
                                if (hitGO == cards[i])
                                {
                                    GameObject.Find("SceneController").GetComponent<CardsMonitoring>().PlayerIncoming(sbyte.Parse(cards[i].transform.parent.name), id);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        Transform device = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (device)
            gameObject.transform.SetPositionAndRotation(device.position, device.rotation);
    }
    public void ReplacingMenu(GameObject menu)
    {
        if (firstFrame)
        {
            firstFrame = false;
            menu.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
            GameObject.Find("Recomendation").GetComponent<Recomendation>().Show("Для более стабильной работы, " +
                "рекомендуем медленно двигать телефоном, и захватывать много территории");
        }
    }
}

