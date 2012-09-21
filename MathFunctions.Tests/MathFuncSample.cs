using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathFunctions.Tests
{
	public static class MathFuncSample
	{
		public static double Func(double x, double y, decimal z)
		{
			//return x * 2.0 + 3.0 - Math.Sqrt(y) + x + (double)4.0m;
			return (-x * 2 + 1) * (2 * x + 1);
		}
	}
}
