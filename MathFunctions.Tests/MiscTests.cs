using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace MathExpressions.NET.Tests
{
	[TestFixture]
	public class MiscTests
	{
		[Test]
		public void IntPowerTest()
		{
			for (int i = 0; i < 10; i++)
				for (int j = 1; j < 10; j++)
					Assert.AreEqual(Math.Pow(i, j), IntPow(i, j));
		}

		int IntPow(int x, int pow)
		{
			int ret = x;
			
			pow--;
			do
			{
				if ((pow & 1) == 1)
					ret *= x;
				x *= x;
				pow >>= 1;
			}
			while (pow != 0);
			
			return ret;
		}
	}
}
