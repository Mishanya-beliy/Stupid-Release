using UnityEngine;
using GoogleARCore;

public class StartMenu : MonoBehaviour
{
    public GameObject chooseCountPlayers;

    private GameObject buttoReloadMenuPosition;
    private GameObject[] buttonsStartMenu;
    private bool first = true;

    private void Start()
    {
        gameObject.name = "StartMenu";

        buttonsStartMenu = new GameObject[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            buttonsStartMenu[i] = gameObject.transform.GetChild(i).gameObject;
        }
        buttoReloadMenuPosition = GameObject.Find("Reload Menu Position");

        SetVisible(false);

        ShowMessage show = new ShowMessage();
        show.ShowAndroidToastMessage("Двигайте телефоном");
        GameObject.Find("SceneController").GetComponent<AdMobController>().SetAllAds();
    }

    void Update()
    {
        if (Session.Status == SessionStatus.Tracking)
        {
            if (first)
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player>().ReplacingMenu(gameObject);
                first = false;
            }

            SetVisible(true);

            //Обработка нажатий с экрана
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                var ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    var hitGO = hit.transform.gameObject;
                    if (hitGO != null)
                    {
                        for (int i = 0; i < buttonsStartMenu.Length; i++)
                        {
                            if (hitGO == buttonsStartMenu[i])
                            {
                                Touch touch = Input.GetTouch(0);
                                switch (buttonsStartMenu[i].name)
                                {
                                    case "PlaySingleButton": //Play
                                        GameObject.Find("Recomendation").GetComponent<Recomendation>().Show(false);
                                        GameObject next = GameObject.Find("PlayNext");
                                        if(next != null)
                                            Destroy(next);

                                        chooseCountPlayers = Instantiate(chooseCountPlayers);
                                        chooseCountPlayers.transform.SetPositionAndRotation(gameObject.transform.position,
                                            gameObject.transform.rotation);

                                        GameObject.Find("Screen").transform.Find("Netrwork").GetChild(0).gameObject.
                                                SetActive(false);
                                        Destroy(gameObject);
                                        return;
                                    case "PlayOnlineButton": //Online
                                        GameObject.Find("Recomendation").GetComponent<Recomendation>().Show("Скоро будет добавлен");

                                        GameObject.Find("Screen").transform.Find("Netrwork").GetChild(0).GetComponent<NetworkUI>().
                                            ShowPanelListMatchesAndStartFind();
                                        buttonsStartMenu[i].SetActive(false);
                                        return;
                                    case "SettingsButton": //Settings
                                        return;
                                }
                            }
                        }
                    }
                }
            }
        }
        else
            SetVisible(false);
    }    
    void SetVisible(bool visible)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = visible;
        buttoReloadMenuPosition.GetComponent<ReplacingMenu>().SetVisibleButton(visible);
    }
}
