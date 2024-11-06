using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField] private SunController _sunController;
    [SerializeField] private float _addRainIntensity;
    [SerializeField] private float _addCloudIntensity;
    [SerializeField] private ParticleSystem _rainParticles;

    public const string RAIN = "rain";
    public const string SUN = "sunny";
    public const string CLOUDY = "cloudy";
    public const string MODIFIER = "SsnWthr";

    private string _oldWeather = "None";

    private void Start()
    {
        SeasonsController.SeasonsUpdated += OnWeatherChange;
    }

    private void OnDestroy()
    {
        SeasonsController.SeasonsUpdated -= OnWeatherChange;
    }

    private void OnWeatherChange(SeasonsJson json)
    {
        if (json.weather == _oldWeather)
            return;
        _oldWeather = json.weather;

        if(json.weather == RAIN)
        {
            _sunController.Intensities.Set(MODIFIER, _addRainIntensity);
            _rainParticles.Play();
        }
        else if(json.weather == CLOUDY)
        {
            _sunController.Intensities.Set(MODIFIER, _addCloudIntensity);
            _rainParticles.Stop();
        }
        else
        {
            _rainParticles.Stop();
            _sunController.Intensities.Set(MODIFIER, 0f);
        }
        _sunController.UpdateSun(SeasonsController.TransitionTime);
    }
}
