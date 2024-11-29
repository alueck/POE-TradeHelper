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

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StaticDataServiceTests
    {
        private readonly IPoeTradeApiJsonSerializer poeTradeApiJsonSerializerMock;
        private readonly StaticDataService staticDataService;
        private readonly IHttpClientWrapper httpClientWrapperMock;

        public StaticDataServiceTests()
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

            this.staticDataService =
                new StaticDataService(httpClientFactoryWrapperMock, this.poeTradeApiJsonSerializerMock);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.staticDataService.OnInitAsync();

            await this.httpClientWrapperMock
                .Received()
                .GetAsync(Resources.PoeTradeApiStaticDataEndpoint, Arg.Any<CancellationToken>());
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

            await this.staticDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock
                .Received()
                .Deserialize<QueryResult<Data<StaticData>>>(content);
        }

        [Test]
        public async Task OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            Func<Task> action = this.staticDataService.OnInitAsync;

            await action.Should()
                .ThrowAsync<PoeTradeApiCommunicationException>()
                .Where(x => x.Message.Contains(Resources.PoeTradeApiStaticDataEndpoint));
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForItemName()
        {
            const string expected = "wis";
            const string itemName = "Scroll of Wisdom";

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Currency",
                            Entries =
                            [
                                new() { Id = "alt", Text = "Orb of Alteration" },
                                new() { Id = expected, Text = itemName },
                            ],
                        },
                    ],
                });

            await this.staticDataService.OnInitAsync();

            string? result = this.staticDataService.GetId(itemName);

            result.Should().Be(expected);
        }

        [Test]
        public void GetIdShouldReturnNullIfItemIdIsNotFound()
        {
            string? result = this.staticDataService.GetId("random name");

            result.Should().BeNull();
        }

        [Test]
        public async Task GetTextShouldReturnCorrectTextForId()
        {
            const string expected = "Scroll of Wisdom";
            const string id = "wis";

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Currency",
                            Entries =
                            [
                                new() { Id = "alt", Text = "Orb of Alteration" },
                                new() { Id = id, Text = expected },
                            ],
                        },
                    ],
                });

            await this.staticDataService.OnInitAsync();

            string result = this.staticDataService.GetText(id);

            result.Should().Be(expected);
        }

        [Test]
        public async Task GetTextShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = [],
                });

            await this.staticDataService.OnInitAsync();

            Action action = () => this.staticDataService.GetText("abc");

            action.Should().Throw<KeyNotFoundException>();
        }

        [Test]
        public async Task GetImageUrlShouldReturnCorrectUrlForId()
        {
            const string entryUrl = "image/Art/2DItems/Currency/CurrencyRerollRare.png";
            string expected = Resources.PoeCdnUrl + entryUrl;
            const string id = "wis";

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = "Currency",
                            Entries =
                            [
                                new() { Id = "alt", Image = "abc" },
                                new() { Id = id, Image = entryUrl },
                            ],
                        },
                    ],
                });

            await this.staticDataService.OnInitAsync();

            Uri result = this.staticDataService.GetImageUrl(id);

            result.ToString().Should().BeEquivalentTo(expected);
        }

        [Test]
        public async Task GetImageUrlShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StaticData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = [],
                });

            await this.staticDataService.OnInitAsync();

            Action action = () => this.staticDataService.GetImageUrl("abc");

            action.Should().Throw<KeyNotFoundException>();
        }
    }
}