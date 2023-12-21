using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class ItemDataServiceTests
    {
        private IPoeTradeApiJsonSerializer poeTradeApiJsonSerializerMock;
        private ItemDataService itemDataService;
        private IHttpClientWrapper httpClientWrapperMock;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = Substitute.For<IHttpClientWrapper>();
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(string.Empty),
                });

            IHttpClientFactoryWrapper httpClientFactoryWrapperMock = Substitute.For<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.CreateClient(HttpClientNames.PoeTradeApiDataClient)
                .Returns(this.httpClientWrapperMock);

            this.poeTradeApiJsonSerializerMock = Substitute.For<IPoeTradeApiJsonSerializer>();

            this.itemDataService = new ItemDataService(httpClientFactoryWrapperMock, this.poeTradeApiJsonSerializerMock);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.itemDataService.OnInitAsync();

            await this.httpClientWrapperMock
                .Received()
                .GetAsync(Resources.PoeTradeApiItemDataEndpoint, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task OnInitShouldDeserializeGetAsyncResponseAsQueryResult()
        {
            const string content = "serialized content";

            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    Content = new StringContent(content),
                });

            await this.itemDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock
                .Received()
                .Deserialize<QueryResult<Data<ItemData>>>(content);
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            AsyncTestDelegate testDelegate = async () => await this.itemDataService.OnInitAsync();

            PoeTradeApiCommunicationException
                exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            exception.Message.Should().Contain(Resources.PoeTradeApiItemDataEndpoint);
        }

        [TestCase("Sanguine Leather Belt of the Whelpling")]
        [TestCase("Leather Belt")]
        public async Task GetTypeShouldReturnCorrectTypeForItemName(string name)
        {
            const string expectedType = "Leather Belt";
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result = new List<Data<ItemData>>
                    {
                        new()
                        {
                            Id = "Accessories",
                            Entries = new List<ItemData>
                            {
                                new() { Name = "Belt", Type = "Belt" },
                                new() { Name = expectedType, Type = expectedType },
                            },
                        },
                    },
                });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetType(name);

            result.Should().Be(expectedType);
        }

        [Test]
        public async Task GetTypeShouldReturnCorrectTypeForMap()
        {
            const string expectedType = "Primordial Pool Map";
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result = new List<Data<ItemData>>
                    {
                        new()
                        {
                            Id = "Gems",
                            Entries = new List<ItemData>
                            {
                                new() { Name = "Blight", Type = "Blight" },
                            },
                        },
                        new()
                        {
                            Id = "Maps",
                            Entries = new List<ItemData>
                            {
                                new() { Name = expectedType, Type = expectedType },
                            },
                        },
                    },
                });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetType("Blight-ravaged Primordial Pool Map");

            result.Should().Be(expectedType);
        }

        [Test]
        public async Task GetTypeShouldReturnEmptyStringIfNoMatchFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result = new List<Data<ItemData>>
                    {
                        new()
                        {
                            Id = "Accessories",
                            Entries = new List<ItemData>
                            {
                                new() { Name = "Wurm's Molt", Type = "Leather Belt" },
                                new() { Name = "Test", Type = "Belt" },
                                new() { Name = "Leather Belt", Type = "Leather Belt" },
                            },
                        },
                    },
                });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetType("No Match");

            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetCategoryShouldReturnCorrectCategory()
        {
            const string expectedCategory = "Weapons";
            const string itemType = "Elegant Sword";
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result = new List<Data<ItemData>>
                    {
                        new()
                        {
                            Id = "Accessories",
                            Entries = new List<ItemData>
                            {
                                new() { Name = "Wurm's Molt", Type = "Leather Belt" },
                            },
                        },
                        new()
                        {
                            Id = expectedCategory,
                            Entries = new List<ItemData>
                            {
                                new() { Name = "Broken Sword", Type = "Broken Sword" },
                                new() { Name = "Sword", Type = itemType },
                            },
                        },
                    },
                });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetCategory(itemType);

            result.Should().Be(expectedCategory);
        }

        [Test]
        public async Task GetCategoryShouldReturnNullIfNoMatchFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result = new List<Data<ItemData>>
                    {
                        new()
                        {
                            Id = "Accessories",
                            Entries = new List<ItemData>
                            {
                                new() { Name = "Wurm's Molt", Type = "Leather Belt" },
                            },
                        },
                    },
                });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetCategory("abc");

            result.Should().BeNull();
        }
    }
}