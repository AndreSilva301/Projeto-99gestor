using ManiaDeLimpeza.Application.Common;

namespace ManiaDeLimpeza.Application.UnitTests.Common
{
    [TestClass]
    public class StringUtilsTests
    {
        #region IsValidPassword Tests

        [TestMethod]
        public void IsValidPassword_WithValidPasswordContainingSpecialCharacters_ShouldReturnTrue()
        {
            // Arrange
            string password = "EseBLcd999bY@b2";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password 'EseBLcd999bY@b2' should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithMinimumRequirements_ShouldReturnTrue()
        {
            // Arrange - 8 characters, at least one letter and one digit
            string password = "Password1";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with minimum requirements should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithExactly8Characters_ShouldReturnTrue()
        {
            // Arrange
            string password = "Abcd1234";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with exactly 8 characters should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithLongPassword_ShouldReturnTrue()
        {
            // Arrange
            string password = "VeryLongPassword123456789";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Long password should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithUppercaseLettersAndDigits_ShouldReturnTrue()
        {
            // Arrange
            string password = "UPPERCASE123";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with uppercase letters and digits should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithLowercaseLettersAndDigits_ShouldReturnTrue()
        {
            // Arrange
            string password = "lowercase456";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with lowercase letters and digits should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithMixedCaseLettersAndDigits_ShouldReturnTrue()
        {
            // Arrange
            string password = "MixedCase789";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with mixed case letters and digits should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithNullString_ShouldReturnFalse()
        {
            // Arrange
            string? password = null;

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Null password should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithEmptyString_ShouldReturnFalse()
        {
            // Arrange
            string password = "";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Empty password should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithWhitespaceOnly_ShouldReturnFalse()
        {
            // Arrange
            string password = "   ";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Whitespace-only password should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithLessThan8Characters_ShouldReturnFalse()
        {
            // Arrange
            string password = "Short1";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with less than 8 characters should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_With7Characters_ShouldReturnFalse()
        {
            // Arrange
            string password = "Pass123";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with exactly 7 characters should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithOnlyLetters_ShouldReturnFalse()
        {
            // Arrange
            string password = "OnlyLetters";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with only letters should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithOnlyDigits_ShouldReturnFalse()
        {
            // Arrange
            string password = "12345678";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with only digits should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithSpecialCharactersOnly_ShouldReturnFalse()
        {
            // Arrange
            string password = "!@#$%^&*";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with only special characters should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithLettersAndSpecialCharactersButNoDigits_ShouldReturnFalse()
        {
            // Arrange
            string password = "Password!@#";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with letters and special characters but no digits should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithDigitsAndSpecialCharactersButNoLetters_ShouldReturnFalse()
        {
            // Arrange
            string password = "123456!@#";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with digits and special characters but no letters should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithSpacesInMiddle_ShouldReturnFalse()
        {
            // Arrange
            string password = "Pass word123";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with spaces should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithLeadingSpace_ShouldReturnFalse()
        {
            // Arrange
            string password = " Password1";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with leading space should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithTrailingSpace_ShouldReturnFalse()
        {
            // Arrange
            string password = "Password1 ";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with trailing space should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithTabCharacter_ShouldReturnFalse()
        {
            // Arrange
            string password = "Pass\tword1";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with tab character should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithNewlineCharacter_ShouldReturnFalse()
        {
            // Arrange
            string password = "Pass\nword1";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with newline character should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithUnicodeCharacters_ShouldReturnTrue()
        {
            // Arrange
            string password = "Pãsswörd123";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with unicode characters should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithEmoji_ShouldReturnFalse()
        {
            // Arrange
            string password = "Password1??";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsFalse(result, "Password with emoji should be invalid");
        }

        [TestMethod]
        public void IsValidPassword_WithSingleLetter_ShouldReturnFalse()
        {
            // Arrange
            string password = "A1234567";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with single letter and multiple digits should be valid");
        }

        [TestMethod]
        public void IsValidPassword_WithSingleDigit_ShouldReturnTrue()
        {
            // Arrange
            string password = "Abcdefg1";

            // Act
            bool result = password.IsValidPassword();

            // Assert
            Assert.IsTrue(result, "Password with multiple letters and single digit should be valid");
        }

        #endregion
    }
}
