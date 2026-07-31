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
            .Respond("application/json", GetResponseJson());

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

    private static string GetResponseJson()
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
}