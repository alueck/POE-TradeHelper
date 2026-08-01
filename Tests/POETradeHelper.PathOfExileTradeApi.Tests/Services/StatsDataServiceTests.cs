using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

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
using POETradeHelper.RePoE.Models;
using POETradeHelper.RePoE.Services;

namespace POETradeHelper.PathOfExileTradeApi.Tests.Services
{
    public class StatsDataServiceTests
    {
        private readonly IPoeTradeApiJsonSerializer poeTradeApiJsonSerializerMock;
        private readonly IAlternativeStatTextsService alternativeStatTextsServiceMock;
        private readonly FakeLogger<StatsDataService> loggerMock;
        private readonly StatsDataService statsDataService;
        private readonly IHttpClientWrapper httpClientWrapperMock;

        public StatsDataServiceTests()
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
            this.alternativeStatTextsServiceMock = Substitute.For<IAlternativeStatTextsService>();
            this.loggerMock = new FakeLogger<StatsDataService>();

            this.statsDataService = new StatsDataService(
                httpClientFactoryWrapperMock,
                this.poeTradeApiJsonSerializerMock,
                this.alternativeStatTextsServiceMock,
                this.loggerMock);
        }

        [Test]
        public async Task OnInit_ShouldCallGetAsyncOnHttpClientWrapper()
        {
            await this.statsDataService.OnInitAsync();

            await this.httpClientWrapperMock
                .Received()
                .GetAsync(Resources.PoeTradeApiStatsDataEndpoint, Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task OnInit_ShouldDeserializeGetAsyncResponseAsQueryResult()
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
        public async Task OnInit_ShouldThrowPoeTradeApiCommunicationException_IfStatusCodeIsNotSuccess()
        {
            this.httpClientWrapperMock.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            Func<Task> action = this.statsDataService.OnInitAsync;

            await action.Should()
                .ThrowAsync<PoeTradeApiCommunicationException>()
                .Where(x => x.Message.Contains(Resources.PoeTradeApiStatsDataEndpoint));
        }

        [Test]
        public async Task OnInit_ShouldFetchAlternativeStatTexts()
        {
            // arrange
            this.poeTradeApiJsonSerializerMock
                .Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>());

            // act
            await this.statsDataService.OnInitAsync();

            // assert
            this.alternativeStatTextsServiceMock
                .Received()
                .GetAlternativeStatTexts();
        }

        [Test]
        public async Task OnInit_ShouldCatchAndLogExceptionFromAlternativeStatTextsService()
        {
            // arrange
            this.poeTradeApiJsonSerializerMock
                .Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>());

            Exception exception = new("Test exception");
            this.alternativeStatTextsServiceMock
                .GetAlternativeStatTexts()
                .Throws(exception);

            // act
            await this.statsDataService.OnInitAsync();

            // assert
            this.loggerMock.LatestRecord.Level.Should().Be(LogLevel.Warning);
            this.loggerMock.LatestRecord.Exception.Should().Be(exception);
        }

        [TestCase("+39 to maximum life", "# to maximum life")]
        [TestCase("60% chance for Poisons inflicted with this Weapon to deal 100% more Damage", "#% chance for Poisons inflicted with this Weapon to deal 100% more Damage")]
        [TestCase("0.37% of Physical Attack Damage Leeched as Life", "#% of Physical Attack Damage Leeched as Life")]
        [TestCase("Added Small Passive Skills also grant: +8 to Maximum Mana", "Added Small Passive Skills also grant: +# to Maximum Mana")]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataForExplicitStat(string statText, string statDataText)
        {
            string statCategory = StatCategory.Explicit.GetDisplayName();

            StatData expected = new() { Id = "explicit.stat_3299347043", Text = statDataText, Type = statCategory.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = statCategory,
                            Entries =
                            [
                                new()
                                {
                                    Id = "explicit.stat_4220027924", Text = "#% to Cold Resistance",
                                    Type = statCategory.ToLower(),
                                },
                                expected,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            IStatData? result = this.statsDataService.TryGetStatData([statText], false, statCategory);

            result.Should().Be(expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatData()
        {
            string statCategory = StatCategory.Explicit.GetDisplayName();
            const string itemStatText = "Adds 10 to 20 Chaos Damage";

            StatData expected = new() { Id = "explicit.stat_3299347043", Text = "Adds # to # Chaos Damage", Type = statCategory.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = statCategory,
                            Entries =
                            [
                                new()
                                {
                                    Id = "explicit.stat_4220027924", Text = "Adds # to # Chaos Damage to Attacks",
                                    Type = statCategory.ToLower(),
                                },
                                expected,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            IStatData? result = this.statsDataService.TryGetStatData([itemStatText], false, statCategory);

            result.Should().Be(expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataForExplicitStatWithFixedValues()
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
                    Result =
                    [
                        new()
                        {
                            Id = statCategory,
                            Entries =
                            [
                                new()
                                {
                                    Id = "explicit.stat_4220027924",
                                    Text = "#% chance for Poisons inflicted with this Weapon to deal 300% more Damage",
                                    Type = statCategory.ToLower(),
                                },
                                expected,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            IStatData? result = this.statsDataService.TryGetStatData([itemStatText], false, statCategory);

            result.Should().Be(expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataForImplicitStat()
        {
            const StatCategory statCategory = StatCategory.Implicit;
            ItemStat implicitItemStat = new(statCategory) { Text = "3% increased Movement Speed" };
            StatData expected = new()
            {
                Id = "stat_12345678", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower(),
            };

            await this.TryGetStatData_ShouldReturnCorrectStatData(implicitItemStat, expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataForCraftedStat()
        {
            const StatCategory statCategory = StatCategory.Crafted;
            ItemStat craftedItemStat = new(statCategory) { Text = "10% increased Movement Speed" };

            StatData expected = new() { Id = "stat_1234", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower() };

            await this.TryGetStatData_ShouldReturnCorrectStatData(craftedItemStat, expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataForEnchantedStat()
        {
            const StatCategory statCategory = StatCategory.Enchant;
            ItemStat craftedItemStat = new(statCategory) { Text = "10% increased Movement Speed" };

            StatData expected = new() { Id = "stat_1234", Text = "#% increased Movement Speed", Type = statCategory.GetDisplayName().ToLower() };

            await this.TryGetStatData_ShouldReturnCorrectStatData(craftedItemStat, expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataForMonsterStat()
        {
            const StatCategory statCategory = StatCategory.Monster;
            ItemStat monsterItemStat = new(statCategory) { Text = "Drops additional Currency Items" };

            StatData expected = new()
            {
                Id = "stat_2250533757", Text = "Drops additional Currency Items (×#)",
                Type = statCategory.GetDisplayName().ToLower(),
            };

            await this.TryGetStatData_ShouldReturnCorrectStatData(monsterItemStat, expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataEven_IfTextWithPlaceholdersDoesNotFullyMatch()
        {
            const StatCategory statCategory = StatCategory.Implicit;
            ItemStat explicitItemStat = new(statCategory) { Text = "+10 to Maximum Mana per Green Socket" };

            StatData expected = new()
            {
                Id = "stat_2250533757", Text = "+# to Maximum Mana per Green Socket",
                Type = statCategory.GetDisplayName().ToLower(),
            };

            await this.TryGetStatData_ShouldReturnCorrectStatData(explicitItemStat, expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnOnlyMatchingStatDataFromGivenCategories()
        {
            string statCategoryToSearch = StatCategory.Implicit.GetDisplayName();
            const string itemStatText = "3% increased Movement Speed";
            StatData expectedStatData = new() { Id = "expectedId", Text = "#% increased Movement Speed", Type = statCategoryToSearch.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries =
                            [
                                new()
                                {
                                    Id = "random id", Text = expectedStatData.Text,
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                            ],
                        },
                        new()
                        {
                            Id = statCategoryToSearch,
                            Entries =
                            [
                                expectedStatData,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            IStatData? result = this.statsDataService.TryGetStatData([itemStatText], false, statCategoryToSearch);

            result.Should().Be(expectedStatData);
        }

        [TestCase("")]
        [TestCase("non existing id")]
        public async Task GetStatDataWithIdShouldReturnNull(string itemStatId)
        {
            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries =
                            [
                                new()
                                {
                                    Id = "random id",
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            IStatData? result = this.statsDataService.GetStatDataById(itemStatId);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetStatDataWithIdShouldReturnCorrectStatData()
        {
            StatData expected = new() { Id = "expectedId", Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower() };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries =
                            [
                                new()
                                {
                                    Id = "random id",
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                                expected,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            IStatData? result = this.statsDataService.GetStatDataById(expected.Id);

            result.Should().Be(expected);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnNullForNonExactMatch()
        {
            // arrange
            const string itemStatText = "+15% reduced Cast Speed";

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries =
                            [
                                new()
                                {
                                    Id = "random id",
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                    Text = "Enemies you Shock have #% reduced Cast Speed",
                                },
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            // act
            IStatData? result = this.statsDataService.TryGetStatData([itemStatText], false);

            // assert
            result.Should().BeNull();
        }

        [Test]
        public async Task TryGetStatData_ShouldPreferLocalStat()
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
                    Result =
                    [
                        new()
                        {
                            Id = StatCategory.Explicit.GetDisplayName(),
                            Entries =
                            [
                                new() { Id = "random id", Type = expectedStatData.Type, Text = itemStatText },
                                expectedStatData,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            // act
            IStatData? result = this.statsDataService.TryGetStatData([itemStatText], true);

            // assert
            result.Should().Be(expectedStatData);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnLast_IfMultipleWithSameId()
        {
            // arrange
            const string itemStatText = "+15 % attack speed";
            StatData expectedStatData = new()
            {
                Id = "expected id",
                Text = $"{itemStatText}",
                Type = StatCategory.Explicit.GetDisplayName().ToLower(),
            };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = StatCategory.Explicit.GetDisplayName(),
                            Entries =
                            [
                                expectedStatData with { Id = "other id" },
                                expectedStatData,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            // act
            IStatData? result = this.statsDataService.TryGetStatData([itemStatText], true);

            // assert
            result.Should().Be(expectedStatData);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatDataByAlternativeText()
        {
            // arrange
            StatData expectedStatData = new()
            {
                Id = "expected id",
                Text = $"#% chance to Trigger Edict of Frost on Kill",
                Type = StatCategory.Explicit.GetDisplayName().ToLower(),
            };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = StatCategory.Explicit.GetDisplayName(),
                            Entries =
                            [
                                expectedStatData,
                            ],
                        },
                    ],
                });

            const string alternativeStatText = "Trigger Edict of Frost on Kill";
            this.alternativeStatTextsServiceMock
                .GetAlternativeStatTexts()
                .Returns(new List<StatTextsGroup>
                    {
                        new(expectedStatData.Id, [expectedStatData.Text, alternativeStatText]),
                    }.ToAsyncEnumerable());

            await this.statsDataService.OnInitAsync();

            // act
            IStatData? result = this.statsDataService.TryGetStatData([alternativeStatText], true);

            // assert
            result.Should().Be(expectedStatData);
        }

        [Test]
        public async Task TryGetStatData_ShouldReturnCorrectStatData_IfMultiline()
        {
            // arrange
            const string itemStatText = "Area is infested with Fungal Growths\nMap's Item Quantity Modifiers also affect Blight Chest count at 25% value\nCan be Anointed up to 3 times\nNatural inhabitants of this area have been removed";
            StatData expectedStatData = new()
            {
                Id = "expected id",
                Text = "Area is infested with Fungal Growths\nMap's Item Quantity Modifiers also affect Blight Chest count at 25% value\nCan be Anointed up to 3 times",
                Type = StatCategory.Implicit.GetDisplayName().ToLower(),
            };

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = StatCategory.Implicit.GetDisplayName(),
                            Entries =
                            [
                                expectedStatData,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            // act
            IStatData? result = this.statsDataService.TryGetStatData([itemStatText], true);

            // assert
            result.Should().Be(expectedStatData);
        }

        private async Task TryGetStatData_ShouldReturnCorrectStatData(ItemStat itemStat, StatData expectedStatData)
        {
            string statCategory = itemStat.StatCategory.GetDisplayName();

            this.poeTradeApiJsonSerializerMock.Deserialize<QueryResult<Data<StatData>>>(Arg.Any<string>())
                .Returns(new QueryResult<Data<StatData>>
                {
                    Result =
                    [
                        new()
                        {
                            Id = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit,
                            Entries =
                            [
                                new()
                                {
                                    Id = "random id", Text = expectedStatData.Text,
                                    Type = ItemSearch.Contract.Properties.Resources.StatCategoryExplicit.ToLower(),
                                },
                            ],
                        },
                        new()
                        {
                            Id = statCategory,
                            Entries =
                            [
                                expectedStatData,
                            ],
                        },
                    ],
                });

            await this.statsDataService.OnInitAsync();

            IStatData? result = this.statsDataService.TryGetStatData([itemStat.Text], false, itemStat.StatCategory.GetDisplayName());

            result.Should().Be(expectedStatData);
        }
    }
}