using System;
using System.Collections;
using UnityEngine;

public class SeasonsController : MonoBehaviour
{
    [SerializeField] private float _updateTime;

    public static event Action<SeasonsJson> SeasonsUpdated;

    private Coroutine _startCor;

    private void Start()
    {
        _startCor = StartCoroutine(MainLoop());
    }

    private IEnumerator MainLoop()
    {
        while(true)
        {
            SeasonsJson seasons = new();
            yield return ServerConnection.GetSeasons(seasons);
            SeasonsUpdated?.Invoke(seasons);
            yield return new WaitForSeconds(_updateTime);
        }
    }

    private void OnDestroy()
    {
        if(_startCor != null)
        {
            StopCoroutine(_startCor);
        }
    }
}
