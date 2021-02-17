using TMPro;
using UnityEngine;

public class CountPlayers : MonoBehaviour
{
    public void ChangeValue(string text)
    {
        int count = 2;
        if (int.TryParse(text, out count))
        {
            if (count > 5)
                text = "5";
            else if (count < 2)
                text = "2";
            else
                return;
        }
        else
            text = "2";
        GetComponent<TMP_InputField>().text = text;
    }
}
