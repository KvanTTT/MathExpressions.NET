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
			InitializeComponent();

			btnRebuildDerivatives_Click(null, null);
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
				tbDerivativeIlCode.Text = null;
			}

			if (tbSimplification.Text != string.Empty)
				try
				{
					var compileDerivativeFunc = new MathFunc(tbInput.Text).GetDerivative();

					tbDerivative.Text = compileDerivativeFunc.ToString();

					compileDerivativeFunc.Compile();
					var sb = new StringBuilder();
					foreach (var instr in compileDerivativeFunc.Instructions)
						sb.AppendLine(instr.ToString());

					tbDerivativeIlCode.Text = sb.ToString(); 
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
			Helper.InitDerivatives(tbDerivatives.Text);
		}
	}
}
