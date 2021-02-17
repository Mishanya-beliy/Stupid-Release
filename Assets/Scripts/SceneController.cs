using GoogleARCore;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    void Start()
    {
        QuitOnConnectionErrors();
    }
    void Update()
    {
        // The session status must be Tracking in order to access the Frame.
        if (Session.Status != SessionStatus.Tracking)
        {
            int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    void QuitOnConnectionErrors()
    {
        ShowMessage message = new ShowMessage();
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            message.ShowAndroidToastMessage("Camera permission is needed to run this application.");
        else if (Session.Status.IsError())
        {
            // This covers a variety of errors.  See reference for details
            // https://developers.google.com/ar/reference/unity/namespace/GoogleARCore
            message.ShowAndroidToastMessage("ARCore encountered a problem connecting. Please restart the app.");
        }
    }
}
