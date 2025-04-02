using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class ServerConnection
{
    public static IEnumerator SendRequest<T>(string message, ICopiable<T> result) where T : class
    {
        yield return SendRequest("Default request: ", message, result);
    }

    public static IEnumerator SendRequest<T>(string requestName, string message, ICopiable<T> result) where T : class
    {
        UnityWebRequest request = new("http://localhost:9898" + message, "GET");
        var bodyRaw = Encoding.UTF8.GetBytes("");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Set the certificate handler
        request.certificateHandler = new AcceptAllCertificates();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonAnswer = request.downloadHandler.text;
            
            try
            {
                var objAnswer = JsonConvert.DeserializeObject<T>(jsonAnswer);
                result.Copy(objAnswer);
            }
            catch(Exception e)
            {
                Debug.LogError($"Answer: {jsonAnswer}\n Error: {e.Message}");
            }
        }
        else
        {
            Debug.LogError($"{requestName} Request failed with error: {request.error}");
        }
    }
}
