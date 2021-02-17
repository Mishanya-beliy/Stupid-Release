using UnityEngine;

public class ManipulationOfCards : MonoBehaviour
{
    private Transform point;
    private bool isGo = false;
    private new string tag;
    private float speed;
    private float speedRoate;
    private float high;
    private float x;
    private byte numberOfCard;
    private bool discardPile = false;
    public void GoDrop(Transform point, string tag, bool fast)
    {
        this.point = point;
        this.tag = tag;

        if (fast)
        {
            speed = Vector3.Distance(transform.position, point.position) * 5;
            speedRoate = Quaternion.Angle(transform.rotation, point.rotation) * 5;
        }
        else
        {
            speed = Vector3.Distance(transform.position, point.position);
            speedRoate = Quaternion.Angle(transform.rotation, point.rotation);
        }

        isGo = true;
    }
    public void GoDrop(Transform point, string tag, bool discard, byte numberOfCard)
    {
        this.point = point;
        this.tag = tag;
        this.numberOfCard = numberOfCard;

        Vector3 from = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 to = new Vector3(point.position.x, 0f, point.position.z);
        speed = Vector3.Distance(from, to);
        speedRoate = Quaternion.Angle(transform.rotation, point.rotation);

        high = transform.position.y + 0.2f;
        x = -1f;


        discardPile = discard;

        isGo = true;
    }
    private void Update()
    {
        if (isGo)
        {
            if (discardPile)
            {
                Vector3 from = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 to = new Vector3(point.position.x, 0f, point.position.z);
                Vector3 newPosition = Vector3.MoveTowards(from, to, speed * Time.deltaTime);
                newPosition = new Vector3(newPosition.x, Mathf.Pow(x, 2) * -0.1f + high, newPosition.z);

                transform.position = newPosition;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, point.rotation, speedRoate * Time.deltaTime);
                x += 2 * Time.deltaTime;

                if (transform.position.x == point.position.x && transform.position.z == transform.position.z)
                {
                    transform.rotation = point.rotation;
                    transform.position = new Vector3(point.position.x, point.position.y + 0.0003f * numberOfCard, point.position.z);
                    transform.SetParent(point);
                    isGo = false;
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, point.position, speed * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, point.rotation, speedRoate * Time.deltaTime);

                if (transform.position == point.position)
                {
                    transform.rotation = point.rotation;
                    transform.SetParent(point);

                    gameObject.tag = tag;
                    isGo = false;
                    GameObject.Find("SceneController").GetComponent<CardsMonitoring>().RestartAfterDropCard();
                }
            }
        }
    }
}
