using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GOLD;

namespace MathFunctions.GUI
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			var derivatives = new StringBuilder();

			derivatives.AppendLine("(f(x) / g(x))' = (f(x)' * g(x) + f(x) * g(x)') / g(x)^2;");
			derivatives.AppendLine("(f(x) ^ g(x))' = f(x) ^ g(x) * (f(x)' * g(x) / f(x) + g(x)' * ln(f(x)));");

			derivatives.AppendLine("neg(f(x))' = neg(f(x)');");

			derivatives.AppendLine("sin(f(x))' = cos(f(x)) * f(x)';											   ");
			derivatives.AppendLine("cos(f(x))' = -sin(f(x)) * f(x)';											   ");
			derivatives.AppendLine("tan(f(x))' = f(x)' / cos(f(x)) ^ 2;									   ");
			derivatives.AppendLine("cot(f(x))' = -f(x)' / sin(f(x)) ^ 2;									   ");

			derivatives.AppendLine("arcsin(f(x))' = f(x)' / sqrt(1 - f(x) ^ 2);							   ");
			derivatives.AppendLine("arccos(f(x))' = -f(x)' / sqrt(1 - f(x) ^ 2);							   ");
			derivatives.AppendLine("arctan(f(x))' = f(x)' / (1 + f(x) ^ 2);								   ");
			derivatives.AppendLine("arccot(f(x))' = -f(x)' / (1 + f(x) ^ 2);								   ");

			derivatives.AppendLine("sinh(f(x))' = f(x)' * cosh(x);											   ");
			derivatives.AppendLine("cosh(f(x))' = f(x)' * sinh(x);											   ");

			derivatives.AppendLine("arcsinh(f(x))' = f(x)' / sqrt(f(x) ^ 2 + 1);							   ");
			derivatives.AppendLine("arcosh(f(x))' = f(x)' / sqrt(f(x) ^ 2 - 1);							   ");

			derivatives.AppendLine("ln(f(x))' = f(x)' / f(x);												   ");
			derivatives.AppendLine("log(f(x), g(x))' = g'(x)/(g(x)*ln(f(x))) - (f'(x)*ln(g(x)))/(f(x)*ln(f(x))^2);");

			Helper.InitDerivatives(derivatives.ToString());

			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			tbInput_TextChanged(sender, e);
		}

		private void tbInput_TextChanged(object sender, EventArgs e)
		{
			dgvErrors.Rows.Clear();

			try
			{
				var simpleFunc = new MathFunc(tbInput.Text).Simplify();
				tbSimplification.Text = simpleFunc.ToString();
				
				var compileFunc = new MathFunc(simpleFunc.Root, true);
				compileFunc.Compile();

				var sb = new StringBuilder();
				foreach (var instr in compileFunc.Instructions)
					sb.AppendLine(instr.ToString());
				
				tbIlCode.Text = sb.ToString();
			}
			catch (Exception ex)
			{
				dgvErrors.Rows.Add(string.Empty, ex.Message);
				foreach (var error in Helper.Parser.Errors)
					dgvErrors.Rows.Add(error.Position == null ? string.Empty : error.Position.Column.ToString(), error.Message);
				tbSimplification.Text = null;
				tbDerivative.Text = null;
				tbIlCode.Text = null;
			}

			if (tbSimplification.Text != string.Empty)
				try
				{
					tbDerivative.Text = new MathFunc(tbInput.Text).GetDerivative().ToString();
				}
				catch (Exception ex)
				{
					dgvErrors.Rows.Add(string.Empty, ex.Message);
					foreach (var error in Helper.Parser.Errors)
						dgvErrors.Rows.Add(error.Position == null ? string.Empty : error.Position.Column.ToString(), error.Message);
					tbDerivative.Text = null;
					tbIlCode.Text = null;
				}
		}

		private void dgvErrors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			int pos;
			if (int.TryParse(dgvErrors[0, e.RowIndex].Value.ToString(), out pos))
			{
				tbInput.Select(pos, 0);
				tbInput.Focus();
			}
		}
	}
}
