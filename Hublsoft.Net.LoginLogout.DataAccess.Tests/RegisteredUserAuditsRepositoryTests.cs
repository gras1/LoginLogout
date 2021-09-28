using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Hublsoft.Net.LoginLogout.DataAccess.Tests
{
    [ExcludeFromCodeCoverage]
    public class RegisteredUserAuditsRepositoryTests
    {
        private readonly IRegisteredUserAuditsRepository _registeredUserAuditsRepository;
        private readonly Mock<IOptions<DatabaseOptions>> _databaseOptionsMock;

        public RegisteredUserAuditsRepositoryTests()
        {
            _databaseOptionsMock = new Mock<IOptions<DatabaseOptions>>();
            _registeredUserAuditsRepository = new RegisteredUserAuditsRepository(_databaseOptionsMock.Object);
        }

        [Fact]
        public void RegisteredUserAuditsRepository_IsAssignableFrom_IRegisteredUserAuditsRepository()
        {
            //Assert
            Assert.IsAssignableFrom<IRegisteredUserAuditsRepository>(_registeredUserAuditsRepository);
        }

        [Fact]
        public void Constructor_WhenIOptionsDatabaseOptionsIsNull_ThrowsArgumentNullException()
        {
            //Act/Assert
            Assert.Throws<ArgumentNullException>(() => new RegisteredUserAuditsRepository(null));
        }

        [Fact]
        public void AddFailedLoginAttemptAuditRecordAsync_WhenIdIsZero_ThrowsArgumentException()
        {
            //Act/Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _registeredUserAuditsRepository.AddFailedLoginAttemptAuditRecordAsync(0));
        }
    }
}
