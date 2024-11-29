using System;
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
        private readonly IPoeTradeApiJsonSerializer poeTradeApiJsonSerializerMock;
        private readonly ItemDataService itemDataService;
        private readonly IHttpClientWrapper httpClientWrapperMock;

        public ItemDataServiceTests()
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
        public async Task OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            Func<Task> action = this.itemDataService.OnInitAsync;

            await action.Should()
                .ThrowAsync<PoeTradeApiCommunicationException>()
                .Where(x => x.Message.Contains(Resources.PoeTradeApiItemDataEndpoint));
        }

        [TestCase("Sanguine Leather Belt of the Whelpling")]
        [TestCase("Leather Belt")]
        public async Task GetTypeShouldReturnCorrectTypeForItemName(string name)
        {
            const string expectedType = "Leather Belt";
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Accessories",
                            Entries =
                            [
                                new() { Text = "Belt", Type = "Belt" },
                                new() { Text = expectedType, Type = expectedType },
                            ],
                        },
                    ],
                });

            await this.itemDataService.OnInitAsync();

            ItemType? result = this.itemDataService.GetType(name);

            result.Should().BeEquivalentTo(new ItemType(expectedType));
        }

        [Test]
        public async Task GetTypeShouldReturnCorrectTypeForMap()
        {
            const string expectedType = "Primordial Pool Map";
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Gems",
                            Entries =
                            [
                                new() { Text = "Blight", Type = "Blight" },
                            ],
                        },
                        new()
                        {
                            Id = "Maps",
                            Entries =
                            [
                                new() { Text = expectedType, Type = expectedType },
                            ],
                        },
                    ],
                });

            await this.itemDataService.OnInitAsync();

            ItemType? result = this.itemDataService.GetType("Blight-ravaged Primordial Pool Map");

            result.Should().BeEquivalentTo(new ItemType(expectedType));
        }

        [Test]
        public async Task GetTypeShouldReturnCorrectTypeForGem()
        {
            ItemType expectedType = new("Eye of Winter", "alt_y");
            const string gemName = "Eye of Winter of Transience";

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Gems",
                            Entries =
                            [
                                new() { Text = expectedType.Type, Type = expectedType.Type },
                                new() { Text = gemName, Type = expectedType.Type, Disc = expectedType.Discriminator },
                            ],
                        },
                    ],
                });

            await this.itemDataService.OnInitAsync();

            ItemType? result = this.itemDataService.GetType(gemName);

            result.Should().BeEquivalentTo(expectedType);
        }

        [Test]
        public async Task GetTypeShouldReturnEmptyStringIfNoMatchFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Accessories",
                            Entries =
                            [
                                new() { Text = "Wurm's Molt Leather Belt", Type = "Leather Belt" },
                                new() { Text = "Test", Type = "Belt" },
                                new() { Text = "Leather Belt", Type = "Leather Belt" },
                            ],
                        },
                    ],
                });

            await this.itemDataService.OnInitAsync();

            ItemType? result = this.itemDataService.GetType("No Match");

            result.Should().BeNull();
        }

        [Test]
        public async Task GetCategoryShouldReturnCorrectCategory()
        {
            const string expectedCategory = "Weapons";
            const string itemType = "Elegant Sword";
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Accessories",
                            Entries =
                            [
                                new() { Text = "Wurm's Molt Leather Bealt", Type = "Leather Belt" },
                            ],
                        },
                        new()
                        {
                            Id = expectedCategory,
                            Entries =
                            [
                                new() { Text = "Broken Sword", Type = "Broken Sword" },
                                new() { Text = "Sword", Type = itemType },
                            ],
                        },

                    ],
                });

            await this.itemDataService.OnInitAsync();

            string? result = this.itemDataService.GetCategory(itemType);

            result.Should().Be(expectedCategory);
        }

        [Test]
        public async Task GetCategoryShouldReturnNullIfNoMatchFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<ItemData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<ItemData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Accessories",
                            Entries =
                            [
                                new() { Text = "Wurm's Molt Leather Belt", Type = "Leather Belt" },
                            ],
                        },
                    ],
                });

            await this.itemDataService.OnInitAsync();

            string? result = this.itemDataService.GetCategory("abc");

            result.Should().BeNull();
        }
    }
}