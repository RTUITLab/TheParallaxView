using UnityEngine;
using System.Collections;

public class Tips : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        StreamViewModel.Instance.depthRecord.onValueChanged += (value) =>
        {
            if (!value)
            {
                Show();
                StartCoroutine(DelayHide());
            }
        };
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator DelayHide()
    {
        yield return new WaitForSeconds(3f);
        Hide();
    }
}
