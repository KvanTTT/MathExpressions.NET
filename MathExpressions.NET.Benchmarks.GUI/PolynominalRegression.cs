using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class PolynominalRegression
{
	private int _order;
	private Vector<double> _coefs;

	/// <summary>
	/// Calculates polynom regression for xData = [x1, x2, ... , xn] and yData = [y1, y2, ... , yn].
	/// </summary>
	/// <param name="order">Order of output polynom.</param>
	public PolynominalRegression(DenseVector xData, DenseVector yData, int order)
	{
		_order = order;

		var vandMatrix = new DenseMatrix(xData.Count, order + 1);
		for (int i = 0; i < xData.Count; i++)
		{
			double mult = 1;
			for (int j = 0; j < order + 1; j++)
			{
				vandMatrix[i, j] = mult;
				mult *= xData[i];
			}
		}

		// var vandMatrixT = vandMatrix.Transpose();
		// 1 variant:
		//_coefs = (vandMatrixT * vandMatrix).Inverse() * vandMatrixT * yData;
		// 2 variant:
		//_coefs = (vandMatrixT * vandMatrix).LU().Solve(vandMatrixT * yData);
		// 3 variant (most fast I think. Possible LU decomposion also can be replaced with one triangular matrix):
		_coefs = vandMatrix.TransposeThisAndMultiply(vandMatrix).LU().Solve(TransposeAndMult(vandMatrix, yData));
	}

	/// <summary>
	/// Calculates polynom regression for xData = [0, 1, ... , n] and yData = [y1, y2, ... , yn].
	/// </summary>
	/// <param name="order">Order of output polynom.</param>
	public PolynominalRegression(DenseVector yData, int order)
	{
		_order = order;

		var vandMatrix = new DenseMatrix(yData.Count, order + 1);
		
		for (int i = 0; i < yData.Count; i++) 
		{
			double mult = 1;
			for (int j = 0; j < order + 1; j++)
			{
				vandMatrix[i, j] = mult;
				mult *= i;
			}
		}

		_coefs = vandMatrix.TransposeThisAndMultiply(vandMatrix).LU().Solve(TransposeAndMult(vandMatrix, yData));
	}

	private Vector<double> VandermondeRow(double x)
	{
		double[] result = new double[_order + 1];
		double mult = 1;
		for (int i = 0; i <= _order; i++)
		{
			result[i] = mult;
			mult *= x;
		}
		return new DenseVector(result);
	}

	private static DenseVector TransposeAndMult(Matrix m, Vector v)
	{
		var result = new DenseVector(m.ColumnCount);
		for (int j = 0; j < m.RowCount; j++)
			for (int i = 0; i < m.ColumnCount; i++)
				result[i] += m[j, i] * v[j];
		return result;
	}

	public double Calculate(double x)
	{
		return VandermondeRow(x) * _coefs;
	}
}

