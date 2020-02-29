using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Extensions;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.ItemSearch.Contract.Models.ItemStats;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StatsDataServiceTests
    {
        private Mock<IPoeTradeApiJsonSerializer> poeTradeApiJsonSerializerMock;
        private StatsDataService statsDataService;
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

            this.statsDataService = new StatsDataService(httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object);
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.statsDataService.OnInitAsync();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStatsDataEndpoint, It.IsAny<CancellationToken>()));
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

            await this.statsDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock.Verify(x => x.Deserialize<QueryResult<Data<StatData>>>(content));
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            AsyncTestDelegate testDelegate = async () => await this.statsDataService.OnInitAsync();

            var exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiBaseUrl + Resources.PoeTradeApiStatsDataEndpoint));
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForExplicitStat()
        {
            string statCategory = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryExplicit;
            var explicitItemStat = new ExplicitItemStat { Text = "+39 to maximum life", TextWithPlaceholders = "# to maximum life" };

            var expected = new StatData { Id = "explicit.stat_3299347043", Text = explicitItemStat.TextWithPlaceholders, Type = statCategory.ToLower() };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StatData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                                    new Data<StatData>
                                    {
                                        Id = statCategory,
                                        Entries = new List<StatData>
                                        {
                                            new StatData { Id = "explicit.stat_4220027924", Text = "#% to Cold Resistance", Type = statCategory.ToLower() },
                                            expected
                                        }
                                    }
                    }
                });

            await this.statsDataService.OnInitAsync();

            string result = this.statsDataService.GetId(explicitItemStat);

            Assert.That(result, Is.EqualTo(expected.Id));
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForImplicitStat()
        {
            var implicitItemStat = new ImplicitItemStat { Text = "3% increased Movement Speed", TextWithPlaceholders = "#% increased Movement Speed" };
            var expected = new StatData { Id = "stat_12345678", Text = implicitItemStat.TextWithPlaceholders, Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryImplicit.ToLower() };

            await this.GetIdShouldReturnCorrectId(implicitItemStat, expected);
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForCraftedStat()
        {
            var craftedItemStat = new CraftedItemStat { Text = "10% increased Movement Speed", TextWithPlaceholders = "#% increased Movement Speed" };

            var expected = new StatData { Id = "stat_1234", Text = craftedItemStat.TextWithPlaceholders, Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryCrafted.ToLower() };

            await this.GetIdShouldReturnCorrectId(craftedItemStat, expected);
        }

        [Test]
        public async Task GetIdShouldReturnCorrectIdForMonsterStat()
        {
            var monsterItemStat = new MonsterItemStat { Text = "Drops additional Currency Items", TextWithPlaceholders = "Drops additional Currency Items (×#)", Count = 2 };

            var expected = new StatData { Id = "stat_2250533757", Text = monsterItemStat.TextWithPlaceholders, Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryMonster.ToLower() };

            await this.GetIdShouldReturnCorrectId(monsterItemStat, expected);
        }

        private async Task GetIdShouldReturnCorrectId(ItemStat itemStat, StatData expectedStatData)
        {
            string statCategory = itemStat.StatCategory.GetDisplayName();

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StatData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                                    new Data<StatData>
                                    {
                                        Id = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                                        Entries = new List<StatData>
                                        {
                                            new StatData { Id = "random id", Text = expectedStatData.Text, Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower() }
                                        }
                                    },
                                    new Data<StatData>
                                    {
                                        Id = statCategory,
                                        Entries = new List<StatData>
                                        {
                                            expectedStatData
                                        }
                                    }
                    }
                });

            await this.statsDataService.OnInitAsync();

            string result = this.statsDataService.GetId(itemStat);

            Assert.That(result, Is.EqualTo(expectedStatData.Id));
        }
    }
}