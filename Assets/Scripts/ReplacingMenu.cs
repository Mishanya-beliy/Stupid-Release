using UnityEngine;

public class ReplacingMenu : MonoBehaviour
{
    public void SetVisibleButton(bool visible)
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(visible);
    }
    public void ReplacingAllMenu()//Replace all what is active on scene
    {
        //Set position and rotation of device
        Transform device = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Quaternion rotation = device.rotation;
        Vector3 position = device.position;

        //Find all menu game objects
        GameObject play = GameObject.Find("StartMenu");
        GameObject playAgain = GameObject.Find("PlayNext");
        GameObject countPlayers = GameObject.Find("Choose Count Players");

        //If eweryone is active set device position and rotation
        if (play != null)
            play.transform.SetPositionAndRotation(position, rotation);
        if (playAgain != null)
            playAgain.transform.SetPositionAndRotation(position, rotation);
        if (countPlayers != null)
            countPlayers.transform.SetPositionAndRotation(position, rotation);
    }
}
