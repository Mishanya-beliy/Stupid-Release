using UnityEngine;

public class Transits : MonoBehaviour
{
    public GameObject gameTable;
    public void FromNetworkUIToGameTable()
    {
        //Hide UI network | start menu | canvas button reload position | recomendation
        GameObject.Find("User Interface").SetActive(false);
        Destroy(GameObject.Find("StartMenu"));
        GameObject.Find("Reload Menu Position").GetComponent<ReplacingMenu>().SetVisibleButton(false);
        GameObject.Find("Recomendation").GetComponent<Recomendation>().Show(false);


        if (GameObject.FindGameObjectWithTag("GameTable") == null)
            Instantiate(gameTable).GetComponent<PlaceTable>().online = true;

        GetComponent<AdMobController>().ShowReward();
    }
}
