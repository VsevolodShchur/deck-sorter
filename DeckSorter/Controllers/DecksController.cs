using System.Linq;
using DeckSorter.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeckSorter.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly IDeckStorage deckStorage;
        private readonly IShuffler shuffler;

        public DecksController(IDeckStorage deckStorage, IShuffler shuffler)
        {
            this.deckStorage = deckStorage;
            this.shuffler = shuffler;
        }

        [HttpGet("names")]
        public IActionResult GetDeckNames()
        {
            return Ok(new {names = deckStorage.GetAll().Select(deck => deck.Name)});
        }

        [HttpGet("{name}")]
        public IActionResult GetDeckByName(string name)
        {
            var deck = deckStorage.Get(name);
            return deck != null
                ? (IActionResult) Ok(deck)
                : BadRequest(DeckErrorMessages.NotFound(name));
        }

        [HttpPost]
        public IActionResult CreateDeck([FromBody] CreateArguments args)
        {
            var name = args.Name;
            if (name == "names")
                return BadRequest(DeckErrorMessages.ForbiddenName(name));

            if (deckStorage.Get(name) != null)
                return BadRequest(DeckErrorMessages.AlreadyExists(name));

            var deck = Deck.Create52Deck(name);
            var success = deckStorage.InsertOrUpdate(deck);
            return success
                ? (IActionResult) Ok(deck)
                : StatusCode(500);
        }

        [HttpDelete("{name}")]
        public IActionResult DeleteDeck(string name)
        {
            if (name == "names")
                return BadRequest(DeckErrorMessages.ForbiddenName(name));

            var success = deckStorage.Delete(name);
            return success
                ? (IActionResult) Ok()
                : BadRequest(DeckErrorMessages.NotFound(name));
        }

        [HttpPost("{name}/shuffle")]
        public IActionResult ShuffleDeck(string name)
        {
            if (name == "names")
                return BadRequest(DeckErrorMessages.ForbiddenName(name));

            var deck = deckStorage.Get(name);
            if (deck == null)
                return BadRequest(DeckErrorMessages.NotFound(name));

            deck = new Deck(deck.Name, shuffler.Shuffle(deck.Cards));
            var success = deckStorage.InsertOrUpdate(deck);
            return success
                ? (IActionResult) Ok(deck)
                : StatusCode(500);
        }
    }

    public class CreateArguments
    {
        public string Name { get; set; }
    }

    public static class DeckErrorMessages
    {
        public static string NotFound(string name)
            => $"Колоды с именем {name} не найдено";

        public static string AlreadyExists(string name)
            => $"Колода с именем {name} уже существует";

        public static string ForbiddenName(string name)
            => $"Колоду с именем {name} нельзя создать";
    }
}