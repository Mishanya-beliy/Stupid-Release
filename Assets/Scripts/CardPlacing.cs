using UnityEngine;

public class CardPlacing : MonoBehaviour
{
    public float diapasonRazpolozeniyaKart = 30f;
    public string nameCards = "CardsPlayer";
    public float distance = 0.2f;

    void Update()
    {
        GameObject[] cardsPlayer = GameObject.FindGameObjectsWithTag(nameCards);

        if (cardsPlayer != null)
        {
            for (int i = 0; i < cardsPlayer.Length; i++)
            {
                PositionAndRotation card = PlacingCard(i, (sbyte)cardsPlayer.Length);
                cardsPlayer[i].transform.SetPositionAndRotation(card.position, card.rotation);
            }
        }
    }

    PositionAndRotation PlacingCard(int numberCard, sbyte countCard)
    {
        float gradus;
        if (countCard == 1)
            gradus = 0;
        else
        {
            gradus = diapasonRazpolozeniyaKart / --countCard;
            gradus = (gradus * numberCard) - (diapasonRazpolozeniyaKart / 2);
        }


        Vector3 forrwardWithAngle = transform.TransformDirection(Quaternion.AngleAxis(gradus, Vector3.up) * Vector3.forward);
        Vector3 upWithAngle = transform.TransformDirection(Quaternion.AngleAxis(gradus, Vector3.back) * Vector3.up);
        Ray ray = new Ray(transform.TransformPoint(0.01f, -0.1f, 0), forrwardWithAngle);

        float a = distance - gradus * 0.0009f;
        return new PositionAndRotation(ray.GetPoint(a), Quaternion.LookRotation(forrwardWithAngle, upWithAngle));
    }

    public PositionAndRotation[] PointsForNewCards(byte countAddCard)
    {
        GameObject[] cardsPlayer = GameObject.FindGameObjectsWithTag(nameCards);
        sbyte count;
        if (cardsPlayer != null)
            count = (sbyte)cardsPlayer.Length;
        else
            count = 0;

        PositionAndRotation[] newCard = new PositionAndRotation[countAddCard];
        for (int i = 0; i < countAddCard; i++)
            newCard[i] = PlacingCard(count + i, (sbyte)(count + countAddCard));

        return newCard;
    }
}
public class PositionAndRotation
{
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; }
    public PositionAndRotation(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
