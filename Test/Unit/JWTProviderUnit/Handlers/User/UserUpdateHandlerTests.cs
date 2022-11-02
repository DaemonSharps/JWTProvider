using DeepEqual.Syntax;
using JWTProvider.Common.Exceptions;
using JWTProvider.User.Commands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using DB = Infrastructure.DataBase.Entities;

namespace Handlers.User
{
    public class UserUpdateHandlerTests
    {
        [Fact]
        public async void NewUser_Throw()
        {
            //Arrange
            const string ExpectedErrorCode = "USER_NOT_FOUND";
            const string ExpectedErrorMessage = "User not found";
            var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
            var handler = new UserUpdateHandler(TestDBContext.CreateInMemoryContext(), null);
            var command = new UserUpdateCommand
            {
                Email = "tes@mail.ru"
            };
            //Act
            var action = handler.Handle(command, default);
            //Assert
            var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => action);
            HttpExceptionAssert.IsValidHttpException(exception, expectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
        }

        [Fact]
        public async void DBUpdateError_Throw()
        {
            //Arrange
            const string ExpectedErrorCode = "USER_UPDATE_FAILED";
            const string ExpectedErrorMessage = "DB error";
            const string DBUpdateExceptionMessage = "db error test";
            var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
            const string TestEmail = "test@mail.ru";
            var loggerMock = new Mock<ILogger<UserUpdateHandler>>();

            var dbContext = TestDBContext.CreateContextMock();
            var innerException = new DbUpdateException(DBUpdateExceptionMessage);
            dbContext.Setup(c => c.SaveChangesAsync(default))
                .Throws(innerException);
            dbContext.Setup(c => c.Users).ReturnsDbSet(new[] { new DB.User { Email = TestEmail } });

            var handler = new UserUpdateHandler(dbContext.Object, loggerMock.Object);
            var command = new UserUpdateCommand
            {
                Email = TestEmail
            };
            //Act
            var action = handler.Handle(command, default);
            //Assert
            var exception = await Assert.ThrowsAsync<UserUpdateException>(() => action);
            HttpExceptionAssert.IsValidHttpException(exception, expectedStatusCode, ExpectedErrorCode, ExpectedErrorMessage);
            var invocation = loggerMock.Invocations.Single();
            LoggerAssert.HasLogError(invocation, "Update user failed. Handler request:", innerException);
        }

        [Theory]
        [InlineData("fn", "mn", "ln")]
        [InlineData(null, "mn", "ln")]
        [InlineData("fn", null, "ln")]
        [InlineData("fn", "mn", null)]
        [InlineData(null, null, null)]
        public async void ExistingUser(string firstName, string patronymic, string lastName)
        {
            //Arrange
            var dbContext = TestDBContext.CreateInMemoryContext();
            var handler = new UserUpdateHandler(dbContext, null);
            var command = new UserUpdateCommand
            {
                Email = "test@mail.ru",
                FirstName = firstName,
                LastName = lastName,
                Patronymic = patronymic,
            };
            //Act
            var result = await handler.Handle(command, default);
            //Assert
            command.FirstName ??= "Денис";
            command.LastName ??= "Смирнов";
            command.Patronymic ??= "Алексеевич";
            result.WithDeepEqual(command)
                .IgnoreSourceProperty(u => u.Password)
                .IgnoreSourceProperty(u => u.FullName)
                .IgnoreSourceProperty(u => u.Id)
                .IgnoreSourceProperty(u => u.Sessions)
                .IgnoreSourceProperty(u => u.LastUpdate)
                .IgnoreSourceProperty(u => u.CreationDate)
                .IgnoreSourceProperty(u => u.FinishDate)
                .Assert();

            var fullName = string.Join(" ", command.FirstName, command.LastName, command.Patronymic);
            Assert.Equal(fullName, result.FullName);

            var dbUser = dbContext.Users.Single();
            dbUser.WithDeepEqual(command)
                .IgnoreSourceProperty(u => u.Password)
                .IgnoreSourceProperty(u => u.FullName)
                .IgnoreSourceProperty(u => u.Id)
                .IgnoreSourceProperty(u => u.Sessions)
                .IgnoreSourceProperty(u => u.LastUpdate)
                .IgnoreSourceProperty(u => u.CreationDate)
                .IgnoreSourceProperty(u => u.FinishDate)
                .Assert();

            Assert.Equal(fullName, dbUser.FullName);
        }
    }
}
