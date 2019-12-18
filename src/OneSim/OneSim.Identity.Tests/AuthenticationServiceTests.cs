namespace OneSim.Identity.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using OneSim.Identity.Application;

    /// <summary>
    ///     The <see cref="AuthenticationService"/> Tests.
    /// </summary>
    [TestFixture]
    public class AuthenticationServiceTests
    {
        /// <summary>
        ///     To assert whether or not the user can log in with a password.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not the user can log in with a password.")]
        public async Task CanLogInWithPassword()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not the user can log in with a 2FA code.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not the user can log in with a 2FA code.")]
        public async Task CanLogInWithTwoFactorAuthenticationCode()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not the user can log in with a 2FA recovery code.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not the user can log in with a 2FA recovery code.")]
        public async Task CanLogInWithRecoveryCode()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not a 2FA code is sent to the user when required.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not a 2FA code is sent to the user when required.")]
        public async Task TwoFactorAuthenticationCodeIsSentWhenRequired()
        {
            await Task.Yield();
            Assert.Fail();
        }
    }
}