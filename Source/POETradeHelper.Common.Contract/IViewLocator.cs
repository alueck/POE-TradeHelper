namespace POETradeHelper.Common.Contract
{
    public interface IViewLocator
    {
        object GetView(object viewModel);
    }
}