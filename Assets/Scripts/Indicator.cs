
// Получить доступ к классам UI
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    // Отслеживаемый объект.
    public Transform target;
    // Расстояние от 'target' до данного объекта.
    public Transform showDistanceTo;
    // Надпись для отображения расстояния.
    public Text distanceLabel;
    // Расстояние от края экрана.
 public int margin = 50;
    // Цвет оттенка изображения.
    public Color color
    {
        set
        {
            GetComponent<Image>().color = value;
        }
        get
        {
            return GetComponent<Image>().color;
        }
    }
    // Выполняет настройку индикатора
    void Start()
    {
        // Скрыть надпись; она будет сделана видимой
        // в методе Update, если цель target будет назначена
        distanceLabel.enabled = false;
        // На запуске дождаться ближайшего кадра перед отображением
        // для предотвращения визуальных артефактов
        GetComponent<Image>().enabled = false;
    }
    // Обновляет положение индикатора в каждом кадре
    void Update()
    {

        // Цель исчезла? Если да, значит, индикатор тоже надо убрать
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        // Если цель присутствует, вычислить расстояние до нее
        // и показать в distanceLabel
        if (showDistanceTo != null)
        {
            // Показать надпись
            distanceLabel.enabled = true;
            // Вычислить расстояние
            var distance = (int)Vector3.Magnitude(
            showDistanceTo.position - target.position);
            // Показать расстояние в надписи
            distanceLabel.text = distance.ToString() + "m";
        }
        else
        {
            // Скрыть надпись
            distanceLabel.enabled = false;
        }
        GetComponent<Image>().enabled = true;
        //Определить экранные координаты объекта
        var viewportPoint = Camera.main.WorldToViewportPoint(target.position);
        // Объект позади нас?
        if (viewportPoint.z < 0)
        {
            // Сместить к границе экрана
            viewportPoint.z = 0;
            viewportPoint = viewportPoint.normalized;
            viewportPoint.x *= -Mathf.Infinity;
        }
        // Определить видимые координаты для индикатора
        var screenPoint =
        Camera.main.ViewportToScreenPoint(viewportPoint);
        // Ограничить краями экрана
        screenPoint.x = Mathf.Clamp(
        screenPoint.x,
        margin,
        Screen.width - margin * 2);
        screenPoint.y = Mathf.Clamp(
        screenPoint.y,
        margin,
        Screen.height - margin * 2);
        // Определить, где в области холста находится видимая координата
        var localPosition = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        transform.parent.GetComponent<RectTransform>(),
        screenPoint,
        Camera.main,
        out localPosition);
        // Обновить позицию индикатора
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = localPosition;
    }
}