using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MathExpressionsNET.GUI
{
	public partial class frmMain : Form
	{
		const long TimerDelay = 1000;
		MathFuncAssemblyCecil Assembly;
		System.Threading.Timer UpdateTimer;
		string SettingsFilePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"MathExpressions.NET.xml");
		static Settings Settings = new Settings();

		public frmMain()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			UpdateTimer = new System.Threading.Timer(_ => UpdateResult());

			InitializeComponent();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			Icon = new Icon("icon_small.ico");

			if (!Settings.TryLoad(SettingsFilePath, out Settings))
			{
				string currentDir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
				Settings = new Settings
				{
					Derivatives = File.ReadAllText(Path.Combine(currentDir, "DefaultDerivatives.txt"))
				};
			}

			tbDerivatives.Text = Settings.Derivatives;
			tbInput.Text = Settings.InputExpression;
			cbRealTimeUpdate.Checked = Settings.RealTimeUpdate;
			btnRebuildDerivatives_Click(null, null);
		}

		private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			Settings.InputExpression = tbInput.Text;
			Settings.TrySave(SettingsFilePath);
		}

		private void tbInput_TextChanged(object sender, EventArgs e)
		{
			if (cbRealTimeUpdate.Checked)
				UpdateTimer.Change(TimerDelay, Timeout.Infinite);
		}

		private void dgvErrors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			if (int.TryParse(dgvErrors[0, e.RowIndex].Value.ToString(), out int pos))
			{
				tbInput.Select(pos, 0);
				tbInput.Focus();
			}
		}

		private void btnRebuildDerivatives_Click(object sender, EventArgs e)
		{
			//try
			{
				Helper.InitDerivatives(tbDerivatives.Text);
				btnCalculate_Click(sender, e);
				Settings.Derivatives = tbDerivatives.Text;
				Settings.TrySave(SettingsFilePath);
			}
			/*catch (Exception ex)
			{
				var parserErrors = Helper.Parser.Errors;
				if (parserErrors.Count != 0)
					MessageBox.Show(string.Format("Threa are errors in derivatives list: {0} at position {1}",
						Helper.Parser.Errors.First().Message, Helper.Parser.Errors.First().Position));
				else
					MessageBox.Show("Derivatives: " + ex.Message);
			}*/
		}

		private void cbRealTimeUpdate_CheckedChanged(object sender, EventArgs e)
		{
			Settings.RealTimeUpdate = cbRealTimeUpdate.Checked;
			Settings.TrySave(SettingsFilePath);

			if (cbRealTimeUpdate.Checked)
				UpdateTimer.Change(TimerDelay, Timeout.Infinite);
		}

		private void btnCalculate_Click(object sender, EventArgs e)
		{
			UpdateResult();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if (Assembly != null)
					Assembly.SaveToFile(Path.GetDirectoryName(saveFileDialog1.FileName), Path.GetFileName(saveFileDialog1.FileName));
			}
		}

		private void btnGenerateFunc_Click(object sender, EventArgs e)
		{
			MathFuncGenerator generator = new MathFuncGenerator();
			var func = generator.Generate(tbVar.Text, new string[] { "a", "b" }, null);
			tbInput.Text = func.ToString().Replace("√", "sqrt");
		}

		private void tbIlCode_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control && e.KeyCode == Keys.A)
			{
				((TextBox)sender).SelectAll();
				e.SuppressKeyPress = true;
			}
		}

		private void UpdateResult()
		{
			this.Invoke(new Action(() =>
			{
				dgvErrors.Rows.Clear();

				Assembly = new MathFuncAssemblyCecil();
				Assembly.Init();

				var variable = string.IsNullOrEmpty(tbVar.Text) ? null : new VarNode(tbVar.Text.ToLowerInvariant());

				string input = tbInput.Text.Replace(Environment.NewLine, "");
				MathFunc simplifiedFunc = null;
				try
				{
					simplifiedFunc = new MathFunc(input, tbVar.Text).Simplify();
					tbSimplification.Text = simplifiedFunc.ToString();
					tbSimplifiedOpt.Text = simplifiedFunc.GetPrecompilied().ToString();
				}
				catch (Exception ex)
				{
					dgvErrors.Rows.Add(string.Empty, ex.Message);
					//foreach (var error in Helper.Parser.Errors)
					//	dgvErrors.Rows.Add(error.Position == null ? string.Empty : error.Position.Column.ToString(), error.Message);
					tbSimplification.Text = null;
					tbSimplifiedOpt.Text = null;
					tbDerivative.Text = null;
					tbDerivativeOpt.Text = null;
					tbIlCode.Text = null;
					tbDerivativeIlCode.Text = null;
				}

				try
				{
					var compileFunc = new MathFunc(input, tbVar.Text, true, true);
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
						derivativeFunc = new MathFunc(input, tbVar.Text).GetDerivative();
						tbDerivative.Text = derivativeFunc.ToString();
						tbDerivativeOpt.Text = derivativeFunc.GetPrecompilied().ToString();
					}
					catch (Exception ex)
					{
						dgvErrors.Rows.Add(string.Empty, ex.Message);
						//foreach (var error in Helper.Parser.Errors)
						//	dgvErrors.Rows.Add(error.Position == null ? string.Empty : error.Position.Column.ToString(), error.Message);
						tbDerivative.Text = null;
						tbDerivativeOpt.Text = null;
						tbDerivativeIlCode.Text = null;
					}

					try
					{
						var compileDerivativeFunc = new MathFunc(tbDerivative.Text, tbVar.Text, true, true);
						compileDerivativeFunc.Compile(Assembly, "FuncDerivative");
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
			}));
		}
	}
}