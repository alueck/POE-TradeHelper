using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;

using NSubstitute;

using NUnit.Framework;

using POETradeHelper.Common.Extensions;
using POETradeHelper.Common.Wrappers;
using POETradeHelper.ItemSearch.Contract.Models;
using POETradeHelper.PathOfExileTradeApi.Constants;
using POETradeHelper.PathOfExileTradeApi.Exceptions;
using POETradeHelper.PathOfExileTradeApi.Models;
using POETradeHelper.PathOfExileTradeApi.Properties;
using POETradeHelper.PathOfExileTradeApi.Services;
using POETradeHelper.PathOfExileTradeApi.Services.Implementations;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StatsDataServiceTests
    {
        private IPoeTradeApiJsonSerializer poeTradeApiJsonSerializerMock;
        private StatsDataService statsDataService;
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

            this.statsDataService = new StatsDataService(
                httpClientFactoryWrapperMock,
                this.poeTradeApiJsonSerializerMock,
                Substitute.For<ILogger<StatsDataService>>());
        }

        [Test]
        public async Task OnInitShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.statsDataService.OnInitAsync();

            await this.httpClientWrapperMock
                .Received()
                .GetAsync(Resources.PoeTradeApiStatsDataEndpoint, Arg.Any<CancellationToken>());
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

            await this.statsDataService.OnInitAsync();

            this.poeTradeApiJsonSerializerMock
                .Received()
                .Deserialize<QueryResult<Data<StatData>>>(content);
        }

        [Test]
        public void OnInitShouldThrowPoeTradeApiCommunicationExceptionIfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            AsyncTestDelegate testDelegate = async () => await this.statsDataService.OnInitAsync();

            PoeTradeApiCommunicationException
                exception = Assert.CatchAsync<PoeTradeApiCommunicationException>(testDelegate);
            Assert.That(exception.Message, Contains.Substring(Resources.PoeTradeApiStatsDataEndpoint));
        }

        [TestCase("+39 to maximum life", "# to maximum life")]
        [TestCase("60% chance for Poisons inflicted with this Weapon to deal 100% more Damage", "#% chance for Poisons inflicted with this Weapon to deal 100% more Damage")]
        [TestCase("0.37% of Physical Attack Damage Leeched as Life", "#% of Physical Attack Damage Leeched as Life")]
        [TestCase("Added Small Passive Skills also grant: +8 to Maximum Mana", "Added Small Passive Skills also grant: +# to Maximum Mana")]
        public async Task GetStatDataShouldReturnCorrectStatDataForExplicitStat(string statText, string statDataText)
        {
            string statCategory = StatCategory.Explicit.GetDisplayName();

            StatData expected = new()
                { Id = "explicit.stat_3299347043", Text = statDataText, Type = statCategory.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = statCategory,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "explicit.stat_4220027924", Text = "#% to Cold Resistance",
                                    Type = statCategory.ToLower(),
                                },
                                expected,
                            },
                        },
                    },
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

            StatData expected = new()
                { Id = "explicit.stat_3299347043", Text = "Adds # to # Chaos Damage", Type = statCategory.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = statCategory,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "explicit.stat_4220027924", Text = "Adds # to # Chaos Damage to Attacks",
                                    Type = statCategory.ToLower(),
                                },
                                expected,
                            },
                        },
                    },
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

            StatData expected = new()
            {
                Id = "explicit.stat_3299347043",
                Text = "#% chance for Poisons inflicted with this Weapon to deal 100% more Damage",
                Type = statCategory.ToLower(),
            };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = statCategory,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "explicit.stat_4220027924",
                                    Text = "#% chance for Poisons inflicted with this Weapon to deal 300% more Damage",
                                    Type = statCategory.ToLower(),
                                },
                                expected,
                            },
                        },
                    },
                });

            await this.statsDataService.OnInitAsync();

            StatData result = this.statsDataService.GetStatData(itemStatText, false, statCategory);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForImplicitStat()
        {
            const StatCategory statCategory = StatCategory.Implicit;
            ItemStat implicitItemStat = new(statCategory) { Text = "3% increased Movement Speed" };
            StatData expected = new()
            {
                Id = "stat_12345678", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower(),
            };

            await this.GetStatDataShouldReturnCorrectStatData(implicitItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForCraftedStat()
        {
            const StatCategory statCategory = StatCategory.Crafted;
            ItemStat craftedItemStat = new(statCategory) { Text = "10% increased Movement Speed" };

            StatData expected = new()
                { Id = "stat_1234", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower() };

            await this.GetStatDataShouldReturnCorrectStatData(craftedItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForEnchantedStat()
        {
            const StatCategory statCategory = StatCategory.Enchant;
            ItemStat craftedItemStat = new(statCategory) { Text = "10% increased Movement Speed" };

            StatData expected = new()
                { Id = "stat_1234", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower() };

            await this.GetStatDataShouldReturnCorrectStatData(craftedItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataForMonsterStat()
        {
            const StatCategory statCategory = StatCategory.Monster;
            ItemStat monsterItemStat = new(statCategory) { Text = "Drops additional Currency Items" };

            StatData expected = new()
            {
                Id = "stat_2250533757", Text = "Drops additional Currency Items (×#)",
                Type = statCategory.GetDisplayName().ToLower(),
            };

            await this.GetStatDataShouldReturnCorrectStatData(monsterItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnCorrectStatDataEvenIfTextWithPlaceholdersDoesNotFullyMatch()
        {
            const StatCategory statCategory = StatCategory.Implicit;
            ItemStat explicitItemStat = new(statCategory) { Text = "+10 to Maximum Mana per Green Socket" };

            StatData expected = new()
            {
                Id = "stat_2250533757", Text = "+# to Maximum Mana per Green Socket",
                Type = statCategory.GetDisplayName().ToLower(),
            };

            await this.GetStatDataShouldReturnCorrectStatData(explicitItemStat, expected);
        }

        [Test]
        public async Task GetStatDataShouldReturnOnlyMatchingStatDataFromGivenCategories()
        {
            string statCategoryToSearch = StatCategory.Implicit.GetDisplayName();
            const string itemStatText = "3% increased Movement Speed";
            StatData expectedStatData = new()
                { Id = "expectedId", Text = "#% increased Movement Speed", Type = statCategoryToSearch.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "random id", Text = expectedStatData.Text,
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                            },
                        },
                        new()
                        {
                            Id = statCategoryToSearch,
                            Entries = new List<StatData>
                            {
                                expectedStatData,
                            },
                        },
                    },
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
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "random id",
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                            },
                        },
                    },
                });

            await this.statsDataService.OnInitAsync();

            StatData result = this.statsDataService.GetStatDataById(itemStatId);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetStatDataWithIdShouldReturnCorrectStatData()
        {
            StatData expected = new()
                { Id = "expectedId", Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "random id",
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                                expected,
                            },
                        },
                    },
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

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "random id",
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                    Text = "Enemies you Shock have #% reduced Cast Speed",
                                },
                            },
                        },
                    },
                });

            await this.statsDataService.OnInitAsync();

            // act
            StatData result = this.statsDataService.GetStatData(itemStatText, false);

            // assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetStatDataShouldPreferLocalStat()
        {
            // arrange
            const string itemStatText = "+15 % attack speed";
            StatData expectedStatData = new()
            {
                Id = "expected id",
                Text = $"{itemStatText} ({Resources.LocalKeyword})",
                Type = StatCategory.Explicit.GetDisplayName().ToLower(),
            };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = StatCategory.Explicit.GetDisplayName(),
                            Entries = new List<StatData>
                            {
                                new() { Id = "random id", Type = expectedStatData.Type, Text = itemStatText },
                                expectedStatData,
                            },
                        },
                    },
                });

            await this.statsDataService.OnInitAsync();

            // act
            StatData result = this.statsDataService.GetStatData(itemStatText, true);

            // assert
            Assert.That(result, Is.EqualTo(expectedStatData));
        }

        private async Task GetStatDataShouldReturnCorrectStatData(ItemStat itemStat, StatData expectedStatData)
        {
            string statCategory = itemStat.StatCategory.GetDisplayName();

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result = new List<Data<StatData>>
                    {
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries = new List<StatData>
                            {
                                new()
                                {
                                    Id = "random id", Text = expectedStatData.Text,
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                            },
                        },
                        new()
                        {
                            Id = statCategory,
                            Entries = new List<StatData>
                            {
                                expectedStatData,
                            },
                        },
                    },
                });

            await this.statsDataService.OnInitAsync();

            StatData result =
                this.statsDataService.GetStatData(itemStat.Text, false, itemStat.StatCategory.GetDisplayName());

            Assert.That(result, Is.EqualTo(expectedStatData));
        }
    }
}