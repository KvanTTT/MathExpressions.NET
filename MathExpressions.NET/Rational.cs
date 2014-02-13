#region License

/*---------------------------------------------------------------------------------*\

	Distributed under the terms of an MIT-style license:

	The MIT License

	Copyright (c) 2005-2010 Stephen M. McKamey

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.

\*---------------------------------------------------------------------------------*/

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace MathExpressionsNET
{
	/// <summary>
	/// Represents a rational number
	/// </summary>
	[Serializable]
	public struct Rational<T> :
		IConvertible,
		IComparable,
		IComparable<T>
		where T : IConvertible
	{
		#region Delegate Types

		private delegate T ParseDelegate(string value);
		private delegate bool TryParseDelegate(string value, out T rational);

		#endregion Delegate Types

		#region Constants

		private const char Delim = '/';
		private static readonly char[] DelimSet = new char[] { Delim };

		#endregion Constants

		#region Fields

		public static readonly Rational<T> Empty = new Rational<T>();

		private static ParseDelegate Parser;
		private static TryParseDelegate TryParser;
		private static decimal maxValue;

		private readonly T numerator;
		private readonly T denominator;

		#endregion Fields

		#region Init

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="numerator">The numerator of the rational number.</param>
		/// <param name="denominator">The denominator of the rational number.</param>
		/// <param name="reduce">determines if should reduce by greatest common divisor</param>
		/// /// <remarks>reduces by default</remarks>
		public Rational(T numerator, T denominator, bool reduce = true)
		{
			this.numerator = numerator;
			this.denominator = denominator;

			if (reduce)
			{
				Rational<T>.Reduce(ref this.numerator, ref this.denominator);
			}
		}

		public Rational(ValueType numerator, ValueType denominator, bool reduce = true)
			: this((T)Convert.ChangeType(numerator, typeof(T)), (T)Convert.ChangeType(denominator, typeof(T)), reduce)
		{
		}

		#endregion Init

		#region Properties

		/// <summary>
		/// Gets and sets the numerator of the rational number
		/// </summary>
		public T Numerator
		{
			get { return this.numerator; }
		}

		/// <summary>
		/// Gets and sets the denominator of the rational number
		/// </summary>
		public T Denominator
		{
			get { return this.denominator; }
		}

		/// <summary>
		/// Gets a value indicating if this is an empty instance
		/// </summary>
		public bool IsEmpty
		{
			get { return this.Equals(Rational<T>.Empty); }
		}

		public bool IsInteger
		{
			get { return this.denominator.ToUInt64(CultureInfo.InvariantCulture) == 1; }
		}

		public bool IsLessOrEqualThanOne
		{
			get { return Math.Abs(this.denominator.ToInt64(CultureInfo.InvariantCulture)) == 1; }
		}

		public Rational<T> Abs()
		{
			return this < 0 ? -this : this;
		}

		/// <summary>
		/// Gets the MaxValue
		/// </summary>
		private static decimal MaxValue
		{
			get
			{
				if (Rational<T>.maxValue == default(decimal))
				{
					FieldInfo maxValue = typeof(T).GetField("MaxValue", BindingFlags.Static | BindingFlags.Public);
					if (maxValue != null)
					{
						try
						{
							Rational<T>.maxValue = Convert.ToDecimal(maxValue.GetValue(null));
						}
						catch (OverflowException)
						{
							Rational<T>.maxValue = Decimal.MaxValue;
						}
					}
					else
					{
						Rational<T>.maxValue = Int32.MaxValue;
					}
				}

				return Rational<T>.maxValue;
			}
		}

		#endregion Properties

		#region Parse Methods

		/// <summary>
		/// Approximate the decimal value accurate to a certain precision
		/// </summary>
		/// <param name="value">decimal value to approximate</param>
		/// <param name="epsilon">maximum precision to converge</param>
		/// <returns>an approximation of the value as a rational number</returns>
		/// <remarks>
		/// http://stackoverflow.com/questions/95727
		/// </remarks>
		public static Rational<T> Approximate(decimal value, decimal epsilon = 0.0000001m)
		{
			long numerator = (long)Decimal.Truncate(value);
			long denominator = 1;
			decimal fraction = Decimal.Divide(numerator, denominator);
			long maxValue = (long)Rational<T>.MaxValue;

			while (Math.Abs(fraction - value) > epsilon && (denominator < maxValue) && (numerator < maxValue))
			{
				if (fraction < value)
				{
					numerator++;
				}
				else
				{
					denominator++;

					long temp = (long)Math.Round(Decimal.Multiply(value, denominator));
					if (temp > maxValue)
					{
						denominator--;
						break;
					}

					numerator = temp;
				}

				fraction = Decimal.Divide(numerator, denominator);
			}

			return new Rational<T>(
				(T)Convert.ChangeType(numerator, typeof(T)),
				(T)Convert.ChangeType(denominator, typeof(T)));
		}

		/// <summary>
		/// Convert decimal to fraction
		/// </summary>
		/// <param name="value">decimal value to convert</param>
		/// <param name="result">result fraction if conversation is succsess</param>
		/// <param name="decimalPlaces">precision of considereation frac part of value</param>
		/// <param name="trimZeroes">trim zeroes on the right part of the value or not</param>
		/// <param name="minPeriodRepeat">minimum period repeating</param>
		/// <param name="digitsForReal">precision for determination value to real if period has not been founded</param>
		/// <returns></returns>
		public static bool FromDecimal(decimal value, out Rational<T> result, 
			int decimalPlaces = 28, bool trimZeroes = false, decimal minPeriodRepeat = 2, int digitsForReal = 9)
		{
			var valueStr = value.ToString("0.0000000000000000000000000000", CultureInfo.InvariantCulture);
			var strs = valueStr.Split('.');

			long intPart = long.Parse(strs[0]);
			string fracPartTrimEnd = strs[1].TrimEnd(new char[] { '0' });
			string fracPart;

			if (trimZeroes)
			{
				fracPart = fracPartTrimEnd;
				decimalPlaces = Math.Min(decimalPlaces, fracPart.Length);
			}
			else
				fracPart = strs[1];

			result = new Rational<T>();
			try
			{
				string periodPart;
				bool periodFound = false;
				
				int i;
				for (i = 0; i < fracPart.Length; i++)
				{
					if (fracPart[i] == '0' && i != 0)
						continue;

					for (int j = i + 1; j < fracPart.Length; j++)
					{
						periodPart = fracPart.Substring(i, j - i);
						periodFound = true;
						decimal periodRepeat = 1;
						decimal periodStep = 1.0m / periodPart.Length;
						var upperBound = Math.Min(fracPart.Length, decimalPlaces);
						int k;
						for (k = i + periodPart.Length; k < upperBound; k += 1)
						{
							if (periodPart[(k - i) % periodPart.Length] != fracPart[k])
							{
								periodFound = false;
								break;
							}
							periodRepeat += periodStep;
						}

						if (!periodFound && upperBound - k <= periodPart.Length && periodPart[(upperBound - i) % periodPart.Length] > '5')
						{
							var ind = (k - i) % periodPart.Length;
							var regroupedPeriod = (periodPart.Substring(ind) + periodPart.Remove(ind)).Substring(0, upperBound - k);
							ulong periodTailPlusOne = ulong.Parse(regroupedPeriod) + 1;
							ulong fracTail = ulong.Parse(fracPart.Substring(k, regroupedPeriod.Length));
							if (periodTailPlusOne == fracTail)
								periodFound = true;
						}

						if (periodFound && periodRepeat >= minPeriodRepeat)
						{
							result = FromDecimal(strs[0], fracPart.Substring(0, i), periodPart);
							break;
						}
						else
							periodFound = false;
					}

					if (periodFound)
						break;
				}

				if (!periodFound)
				{
					result = new Rational<T>(long.Parse(strs[0]), 1, false);
					if (fracPartTrimEnd.Length != 0)
						result += new Rational<T>(ulong.Parse(fracPartTrimEnd), TenInPower(fracPartTrimEnd.Length));
					return fracPartTrimEnd.Length < digitsForReal;
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		public static Rational<T> FromDecimal(string intPart, string fracPart, string periodPart)
		{
			Rational<T> firstFracPart;
			if (fracPart != null && fracPart.Length != 0)
			{
				ulong denominator = TenInPower(fracPart.Length);
				firstFracPart = new Rational<T>(ulong.Parse(fracPart), denominator);
			}
			else
				firstFracPart = new Rational<T>(0, 1, false);

			Rational<T> secondFracPart;
			if (periodPart != null && periodPart.Length != 0)
				secondFracPart =
					new Rational<T>(ulong.Parse(periodPart), TenInPower(fracPart.Length)) *
					new Rational<T>(1, Nines((ulong)periodPart.Length), false);
			else
				secondFracPart = new Rational<T>(0, 1, false);

			var result = firstFracPart + secondFracPart;
			if (intPart != null && intPart.Length != 0)
			{
				long intPartLong = long.Parse(intPart);
				result = new Rational<T>(intPartLong, 1, false) + (intPartLong == 0 ? 1 : Math.Sign(intPartLong)) * result;
			}

			return result;
		}

		private static ulong TenInPower(int power)
		{
			ulong result = 1;
			for (int l = 0; l < power; l++)
				result *= 10;
			return result;
		}

		private static decimal TenInNegPower(int power)
		{
			decimal result = 1;
			for (int l = 0; l > power; l--)
				result /= 10.0m;
			return result;
		}

		private static ulong Nines(ulong power)
		{
			ulong result = 9;
			if (power >= 0)
				for (ulong l = 0; l < power - 1; l++)
					result = result * 10 + 9;
			return result;
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational&lt;T&gt;"/> equivalent.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Rational<T> Parse(string value)
		{
			if (String.IsNullOrEmpty(value))
			{
				return Rational<T>.Empty;
			}

			if (Rational<T>.Parser == null)
			{
				Rational<T>.Parser = Rational<T>.BuildParser();
			}

			string[] parts = value.Split(Rational<T>.DelimSet, 2, StringSplitOptions.RemoveEmptyEntries);
			T numerator = Rational<T>.Parser(parts[0]);
			T denominator;
			if (parts.Length > 1)
			{
				denominator = Rational<T>.Parser(parts[1]);
			}
			else
			{
				denominator = (T)Convert.ChangeType(1, typeof(T)); //default(T);
			}

			return new Rational<T>(numerator, denominator);
		}

		/// <summary>
		/// Converts the string representation of a number to its <see cref="Rational&lt;T&gt;"/> equivalent.
		/// A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="rational"></param>
		/// <returns></returns>
		public static bool TryParse(string value, out Rational<T> rational)
		{
			if (String.IsNullOrEmpty(value))
			{
				rational = Rational<T>.Empty;
				return false;
			}

			if (Rational<T>.TryParser == null)
			{
				Rational<T>.TryParser = Rational<T>.BuildTryParser();
			}

			T numerator, denominator;
			string[] parts = value.Split(Rational<T>.DelimSet, 2, StringSplitOptions.RemoveEmptyEntries);
			if (!Rational<T>.TryParser(parts[0], out numerator))
			{
				rational = Rational<T>.Empty;
				return false;
			}
			if (parts.Length > 1)
			{
				if (!Rational<T>.TryParser(parts[1], out denominator))
				{
					rational = Rational<T>.Empty;
					return false;
				}
			}
			else
			{
				denominator = (T)Convert.ChangeType(numerator, typeof(T)); //default(T);
			}

			rational = new Rational<T>(numerator, denominator);
			return (parts.Length == 2);
		}

		private static Rational<T>.ParseDelegate BuildParser()
		{
			MethodInfo parse = typeof(T).GetMethod(
				"Parse",
				BindingFlags.Public | BindingFlags.Static,
				null,
				new Type[] { typeof(string) },
				null);

			if (parse == null)
			{
				throw new InvalidOperationException("Underlying Rational type T must support Parse in order to parse Rational<T>.");
			}

			return new Rational<T>.ParseDelegate(
				delegate(string value)
				{
					try
					{
						return (T)parse.Invoke(null, new object[] { value });
					}
					catch (TargetInvocationException ex)
					{
						if (ex.InnerException != null)
						{
							throw ex.InnerException;
						}
						throw;
					}
				});
		}

		private static Rational<T>.TryParseDelegate BuildTryParser()
		{
			// http://stackoverflow.com/questions/1933369

			MethodInfo tryParse = typeof(T).GetMethod(
				"TryParse",
				BindingFlags.Public | BindingFlags.Static,
				null,
				new Type[] { typeof(string), typeof(T).MakeByRefType() },
				null);

			if (tryParse == null)
			{
				throw new InvalidOperationException("Underlying Rational type T must support TryParse in order to try-parse Rational<T>.");
			}

			return new Rational<T>.TryParseDelegate(
				delegate(string value, out T output)
				{
					object[] args = new object[] { value, (T)Convert.ChangeType(1, typeof(T)) /*default(T)*/ };
					try
					{
						bool success = (bool)tryParse.Invoke(null, args);
						output = (T)args[1];
						return success;
					}
					catch (TargetInvocationException ex)
					{
						if (ex.InnerException != null)
						{
							throw ex.InnerException;
						}
						throw;
					}
				});
		}

		#endregion Parse Methods

		#region Math Methods

		/// <summary>
		/// Finds the greatest common divisor and reduces the fraction by this amount.
		/// </summary>
		/// <returns>the reduced rational</returns>
		public Rational<T> Reduce()
		{
			T numerator = this.numerator;
			T denominator = this.denominator;

			Rational<T>.Reduce(ref numerator, ref denominator);

			return new Rational<T>(numerator, denominator);
		}

		/// <summary>
		/// Finds the greatest common divisor and reduces the fraction by this amount.
		/// </summary>
		/// <returns>the reduced rational</returns>
		private static void Reduce(ref T numerator, ref T denominator)
		{
			bool reduced = false;

			decimal n = Convert.ToDecimal(numerator);
			decimal d = Convert.ToDecimal(denominator);

			if (n == 1 && d == 1)
				return;

			// greatest commondivisor
			decimal gcd = Rational<T>.GCD(n, d);
			if (gcd != Decimal.One && gcd != Decimal.Zero)
			{
				reduced = true;
				n /= gcd;
				d /= gcd;
			}

			// cancel out signs
			if (d < Decimal.Zero)
			{
				reduced = true;
				n = -n;
				d = -d;
			}

			if (reduced)
			{
				numerator = (T)Convert.ChangeType(n, typeof(T));
				denominator = (T)Convert.ChangeType(d, typeof(T));
			}
		}

		/// <summary>
		/// Lowest Common Denominator
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static decimal LCD(decimal a, decimal b)
		{
			if (a == Decimal.Zero && b == Decimal.Zero)
			{
				return Decimal.Zero;
			}

			return (a * b) / Rational<T>.GCD(a, b);
		}

		/// <summary>
		/// Greatest Common Devisor
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static decimal GCD(decimal a, decimal b)
		{
			if (a < Decimal.Zero)
			{
				a = -a;
			}
			if (b < Decimal.Zero)
			{
				b = -b;
			}

			while (a != b)
			{
				if (a == Decimal.Zero)
				{
					return b;
				}
				if (b == Decimal.Zero)
				{
					return a;
				}

				if (a > b)
				{
					a %= b;
				}
				else
				{
					b %= a;
				}
			}
			return a;
		}

		#endregion Math Methods

		#region IConvertible Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public string ToString(IFormatProvider provider)
		{
			return !IsInteger ? String.Concat(
				this.numerator.ToString(provider),
				Rational<T>.Delim,
				this.denominator.ToString(provider)) :
				this.numerator.ToString(provider);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public decimal ToDecimal(IFormatProvider provider)
		{
			try
			{
				decimal denominator = this.denominator.ToDecimal(provider);
				if (denominator == Decimal.Zero)
				{
					return Decimal.Zero;
				}
				return this.numerator.ToDecimal(provider) / denominator;
			}
			catch (InvalidCastException)
			{
				long denominator = this.denominator.ToInt64(provider);
				if (denominator == 0L)
				{
					return 0L;
				}
				return ((IConvertible)this.numerator.ToInt64(provider)).ToDecimal(provider) /
					((IConvertible)denominator).ToDecimal(provider);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public double ToDouble(IFormatProvider provider = null)
		{
			double denominator = provider != null ? this.denominator.ToDouble(provider) : this.denominator.ToDouble(CultureInfo.InvariantCulture);
			if (denominator == 0.0)
			{
				return 0.0;
			}
			return provider != null ? numerator.ToDouble(provider) : numerator.ToDouble(CultureInfo.InvariantCulture) / denominator;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="provider"></param>
		/// <returns></returns>
		public float ToSingle(IFormatProvider provider)
		{
			float denominator = this.denominator.ToSingle(provider);
			if (denominator == 0.0f)
			{
				return 0.0f;
			}
			return this.numerator.ToSingle(provider) / denominator;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToBoolean(provider);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToByte(provider);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToChar(provider);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToInt16(provider);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToInt32(provider);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToInt64(provider);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToSByte(provider);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToUInt16(provider);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToUInt32(provider);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return ((IConvertible)this.ToDecimal(provider)).ToUInt64(provider);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return new DateTime(((IConvertible)this).ToInt64(provider));
		}

		TypeCode IConvertible.GetTypeCode()
		{
			return this.numerator.GetTypeCode();
		}

		object IConvertible.ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType == null)
			{
				throw new ArgumentNullException("conversionType");
			}

			Type thisType = this.GetType();
			if (thisType == conversionType)
			{
				// no conversion needed
				return this;
			}

			if (!conversionType.IsGenericType ||
				typeof(Rational<>) != conversionType.GetGenericTypeDefinition())
			{
				// fall back to basic conversion
				return Convert.ChangeType(this, conversionType, provider);
			}

			// auto-convert between Rational<T> types by converting Numerator/Denominator
			Type genericArg = conversionType.GetGenericArguments()[0];
			object[] ctorArgs =
			{
				Convert.ChangeType(this.Numerator, genericArg, provider),
				Convert.ChangeType(this.Denominator, genericArg, provider)
			};

			ConstructorInfo ctor = conversionType.GetConstructor(new Type[] { genericArg, genericArg });
			if (ctor == null)
			{
				throw new InvalidCastException("Unable to find constructor for Rational<" + genericArg.Name + ">.");
			}
			return ctor.Invoke(ctorArgs);
		}

		#endregion IConvertible Members

		#region IComparable Members

		/// <summary>
		/// Compares this instance to a specified System.Object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object that)
		{
			if (that is Rational<T>)
			{
				// differentiate between a real zero and a divide by zero
				// work around divide by zero value to get meaningful comparisons
				Rational<T> other = (Rational<T>)that;
				if (Convert.ToDecimal(this.denominator) == Decimal.Zero)
				{
					if (Convert.ToDecimal(other.denominator) == Decimal.Zero)
					{
						return Convert.ToDecimal(this.numerator).CompareTo(Convert.ToDecimal(other.numerator));
					}
					else if (Convert.ToDecimal(other.numerator) == Decimal.Zero)
					{
						return Convert.ToDecimal(this.denominator).CompareTo(Convert.ToDecimal(other.denominator));
					}
				}
				else if (Convert.ToDecimal(other.denominator) == Decimal.Zero)
				{
					if (Convert.ToDecimal(this.numerator) == Decimal.Zero)
					{
						return Convert.ToDecimal(this.denominator).CompareTo(Convert.ToDecimal(other.denominator));
					}
				}
			}

			return Convert.ToDecimal(this).CompareTo(Convert.ToDecimal(that));
		}

		#endregion IComparable Members

		#region IComparable<T> Members

		/// <summary>
		/// Compares this instance to another <typeparamref name="T"/> instance.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(T that)
		{
			return Decimal.Compare(Convert.ToDecimal(this), Convert.ToDecimal(that));
		}

		#endregion IComparable<T> Members

		#region Operators

		public static Rational<T> operator -(Rational<T> r)
		{
			T numerator = (T)Convert.ChangeType(-Convert.ToDecimal(r.numerator), typeof(T));
			return new Rational<T>(numerator, r.denominator);
		}

		public static Rational<T> operator +(Rational<T> r1, Rational<T> r2)
		{
			decimal n1 = Convert.ToDecimal(r1.numerator);
			decimal d1 = Convert.ToDecimal(r1.denominator);
			decimal n2 = Convert.ToDecimal(r2.numerator);
			decimal d2 = Convert.ToDecimal(r2.denominator);

			decimal denominator = Rational<T>.LCD(d1, d2);
			if (denominator > d1)
			{
				n1 *= (denominator / d1);
			}
			if (denominator > d2)
			{
				n2 *= (denominator / d2);
			}

			decimal numerator = n1 + n2;

			return new Rational<T>((T)Convert.ChangeType(numerator, typeof(T)), (T)Convert.ChangeType(denominator, typeof(T)));
		}

		public static Rational<T> operator -(Rational<T> r1, Rational<T> r2)
		{
			return r1 + (-r2);
		}

		public static Rational<T> operator *(Rational<T> r1, Rational<T> r2)
		{
			decimal numerator = Convert.ToDecimal(r1.numerator) * Convert.ToDecimal(r2.numerator);
			decimal denominator = Convert.ToDecimal(r1.denominator) * Convert.ToDecimal(r2.denominator);

			return new Rational<T>((T)Convert.ChangeType(numerator, typeof(T)), (T)Convert.ChangeType(denominator, typeof(T)));
		}

		public static Rational<T> operator /(Rational<T> r1, Rational<T> r2)
		{
			return r1 * new Rational<T>(r2.denominator, r2.numerator);
		}

		public static bool operator <(Rational<T> r1, Rational<T> r2)
		{
			return r1.CompareTo(r2) < 0;
		}

		public static bool operator <=(Rational<T> r1, Rational<T> r2)
		{
			return r1.CompareTo(r2) <= 0;
		}

		public static bool operator >(Rational<T> r1, Rational<T> r2)
		{
			return r1.CompareTo(r2) > 0;
		}

		public static bool operator >=(Rational<T> r1, Rational<T> r2)
		{
			return r1.CompareTo(r2) >= 0;
		}

		public static bool operator ==(Rational<T> r1, Rational<T> r2)
		{
			return r1.CompareTo(r2) == 0;
		}

		public static bool operator !=(Rational<T> r1, Rational<T> r2)
		{
			return r1.CompareTo(r2) != 0;
		}

		public static implicit operator Rational<T>(int d)
		{
			return new Rational<T>(d, 1, false);
		}

		#endregion Operators

		#region Object Overrides

		public override string ToString()
		{
			return Convert.ToString(this);
		}

		public override bool Equals(object obj)
		{
			return (this.CompareTo(obj) == 0);
		}

		public override int GetHashCode()
		{
			// adapted from Anonymous Type: { uint Numerator, uint Denominator }
			int num = 0x1fb8d67d;
			num = (-1521134295 * num) + EqualityComparer<T>.Default.GetHashCode(this.numerator);
			return ((-1521134295 * num) + EqualityComparer<T>.Default.GetHashCode(this.denominator));
		}

		#endregion Object Overrides
	}
}

