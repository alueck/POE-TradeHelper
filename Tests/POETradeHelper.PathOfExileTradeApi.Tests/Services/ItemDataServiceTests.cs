using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Moq;

using NUnit.Framework;

using POETradeHelper.Common.Wrappers;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class ItemDataServiceTests
    {
        private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
        private ItemDataService itemDataService;
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
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient(Constants.HttpClientNames.PoeTradeApiDataClient))
                .Returns(this.httpClientWrapperMock.Object);

            this.poeTradeApiJsonSerializerMock = new Mock<IPoeTradeApiJsonSerializer>();

            this.itemDataService = new ItemDataService(httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.itemDataService.OnInitAsync();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(Resources.PoeTradeApiItemDataEndpoint, It.IsAny<CancellationToken>()));
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

            await this.itemDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<Data<ItemData>>>(content));
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            AsyncTestDelegate testDelegate = async () => await this.itemDataService.OnInitAsync();

            var exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiItemDataEndpoint));
        }

        [TestCase("Sanguine Leather Belt of the Whelpling")]
        [TestCase("Leather Belt")]
        [TestCase("Belt")]
        public async Task GetTypeShouldReturnCorrectTypeForItemName(string name)
        {
            const string expectedType = "Leather Belt";
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<ItemData>>>(It.IsAny<string>()))
               .Returns(new QueryResult<Data<ItemData>>
               {
                   Result = new List<Data<ItemData>>
                   {
                        new Data<ItemData>
                        {
                            Id = "Accessories",
                            Entries = new List<ItemData>
                            {
                                new ItemData { Name = "Wurm's Molt", Type = expectedType },
                                new ItemData { Name = "Test" , Text = "Belt" },
                                new ItemData { Name = expectedType , Text = expectedType }
                            }
                        }
                   }
               });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetType(name);

            Assert.That(result, Is.EqualTo(expectedType));
        }

        [Test]
        public async Task GetTypeShouldReturnNullIfNoMatchFound()
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<ItemData>>>(It.IsAny<string>()))
               .Returns(new QueryResult<Data<ItemData>>
               {
                   Result = new List<Data<ItemData>>
                   {
                                    new Data<ItemData>
                                    {
                                        Id = "Accessories",
                                        Entries = new List<ItemData>
                                        {
                                            new ItemData { Name = "Wurm's Molt", Type = "Leather Belt" },
                                            new ItemData { Name = "Test" , Text = "Belt" },
                                            new ItemData { Name = "Leather Belt" , Text = "Leather Belt" }
                                        }
                                    }
                   }
               });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetType("No Match");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCategoryShouldReturnCorrectCategory()
        {
            const string expectedCategory = "Weapons";
            const string itemType = "Elegant Sword";
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<ItemData>>>(It.IsAny<string>()))
               .Returns(new QueryResult<Data<ItemData>>
               {
                   Result = new List<Data<ItemData>>
                   {
                                    new Data<ItemData>
                                    {
                                        Id = "Accessories",
                                        Entries = new List<ItemData>
                                        {
                                            new ItemData { Name = "Wurm's Molt", Type = "Leather Belt" }
                                        }
                                    },
                                    new Data<ItemData>
                                    {
                                        Id = expectedCategory,
                                        Entries = new List<ItemData>
                                        {
                                            new ItemData { Name = "Broken Sword", Type = "Broken Sword" },
                                            new ItemData { Name = "Sword", Type = itemType },
                                        }
                                    }
                   }
               });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetCategory(itemType);

            Assert.That(result, Is.EqualTo(expectedCategory));
        }

        [Test]
        public async Task GetCategoryShouldReturnNullIfNoMatchFound()
        {
            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<ItemData>>>(It.IsAny<string>()))
               .Returns(new QueryResult<Data<ItemData>>
               {
                   Result = new List<Data<ItemData>>
                   {
                                    new Data<ItemData>
                                    {
                                        Id = "Accessories",
                                        Entries = new List<ItemData>
                                        {
                                            new ItemData { Name = "Wurm's Molt", Type = "Leather Belt" }
                                        }
                                    }
                   }
               });

            await this.itemDataService.OnInitAsync();

            string result = this.itemDataService.GetCategory("abc");

            Assert.That(result, Is.Null);
        }
    }
}
