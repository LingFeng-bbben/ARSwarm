public class DeviceInfo
{
    public int givenTag { get; set; } = -1;
    public string macAddress { get; set; } = string.Empty;
    public int[] rBump { get; set; } = new int[2];
    public int[] rEncoder { get; set; } = new int[2];
    public DeviceInfo(int givenTag, string macAddress)
    {
        this.givenTag = givenTag;
        this.macAddress = macAddress;
    }
}

public class DeviceSensor
{
    public int givenTag { get; set; } = -1;
    public string message = string.Empty;
    public DeviceSensor(int givenTag, string message)
    {
        this.givenTag = givenTag;
        this.message = message;
    }
}