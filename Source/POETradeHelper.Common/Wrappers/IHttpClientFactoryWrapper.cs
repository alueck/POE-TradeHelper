namespace POETradeHelper.Common.Wrappers
{
    public interface IHttpClientFactoryWrapper
    {
        IHttpClientWrapper CreateClient();
    }
}