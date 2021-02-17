using TMPro;
using UnityEngine;

public class WictoryText : MonoBehaviour
{
    public GameObject startMenu, nextPlay;
    private bool visible;
    private TextMeshPro text;
    private float timer;
    private void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        text = gameObject.transform.GetChild(0).GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                //Создание главного меню 
                Transform table = GameObject.FindGameObjectWithTag("GameTable").transform;
                Vector3 position = new Vector3(table.position.x, table.position.y + 0.2f, table.position.z);
                Quaternion rotation = Quaternion.LookRotation(table.transform.TransformDirection(Vector3.forward), Vector3.up);
                Instantiate(startMenu, position, rotation);
                Instantiate(nextPlay, position, rotation);

                foreach (Renderer r in GetComponentsInChildren<Renderer>())
                {
                    r.enabled = true;
                }
                timer = 0;
            }
        }

        if(visible)
            gameObject.transform.LookAt(GameObject.FindGameObjectWithTag("MainCamera").transform);
    }
    public void SetVisibleAndText(bool state, string text)
    {
        visible = state;
        this.text.text = text;
        if (state)
            timer = 5.68f;
        else
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
    }
}
