using UnityEngine;
using UnityEngine.Networking;

public class OnlinePlayer : NetworkBehaviour
{
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        //Transform device = GameObject.FindGameObjectWithTag("MainCamera").transform;
        //gameObject.transform.SetPositionAndRotation(device.position, device.rotation);
    }
    public override void OnStartLocalPlayer()
    {
        //Выполняется у каждого игрока локально
    }
}
