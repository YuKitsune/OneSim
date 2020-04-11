namespace OneSim.Models
{
	using System;

	using OneSim.Models.Attributes;

	/// <summary>
	/// 	Object used to use kilograms and pounds together.
	/// </summary>
	public class Weight
	{
		/// <summary>
		/// 	The current <see cref="Weight"/> in kilograms.
		/// </summary>
		private double _kilograms;

		/// <summary>
		/// 	The current <see cref="Weight"/> in pounds.
		/// </summary>
		private double _pounds;

		/// <summary>
		/// 	Gets or sets the current <see cref="Weight"/> in kilograms.
		/// </summary>
		[Abbreviation("KGS", "Kilograms")]
		public double Kilograms
		{
			get => _kilograms;
			set
			{
				_kilograms = value;
				_pounds = KilogramsToPounds(_kilograms);
			}
		}

		/// <summary>
		/// 	Gets or sets the current <see cref="Weight"/> in pounds.
		/// </summary>
		[Abbreviation("LBS", "Pounds")]
		public double Pounds
		{
			get => _pounds;
			set
			{
				_pounds = value;
				_kilograms = PoundsToKilograms(_pounds);
			}
		}

		/// <summary>
		/// 	Initializes a new instance of the <see cref="Weight"/> class.
		/// </summary>
		public Weight() => Kilograms = 0;

		/// <summary>
		/// 	Gets the weight given a <see cref="WeightUnit"/>.
		/// </summary>
		/// <param name="unit">
		///		The <see cref="WeightUnit"/> to get.
		/// </param>
		/// <returns>
		///		The weight.
		/// </returns>
		/// <exception cref="NotImplementedException">
		///		Throws a <see cref="NotImplementedException"/> if the given <see cref="WeightUnit"/> value has not been
		/// 	implemented in this method.
		/// </exception>
		public double GetWeight(WeightUnit unit)
		{
			switch (unit)
			{
				case WeightUnit.Kilograms: return Kilograms;

				case WeightUnit.Pounds: return Pounds;

				default: throw new NotImplementedException($"The given {nameof(WeightUnit)} ({unit}) has not been implemented.");
			}
		}

		/// <summary>
		/// 	Converts the given <paramref name="kilograms"/> value to pounds.
		/// </summary>
		/// <param name="kilograms">
		///		The weight in kilograms.
		/// </param>
		/// <returns>
		///		The weight in pounds.
		/// </returns>
		/// Todo: Find a more accurate way to convert
		public static double KilogramsToPounds(double kilograms) => kilograms * 2.20462262185;

		/// <summary>
		/// 	Converts the given <paramref name="pounds"/> value to kilograms.
		/// </summary>
		/// <param name="pounds">
		///		The weight in pounds.
		/// </param>
		/// <returns>
		///		The weight in kilograms.
		/// </returns>
		/// Todo: Find a more accurate way to convert
		public static double PoundsToKilograms(double pounds) => pounds * 0.45359237;
	}
}