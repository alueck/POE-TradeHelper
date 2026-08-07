using AwesomeAssertions;

using NUnit.Framework;

using POETradeHelper.RePoE.Services;

using RichardSzalay.MockHttp;

namespace POETradeHelper.RePoE.Tests.Services;

public class AlternativeStatTextsServiceTests
{
    [Test]
    public async Task GetAlternativeStatTexts_ReturnsExpectedData()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://repoe-fork.github.io/stat_translations.min.json")
            .Respond("application/json", GetPositiveTestCaseResponseJson());

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://repoe-fork.github.io/");
        AlternativeStatTextsService sut = new(httpClient);

        // Act
        var result = await sut.GetAlternativeStatTexts().ToArrayAsync();

        // Assert
        result.Should().SatisfyRespectively(
            x => x.Id.Should().Be("explicit.stat_4164174520"),
            x => x.Id.Should().Be("fractured.stat_4164174520")
        );
        result.Should().AllSatisfy(x => x.StatTexts.Should().BeEquivalentTo([
            "Monsters have #% chance to Maim on Hit with Attacks",
            "Monsters Maim on Hit with Attacks",
        ]));
    }

    [Test]
    public async Task GetAlternativeStatText_DoesNotReturnItemWithDifferentTradeStatIds()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://repoe-fork.github.io/stat_translations.min.json")
            .Respond("application/json", GetNegativeTestCaseResponseJson());

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://repoe-fork.github.io/");
        AlternativeStatTextsService sut = new(httpClient);

        // Act
        var result = await sut.GetAlternativeStatTexts().ToArrayAsync();

        // Assert
        result.Should().BeEmpty();
    }

    private static string GetPositiveTestCaseResponseJson()
    {
        return """
               [
                   {
                     "English": [
                       {
                         "condition": [
                           {
                             "min": 1
                           }
                         ],
                         "format": [
                           "+#"
                         ],
                         "index_handlers": [
                           []
                         ],
                         "string": "You and your Minions prevent {0}% of Reflected Damage"
                       }
                     ],
                     "ids": [
                       "%_of_your_and_your_minions_damage_cannot_be_reflected"
                     ],
                     "trade_stats": [
                       {
                         "id": "explicit.stat_1567747544",
                         "text": "You and your Minions prevent +#% of Reflected Damage",
                         "type": "explicit"
                       },
                       {
                         "id": "fractured.stat_1567747544",
                         "text": "You and your Minions prevent +#% of Reflected Damage",
                         "type": "fractured"
                       }
                     ]
                   },
                   {
                     "English": [
                       {
                         "condition": [
                           {
                             "min": 1,
                             "max": 99,
                             "negated": null
                           }
                         ],
                         "format": [
                           "#"
                         ],
                         "index_handlers": [
                           []
                         ],
                         "string": "Monsters have {0}% chance to Maim on Hit with Attacks",
                         "reminder_text": "(Maimed enemies have 30% reduced Movement Speed)",
                         "is_markup": null
                       },
                       {
                         "condition": [
                           {
                             "min": 100,
                             "max": null,
                             "negated": null
                           }
                         ],
                         "format": [
                           "ignore"
                         ],
                         "index_handlers": [
                           []
                         ],
                         "string": "Monsters Maim on Hit with Attacks",
                         "reminder_text": "(Maimed enemies have 30% reduced Movement Speed)",
                         "is_markup": null
                       }
                     ],
                     "ids": [
                       "map_monsters_maim_on_hit_%_chance"
                     ],
                     "trade_stats": [
                       {
                         "id": "explicit.stat_4164174520",
                         "text": "Monsters have #% chance to Maim on Hit with Attacks",
                         "type": "explicit",
                         "option": null
                       },
                       {
                         "id": "fractured.stat_4164174520",
                         "text": "Monsters have #% chance to Maim on Hit with Attacks",
                         "type": "fractured",
                         "option": null
                       }
                     ],
                     "hidden": null,
                     "French": null,
                     "German": null,
                     "Japanese": null,
                     "Korean": null,
                     "Portuguese": null,
                     "Russian": null,
                     "Spanish": null,
                     "Thai": null,
                     "Traditional Chinese": null
                   }
               ]
               """;
    }

    private static string GetNegativeTestCaseResponseJson()
    {
        return """
               [
                   {
                     "English": [
                       {
                         "condition": [
                           {
                             "min": 0,
                             "max": 0,
                             "negated": true
                           },
                           {
                             "min": 0,
                             "max": 0,
                             "negated": null
                           }
                         ],
                         "format": [
                           "ignore",
                           "ignore"
                         ],
                         "index_handlers": [
                           [],
                           []
                         ],
                         "string": "Passive Skills in Radius can be Allocated without being connected to your tree\nPassage",
                         "reminder_text": null,
                         "is_markup": null
                       },
                       {
                         "condition": [
                           {
                             "min": 0,
                             "max": 0,
                             "negated": null
                           },
                           {
                             "min": 0,
                             "max": 0,
                             "negated": true
                           }
                         ],
                         "format": [
                           "ignore",
                           "+#"
                         ],
                         "index_handlers": [
                           [],
                           []
                         ],
                         "string": "{1}% to all Elemental Resistances",
                         "reminder_text": null,
                         "is_markup": null
                       },
                       {
                         "condition": [
                           {
                             "min": 0,
                             "max": 0,
                             "negated": true
                           },
                           {
                             "min": 0,
                             "max": 0,
                             "negated": true
                           }
                         ],
                         "format": [
                           "ignore",
                           "+#"
                         ],
                         "index_handlers": [
                           [],
                           []
                         ],
                         "string": "Passive Skills in Radius can be Allocated without being connected to your tree\n{1}% to all Elemental Resistances\nPassage",
                         "reminder_text": null,
                         "is_markup": null
                       }
                     ],
                     "ids": [
                       "local_unique_jewel_nearby_disconnected_passives_can_be_allocated",
                       "unique_thread_of_hope_base_resist_all_elements_%"
                     ],
                     "trade_stats": [
                       {
                         "id": "explicit.stat_1725885727",
                         "text": "Passive Skills in Radius can be Allocated without being connected to your tree\nPassage",
                         "type": "explicit",
                         "option": null
                       },
                       {
                         "id": "crafted.stat_2901986750",
                         "text": "+#% to all Elemental Resistances",
                         "type": "crafted",
                         "option": null
                       },
                       {
                         "id": "explicit.stat_2901986750",
                         "text": "+#% to all Elemental Resistances",
                         "type": "explicit",
                         "option": null
                       },
                       {
                         "id": "fractured.stat_2901986750",
                         "text": "+#% to all Elemental Resistances",
                         "type": "fractured",
                         "option": null
                       },
                       {
                         "id": "implicit.stat_2901986750",
                         "text": "+#% to all Elemental Resistances",
                         "type": "implicit",
                         "option": null
                       },
                       {
                         "id": "scourge.stat_2901986750",
                         "text": "+#% to all Elemental Resistances",
                         "type": "scourge",
                         "option": null
                       }
                     ],
                     "hidden": null,
                     "French": null,
                     "German": null,
                     "Japanese": null,
                     "Korean": null,
                     "Portuguese": null,
                     "Russian": null,
                     "Spanish": null,
                     "Thai": null,
                     "Traditional Chinese": null
                   }
               ]
               """;
    }
}