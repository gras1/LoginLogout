using System;
using FluentAssertions;
using Xunit;
using Hublsoft.Net.LoginLogout.DataAccess;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Hublsoft.Net.LoginLogout.Bll.Tests
{
    [ExcludeFromCodeCoverage]
    public class UserManagerTests
    {
        private readonly IUserManager _userManager;
        private readonly Mock<IRegisteredUsersRepository> _registeredUsersRepoMock;
        private readonly Mock<IRegisteredUserAuditsRepository> _registeredUserAuditsRepoMock;

        public UserManagerTests()
        {
            _registeredUsersRepoMock = new Mock<IRegisteredUsersRepository>();
            _registeredUserAuditsRepoMock = new Mock<IRegisteredUserAuditsRepository>();
            _userManager = new UserManager(_registeredUsersRepoMock.Object, _registeredUserAuditsRepoMock.Object);
        }

        [Fact]
        public void UserManager_IsAssignableFrom_IUserManager()
        {
            //Assert
            Assert.IsAssignableFrom<IUserManager>(_userManager);
        }

        [Fact]
        public void Constructor_WhenRegisteredUsersRepoIsNull_ThrowsArgumentNullException()
        {
            //Act/Assert
            Assert.Throws<ArgumentNullException>(() => new UserManager(null, _registeredUserAuditsRepoMock.Object));
        }

        [Fact]
        public void Constructor_WhenRegisteredUserAuditsRepoIsNull_ThrowsArgumentNullException()
        {
            //Act/Assert
            Assert.Throws<ArgumentNullException>(() => new UserManager(_registeredUsersRepoMock.Object, null));
        }

        [Fact]
        public void AuthenticateUserAsync_WhenEmailAddressIsNullOrEmpty_ThrowsArgumentException()
        {
            //Act/Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userManager.AuthenticateUserAsync("", "test"));
        }

        [Fact]
        public void AuthenticateUserAsync_WhenPasswordIsNullOrEmpty_ThrowsArgumentException()
        {
            //Act/Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _userManager.AuthenticateUserAsync("test", ""));
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenEmailAddressIsNotEmpty_CallsGetIdByEmailAddressAsyncOnce()
        {
            //Act
            _ = await _userManager.AuthenticateUserAsync("test", "test");

            //Assert
            _registeredUsersRepoMock.Verify(x => x.GetIdByEmailAddressAsync("test"), Times.Once());
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenGetIdByEmailAddressAsyncReturnsZero_ResultIsEmptyGuid()
        {
            //Arrange
            var expectedResult = Guid.Empty;
            _registeredUsersRepoMock.Setup(m => m.GetIdByEmailAddressAsync("test")).Returns(Task.FromResult(0));

            //Act
            var actualResult = await _userManager.AuthenticateUserAsync("test", "test");

            //Assert
            actualResult.Should().Be(expectedResult);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenGetIdByEmailAddressAsyncReturnsOne_CallsGetUserAccountDetailsAsyncOnce()
        {
            //Arrange
            _registeredUsersRepoMock.Setup(m => m.GetIdByEmailAddressAsync("test")).Returns(Task.FromResult(1));

            //Act
            _ = await _userManager.AuthenticateUserAsync("test", "test");
            
            //Assert
            _registeredUsersRepoMock.Verify(x => x.GetUserAccountDetailsAsync(1, "test"), Times.Once());
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenGetUserAccountDetailsReturnsNull_CallsIncrementFailedLoginAttemptsAsyncOnce()
        {
            //Arrange
            _registeredUsersRepoMock.Setup(m => m.GetIdByEmailAddressAsync("test")).Returns(Task.FromResult(1));
            _registeredUsersRepoMock.Setup(m => m.GetUserAccountDetailsAsync(1, "test")).Returns(Task.FromResult(default(UserAccountDetails)));

            //Act
            _ = await _userManager.AuthenticateUserAsync("test", "test");
            
            //Assert
            _registeredUsersRepoMock.Verify(x => x.IncrementFailedLoginAttemptsAsync(1), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenGetUserAccountDetailsReturnsNull_CallsAddFailedLoginAttemptAuditRecordAsyncOnce()
        {
            //Arrange
            _registeredUsersRepoMock.Setup(m => m.GetIdByEmailAddressAsync("test")).Returns(Task.FromResult(1));
            _registeredUsersRepoMock.Setup(m => m.GetUserAccountDetailsAsync(1, "test")).Returns(Task.FromResult(default(UserAccountDetails)));

            //Act
            _ = await _userManager.AuthenticateUserAsync("test", "test");
            
            //Assert
            _registeredUserAuditsRepoMock.Verify(x => x.AddFailedLoginAttemptAuditRecordAsync(1), Times.Once);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenGetUserAccountDetailsReturnsNull_ReturnsEmptyGuid()
        {
            //Arrange
            var expectedResult = Guid.Empty;
            _registeredUsersRepoMock.Setup(m => m.GetIdByEmailAddressAsync("test")).Returns(Task.FromResult(1));
            _registeredUsersRepoMock.Setup(m => m.GetUserAccountDetailsAsync(1, "test")).Returns(Task.FromResult(default(UserAccountDetails)));

            //Act
            var actualResult = await _userManager.AuthenticateUserAsync("test", "test");
            
            //Assert
            actualResult.Should().Be(expectedResult);
        }

        [Fact]
        public async Task AuthenticateUserAsync_WhenEmailAddressAndPasswordAreCorrect_ReturnsGuid()
        {
            //Arrange
            var userAccountDetails = new UserAccountDetails{
                PublicId = Guid.NewGuid()
            };
            var expectedResult = userAccountDetails.PublicId;
            _registeredUsersRepoMock.Setup(m => m.GetIdByEmailAddressAsync("test")).Returns(Task.FromResult(1));
            _registeredUsersRepoMock.Setup(m => m.GetUserAccountDetailsAsync(1, "test")).Returns(Task.FromResult(userAccountDetails));

            //Act
            var actualResult = await _userManager.AuthenticateUserAsync("test", "test");
            
            //Assert
            actualResult.Should().Be(expectedResult);
        }
    }
}
