using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[UnityEngine.RequireComponent(typeof(Text))]
public class LangueSupport : MonoBehaviour
{
    public string chinese;

    // Use this for initialization
    void Start()
    {
        if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified)
        {
            if (!string.IsNullOrEmpty(chinese))
            {
                GetComponent<Text>().text = chinese;
            }
        }
    }
}
