using System.Collections;
using UnityEngine;

public static class ServerConnection
{
    public static IEnumerator SendRequest<T>(string message, ICopiable<T> result) where T : class
    {
        yield return SendRequest("Default request: ", message, result);
    }

    public static IEnumerator SendRequest<T>(string requestName, string message, ICopiable<T> result) where T : class
    {
        //TODO: Write code with request later
        //Code for test

        yield return null;

        if(result is SeasonsJson seasonsResult)
        {
            var data = new SeasonsJson();
            data.time = message;
            data.weather = "sun";
            data.voice = "none";
            seasonsResult.Copy(data);
        }
        else
        {
            Debug.LogError($"{requestName}: Output type is incorrect");
        }

        yield return null;
    }
}
