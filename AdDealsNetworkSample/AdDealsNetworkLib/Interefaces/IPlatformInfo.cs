namespace AdDealsNetworkLib
{
    public interface IPlatformInfo
    {
        string GetDeviceId();

        string GetMobileOperatorName();

        string GetMobileConnectionType();
    }
}
