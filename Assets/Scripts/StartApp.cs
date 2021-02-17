using GoogleARCore;
using GoogleMobileAds.Api;
using System.Collections;
using System.IO;
using System.Xml.Linq;
using TMPro;
using UnityEngine;

public class StartApp : MonoBehaviour
{
    public TextMeshProUGUI testHardwareMessage;
    public GameObject confidential, start;
    void Start()
    {        
        StartCoroutine(CheckCompatibility());
        MobileAds.Initialize(initStatus => { });
    }

    private IEnumerator CheckCompatibility()
    {
        AsyncTask<ApkAvailabilityStatus> checkTask = Session.CheckApkAvailability();
        CustomYieldInstruction customYield = checkTask.WaitForCompletion();
        yield return customYield;
        ApkAvailabilityStatus result = checkTask.Result;
        testHardwareMessage.gameObject.SetActive(true);
        switch (result)
        {
            case ApkAvailabilityStatus.SupportedApkTooOld:
                testHardwareMessage.text = "Supported apk too old";
                break;
            case ApkAvailabilityStatus.SupportedInstalled:
                Destroy(testHardwareMessage.gameObject);
                AllGoodStartApp();
                break;
            case ApkAvailabilityStatus.SupportedNotInstalled:
                StartCoroutine(CheckInstall());
                testHardwareMessage.text = "Supported, not installed, requesting installation";
                break;
            case ApkAvailabilityStatus.UnknownChecking:
                testHardwareMessage.text = ("Unknown Checking");
                break;
            case ApkAvailabilityStatus.UnknownError:
                testHardwareMessage.text = ("Unknown Error");
                break;
            case ApkAvailabilityStatus.UnknownTimedOut:
                testHardwareMessage.text = ("Unknown Timed out");
                break;
            case ApkAvailabilityStatus.UnsupportedDeviceNotCapable:
                testHardwareMessage.text = ("Unsupported Device Not Capable");
                break;
        }
    }

    private IEnumerator CheckInstall()
    {
        AsyncTask<ApkInstallationStatus> checkTask = Session.RequestApkInstallation(false);
        CustomYieldInstruction customYield = checkTask.WaitForCompletion();
        yield return customYield;
        ApkInstallationStatus result = checkTask.Result;
        switch (result)
        {
            case ApkInstallationStatus.Success:
                AllGoodStartApp();
                break;
        }
    }

    private void AllGoodStartApp()
    {
        CheckAndSetExplanationAndRecommendation();

        confidential.GetComponent<Confidential>().CheckConfidential();

        Destroy(start);
        Destroy(testHardwareMessage.gameObject);
    }

    void CheckAndSetExplanationAndRecommendation()
    {

        bool explanationDefense = false, explanationThrowIn = false,
            recommendationDefense = false, recommendationThrowIn = false, recommendationAttack = false,
            recommendationWaitBotsPlay = false;

        if (File.Exists(Application.persistentDataPath + "/ExplanationAndRecommendation.xml"))
        {
            XDocument xmlDocument = XDocument.Load(Application.persistentDataPath + "/ExplanationAndRecommendation.xml");
            foreach (XElement explanationElement in xmlDocument.Element("ExplanationAndRecommendation").
                Elements("Explanation"))
            {
                XElement explanationDefenseElement = explanationElement.Element("explanationDefense");
                XElement explanationThrowInElement = explanationElement.Element("explanationThrowIn");

                if (explanationDefenseElement != null)
                    if (explanationDefenseElement.Value == "True")
                        explanationDefense = true;

                if (explanationThrowInElement != null)
                    if (explanationThrowInElement.Value == "True")
                        explanationThrowIn = true;
            }
            foreach (XElement explanationElement in xmlDocument.Element("ExplanationAndRecommendation").
                Elements("Recommendation"))
            {
                XElement recommendationDefenseElement = explanationElement.Element("recommendationDefense");
                XElement recommendationThrowInElement = explanationElement.Element("recommendationThrowIn");
                XElement recommendationAttackElement = explanationElement.Element("recommendationAttack");
                XElement waitBotsPlayElement = explanationElement.Element("recommendationWaitBotsPlay");

                if (recommendationDefenseElement != null)
                    if (recommendationDefenseElement.Value == "True")
                        recommendationDefense = true;

                if (recommendationThrowInElement != null)
                    if (recommendationThrowInElement.Value == "True")
                        recommendationThrowIn = true;

                if (recommendationAttackElement != null)
                    if (recommendationAttackElement.Value == "True")
                        recommendationAttack = true;

                if (waitBotsPlayElement != null)
                    if (waitBotsPlayElement.Value == "True")
                        recommendationWaitBotsPlay = true;
            }
        }
        else
        {
            XDocument xmlDocument = new XDocument(new XElement("ExplanationAndRecommendation",
                                               new XElement("Explanation",
                                                   new XElement("explanationDefense", "False"),
                                                   new XElement("explanationThrowIn", "False")),
                                               new XElement("Recommendation",
                                                   new XElement("recommendationDefense", "False"),
                                                   new XElement("recommendationThrowIn", "False"),
                                                   new XElement("recommendationAttack", "False"),
                                                   new XElement("recommendationWaitBotsPlay", "False"))));
            xmlDocument.Save(Application.persistentDataPath + "/ExplanationAndRecommendation.xml");
        }
        GameObject.Find("SceneController").GetComponent<CardsMonitoring>().SetExplanationAndRecommendation(
            explanationDefense, explanationThrowIn, recommendationDefense, recommendationThrowIn,
            recommendationAttack, recommendationWaitBotsPlay);
    }
}
