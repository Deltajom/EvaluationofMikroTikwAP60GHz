using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System;

#if WINDOWS_UWP
using Windows.Web.Http;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Networking.Connectivity;
#endif

public class NetworkTest : MonoBehaviour
{
    public int count = 1;
    public bool onheadset = false;
    public string webaddress = "http://192.168.88.2/test.txt";
    string httpResponseBody = "";
    public TextMeshPro mText;
    // Start is called before the first frame update
    void Start()
    {
    }

    public async void TestNetwork()
    {
        Debug.Log("Downloading file....");
        mText.text = "Status: Start Download";
        var watch = new System.Diagnostics.Stopwatch();
        byte[] data;

#if WINDOWS_UWP
        //Create an HTTP client object
        Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
        mText.text = "Status: Client OBJ Made";
        //Add a user-agent header to the GET request. 
        var headers = httpClient.DefaultRequestHeaders;

        //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
        //especially if the header value is coming from user input.
        string header = "ie";
        if (!headers.UserAgent.TryParseAdd(header))
        {
            throw new Exception("Invalid header value: " + header);
        }

        header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        if (!headers.UserAgent.TryParseAdd(header))
        {
            throw new Exception("Invalid header value: " + header);
        }

        Uri requestUri = new Uri(webaddress);

        mText.text = "Status: WroteHeaders";
        mText.text = "Status: SetUri = " + requestUri.ToString();

        //Send the GET request asynchronously and retrieve the response as a string.
        Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
 
        //mText.text = "Status: CreatedResponseSetting";
        try
        {
            watch.Start();
            //Send the GET request
            httpResponse = await httpClient.GetAsync(requestUri);
            httpResponse.EnsureSuccessStatusCode();
            watch.Stop();
            httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("ASYNC RECIEVE ERROR");
        }
        mText.text = "Status: Subs-" + httpResponseBody.Substring(0,12);

#else

        using (var client = new System.Net.WebClient())
        {
            mText.text = "Status: Downloading";
            watch.Start();
            data = client.DownloadData(webaddress);
            watch.Stop();
        }
#endif
        //mText.text = "Status: OnHeadset" + onheadset.ToString();
        Debug.Log("Isheadset - " + onheadset.ToString());

#if WINDOWS_UWP
        var speed = (httpResponseBody.Length * 8) / watch.Elapsed.TotalSeconds; // instead of [Seconds] property  
#else
        var speed = data.LongLength / watch.Elapsed.TotalSeconds; // instead of [Seconds] property
        //mText.text = "Status: speed="+speed.ToString()+"bps";
#endif
        Debug.Log(string.Format("Download duration: {0}", watch.Elapsed.TotalSeconds));
#if WINDOWS_UWP
        Debug.Log(string.Format("File size: {0}", (httpResponseBody.Length * 8)));
#else
        Debug.Log(string.Format("File size: {0}", data.Length.ToString("N0")));
#endif

        Debug.Log(string.Format("Speed: {0} bps ", speed.ToString("N0")));
        //DebugText.text = "WriteTruePos";
        string downloadpath = string.Format("{0}/{1}.txt", Application.persistentDataPath, "DownloadTest" + count);
        string downloadtext = "DownloadTest";
        downloadtext += string.Format("Download duration: {0}", watch.Elapsed.TotalSeconds);
#if WINDOWS_UWP
        downloadtext += string.Format("File size: {0}", (httpResponseBody.Length * 8));
#else
        downloadtext += string.Format("File size: {0}", data.Length.ToString("N0"));
#endif
        downloadtext += string.Format("Speed: {0} bps ", speed.ToString("N0"));
        byte[] downloaddata = Encoding.ASCII.GetBytes(downloadtext);
        UnityEngine.Windows.File.WriteAllBytes(downloadpath, downloaddata);
        count += 1;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
