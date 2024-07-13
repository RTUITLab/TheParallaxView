using System.Collections;
using UnityEngine;

public static class ServerConnection
{
    public static IEnumerator GetSeasons(SeasonsJson result, string requestName = "Get seasons")
    {
        yield return SendRequest<SeasonsJson>(requestName, "Get seasons", result);
    }

    public static IEnumerator SendRequest<T>(string requestName, string message, ICopiable<T> result) where T : class
    {
        //TODO: Write code with request later
        //Code for test

        yield return null;

        if(result is SeasonsJson seasonsResult)
        {
            var data = new SeasonsJson();
            seasonsResult.Copy(data);
        }
        else
        {
            Debug.LogError($"{requestName}: Output type is incorrect");
        }

        yield return null;
    }
}
