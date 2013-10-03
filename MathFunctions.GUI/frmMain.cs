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
using MathFunctions.GUI.Properties;
using System.Globalization;
using System.Threading;

namespace MathFunctions.GUI
{
	public partial class frmMain : Form
	{
		MathFuncAssemblyCecil Assembly;

		public frmMain()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			tbDerivatives.Text = Settings.Default.Derivatives;
			btnRebuildDerivatives_Click(null, null);
			tbInput.Text = Settings.Default.InputExpression;
			cbRealTimeUpdate.Checked = Settings.Default.RealTimeUpdate;
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings.Default.InputExpression = tbInput.Text;
			Settings.Default.Save();
		}

		private void tbInput_TextChanged(object sender, EventArgs e)
		{
			if (cbRealTimeUpdate.Checked)
				btnCalculate_Click(sender, e);
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

		private void btnRebuildDerivatives_Click(object sender, EventArgs e)
		{
			try
			{
				Helper.InitDerivatives(tbDerivatives.Text);
				btnCalculate_Click(sender, e);
				Settings.Default.Derivatives = tbDerivatives.Text;
				Settings.Default.Save();
			}
			catch (Exception ex)
			{
				var parserErrors = Helper.Parser.Errors;
				if (parserErrors.Count != 0)
					MessageBox.Show(string.Format("Threa are errors in derivatives list: {0} at position {1}",
						Helper.Parser.Errors.First().Message, Helper.Parser.Errors.First().Position));
				else
					MessageBox.Show("Derivatives: " + ex.Message);
			}
		}

		private void cbRealTimeUpdate_CheckedChanged(object sender, EventArgs e)
		{
			Settings.Default.RealTimeUpdate = cbRealTimeUpdate.Checked;
			Settings.Default.Save();

			if (cbRealTimeUpdate.Checked)
				btnCalculate_Click(sender, e);
		}

		private void btnCalculate_Click(object sender, EventArgs e)
		{
			dgvErrors.Rows.Clear();

			Assembly = new MathFuncAssemblyCecil();
			Assembly.Init();

			var variable = string.IsNullOrEmpty(tbVar.Text) ? null : new VarNode(tbVar.Text.ToLowerInvariant());

			MathFunc simplifiedFunc = null;
			try
			{
				simplifiedFunc = new MathFunc(tbInput.Text, tbVar.Text).Simplify();
				tbSimplification.Text = simplifiedFunc.ToString();
			}
			catch (Exception ex)
			{
				dgvErrors.Rows.Add(string.Empty, ex.Message);
				foreach (var error in Helper.Parser.Errors)
					dgvErrors.Rows.Add(error.Position == null ? string.Empty : error.Position.Column.ToString(), error.Message);
				tbSimplification.Text = null;
				tbDerivative.Text = null;
				tbIlCode.Text = null;
				tbDerivativeIlCode.Text = null;
			}

			try
			{
				var compileFunc = new MathFunc(tbInput.Text, tbVar.Text, true, true);
				compileFunc.Compile(Assembly, "Func");

				var sb = new StringBuilder();
				compileFunc.Instructions.ToList().ForEach(instr => sb.AppendLine(instr.ToString().Replace("IL_0000: ", "")));
				tbIlCode.Text = sb.ToString();
			}
			catch (Exception ex)
			{
				dgvErrors.Rows.Add(string.Empty, ex.Message);
				tbIlCode.Text = null;
			}

			if (tbSimplification.Text != string.Empty)
			{
				MathFunc derivativeFunc = null;
				try
				{
					derivativeFunc = new MathFunc(tbInput.Text, tbVar.Text).GetDerivative();
					tbDerivative.Text = derivativeFunc.ToString();
				}
				catch (Exception ex)
				{
					dgvErrors.Rows.Add(string.Empty, ex.Message);
					foreach (var error in Helper.Parser.Errors)
						dgvErrors.Rows.Add(error.Position == null ? string.Empty : error.Position.Column.ToString(), error.Message);
					tbDerivative.Text = null;
					tbDerivativeIlCode.Text = null;
				}

				try
				{
					var compileDerivativeFunc = new MathFunc(tbDerivative.Text, tbVar.Text, true, true);
					compileDerivativeFunc.DerivativeDelta = double.Parse(tbDerivativeDelta.Text);
					compileDerivativeFunc.Compile(Assembly, "FuncDer");
					var sb = new StringBuilder();
					compileDerivativeFunc.Instructions.ToList().ForEach(instr => sb.AppendLine(instr.ToString().Replace("IL_0000: ", "")));

					tbDerivativeIlCode.Text = sb.ToString();
				}
				catch (Exception ex)
				{
					dgvErrors.Rows.Add(string.Empty, ex.Message);
					tbDerivativeIlCode.Text = null;
				}
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (Assembly != null)
					Assembly.Finalize(Path.GetDirectoryName(saveFileDialog1.FileName), Path.GetFileName(saveFileDialog1.FileName));
			}
		}

		private void tbResultExpression_Click(object sender, EventArgs e)
		{
			(sender as TextBox).SelectAll();
		}
	}
}