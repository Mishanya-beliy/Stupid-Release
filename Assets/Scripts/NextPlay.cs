using GoogleARCore;
using UnityEngine;

public class NextPlay : MonoBehaviour
{
    private GameObject button, buttoReloadMenuPosition;
    void Start()
    {
        gameObject.name = "PlayNext";

        button = gameObject.transform.GetChild(0).gameObject;
        buttoReloadMenuPosition = GameObject.Find("Reload Menu Position");

        SetVisible(false);

        GameObject.Find("Recomendation").GetComponent<Recomendation>().Show("Mожете сыграть еще одну игру с этим колличеством" +
            "игроков или начать новую");
    }

    void Update()
    {
        if (Session.Status == SessionStatus.Tracking)
        {
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
                        if (hitGO == button)
                        {
                            Touch touch = Input.GetTouch(0);
                            GameObject sceneController = GameObject.Find("SceneController");
                            GameObject.Find("Recomendation").GetComponent<Recomendation>().Show(false);

                            sceneController.GetComponent<CardsMonitoring>().DistributionOfCards(0);
                            sceneController.GetComponent<AdMobController>().ShowReward();

                            SetVisible(false);
                            Destroy(GameObject.Find("StartMenu"));
                            Destroy(gameObject);
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
