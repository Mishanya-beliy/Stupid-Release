using UnityEngine;

public class Pass : MonoBehaviour
{    public void PlayersPass()
    {
        GameObject.Find("First Person Camera").GetComponent<Player>().YouTurn(false, "");
        GameObject.Find("SceneController").GetComponent<CardsMonitoring>().WhoActivePlayer(0, true);
    }
}
