﻿using System;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Services;

namespace POETradeHelper.ItemSearch.Services.Parsers
{
    public class ItemTypeParser : IItemTypeParser
    {
        private IItemDataService itemDataService;

        public ItemTypeParser(IItemDataService itemDataService)
        {
            this.itemDataService = itemDataService;
        }

        public string ParseType(string[] itemStringLines, ItemRarity itemRarity, bool isIdentified)
        {
            if (itemRarity != ItemRarity.Normal && itemRarity != ItemRarity.Magic && itemRarity != ItemRarity.Rare && itemRarity != ItemRarity.Unique)
            {
                throw new ArgumentException($"Item rarity {itemRarity} is not supported.", nameof(itemRarity));
            }

            bool itemHasName = isIdentified && itemRarity >= ItemRarity.Rare;
            int typeLineIndex = itemHasName ? 2 : 1;

            return this.itemDataService.GetType(itemStringLines[typeLineIndex]);
        }
    }
}