namespace OneSim.Identity.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using NUnit.Framework;
    using OneSim.Identity.Application.Interfaces;
    using OneSim.Identity.Application.Queries;
    using OneSim.Identity.Application.Queries.GetUserById;
    using OneSim.Identity.Domain.Entities;
    using OneSim.Identity.Tests.Utils;

    /// <summary>
    ///     The Query Tests.
    /// </summary>
    [TestFixture]
    public class QueryTests
    {
        /// <summary>
        ///     To assert whether or not the <see cref="GetUserByIdRequest"/> works.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test]
        public async Task GetUserByIdWorks()
        {
            // Arrange
            IIdentityDbContext database = Helpers.GetDbContext();
            ApplicationUser testUser = new ApplicationUser
            {
                UserName = "TestUser"
            };
            database.Users.Add(testUser);
            
            GetUserByIdRequest request = new GetUserByIdRequest
            {
                UserId = testUser.Id
            };
            
            // Act
            GetUserByIdRequestHandler handler = new GetUserByIdRequestHandler(database);
            GetUserResponse response = await handler.Handle(request, CancellationToken.None);
            
            // Assert
            Assert.AreEqual(testUser, response.User);
        }
    }
}