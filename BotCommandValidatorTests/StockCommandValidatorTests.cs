using BotCommandValidator.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BotCommandValidator.Tests
{
    [TestClass()]
    public class StockCommandValidatorTests
    {
        [TestMethod()]
        public void ShouldReturnFalseForMessageWithoutStockCommand()
        {
            //Arrange
            string message = "This message doesn't have a stock command.";
            IStockCommandValidator validator = new StockCommandValidator();

            //Act
            bool result = validator.MessageHasStockCommands(message);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void ShouldReturnTrueFormMessageWithOneStockCommand()
        {
            //Arrange
            string message = "This message has a stock command. /stock=aapl.us.";
            IStockCommandValidator validator = new StockCommandValidator();

            //Act
            bool result = validator.MessageHasStockCommands(message);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void TheResponseShouldBeInvalidForMessageWithoutCommand()
        {
            //Arrange
            string message = "some message";
            IStockCommandValidator validator = new StockCommandValidator();

            //Act
            IValidationCommandResponse response = validator.ValidateCommand(message);

            //Assert
            Assert.IsFalse(response.IsValid);
        }

        [TestMethod()]
        public void ValidatorShouldReturnAMessageForMessageWithoutCommand()
        {
            //Arrange
            string message = "some message without command";
            IStockCommandValidator validator = new StockCommandValidator();

            //Act
            IValidationCommandResponse response = validator.ValidateCommand(message);

            //Assert
            Assert.IsFalse(response.IsValid);
            Assert.IsNotNull(response.ErrorMessage);
        }

        [TestMethod()]
        public void TheResponseShouldHaveOnlyOneCommandForMessageWithSameCommandMultipleTimes()
        {
            //Arrange
            string message = "/stock=o.us/stock=o.us/stock=o.us";
            IStockCommandValidator validator = new StockCommandValidator();

            //Act
            IValidationCommandResponse response = validator.ValidateCommand(message);

            //Assert
            Assert.IsTrue(response.IsValid);
            Assert.AreEqual(response.Commands.Count, 1);
        }

        [TestMethod()]
        public void ValidatorShouldExtractStockCommandsFromText()
        {
            //Arrange
            string message = "This is some text asking for stock like /stock=o.us or /stock=aapl.us or even like /stock=nnn.us.";
            IStockCommandValidator validator = new StockCommandValidator();
            HashSet<string> expected = new() { "/stock=o.us", "/stock=aapl.us", "/stock=nnn.us" };

            //Act
            IValidationCommandResponse response = validator.ValidateCommand(message);


            //Assert
            Assert.IsTrue(response.IsValid);
            CollectionAssert.AreEqual(expected.ToList(), response.Commands.ToList());
        }

        [TestMethod()]
        public void ValidatorShouldExtractStockCommandsFromTextWithoutRepetition()
        {
            //Arrange
            string message = "This is some text asking for stocks like /stock=o.us or /stock=aapl.us or even like /stock=nnn.us. Also for /stock=aapl.us again";
            IStockCommandValidator validator = new StockCommandValidator();
            HashSet<string> expected = new() { "/stock=o.us", "/stock=aapl.us", "/stock=nnn.us" };

            //Act
            IValidationCommandResponse response = validator.ValidateCommand(message);


            //Assert
            Assert.IsTrue(response.IsValid);
            CollectionAssert.AreEqual(expected.ToList(), response.Commands.ToList());
        }
    }
}