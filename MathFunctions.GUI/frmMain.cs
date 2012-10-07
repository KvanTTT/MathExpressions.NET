﻿using System;
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

namespace MathFunctions.GUI
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
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

			try
			{
				var simpleFunc = new MathFunc(tbInput.Text).Simplify();
				tbSimplification.Text = simpleFunc.ToString();

				//var compileFunc = new MathFunc(simpleFunc.Root, true);
				//compileFunc.Compile();

				//var sb = new StringBuilder();
				//compileFunc.Instructions.ToList().ForEach(instr => sb.AppendLine(instr.ToString()));

				//tbIlCode.Text = sb.ToString();
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

			if (tbSimplification.Text != string.Empty)
				try
				{
					var compileDerivativeFunc = new MathFunc(tbInput.Text).GetDerivative();

					tbDerivative.Text = compileDerivativeFunc.ToString();

					/*compileDerivativeFunc.Compile();
					var sb = new StringBuilder();
					compileDerivativeFunc.Instructions.ToList().ForEach(instr => sb.AppendLine(instr.ToString()));

					tbDerivativeIlCode.Text = sb.ToString();*/
				}
				catch (Exception ex)
				{
					dgvErrors.Rows.Add(string.Empty, ex.Message);
					foreach (var error in Helper.Parser.Errors)
						dgvErrors.Rows.Add(error.Position == null ? string.Empty : error.Position.Column.ToString(), error.Message);
					tbDerivative.Text = null;
					tbIlCode.Text = null;
					tbDerivativeIlCode.Text = null;
				}
		}
	}
}
