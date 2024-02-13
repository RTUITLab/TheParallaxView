using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AstraSDK;

public class Popup : MonoBehaviour
{
    private Button defaultButton;
    private EventSystem eventSystem;
    private GameObject lastSelect;

    void Awake()
    {
        defaultButton = GetComponentInChildren<Button>();
        eventSystem = GameObject.FindObjectOfType<EventSystem>();

        defaultButton.onClick.AddListener(() =>
        {
            Quit();
        });
        AstraSDKManager.Instance.OnInitializeFailed.AddListener(() =>
        {
            Show();
        });
        Hide();
    }

    void OnEnable()
    {
        lastSelect = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(defaultButton.gameObject);
    }

    void OnDisable()
    {
        eventSystem.SetSelectedGameObject(lastSelect);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
