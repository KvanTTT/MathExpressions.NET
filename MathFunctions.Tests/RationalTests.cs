using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Numerics;

namespace MathFunctions.Tests
{
	[TestFixture]
	public class RationalTests
	{
		[Test]
		public void ConversionTest()
		{
			Rational<long> r;

			Assert.IsTrue(Rational<long>.FromDecimal(0.0000000000000000000000000000m, out r));
			Assert.IsTrue(new Rational<long>(0, 1) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(53.0000000000000000000000000000m, out r));
			Assert.IsTrue(new Rational<long>(53, 1) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.3333333333333333333333333333m, out r));
			Assert.IsTrue(new Rational<long>(1, 3) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.5678333333333333333333333333m, out r));
			Assert.IsTrue(new Rational<long>(3407, 6000) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.8888888888888888888888888889m, out r));
			Assert.IsTrue(new Rational<long>(8, 9) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.9889889889889889889889889890m, out r));
			Assert.IsTrue(new Rational<long>(988, 999) == r);
			Assert.IsTrue(Rational<long>.FromDecimal(988m / 999, out r));
			Assert.IsTrue(new Rational<long>(988, 999) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.9899899899899899899899899900m, out r));
			Assert.IsTrue(new Rational<long>(989, 999) == r);
			Assert.IsTrue(Rational<long>.FromDecimal(989m / 999, out r));
			Assert.IsTrue(new Rational<long>(989, 999) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.9989989989989989989989989990m, out r));
			Assert.IsTrue(new Rational<long>(998, 999) == r);
			Assert.IsTrue(Rational<long>.FromDecimal(998m / 999, out r));
			Assert.IsTrue(new Rational<long>(998, 999) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.9999999999999999999999999999m, out r));
			Assert.IsTrue(new Rational<long>(1, 1) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(20.0m / 3, out r));
			Assert.IsTrue(new Rational<long>(20, 3) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(-9.1234567856785678567856785678m, out r, 26));
			Assert.IsTrue(new Rational<long>(-228063611, 24997500) == r);
		}

		[Test]
		public void DecimalPlacesTest()
		{
			Rational<long> r;

			Assert.IsTrue(Rational<long>.FromDecimal(0.33333333m, out r, 8, false));
			Assert.IsTrue(new Rational<long>(1, 3) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.33333333m, out r, 9, false));
			Assert.IsTrue(new Rational<long>(33333333, 100000000) == r);
		}

		[Test]
		public void TrimEndZeroesTest()
		{
			Rational<long> r;

			Assert.IsTrue(Rational<long>.FromDecimal(0.33m, out r, 28, true));
			Assert.IsTrue(new Rational<long>(1, 3) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.33m, out r, 28, false));
			Assert.IsTrue(new Rational<long>(33, 100) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.33m, out r, 2, false));
			Assert.IsTrue(new Rational<long>(1, 3) == r);
		}

		[Test]
		public void MinPeriodRepeatTest()
		{
			Rational<long> r;

			Assert.IsTrue(Rational<long>.FromDecimal(0.123412m, out r, 28, true, 1.5m));
			Assert.IsTrue(new Rational<long>(1234, 9999) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.123412m, out r, 28, false, 1.5m));
			Assert.IsTrue(new Rational<long>(123412, 1000000) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.123412m, out r, 6, false, 1.5m));
			Assert.IsTrue(new Rational<long>(1234, 9999) == r);

			Assert.IsTrue(Rational<long>.FromDecimal(0.123412m, out r, 28, true, 1.6m));
			Assert.IsTrue(new Rational<long>(123412, 1000000) == r);
		}

		[Test]
		public void DigitsForRealTest()
		{
			Rational<long> r;

			Assert.IsTrue(Rational<long>.FromDecimal(0.12345678m, out r, 28, true, 2, 9));
			Assert.IsTrue(new Rational<long>(12345678, 100000000) == r);

			Assert.IsFalse(Rational<long>.FromDecimal(0.12345678m, out r, 28, true, 2, 8));

			Assert.IsTrue(Rational<long>.FromDecimal(0.12121212121212121m, out r, 28, true, 2, 10));
			Assert.IsTrue(new Rational<long>(4, 33) == r);
		}
	}
}
