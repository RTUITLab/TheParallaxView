using UnityEngine;

public class TESTSeasons : MonoBehaviour
{
    [SerializeField] private SeasonsJson _serverAnswer;

    public static SeasonsJson Answer { get; private set; }

    private void Update()
    {
        Answer = _serverAnswer;
    }
}
