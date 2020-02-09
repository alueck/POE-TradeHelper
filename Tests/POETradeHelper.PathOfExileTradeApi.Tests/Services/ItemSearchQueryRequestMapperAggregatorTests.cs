﻿using Moq;
using NUnit.Framework;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Services;
using System.Collections.Generic;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class ItemSearchQueryRequestMapperAggregatorTests
    {
        private Mock<IItemSearchQueryRequestMapper> itemSearchQueryRequestMapperMock1;
        private Mock<IItemSearchQueryRequestMapper> itemSearchQueryRequestMapperMock2;
        private ItemSearchQueryRequestMapperAggregator itemSearchQueryRequestMapperAggregator;

        [SetUp]
        public void Setup()
        {
            this.itemSearchQueryRequestMapperMock1 = new Mock<IItemSearchQueryRequestMapper>();
            this.itemSearchQueryRequestMapperMock2 = new Mock<IItemSearchQueryRequestMapper>();

            this.itemSearchQueryRequestMapperAggregator = new ItemSearchQueryRequestMapperAggregator(new List<IItemSearchQueryRequestMapper>
            {
                this.itemSearchQueryRequestMapperMock1.Object,
                this.itemSearchQueryRequestMapperMock2.Object
            });
        }

        [Test]
        public void MapToQueryRequestShouldCallCanMapOnMappers()
        {
            var item = new CurrencyItem();
            this.itemSearchQueryRequestMapperMock2.Setup(x => x.CanMap(It.IsAny<Item>()))
                .Returns(true);

            this.itemSearchQueryRequestMapperAggregator.MapToQueryRequest(item);

            this.itemSearchQueryRequestMapperMock1.Verify(x => x.CanMap(item));
            this.itemSearchQueryRequestMapperMock2.Verify(x => x.CanMap(item));
        }

        [Test]
        public void MapToQueryRequestShouldReturnResultFromMapperThatCanMap()
        {
            var item = new CurrencyItem();
            SearchQueryRequest expected = new SearchQueryRequest();

            this.itemSearchQueryRequestMapperMock2.Setup(x => x.CanMap(It.IsAny<Item>()))
                .Returns(true);
            this.itemSearchQueryRequestMapperMock2.Setup(x => x.MapToQueryRequest(It.IsAny<Item>()))
                .Returns(expected);

            SearchQueryRequest result = this.itemSearchQueryRequestMapperAggregator.MapToQueryRequest(item);

            Assert.That(result, Is.SameAs(expected));
        }
    }
}