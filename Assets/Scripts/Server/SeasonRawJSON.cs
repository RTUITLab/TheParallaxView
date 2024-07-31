using System;

[Serializable]
public class SeasonRawJSON
{
    [Serializable]
    public struct StringValue
    {
        public int id;
        public string designation;

        public StringValue(int id, string designation)
        {
            this.id = id;
            this.designation = designation;
        }
    }

    public StringValue weather;
    public StringValue time;
    public StringValue voice;
    public StringValue VideoOnBack;
    public StringValue Scene;
}
