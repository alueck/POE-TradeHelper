using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StaticItemDataServiceTests
    {
        private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
        private StaticItemDataService staticItemDataService;
        private Mock<IHttpClientWrapper> httpClientWrapperMock;

        [SetUp]
        public void Setup()
        {
            this.httpClientWrapperMock = new Mock<IHttpClientWrapper>();
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("")
                });

            var httpClientFactoryWrapperMock = new Mock<IHttpClientFactoryWrapper>();
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient())
                .Returns(this.httpClientWrapperMock.Object);

            this.poeTradeApiJsonSerializerMock = new Mock<IPoeTradeApiJsonSerializer>();

            this.staticItemDataService = new StaticItemDataService(httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.staticItemDataService.OnInitAsync();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStaticDataEndpoint, It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task OnInitShouldDeserializeGetAsyncResponseAsQueryResult()
        {
            string content = "serialized content";

            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent(content)
                });

            await this.staticItemDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<Data<StaticData>>>(content));
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            AsyncTestDelegate testDelegate = async () => await this.staticItemDataService.OnInitAsync();

            var exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStaticDataEndpoint));
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForItem()
        {
            const string expected = "wis";
            var item = new CurrencyItem
            {
                Name = "Scroll of Wisdom"
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new Data<StaticData>
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new StaticData { Id = "alt", Text = "Orb of Alteration" },
                                new StaticData { Id = expected, Text = item.Name }
                            }
                        }
                    }
                });

            await this.staticItemDataService.OnInitAsync();

            string result = this.staticItemDataService.GetId(item);

            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(UnsupportedItems))]
        public void GetIdShouldThrowNotSupportedExceptionIfItemIsNotSupported(Item unsupportedItem)
        {
            TestDelegate testDelegate = () => this.staticItemDataService.GetId(unsupportedItem);

            var exception = Assert.Catch<NotSupportedException>(testDelegate);

            Assert.That(exception.Message, Contains.Substring(unsupportedItem.GetType().Name));
        }

        [Test]
        public async Task GetTextShouldReturnCorrectTextForId()
        {
            const string expected = "Scroll of Wisdom";
            const string id = "wis";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new Data<StaticData>
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new StaticData { Id = "alt", Text = "Orb of Alteration" },
                                new StaticData { Id = id, Text = expected }
                            }
                        }
                    }
                });

            await this.staticItemDataService.OnInitAsync();

            string result = this.staticItemDataService.GetText(id);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetTextShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>()
                });

            await this.staticItemDataService.OnInitAsync();

            TestDelegate testDelegate = () => this.staticItemDataService.GetText("abc");

            Assert.Throws<KeyNotFoundException>(testDelegate);
        }

        [Test]
        public async Task GetImageurlShouldReturnCorrectUrlForId()
        {
            const string entryUrl = "image/Art/2DItems/Currency/CurrencyRerollRare.png";
            string expected = Resources.PoeCdnUrl + entryUrl;
            const string id = "wis";

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>
                    {
                        new Data<StaticData>
                        {
                            Id = "Currency",
                            Entries = new List<StaticData>
                            {
                                new StaticData { Id = "alt",  Image = "abc" },
                                new StaticData { Id = id, Image = entryUrl }
                            }
                        }
                    }
                });

            await this.staticItemDataService.OnInitAsync();

            Uri result = this.staticItemDataService.GetImageUrl(id);

            Assert.That(result.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public async Task GetImageUrlShouldThrowKeyNotFoundExceptionIfEntryIsNotFound()
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StaticData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StaticData>>
                {
                    Result = new List<Data<StaticData>>()
                });

            await this.staticItemDataService.OnInitAsync();

            TestDelegate testDelegate = () => this.staticItemDataService.GetImageUrl("abc");

            Assert.Throws<KeyNotFoundException>(testDelegate);
        }

        private static IEnumerable<Item> UnsupportedItems
        {
            get
            {
                yield return new FlaskItem(ItemRarity.Normal);
                yield return new MapItem(ItemRarity.Normal);
                yield return new OrganItem();
                yield return new ProphecyItem();
                yield return new EquippableItem(ItemRarity.Normal);
                yield return new GemItem();
            }
        }
    }
}