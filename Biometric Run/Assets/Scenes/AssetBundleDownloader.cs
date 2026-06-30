using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AssetBundleDownloader : MonoBehaviour
{
    private string assetBundleUrl = "gs://unity-sample-11.appspot.com/Android";  // Fake URL for the prank

    void Start()
    {
        StartCoroutine(DownloadAssetBundle());
    }

    IEnumerator DownloadAssetBundle()
    {
        Debug.Log("Initializing asset bundle download...");

        UnityWebRequest request = UnityWebRequest.Get(assetBundleUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError("Error downloading asset bundle: " + request.error);
        }
        else
        {
            Debug.Log("Successfully downloaded asset bundle: " + request.downloadHandler.text);
            // Pretend to load the asset bundle but we actually do nothing with it
            ProcessAssetBundle(request.downloadHandler.text);
        }
    }

    void ProcessAssetBundle(string data)
    {
        Debug.Log("Processing asset bundle... but it's just a dummy process!");
        // Nothing actually happens here
    }
}
