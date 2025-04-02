using System;

[Serializable]
public class SeasonsJson : ICopiable<SeasonRawJSON>
{
    public string weather = "sun";
    public string time = "day";
    public string voice = "none";
    public string VideoOnBack = "Castle";
    public string Scene = "gardenOfStones";

    public SeasonsJson() { }

    public void Copy(SeasonRawJSON obj)
    {
        weather = obj.weather.designation;
        time = obj.time.designation;
        voice = obj.voice.designation;
        VideoOnBack = obj.VideoOnBack.designation;
        Scene = obj.Scene.designation;
    }

    public string ToDebugString()
    {
        return $"weather: {weather}, time: {time}, voice: {voice}, VideoOnBack: {VideoOnBack}, Scene: {Scene}";
    }
}
