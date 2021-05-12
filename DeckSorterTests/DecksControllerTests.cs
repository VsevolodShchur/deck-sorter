using System;
using System.Collections.Generic;
using DeckSorter.Controllers;
using DeckSorter.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace DeckSorterTests
{
    [TestFixture]
    public class DecksControllerTests
    {
        private DecksController controller;
        private Mock<IDeckStorage> deckStorageMock;
        private Mock<IShuffler> shufflerMock;
        private const string anyDeckName = "AnyDeckName";
        private const string forbiddenDeckName = "names";

        [SetUp]
        public void SetUp()
        {
            
            deckStorageMock = new Mock<IDeckStorage>();
            shufflerMock = new Mock<IShuffler>();
            controller = new DecksController(deckStorageMock.Object, shufflerMock.Object);
        }
        
        private static IEnumerable<TestCaseData> CreateCases
        {
           get
           {
               yield return new TestCaseData(anyDeckName, typeof(OkObjectResult),
                   true, true, false).SetName("ReturnsOk_WhenNothingWrong");
               yield return new TestCaseData(anyDeckName, typeof(BadRequestObjectResult),
                   false, true, true).SetName("ReturnsBadRequest_WhenStorageAlreadyContainsDeck");
               yield return new TestCaseData(forbiddenDeckName, typeof(BadRequestObjectResult),
                   false, true, false).SetName("ReturnsBadRequest_WhenDeckHasForbiddenName");
           }
        }
        
        [TestCaseSource(nameof(CreateCases))]
        public void ControllerCreatesDeck(string deckName, Type expectedResultType, bool insertMethodCalled,
                                          bool insertMethodReturnValue, bool storageAlreadyContainsDeck)
        {
            deckStorageMock.Setup(storage =>
                    storage.InsertOrUpdate(It.Is<Deck>(deck => deck.Name == deckName)))
                .Returns(insertMethodReturnValue);
            deckStorageMock.Setup(storage =>
                    storage.Get(deckName))
                .Returns(storageAlreadyContainsDeck ? new Deck(deckName, new List<Card>()) : null);

            var result = controller.CreateDeck(new CreateArguments {Name = deckName});
            
            result.Should().BeOfType(expectedResultType);
            deckStorageMock.Verify(storage => 
                storage.InsertOrUpdate(It.Is<Deck>(deck => deck.Name == deckName)),
                insertMethodCalled ? Times.Once() : Times.Never());
        }

        [Test]
        public void ControllerGetsDeck()
        {
            var namedDeck = new Deck(anyDeckName, new List<Card>());
            deckStorageMock.Setup(storage =>
                storage.Get(anyDeckName)).Returns(namedDeck);

            var result = controller.GetDeckByName(anyDeckName);

            result.Should().BeEquivalentTo(new OkObjectResult(namedDeck));
            deckStorageMock.Verify(storage => storage.Get(anyDeckName));
        }
        
        [Test]
        public void ControllerGetsAllDeckNames()
        {
            var deckList = new List<Deck>
            {
                new Deck("deck1", new List<Card>()),
                new Deck("deck2", new List<Card>()),
            };
            deckStorageMock.Setup(storage =>
                storage.GetAll()).Returns(deckList);
            
            var result = controller.GetDeckNames();

            result.Should().BeEquivalentTo(new OkObjectResult(
                    new { names = new List<string> {"deck1", "deck2"}}
                ));
        }
        
        private static IEnumerable<TestCaseData> DeleteCases
        {
            get
            {
                yield return new TestCaseData(anyDeckName, typeof(OkResult), true)
                    .SetName("ReturnsOk_WhenNothingWrong");
                yield return new TestCaseData(anyDeckName, typeof(BadRequestObjectResult), false)
                    .SetName("ReturnsBadRequest_WhenDeckNotInStorage");
                yield return new TestCaseData(forbiddenDeckName, typeof(BadRequestObjectResult), false)
                    .SetName("ReturnsBadRequest_WhenDeckHasForbiddenName");
            }
        }
        
        [TestCaseSource(nameof(DeleteCases))]
        public void ControllerDeletesDeck(string deckName, Type expectedResultType, bool storageAlreadyContainsDeck)
        {
            deckStorageMock.Setup(storage => storage.Delete(deckName))
                .Returns(storageAlreadyContainsDeck);
            
            var result = controller.DeleteDeck(deckName);
            
            result.Should().BeOfType(expectedResultType);
        }

        private static IEnumerable<TestCaseData> ShuffleCases
        {
            get
            {
                yield return new TestCaseData(anyDeckName, typeof(OkObjectResult), true, true)
                    .SetName("ReturnsOk_WhenNothingWrong");
                yield return new TestCaseData(anyDeckName, typeof(BadRequestObjectResult), false, false)
                    .SetName("ReturnsBadRequest_WhenDeckNotInStorage");
                yield return new TestCaseData(forbiddenDeckName, typeof(BadRequestObjectResult), false, false)
                    .SetName("ReturnsBadRequest_WhenDeckHasForbiddenName");
            }
        }
        
        [TestCaseSource(nameof(ShuffleCases))]
        public void ControllerShufflesDeck(string deckName, Type expectedResultType, bool storageAlreadyContainsDeck,
            bool isShuffleMethodInvoked)
        {
            deckStorageMock.Setup(storage => storage.Get(deckName))
                .Returns(storageAlreadyContainsDeck ? new Deck(deckName, new List<Card>()) : null);
            deckStorageMock.Setup(storage =>
                    storage.InsertOrUpdate(It.Is<Deck>(deck => deck.Name == deckName)))
                .Returns(true);
            
            var result = controller.ShuffleDeck(deckName);
            
            result.Should().BeOfType(expectedResultType);
            shufflerMock.Verify(shuffler => shuffler.Shuffle(It.IsAny<IEnumerable<Card>>()),
                isShuffleMethodInvoked ? Times.AtLeastOnce() : Times.Never());
        }
    }
}