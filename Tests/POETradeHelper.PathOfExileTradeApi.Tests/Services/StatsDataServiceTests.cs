using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using POETradeHelper.Common.Extensions;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;

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
            httpClientFactoryWrapperMock.Setup(x => x.CreateClient(Constants.HttpClientNames.PoeTradeApiDataClient))
                .Returns(this.httpClientWrapperMock.Object);

            this.poeTradeApiJsonSerializerMock = new Mock<IPoeTradeApiJsonSerializer>();

            this.statsDataService = new StatsDataService(httpClientFactoryWrapperMock.Object, this.poeTradeApiJsonSerializerMock.Object, Mock.Of<ILogger<StatsDataService>>());
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.statsDataService.OnInitAsync();

            this.httpClientWrapperMock.Verify(x => x.GetAsync(Resources.PoeTradeApiStatsDataEndpoint, It.IsAny<CancellationToken>()));
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
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiStatsDataEndpoint));
        }

        [TestCase("+39 to maximum life", "# to maximum life")]
        [TestCase("60% chance for Poisons inflicted with this Weapon to deal 100% more Damage", "#% chance for Poisons inflicted with this Weapon to deal 100% more Damage")]
        [TestCase("0.37% of Physical Attack Damage Leeched as Life", "#% of Physical Attack Damage Leeched as Life")]
        public async Task GetStatDataShouldReturnCorrectStatDataForExplicitStat(string statText, string statDataText)
        {
            string statCategory = StatCategory.Explicit.GetDisplayName();

            var expected = new StatData { Id = "explicit.stat_3299347043", Text = statDataText, Type = statCategory.ToLower() };

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

            StatData result = this.statsDataService.GetStatData(statText, false, statCategory);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatData()
        {
            string statCategory = StatCategory.Explicit.GetDisplayName();
            const string itemStatText = "Adds 10 to 20 Chaos Damage";

            var expected = new StatData { Id = "explicit.stat_3299347043", Text = "Adds # to # Chaos Damage", Type = statCategory.ToLower() };

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
                                            new StatData { Id = "explicit.stat_4220027924", Text = "Adds # to # Chaos Damage to Attacks", Type = statCategory.ToLower() },
                                            expected
                                        }
                                    }
                    }
                });

            await this.statsDataService.OnInitAsync();

            StatData result = this.statsDataService.GetStatData(itemStatText, false, statCategory);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForExplicitStatWithFixedValues()
        {
            string statCategory = StatCategory.Explicit.GetDisplayName();
            const string itemStatText = "60% chance for Poisons inflicted with this Weapon to deal 100% more Damage";

            var expected = new StatData { Id = "explicit.stat_3299347043", Text = "#% chance for Poisons inflicted with this Weapon to deal 100% more Damage", Type = statCategory.ToLower() };

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
                                            new StatData { Id = "explicit.stat_4220027924", Text = "#% chance for Poisons inflicted with this Weapon to deal 300% more Damage", Type = statCategory.ToLower() },
                                            expected
                                        }
                                    }
                    }
                });

            await this.statsDataService.OnInitAsync();

            StatData result = this.statsDataService.GetStatData(itemStatText, false, statCategory);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForImplicitStat()
        {
            const StatCategory statCategory = StatCategory.Implicit;
            var implicitItemStat = new ItemStat(statCategory) { Text = "3% increased Movement Speed" };
            var expected = new StatData { Id = "stat_12345678", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower() };

            await this.GetStatDataShouldReturnCorrectStatData(implicitItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForCraftedStat()
        {
            const StatCategory statCategory = StatCategory.Crafted;
            var craftedItemStat = new ItemStat(statCategory) { Text = "10% increased Movement Speed" };

            var expected = new StatData { Id = "stat_1234", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower() };

            await this.GetStatDataShouldReturnCorrectStatData(craftedItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForEnchantedStat()
        {
            const StatCategory statCategory = StatCategory.Enchant;
            var craftedItemStat = new ItemStat(statCategory) { Text = "10% increased Movement Speed" };

            var expected = new StatData { Id = "stat_1234", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower() };

            await this.GetStatDataShouldReturnCorrectStatData(craftedItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForMonsterStat()
        {
            const StatCategory statCategory = StatCategory.Monster;
            var monsterItemStat = new ItemStat(statCategory) { Text = "Drops additional Currency Items" };

            var expected = new StatData { Id = "stat_2250533757", Text = "Drops additional Currency Items (×#)", Type = statCategory.GetDisplayName().ToLower() };

            await this.GetStatDataShouldReturnCorrectStatData(monsterItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataEvenIfTextWithPlaceholdersDoesNotFullyMatch()
        {
            const StatCategory statCategory = StatCategory.Implicit;
            var explicitItemStat = new ItemStat(statCategory) { Text = "+10 to Maximum Mana per Green Socket" };

            var expected = new StatData { Id = "stat_2250533757", Text = "+# to Maximum Mana per Green Socket", Type = statCategory.GetDisplayName().ToLower() };

            await this.GetStatDataShouldReturnCorrectStatData(explicitItemStat, expected);
        }

        private async Task GetStatDataShouldReturnCorrectStatData(ItemStat itemStat, StatData expectedStatData)
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

            StatData result = this.statsDataService.GetStatData(itemStat.Text, false, itemStat.StatCategory.GetDisplayName());

            Assert.That(result, Is.EqualTo(expectedStatData));
        }

        [Test]
        public async Task GetStatDataShouldReturnOnlyMatchingStatDataFromGivenCategories()
        {
            string statCategoryToSearch = StatCategory.Implicit.GetDisplayName();
            const string itemStatText = "3% increased Movement Speed";
            var expectedStatData = new StatData { Id = "expectedId", Text = "#% increased Movement Speed", Type = statCategoryToSearch.ToLower() };

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
                                                    Id = statCategoryToSearch,
                                                    Entries = new List<StatData>
                                                    {
                                                        expectedStatData
                                                    }
                                                }
                    }
                });

            await this.statsDataService.OnInitAsync();

            StatData result = this.statsDataService.GetStatData(itemStatText, false, statCategoryToSearch);

            Assert.That(result, Is.EqualTo(expectedStatData));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("non existing id")]
        public async Task GetStatDataWithIdShouldReturnNull(string itemStatId)
        {
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
                                new StatData { Id = "random id", Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower() },
                            }
                        }
                    }
                });

            await this.statsDataService.OnInitAsync();

            StatData result = this.statsDataService.GetStatDataById(itemStatId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetStatDataWithIdShouldReturnCorrectStatData()
        {
            StatData expected = new StatData { Id = "expectedId", Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower() };

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
                                            new StatData { Id = "random id", Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower() },
                                            expected
                                        }
                                    }
                    }
                });

            await this.statsDataService.OnInitAsync();

            StatData result = this.statsDataService.GetStatDataById(expected.Id);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetStatDataShouldReturnNullForNonExactMatch()
        {
            // arrange
            const string itemStatText = "+15% reduced Cast Speed";

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
                                                        new StatData { Id = "random id", Type = POETradeHelper.ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(), Text = "Enemies you Shock have #% reduced Cast Speed" }
                                                    }
                                                }
                    }
                });

            await this.statsDataService.OnInitAsync();

            // act
            StatData result = this.statsDataService.GetStatData(itemStatText, false);

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetStatDataShouldPreferLocalStat()
        {
            // arrange
            const string itemStatText = "+15 % attack speed";
            var expectedStatData = new StatData
            {
                Id = "expected id",
                Text = $"{itemStatText} ({Resources.LocalKeyword})",
                Type = StatCategory.Explicit.GetDisplayName().ToLower()
            };

            this.poeTradeApiJsonSerializerMock.Setup(x => x.Deserialize<QueryResult<Data<StatData>>>(It.IsAny<string>()))
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                                                            new Data<StatData>
                                                            {
                                                                Id = StatCategory.Explicit.GetDisplayName(),
                                                                Entries = new List<StatData>
                                                                {
                                                                    new StatData { Id = "random id", Type = expectedStatData.Type, Text = itemStatText },
                                                                    expectedStatData
                                                                }
                                                            }
                    }
                });

            await this.statsDataService.OnInitAsync();

            // act
            StatData result = this.statsDataService.GetStatData(itemStatText, true);

            // assert
            Assert.That(result, Is.EqualTo(expectedStatData));
        }
    }
}