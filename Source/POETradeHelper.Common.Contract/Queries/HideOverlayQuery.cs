using MediatR;

namespace POETradeHelper.Common.Contract.Queries
{
    public class HideOverlayQuery : IRequest<HideOverlayResponse>
    {
    }

    public class HideOverlayResponse
    {
        public HideOverlayResponse(bool handled)
        {
            Handled = handled;
        }

        public bool Handled { get; }
    }
}