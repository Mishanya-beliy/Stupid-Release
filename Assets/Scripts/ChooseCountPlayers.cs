using TMPro;
using UnityEngine;

public class ChooseCountPlayers : MonoBehaviour
{
    public GameObject[] buttons;
    public GameObject delete, gameTable;

    public TextMeshPro count;

    private GameObject buttoReloadMenuPosition;
    private void Start()
    {
        gameObject.name = "Choose Count Players";

        buttoReloadMenuPosition = GameObject.Find("Reload Menu Position");
        buttoReloadMenuPosition.GetComponent<ReplacingMenu>().SetVisibleButton(true);

        GameObject.Find("Recomendation").GetComponent<Recomendation>().Show("Выберите количество игроков");
    }
    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var hitGO = hit.transform.gameObject;
                if (hitGO != null)
                {
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        if (hitGO == buttons[i])
                        {
                            Touch touch = Input.GetTouch(0);
                            switch (i)
                            {
                                case 0: //+
                                    int count = int.Parse(this.count.text);
                                    if (count < 5)
                                        this.count.text = (int.Parse(this.count.text) + 1).ToString();
                                    return;
                                case 1: //-
                                    count = int.Parse(this.count.text);
                                    if (count > 2)
                                        this.count.text = (int.Parse(this.count.text) - 1).ToString();
                                    return;
                                case 2: //Start
                                    GameObject.Find("Recomendation").GetComponent<Recomendation>().Show(false);
                                    
                                    GameObject sceneController = GameObject.Find("SceneController");
                                    sceneController.GetComponent<CardsMonitoring>().CountPlayers(sbyte.Parse(this.count.text));

                                    if (GameObject.FindGameObjectWithTag("GameTable") != null)
                                        sceneController.GetComponent<CardsMonitoring>().DistributionOfCards(-1);
                                    else
                                        Instantiate(gameTable);

                                    buttoReloadMenuPosition.GetComponent<ReplacingMenu>().SetVisibleButton(false);
                                    sceneController.GetComponent<AdMobController>().ShowReward();
                                    Destroy(delete);
                                    return;
                            }
                        }
                    }
                }
            }
        }

    }
}
