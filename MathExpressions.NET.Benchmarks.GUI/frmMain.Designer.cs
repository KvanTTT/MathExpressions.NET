namespace MathPowVsMultTest.Gui
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
			this.btnCalculate = new System.Windows.Forms.Button();
			this.udIterCount = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.udExpIterCount = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.tbExpBase = new System.Windows.Forms.TextBox();
			this.cbMathPow = new System.Windows.Forms.CheckBox();
			this.cbIntPow = new System.Windows.Forms.CheckBox();
			this.cbFastPow = new System.Windows.Forms.CheckBox();
			this.cbApprox = new System.Windows.Forms.CheckBox();
			this.cbPrecise = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.udApproxPolygonPow = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.udIterCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.udExpIterCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.udApproxPolygonPow)).BeginInit();
			this.SuspendLayout();
			// 
			// zedGraphControl1
			// 
			this.zedGraphControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.zedGraphControl1.Location = new System.Drawing.Point(12, 12);
			this.zedGraphControl1.Name = "zedGraphControl1";
			this.zedGraphControl1.ScrollGrace = 0D;
			this.zedGraphControl1.ScrollMaxX = 0D;
			this.zedGraphControl1.ScrollMaxY = 0D;
			this.zedGraphControl1.ScrollMaxY2 = 0D;
			this.zedGraphControl1.ScrollMinX = 0D;
			this.zedGraphControl1.ScrollMinY = 0D;
			this.zedGraphControl1.ScrollMinY2 = 0D;
			this.zedGraphControl1.Size = new System.Drawing.Size(780, 384);
			this.zedGraphControl1.TabIndex = 0;
			// 
			// btnCalculate
			// 
			this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCalculate.Location = new System.Drawing.Point(656, 432);
			this.btnCalculate.Name = "btnCalculate";
			this.btnCalculate.Size = new System.Drawing.Size(134, 40);
			this.btnCalculate.TabIndex = 1;
			this.btnCalculate.Text = "Recalculate";
			this.btnCalculate.UseVisualStyleBackColor = true;
			this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
			// 
			// udIterCount
			// 
			this.udIterCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.udIterCount.Location = new System.Drawing.Point(100, 468);
			this.udIterCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.udIterCount.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
			this.udIterCount.Name = "udIterCount";
			this.udIterCount.Size = new System.Drawing.Size(120, 20);
			this.udIterCount.TabIndex = 2;
			this.udIterCount.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 470);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Iter count";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(22, 444);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Exp iter count";
			// 
			// udExpIterCount
			// 
			this.udExpIterCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.udExpIterCount.Location = new System.Drawing.Point(100, 442);
			this.udExpIterCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
			this.udExpIterCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.udExpIterCount.Name = "udExpIterCount";
			this.udExpIterCount.Size = new System.Drawing.Size(120, 20);
			this.udExpIterCount.TabIndex = 4;
			this.udExpIterCount.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(22, 416);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(51, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Exp base";
			// 
			// tbExpBase
			// 
			this.tbExpBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.tbExpBase.Location = new System.Drawing.Point(100, 416);
			this.tbExpBase.Name = "tbExpBase";
			this.tbExpBase.Size = new System.Drawing.Size(120, 20);
			this.tbExpBase.TabIndex = 8;
			this.tbExpBase.Text = "1.1";
			// 
			// cbMathPow
			// 
			this.cbMathPow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbMathPow.AutoSize = true;
			this.cbMathPow.Checked = true;
			this.cbMathPow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbMathPow.Location = new System.Drawing.Point(308, 419);
			this.cbMathPow.Name = "cbMathPow";
			this.cbMathPow.Size = new System.Drawing.Size(74, 17);
			this.cbMathPow.TabIndex = 9;
			this.cbMathPow.Text = "Math.Pow";
			this.cbMathPow.UseVisualStyleBackColor = true;
			this.cbMathPow.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// cbIntPow
			// 
			this.cbIntPow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbIntPow.AutoSize = true;
			this.cbIntPow.Checked = true;
			this.cbIntPow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbIntPow.Location = new System.Drawing.Point(308, 445);
			this.cbIntPow.Name = "cbIntPow";
			this.cbIntPow.Size = new System.Drawing.Size(62, 17);
			this.cbIntPow.TabIndex = 10;
			this.cbIntPow.Text = "Int Pow";
			this.cbIntPow.UseVisualStyleBackColor = true;
			this.cbIntPow.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// cbFastPow
			// 
			this.cbFastPow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbFastPow.AutoSize = true;
			this.cbFastPow.Checked = true;
			this.cbFastPow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFastPow.Location = new System.Drawing.Point(308, 470);
			this.cbFastPow.Name = "cbFastPow";
			this.cbFastPow.Size = new System.Drawing.Size(70, 17);
			this.cbFastPow.TabIndex = 11;
			this.cbFastPow.Text = "Fast Pow";
			this.cbFastPow.UseVisualStyleBackColor = true;
			this.cbFastPow.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// cbApprox
			// 
			this.cbApprox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbApprox.AutoSize = true;
			this.cbApprox.Checked = true;
			this.cbApprox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbApprox.Location = new System.Drawing.Point(419, 445);
			this.cbApprox.Name = "cbApprox";
			this.cbApprox.Size = new System.Drawing.Size(59, 17);
			this.cbApprox.TabIndex = 13;
			this.cbApprox.Text = "Approx";
			this.cbApprox.UseVisualStyleBackColor = true;
			this.cbApprox.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// cbPrecise
			// 
			this.cbPrecise.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbPrecise.AutoSize = true;
			this.cbPrecise.Checked = true;
			this.cbPrecise.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbPrecise.Location = new System.Drawing.Point(419, 419);
			this.cbPrecise.Name = "cbPrecise";
			this.cbPrecise.Size = new System.Drawing.Size(61, 17);
			this.cbPrecise.TabIndex = 12;
			this.cbPrecise.Text = "Precise";
			this.cbPrecise.UseVisualStyleBackColor = true;
			this.cbPrecise.CheckedChanged += new System.EventHandler(this.checkBox_CheckedChanged);
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(416, 471);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(103, 13);
			this.label4.TabIndex = 15;
			this.label4.Text = "Approx polygon pow";
			// 
			// udApproxPolygonPow
			// 
			this.udApproxPolygonPow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.udApproxPolygonPow.Location = new System.Drawing.Point(525, 467);
			this.udApproxPolygonPow.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.udApproxPolygonPow.Name = "udApproxPolygonPow";
			this.udApproxPolygonPow.Size = new System.Drawing.Size(78, 20);
			this.udApproxPolygonPow.TabIndex = 14;
			this.udApproxPolygonPow.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(804, 513);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.udApproxPolygonPow);
			this.Controls.Add(this.cbApprox);
			this.Controls.Add(this.cbPrecise);
			this.Controls.Add(this.cbFastPow);
			this.Controls.Add(this.cbIntPow);
			this.Controls.Add(this.cbMathPow);
			this.Controls.Add(this.tbExpBase);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.udExpIterCount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.udIterCount);
			this.Controls.Add(this.btnCalculate);
			this.Controls.Add(this.zedGraphControl1);
			this.Name = "frmMain";
			this.Text = "Power benchmarks";
			this.Load += new System.EventHandler(this.frmMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.udIterCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.udExpIterCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.udApproxPolygonPow)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ZedGraph.ZedGraphControl zedGraphControl1;
		private System.Windows.Forms.Button btnCalculate;
		private System.Windows.Forms.NumericUpDown udIterCount;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown udExpIterCount;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbExpBase;
		private System.Windows.Forms.CheckBox cbMathPow;
		private System.Windows.Forms.CheckBox cbIntPow;
		private System.Windows.Forms.CheckBox cbFastPow;
		private System.Windows.Forms.CheckBox cbApprox;
		private System.Windows.Forms.CheckBox cbPrecise;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown udApproxPolygonPow;
	}
}

