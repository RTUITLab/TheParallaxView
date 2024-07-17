using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SunController : MonoBehaviour
{
    [SerializeField] private Light _sun;

    public readonly Dictionary<string, Color> ColorsAdd = new();
    public readonly Dictionary<string, float> Intensities = new();
    public readonly Dictionary<string, Color> ColorsSubtract = new();

    public void UpdateSun(float transitionTime)
    {
        Color color = Color.black;
        foreach(var pair in ColorsAdd) color += pair.Value;
        foreach (var pair in ColorsSubtract) color -= pair.Value;
        color = new(Mathf.Clamp01(color.a), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b));
        float intensity = Mathf.Clamp(Intensities.Sum(pair => pair.Value), 0f, 100f);

        _sun.DOColor(color, transitionTime);
        _sun.DOIntensity(intensity, transitionTime);
    }
}
