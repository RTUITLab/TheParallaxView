using UnityEngine;

public abstract class BaseSeasonElementController : MonoBehaviour
{
    protected virtual void Start()
    {
        SeasonsController.SeasonsUpdated += OnSeasonUpdated;
    }

    protected virtual void OnDestroy()
    {
        SeasonsController.SeasonsUpdated -= OnSeasonUpdated;
    }

    protected abstract void OnSeasonUpdated(SeasonsJson json);
}
