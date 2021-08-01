﻿using System.Threading;
using System.Threading.Tasks;
using POETradeHelper.ItemSearch.Contract.Models;

namespace POETradeHelper.PricePrediction.ViewModels
{
    public interface IPricePredictionViewModel
    {
        Task LoadAsync(Item item, CancellationToken cancellationToken);
    }
}