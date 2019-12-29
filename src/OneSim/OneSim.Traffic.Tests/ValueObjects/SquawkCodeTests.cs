namespace OneSim.Traffic.Tests.ValueObjects
{
	using NUnit.Framework;

	using OneSim.Traffic.Domain.Exceptions;
	using OneSim.Traffic.Domain.ValueObjects;

	/// <summary>
	/// 	The <see cref="SquawkCode"/> Tests.
	/// </summary>
	[TestFixture]
	public class SquawkCodeTests
	{
		/// <summary>
		/// 	Ensures a <see cref="SquawkCode"/> can be constructed from a string.
		/// </summary>
		[Test]
		public void SquawkCodeConstructsFromString()
		{
			// Arrange
			string testCode1 = "1234";
			string testCode2 = "0056";

			// Act
			SquawkCode code1 = new SquawkCode(testCode1);
			SquawkCode code2 = new SquawkCode(testCode2);

			// Assert
			Assert.AreEqual(testCode1, (string) code1);
			Assert.AreEqual(testCode2, (string) code2);
		}

		/// <summary>
		/// 	Ensures a <see cref="SquawkCode"/> can be constructed from an integer.
		/// </summary>
		[Test]
		public void SquawkCodeConstructsFromInt()
		{
			// Arrange
			int testCode1 = 1234;
			int testCode2 = 0056;

			// Act
			SquawkCode code1 = new SquawkCode(testCode1);
			SquawkCode code2 = new SquawkCode(testCode2);

			// Assert
			Assert.AreEqual(testCode1, (int) code1);
			Assert.AreEqual(testCode2, (int) code2);
		}

		/// <summary>
		/// 	Ensures a <see cref="SquawkCode"/> can be implicitly converted from a string.
		/// </summary>
		[Test]
		public void SquawkCodeConvertsFromString()
		{
			// Arrange
			string testCode = "1234";

			// Act
			SquawkCode code = testCode;

			// Assert
			Assert.AreEqual(testCode, (string) code);
		}

		/// <summary>
		/// 	Ensures a <see cref="SquawkCode"/> can be implicitly converted from an integer.
		/// </summary>
		[Test]
		public void SquawkCodeConvertsFromInt()
		{
			// Arrange
			int testCode = 1234;

			// Act
			SquawkCode code = testCode;

			// Assert
			Assert.AreEqual(testCode, (int) code);
		}

		/// <summary>
		/// 	Ensures the <see cref="SquawkCode"/> constructor throws an <see cref="InvalidSquawkCodeException"/>
		/// 	when given a value with a length too long for a <see cref="SquawkCode"/>.
		/// </summary>
		[Test]
		public void SquawkCodeThrowsInvalidLength()
		{
			// Arrange
			string testCode = "12345";

			// Act / Assert
			Assert.Throws<InvalidSquawkCodeException>(() => _ = new SquawkCode(testCode));
		}

		/// <summary>
		/// 	Ensures the <see cref="SquawkCode"/> constructor throws an <see cref="InvalidSquawkCodeException"/>
		/// 	when given a value with invalid digits (Not between 0 and 7).
		/// </summary>
		[Test]
		public void SquawkCodeThrowsInvalidDigits()
		{
			// Arrange
			string testCode = "5678";

			// Act / Assert
			Assert.Throws<InvalidSquawkCodeException>(() => _ = new SquawkCode(testCode));
		}
	}
}