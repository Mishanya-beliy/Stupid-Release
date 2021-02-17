using System.IO;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
public class Confidential : MonoBehaviour
{
    public GameObject arCoreDevice;
    public GameObject startMenu;

    public GameObject confidentialReadButton;
    public GameObject confidentialBackground;
    public GameObject confidentialButton;
    public GameObject confidentialText;

    private bool google = false, personal = false;
    public void CheckConfidential()
    {
        if (File.Exists(Application.persistentDataPath + "/Confidential.xml"))
        {
            XDocument xmlDocument = XDocument.Load(Application.persistentDataPath + "/Confidential.xml");
            foreach (XElement confidentialElement in xmlDocument.Element("Confidential").Elements("Google"))
            {
                XElement arCoreElement = confidentialElement.Element("ARCore");

                if (arCoreElement != null)
                    if (arCoreElement.Value == "True")
                        google = true;
            }
            foreach (XElement confidentialElement in xmlDocument.Element("Confidential").Elements("Personal"))
            {
                XElement personalElement = confidentialElement.Element("Flycricket");

                if (personalElement != null)
                    if (personalElement.Value == "True")
                        personal = true;
            }
        }

        if (google)
            if (personal)
                StartCamera();
            else
                ShowConfidential(false);
        else
            ShowConfidential(true);
    }
    public void AgreeConfidential()
    {
        if (!google)
        {
            XDocument xmlDocument = new XDocument(new XElement("Confidential",
                                               new XElement("Google",
                                                   new XElement("ARCore", "True"))));
            xmlDocument.Save(Application.persistentDataPath + "/Confidential.xml");
            google = true;
            ShowConfidential(false);
        }
        else
        {
            XDocument xmlDocument;
            if (File.Exists(Application.persistentDataPath + "/Confidential.xml"))
            {
                xmlDocument = XDocument.Load(Application.persistentDataPath + "/Confidential.xml");
                XElement confidentialElement = xmlDocument.Element("Confidential");
                confidentialElement.Add(new XElement("Personal",
                                                       new XElement("Flycricket", "True")));
            }
            else
                xmlDocument = new XDocument(new XElement("Confidential",
                                                  new XElement("Personal",
                                                      new XElement("Flycricket", "True"))));

            xmlDocument.Save(Application.persistentDataPath + "/Confidential.xml");
            StartCamera();
        }
    }
    private void ShowConfidential(bool google)
    {
        confidentialBackground.SetActive(true);
        confidentialReadButton.SetActive(true);
        confidentialButton.SetActive(true);
        confidentialText.SetActive(true);

        if (google)
            confidentialText.transform.GetComponent<TextMeshProUGUI>().text = "Это приложение работает на Сервисах " +
                "Google Play для AR (ARCore), предоставляемых компанией Google LLC и регулируемых Политикой конфиденциальности Google.";
        else
            confidentialText.transform.GetComponent<TextMeshProUGUI>().text = "Это приложение созданно на " +
            "основе данной политики конфиденциальности.";
    }
    void StartCamera()
    {
        Instantiate(arCoreDevice, Vector3.zero, Quaternion.LookRotation(Vector3.forward, Vector3.up));
        Instantiate(startMenu);
        Destroy(confidentialBackground.transform.parent.gameObject);
    }
    public void OpenConfidential()
    {
        if (!google)
            Application.OpenURL("https://policies.google.com/privacy");
        else
            Application.OpenURL("https://durak-ar.flycricket.io/privacy.html");
    }
}
