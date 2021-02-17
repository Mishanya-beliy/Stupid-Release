using UnityEngine;

public class TrackableList : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 positionList = gameObject.GetComponent<RectTransform>().position;
            float width = gameObject.GetComponent<RectTransform>().rect.width;
            float height = gameObject.GetComponent<RectTransform>().rect.height;
            if (touch.position.x > positionList.x - width / 2 &&
                touch.position.x < positionList.x + width / 2)
                if(touch.position.y > positionList.y - height / 2 &&
                   touch.position.y < positionList.y + height / 2)
                {
                    gameObject.transform.parent.parent.parent.GetComponent<NetworkMultiplayer>().
                        JoinToMatch(int.Parse(gameObject.name));
                }
        }
    }
}
