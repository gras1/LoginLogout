using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Hublsoft.Net.LoginLogout.DataAccess.Tests
{
    [ExcludeFromCodeCoverage]
    public class RegisteredUsersRepositoryTests
    {
        private readonly IRegisteredUsersRepository _registeredUsersRepository;
        private readonly Mock<IOptions<DatabaseOptions>> _databaseOptionsMock;

        public RegisteredUsersRepositoryTests()
        {
            _databaseOptionsMock = new Mock<IOptions<DatabaseOptions>>();
            _registeredUsersRepository = new RegisteredUsersRepository(_databaseOptionsMock.Object);
        }

        [Fact]
        public void RegisteredUsersRepository_IsAssignableFrom_IRegisteredUsersRepository()
        {
            //Assert
            Assert.IsAssignableFrom<IRegisteredUsersRepository>(_registeredUsersRepository);
        }

        [Fact]
        public void Constructor_WhenIOptionsDatabaseOptionsIsNull_ThrowsArgumentNullException()
        {
            //Act/Assert
            Assert.Throws<ArgumentNullException>(() => new RegisteredUserAuditsRepository(null));
        }

        [Fact]
        public void GetIdByEmailAddressAsync_WhenEmailAddressIsNullOrEmpty_ThrowsArgumentException()
        {
            //Act/Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _registeredUsersRepository.GetIdByEmailAddressAsync(""));
        }

        [Fact]
        public void GetUserAccountDetailsAsync_WhenIdIsZero_ThrowsArgumentException()
        {
            //Act/Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _registeredUsersRepository.GetUserAccountDetailsAsync(0, "test"));
        }

        [Fact]
        public void GetUserAccountDetailsAsync_WhenPasswordIsNullOrEmpty_ThrowsArgumentException()
        {
            //Act/Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _registeredUsersRepository.GetUserAccountDetailsAsync(1, ""));
        }

        [Fact]
        public void IncrementFailedLoginAttemptsAsync_WhenIdIsZero_ThrowsArgumentException()
        {
            //Act/Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _registeredUsersRepository.IncrementFailedLoginAttemptsAsync(0));
        }
    }
}
