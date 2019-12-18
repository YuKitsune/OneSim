namespace OneSim.Identity.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using OneSim.Identity.Application;

    /// <summary>
    ///     The <see cref="UserService"/> Tests.
    /// </summary>
    [TestFixture]
    public class UserServiceTests
    {
        /// <summary>
        ///     To assert whether or not a new user can be created.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not a new user can be created.")]
        public async Task UserCanBeCreated()
        {
            await Task.Yield();
            Assert.Fail();
        }
        
        /// <summary>
        ///     To assert whether or not an existing user can be deleted.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not an existing user can be deleted.")]
        public async Task UserCanBeDeleted()
        {   
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not a password reset email is sent.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not a password reset email is sent.")]
        public async Task PasswordResetEmailIsSent()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not a password email is sent when the email has not been confirmed.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not a password email is sent when the email has not been confirmed.")]
        public async Task PasswordResetEmailIsNotSentToUnConfirmedEmail()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not a password can be reset.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not a password can be reset.")]
        public async Task PasswordCanBeReset()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not a password can be changed.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not a password can be changed.")]
        public async Task PasswordCanBeChanged()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not an email confirmation email is sent.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not an email confirmation email is sent.")]
        public async Task EmailConfirmationEmailIsSent()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not an email address can be confirmed.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not an email address can be confirmed.")]
        public async Task EmailCanBeConfirmed()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not Two-Factor Authentication can be enabled.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not Two-Factor Authentication can be enabled.")]
        public async Task TwoFactorAuthenticationCanBeEnabled()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not Two-Factor Authentication can be reset.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not Two-Factor Authentication can be reset.")]
        public async Task TwoFactorAuthenticationCanBeReset()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not Two-Factor Authentication can be disabled.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not Two-Factor Authentication can be disabled.")]
        public async Task TwoFactorAuthenticationCanBeDisabled()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not Two-Factor Authentication recovery codes can be generated.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not Two-Factor Authentication recovery codes can be generated")]
        public async Task RecoveryCodesCanBeGenerated()
        {
            await Task.Yield();
            Assert.Fail();
        }

        /// <summary>
        ///     To assert whether or not Two-Factor Authentication recovery codes can be generated when Two-Factor
        ///     Authentication is not enabled.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/>.
        /// </returns>
        [Test(
            Author = "Eoin Motherway",
            Description = "To assert whether or not Two-Factor Authentication recovery codes can be generated")]
        public async Task RecoveryCodesCannotBeGeneratedWithoutTwoFactorAuthenticationEnabled()
        {
            await Task.Yield();
            Assert.Fail();
        }
    }
}