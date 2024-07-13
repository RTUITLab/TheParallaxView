public class SeasonsJson : ICopiable<SeasonsJson>
{
    public string weather;
    public string time;
    public string voice;
    public string VideoOnBack;
    public string Scene;

    public SeasonsJson() { }

    public void Copy(SeasonsJson obj)
    {
        weather = obj.weather;
        time = obj.time;
        voice = obj.voice;
        VideoOnBack = obj.VideoOnBack;
        Scene = obj.Scene;
    }
}
