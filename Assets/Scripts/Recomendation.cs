using TMPro;
using UnityEngine;

public class Recomendation : MonoBehaviour
{
    private TextMeshProUGUI text;
    private GameObject background;
    private float timer = 0.0f;
    private void Start()
    {
        background = gameObject.transform.GetChild(0).gameObject;
        text = background.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        text.text = "Для более стабильной работы, рекомендуем медленно двигать телефоном," +
            " и захватывать много территории";

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
    private void OnGUI()
    {
        if (timer != 0)
        {
            Vector2 pos = gameObject.GetComponent<RectTransform>().anchoredPosition;
            if (timer < 2.0f)
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y + 175.0f * Time.deltaTime);
            }
            else
            {
                if (pos.y > -250.0f)
                    gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y - 175.0f * Time.deltaTime);
                if (pos.y < -250.0f)
                    gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -250.0f);
            }
        }
    }
    private void Update()
    {
        if (timer != 0)
        {
            timer -= Time.deltaTime;
        }
        
        if (timer < 0)
        {
            timer = 0;

            if(text.text == "Для более стабильной работы, рекомендуем медленно двигать телефоном," +
            " и захватывать много территории")
                Show("Если меню сильно смещается, попробуйте захватить камерой большую область вокруг, и сбросить позицию меню");
            else
            {
                text.gameObject.SetActive(false);
                background.SetActive(false);
            }
        }
    }

    public void Show(bool visible)
    {
        text.gameObject.SetActive(visible);
        background.SetActive(visible);
        if (visible)
        {
            timer = 10.0f;
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100);
        }
        else
            timer = 0;
    }
    public void Show(string text)
    {
        this.text.text = text;
        Show(true);
    }
}
