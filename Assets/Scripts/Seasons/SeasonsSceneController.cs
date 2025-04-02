using UnityEngine;

public class SeasonsSceneController : BaseSeasonElementController
{
    [SerializeField] private Transform _modelsParent;
    [SerializeField] private BlackScreen _blackScreen;

    private string _prevSceneName = "none";
    private GameObject _prevScene = null;

    protected override void Start()
    {
        base.Start();
        for(int i = 0; i < _modelsParent.childCount; i++)
        {
            _modelsParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    protected override void OnSeasonUpdated(SeasonsJson json)
    {
        if (json.Scene == _prevSceneName)
            return;
        _prevSceneName = json.Scene;

        GameObject target = null;
        for(int i = 0; i < _modelsParent.childCount; i++)
        {
            var child = _modelsParent.GetChild(i).gameObject;
            if(child.name == json.Scene)
            {
                target = child;
                break;
            }
        }

        _blackScreen.StartAnimation(SeasonsController.TransitionTime, () =>
        {
            _prevScene?.SetActive(false);
            _prevScene = target;
            target?.SetActive(true);
        });
    }
}
