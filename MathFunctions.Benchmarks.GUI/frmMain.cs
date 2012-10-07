using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace MathPowVsMultTest.Gui
{
	public partial class frmMain : Form
	{
		public static double MathPow(double x, int pow)
		{
			return Math.Pow(x, pow);
		}
		
		public static double IntPow(double x, int pow)
		{
			double ret = x;

			for (int i = 1; i < pow; i++)
				ret *= x;

			return ret;
		}

		public static double FastPow(double x, int pow)
		{
			if (pow == 0)
				return x;

			double ret = x;

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

		double[] MathPowTimeSpans;
		double[] MathPowResults;
		double[] IntPowTimeSpans;
		double[] IntPowResults;
		double[] FastPowTimeSpans;
		double[] FastPowResults;

		public frmMain()
		{
			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			zedGraphControl1.GraphPane.Title.Text = null;
			zedGraphControl1.GraphPane.YAxis.Title.Text = "time (ticks)";
			zedGraphControl1.GraphPane.XAxis.Title.Text = "exp";

			btnCalculate_Click(sender, e);
		}

		private void btnCalculate_Click(object sender, EventArgs e)
		{
			double expBase = double.Parse(tbExpBase.Text, CultureInfo.InvariantCulture);
			int expCount = (int)udExpIterCount.Value;
			int iterationCount = (int)udIterCount.Value;

			Calculate(MathPow, iterationCount, expBase, expCount, out MathPowTimeSpans, out MathPowResults);
			Calculate(IntPow, iterationCount, expBase, expCount, out IntPowTimeSpans, out IntPowResults);
			Calculate(FastPow, iterationCount, expBase, expCount, out FastPowTimeSpans, out FastPowResults);

			checkBox_CheckedChanged(sender, e);
		}

		private void checkBox_CheckedChanged(object sender, EventArgs e)
		{
			var graphPane1 = zedGraphControl1.GraphPane;
			graphPane1.CurveList.Clear();

			int iterationCount = (int)udIterCount.Value;
			double avgY = 0;
			int graphNumber = 0;

			if (cbMathPow.Checked)
			{
				Draw(iterationCount, MathPowTimeSpans, "Math.Pow", graphPane1, Color.IndianRed);
				avgY += MathPowTimeSpans.Average();
				graphNumber++;
			}

			if (cbIntPow.Checked)
			{
				Draw(iterationCount, IntPowTimeSpans, "Int Pow", graphPane1, Color.ForestGreen);
				avgY += IntPowTimeSpans.Average();
				graphNumber++;
			}

			if (cbFastPow.Checked)
			{
				Draw(iterationCount, FastPowTimeSpans, "Fast Exp", graphPane1, Color.SkyBlue);
				avgY += FastPowTimeSpans.Average();
				graphNumber++;
			}

			avgY /= graphNumber;
			graphPane1.YAxis.Scale.MaxAuto = false;
			graphPane1.YAxis.Scale.Max = avgY * 2;
			zedGraphControl1.AxisChange();
			zedGraphControl1.Refresh();
		}

		private void Calculate(Func<double, int, double> func,
			int iterationCount, double expBase, int expCount, out double[] timeSpans, out double[] results)
		{
			Stopwatch sw;
			double result = 0;
			timeSpans = new double[iterationCount];
			results = new double[iterationCount];

			for (int j = 0; j < iterationCount; j++)
			{
				sw = new Stopwatch();
				sw.Start();
				for (int i = 0; i < expCount; i++)
					result = func(expBase, j);
				sw.Stop();
				timeSpans[j] = (double)sw.Elapsed.Ticks / expCount;
				results[j] = result;
			}
		}

		private void Draw(int iterationCount, double[] yData, string methodName, GraphPane graphPane, Color color)
		{
			double[] xData = Enumerable.Range(0, iterationCount).Select(iter => (double)iter).ToArray();

			if (cbPrecise.Checked)
				graphPane.AddCurve(methodName,
					xData,
					yData,
					color, ZedGraph.SymbolType.None);

			if (cbApprox.Checked)
			{
				var regression = new PolynominalRegression(xData, yData, (int)udApproxPolygonPow.Value);

				var curve = graphPane.AddCurve(methodName + " (approx)",
					xData,
					xData.Select(x => regression.Calculate(x)).ToArray(),
					color, ZedGraph.SymbolType.None);
				curve.Line.Width = 3;
			}
		}
	}
}
