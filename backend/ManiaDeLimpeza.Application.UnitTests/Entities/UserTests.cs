using ManiaDeLimpeza.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ManiaDeLimpeza.Application.UnitTests.Entities
{
    [TestClass]
    public class UserTests
    {
        #region IsSystemAdmin Tests

        [TestMethod]
        public void IsSystemAdmin_ShouldReturnTrue_WhenUserIsSystemAdmin()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "System Admin",
                Email = "sysadmin@test.com",
                CompanyId = 1,
                Profile = UserProfile.SystemAdmin
            };

            // Act
            var result = user.IsSystemAdmin();

            // Assert
            Assert.IsTrue(result, "IsSystemAdmin() should return true for SystemAdmin profile");
        }

        [TestMethod]
        public void IsSystemAdmin_ShouldReturnFalse_WhenUserIsAdmin()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 1,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsSystemAdmin();

            // Assert
            Assert.IsFalse(result, "IsSystemAdmin() should return false for Admin profile");
        }

        [TestMethod]
        public void IsSystemAdmin_ShouldReturnFalse_WhenUserIsEmployee()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Employee",
                Email = "employee@test.com",
                CompanyId = 1,
                Profile = UserProfile.Employee
            };

            // Act
            var result = user.IsSystemAdmin();

            // Assert
            Assert.IsFalse(result, "IsSystemAdmin() should return false for Employee profile");
        }

        [TestMethod]
        public void IsSystemAdmin_ShouldReturnFalse_WhenUserIsInactive()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Inactive User",
                Email = "inactive@test.com",
                CompanyId = 1,
                Profile = UserProfile.Inactive
            };

            // Act
            var result = user.IsSystemAdmin();

            // Assert
            Assert.IsFalse(result, "IsSystemAdmin() should return false for Inactive profile");
        }

        #endregion

        #region IsCompanyAdmin Tests

        [TestMethod]
        public void IsCompanyAdmin_ShouldReturnTrue_WhenUserIsAdminOfSpecifiedCompany()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsCompanyAdmin(10);

            // Assert
            Assert.IsTrue(result, "IsCompanyAdmin() should return true when user is Admin of the specified company");
        }

        [TestMethod]
        public void IsCompanyAdmin_ShouldReturnFalse_WhenUserIsAdminOfDifferentCompany()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsCompanyAdmin(20);

            // Assert
            Assert.IsFalse(result, "IsCompanyAdmin() should return false when user is Admin of a different company");
        }

        [TestMethod]
        public void IsCompanyAdmin_ShouldReturnFalse_WhenUserIsSystemAdmin()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "System Admin",
                Email = "sysadmin@test.com",
                CompanyId = 10,
                Profile = UserProfile.SystemAdmin
            };

            // Act
            var result = user.IsCompanyAdmin(10);

            // Assert
            Assert.IsFalse(result, "IsCompanyAdmin() should return false for SystemAdmin profile");
        }

        [TestMethod]
        public void IsCompanyAdmin_ShouldReturnFalse_WhenUserIsEmployee()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Employee",
                Email = "employee@test.com",
                CompanyId = 10,
                Profile = UserProfile.Employee
            };

            // Act
            var result = user.IsCompanyAdmin(10);

            // Assert
            Assert.IsFalse(result, "IsCompanyAdmin() should return false for Employee profile");
        }

        [TestMethod]
        public void IsCompanyAdmin_ShouldReturnFalse_WhenUserIsInactive()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Inactive",
                Email = "inactive@test.com",
                CompanyId = 10,
                Profile = UserProfile.Inactive
            };

            // Act
            var result = user.IsCompanyAdmin(10);

            // Assert
            Assert.IsFalse(result, "IsCompanyAdmin() should return false for Inactive profile");
        }

        #endregion

        #region IsAdminOrSysAdmin Tests

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnTrue_WhenUserIsSystemAdmin_WithAnyCompanyId()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "System Admin",
                Email = "sysadmin@test.com",
                CompanyId = 10,
                Profile = UserProfile.SystemAdmin
            };

            // Act & Assert
            Assert.IsTrue(user.IsAdminOrSysAdmin(10), "SystemAdmin should return true for their own companyId");
            Assert.IsTrue(user.IsAdminOrSysAdmin(20), "SystemAdmin should return true for any companyId");
            Assert.IsTrue(user.IsAdminOrSysAdmin(999), "SystemAdmin should return true for any companyId");
        }

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnTrue_WhenUserIsAdminOfSameCompany()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsAdminOrSysAdmin(10);

            // Assert
            Assert.IsTrue(result, "IsAdminOrSysAdmin() should return true when user is Admin of the specified company");
        }

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnFalse_WhenUserIsAdminOfDifferentCompany()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsAdminOrSysAdmin(20);

            // Assert
            Assert.IsFalse(result, "IsAdminOrSysAdmin() should return false when user is Admin of a different company");
        }

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnFalse_WhenUserIsEmployee()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Employee",
                Email = "employee@test.com",
                CompanyId = 10,
                Profile = UserProfile.Employee
            };

            // Act
            var result = user.IsAdminOrSysAdmin(10);

            // Assert
            Assert.IsFalse(result, "IsAdminOrSysAdmin() should return false for Employee profile");
        }

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnFalse_WhenUserIsInactive()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Inactive",
                Email = "inactive@test.com",
                CompanyId = 10,
                Profile = UserProfile.Inactive
            };

            // Act
            var result = user.IsAdminOrSysAdmin(10);

            // Assert
            Assert.IsFalse(result, "IsAdminOrSysAdmin() should return false for Inactive profile");
        }

        #endregion

        #region Edge Cases and Multiple Profile Scenarios

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldHandleEdgeCase_CompanyIdZero()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 0,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsAdminOrSysAdmin(0);

            // Assert
            Assert.IsTrue(result, "IsAdminOrSysAdmin() should return true when both user and parameter have companyId = 0");
        }

        [TestMethod]
        public void IsCompanyAdmin_ShouldHandleEdgeCase_CompanyIdZero()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 0,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsCompanyAdmin(0);

            // Assert
            Assert.IsTrue(result, "IsCompanyAdmin() should return true when both user and parameter have companyId = 0");
        }

        [TestMethod]
        public void AllMethods_ShouldWorkWithNewlyCreatedUser()
        {
            // Arrange
            var user = new User
            {
                Name = "New User",
                Email = "newuser@test.com",
                CompanyId = 5,
                Profile = UserProfile.Employee
            };

            // Act & Assert
            Assert.IsFalse(user.IsSystemAdmin(), "New employee should not be SystemAdmin");
            Assert.IsFalse(user.IsCompanyAdmin(5), "New employee should not be CompanyAdmin");
            Assert.IsFalse(user.IsAdminOrSysAdmin(5), "New employee should not be AdminOrSysAdmin");
        }

        [TestMethod]
        public void AllMethods_ShouldWorkWithSystemAdmin_AcrossDifferentCompanies()
        {
            // Arrange
            var systemAdmin = new User
            {
                Id = 1,
                Name = "System Admin",
                Email = "sysadmin@test.com",
                CompanyId = 1,
                Profile = UserProfile.SystemAdmin
            };

            // Act & Assert
            Assert.IsTrue(systemAdmin.IsSystemAdmin(), "Should be SystemAdmin");
            Assert.IsFalse(systemAdmin.IsCompanyAdmin(1), "SystemAdmin is not CompanyAdmin");
            Assert.IsFalse(systemAdmin.IsCompanyAdmin(2), "SystemAdmin is not CompanyAdmin for any company");
            
            Assert.IsTrue(systemAdmin.IsAdminOrSysAdmin(1), "Should be AdminOrSysAdmin for own company");
            Assert.IsTrue(systemAdmin.IsAdminOrSysAdmin(2), "Should be AdminOrSysAdmin for any company");
            Assert.IsTrue(systemAdmin.IsAdminOrSysAdmin(999), "Should be AdminOrSysAdmin for any companyId");
        }

        [TestMethod]
        public void AllMethods_ShouldWorkWithAdmin_OnlyForOwnCompany()
        {
            // Arrange
            var admin = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            // Act & Assert
            Assert.IsFalse(admin.IsSystemAdmin(), "Admin should not be SystemAdmin");
            
            Assert.IsTrue(admin.IsCompanyAdmin(10), "Should be CompanyAdmin for own company");
            Assert.IsFalse(admin.IsCompanyAdmin(20), "Should not be CompanyAdmin for other company");
            
            Assert.IsTrue(admin.IsAdminOrSysAdmin(10), "Should be AdminOrSysAdmin for own company");
            Assert.IsFalse(admin.IsAdminOrSysAdmin(20), "Should not be AdminOrSysAdmin for other company");
        }

        #endregion

        #region Property Initialization Tests

        [TestMethod]
        public void User_ShouldInitializeWithDefaultValues()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            Assert.AreEqual(0, user.Id, "Id should default to 0");
            Assert.AreEqual(string.Empty, user.Name, "Name should default to empty string");
            Assert.AreEqual(string.Empty, user.Email, "Email should default to empty string");
            Assert.AreEqual(string.Empty, user.PasswordHash, "PasswordHash should default to empty string");
            Assert.AreEqual(0, user.CompanyId, "CompanyId should default to 0");
            Assert.AreEqual(default(UserProfile), user.Profile, "Profile should default to default enum value");
            
            // CreatedDate should be close to UtcNow
            var timeDifference = Math.Abs((DateTime.UtcNow - user.CreatedDate).TotalSeconds);
            Assert.IsTrue(timeDifference < 1, "CreatedDate should be initialized to UtcNow");
        }

        [TestMethod]
        public void User_ShouldAllowSettingAllProperties()
        {
            // Arrange
            var testDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var company = new Company { Id = 5, Name = "Test Company" };

            // Act
            var user = new User
            {
                Id = 100,
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "hashed_password_123",
                CompanyId = 5,
                Company = company,
                Profile = UserProfile.Admin,
                CreatedDate = testDate
            };

            // Assert
            Assert.AreEqual(100, user.Id);
            Assert.AreEqual("Test User", user.Name);
            Assert.AreEqual("test@example.com", user.Email);
            Assert.AreEqual("hashed_password_123", user.PasswordHash);
            Assert.AreEqual(5, user.CompanyId);
            Assert.AreEqual(company, user.Company);
            Assert.AreEqual(UserProfile.Admin, user.Profile);
            Assert.AreEqual(testDate, user.CreatedDate);
        }

        #endregion

        #region Negative Tests

        [TestMethod]
        public void IsCompanyAdmin_ShouldReturnFalse_WithNegativeCompanyId()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsCompanyAdmin(-1);

            // Assert
            Assert.IsFalse(result, "IsCompanyAdmin() should return false with negative companyId");
        }

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnFalse_WhenAdminWithNegativeCompanyId()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            // Act
            var result = user.IsAdminOrSysAdmin(-1);

            // Assert
            Assert.IsFalse(result, "IsAdminOrSysAdmin() should return false when companyId doesn't match");
        }

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnTrue_ForSystemAdmin_EvenWithNegativeCompanyId()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "System Admin",
                Email = "sysadmin@test.com",
                CompanyId = 10,
                Profile = UserProfile.SystemAdmin
            };

            // Act
            var result = user.IsAdminOrSysAdmin(-1);

            // Assert
            Assert.IsTrue(result, "IsAdminOrSysAdmin() should return true for SystemAdmin regardless of companyId");
        }

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReturnFalse_WhenAdminWithDifferentCompanyId()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Admin",
                Email = "admin@test.com",
                CompanyId = 5,
                Profile = UserProfile.Admin
            };

            // Act & Assert
            Assert.IsTrue(user.IsAdminOrSysAdmin(5), "Should return true for matching companyId");
            Assert.IsFalse(user.IsAdminOrSysAdmin(10), "Should return false for different companyId");
            Assert.IsFalse(user.IsAdminOrSysAdmin(0), "Should return false for companyId 0 when user's companyId is 5");
        }

        #endregion

        #region Profile Transition Tests

        [TestMethod]
        public void IsAdminOrSysAdmin_ShouldReflectProfileChanges()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "User",
                Email = "user@test.com",
                CompanyId = 10,
                Profile = UserProfile.Employee
            };

            // Act & Assert - Employee
            Assert.IsFalse(user.IsAdminOrSysAdmin(10), "Employee should not be admin");

            // Change to Admin
            user.Profile = UserProfile.Admin;
            Assert.IsTrue(user.IsAdminOrSysAdmin(10), "Admin should be admin for their company");
            Assert.IsFalse(user.IsAdminOrSysAdmin(20), "Admin should not be admin for other companies");

            // Change to SystemAdmin
            user.Profile = UserProfile.SystemAdmin;
            Assert.IsTrue(user.IsAdminOrSysAdmin(10), "SystemAdmin should be admin for any company");
            Assert.IsTrue(user.IsAdminOrSysAdmin(20), "SystemAdmin should be admin for any company");

            // Change to Inactive
            user.Profile = UserProfile.Inactive;
            Assert.IsFalse(user.IsAdminOrSysAdmin(10), "Inactive user should not be admin");
        }

        #endregion
    }
}
