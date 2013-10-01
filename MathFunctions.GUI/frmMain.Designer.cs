namespace MathFunctions.GUI
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.tbVar = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnCalculate = new System.Windows.Forms.Button();
			this.cbRealTimeUpdate = new System.Windows.Forms.CheckBox();
			this.btnRebuildDerivatives = new System.Windows.Forms.Button();
			this.tbDerivatives = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tbSimplification = new System.Windows.Forms.TextBox();
			this.tbDerivative = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbInput = new System.Windows.Forms.TextBox();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.label4 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tbIlCode = new System.Windows.Forms.TextBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tbDerivativeIlCode = new System.Windows.Forms.TextBox();
			this.dgvErrors = new System.Windows.Forms.DataGridView();
			this.clnPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.clnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tbDerivativeDelta = new System.Windows.Forms.TextBox();
			this.btnSave = new System.Windows.Forms.Button();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.dgvErrors);
			this.splitContainer3.Size = new System.Drawing.Size(949, 613);
			this.splitContainer3.SplitterDistance = 461;
			this.splitContainer3.TabIndex = 13;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.btnSave);
			this.splitContainer2.Panel1.Controls.Add(this.tbVar);
			this.splitContainer2.Panel1.Controls.Add(this.label5);
			this.splitContainer2.Panel1.Controls.Add(this.btnCalculate);
			this.splitContainer2.Panel1.Controls.Add(this.cbRealTimeUpdate);
			this.splitContainer2.Panel1.Controls.Add(this.btnRebuildDerivatives);
			this.splitContainer2.Panel1.Controls.Add(this.tbDerivatives);
			this.splitContainer2.Panel1.Controls.Add(this.label2);
			this.splitContainer2.Panel1.Controls.Add(this.label1);
			this.splitContainer2.Panel1.Controls.Add(this.tbSimplification);
			this.splitContainer2.Panel1.Controls.Add(this.tbDerivative);
			this.splitContainer2.Panel1.Controls.Add(this.label3);
			this.splitContainer2.Panel1.Controls.Add(this.tbInput);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer4);
			this.splitContainer2.Size = new System.Drawing.Size(949, 461);
			this.splitContainer2.SplitterDistance = 599;
			this.splitContainer2.TabIndex = 11;
			// 
			// tbVar
			// 
			this.tbVar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tbVar.Location = new System.Drawing.Point(294, 369);
			this.tbVar.Name = "tbVar";
			this.tbVar.Size = new System.Drawing.Size(51, 20);
			this.tbVar.TabIndex = 28;
			this.tbVar.Text = "x";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label5.Location = new System.Drawing.Point(265, 373);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(23, 13);
			this.label5.TabIndex = 27;
			this.label5.Text = "Var";
			// 
			// btnCalculate
			// 
			this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCalculate.Location = new System.Drawing.Point(383, 366);
			this.btnCalculate.Name = "btnCalculate";
			this.btnCalculate.Size = new System.Drawing.Size(77, 26);
			this.btnCalculate.TabIndex = 26;
			this.btnCalculate.Text = "Calculate";
			this.btnCalculate.UseVisualStyleBackColor = true;
			this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
			// 
			// cbRealTimeUpdate
			// 
			this.cbRealTimeUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cbRealTimeUpdate.AutoSize = true;
			this.cbRealTimeUpdate.Checked = true;
			this.cbRealTimeUpdate.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbRealTimeUpdate.Location = new System.Drawing.Point(118, 372);
			this.cbRealTimeUpdate.Name = "cbRealTimeUpdate";
			this.cbRealTimeUpdate.Size = new System.Drawing.Size(112, 17);
			this.cbRealTimeUpdate.TabIndex = 25;
			this.cbRealTimeUpdate.Text = "Real Time Update";
			this.cbRealTimeUpdate.UseVisualStyleBackColor = true;
			this.cbRealTimeUpdate.CheckedChanged += new System.EventHandler(this.cbRealTimeUpdate_CheckedChanged);
			// 
			// btnRebuildDerivatives
			// 
			this.btnRebuildDerivatives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRebuildDerivatives.Location = new System.Drawing.Point(487, 336);
			this.btnRebuildDerivatives.Name = "btnRebuildDerivatives";
			this.btnRebuildDerivatives.Size = new System.Drawing.Size(87, 26);
			this.btnRebuildDerivatives.TabIndex = 24;
			this.btnRebuildDerivatives.Text = "Rebuild derivatives";
			this.btnRebuildDerivatives.UseVisualStyleBackColor = true;
			this.btnRebuildDerivatives.Click += new System.EventHandler(this.btnRebuildDerivatives_Click);
			// 
			// tbDerivatives
			// 
			this.tbDerivatives.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDerivatives.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbDerivatives.Location = new System.Drawing.Point(12, 12);
			this.tbDerivatives.Multiline = true;
			this.tbDerivatives.Name = "tbDerivatives";
			this.tbDerivatives.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDerivatives.Size = new System.Drawing.Size(562, 311);
			this.tbDerivatives.TabIndex = 23;
			this.tbDerivatives.Text = resources.GetString("tbDerivatives.Text");
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.Location = new System.Drawing.Point(15, 399);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 16);
			this.label2.TabIndex = 22;
			this.label2.Text = "Simplification";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(15, 428);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 16);
			this.label1.TabIndex = 21;
			this.label1.Text = "Derivative";
			// 
			// tbSimplification
			// 
			this.tbSimplification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSimplification.Location = new System.Drawing.Point(118, 398);
			this.tbSimplification.Name = "tbSimplification";
			this.tbSimplification.ReadOnly = true;
			this.tbSimplification.Size = new System.Drawing.Size(456, 20);
			this.tbSimplification.TabIndex = 20;
			// 
			// tbDerivative
			// 
			this.tbDerivative.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDerivative.Location = new System.Drawing.Point(118, 424);
			this.tbDerivative.Name = "tbDerivative";
			this.tbDerivative.ReadOnly = true;
			this.tbDerivative.Size = new System.Drawing.Size(456, 20);
			this.tbDerivative.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(13, 339);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(36, 16);
			this.label3.TabIndex = 18;
			this.label3.Text = "Input";
			// 
			// tbInput
			// 
			this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbInput.Location = new System.Drawing.Point(118, 340);
			this.tbInput.Name = "tbInput";
			this.tbInput.Size = new System.Drawing.Size(342, 20);
			this.tbInput.TabIndex = 17;
			this.tbInput.Text = "2 + 2 * 2";
			this.tbInput.TextChanged += new System.EventHandler(this.tbInput_TextChanged);
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.tbDerivativeDelta);
			this.splitContainer4.Panel1.Controls.Add(this.label4);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer4.Size = new System.Drawing.Size(346, 461);
			this.splitContainer4.SplitterDistance = 59;
			this.splitContainer4.TabIndex = 12;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label4.Location = new System.Drawing.Point(12, 15);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 16);
			this.label4.TabIndex = 11;
			this.label4.Text = "IL Code";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(346, 398);
			this.tabControl1.TabIndex = 2;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tbIlCode);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(338, 372);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Simplification";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tbIlCode
			// 
			this.tbIlCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbIlCode.Location = new System.Drawing.Point(3, 3);
			this.tbIlCode.Multiline = true;
			this.tbIlCode.Name = "tbIlCode";
			this.tbIlCode.ReadOnly = true;
			this.tbIlCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbIlCode.Size = new System.Drawing.Size(332, 366);
			this.tbIlCode.TabIndex = 2;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.tbDerivativeIlCode);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(367, 368);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Derivative";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// tbDerivativeIlCode
			// 
			this.tbDerivativeIlCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbDerivativeIlCode.Location = new System.Drawing.Point(3, 3);
			this.tbDerivativeIlCode.Multiline = true;
			this.tbDerivativeIlCode.Name = "tbDerivativeIlCode";
			this.tbDerivativeIlCode.ReadOnly = true;
			this.tbDerivativeIlCode.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbDerivativeIlCode.Size = new System.Drawing.Size(361, 362);
			this.tbDerivativeIlCode.TabIndex = 3;
			// 
			// dgvErrors
			// 
			this.dgvErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clnPos,
            this.clnDescription});
			this.dgvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dgvErrors.Location = new System.Drawing.Point(0, 0);
			this.dgvErrors.Name = "dgvErrors";
			this.dgvErrors.ReadOnly = true;
			this.dgvErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvErrors.Size = new System.Drawing.Size(949, 148);
			this.dgvErrors.TabIndex = 13;
			this.dgvErrors.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvErrors_CellDoubleClick);
			// 
			// clnPos
			// 
			this.clnPos.HeaderText = "Pos";
			this.clnPos.Name = "clnPos";
			this.clnPos.ReadOnly = true;
			this.clnPos.Width = 40;
			// 
			// clnDescription
			// 
			this.clnDescription.HeaderText = "Description";
			this.clnDescription.Name = "clnDescription";
			this.clnDescription.ReadOnly = true;
			this.clnDescription.Width = 500;
			// 
			// tbDerivativeDelta
			// 
			this.tbDerivativeDelta.Location = new System.Drawing.Point(89, 14);
			this.tbDerivativeDelta.Name = "tbDerivativeDelta";
			this.tbDerivativeDelta.Size = new System.Drawing.Size(100, 20);
			this.tbDerivativeDelta.TabIndex = 12;
			this.tbDerivativeDelta.Text = "0.000001";
			// 
			// btnSave
			// 
			this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSave.Location = new System.Drawing.Point(487, 366);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(86, 26);
			this.btnSave.TabIndex = 29;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.DefaultExt = "dll";
			this.saveFileDialog1.FileName = "MathFuncLib";
			this.saveFileDialog1.Filter = "Assemblies (*.dll)|*.dll";
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(949, 613);
			this.Controls.Add(this.splitContainer3);
			this.Name = "frmMain";
			this.Text = "Math func simplification & derivatives";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel1.PerformLayout();
			this.splitContainer4.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
			this.splitContainer4.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DataGridView dgvErrors;
		private System.Windows.Forms.DataGridViewTextBoxColumn clnPos;
		private System.Windows.Forms.DataGridViewTextBoxColumn clnDescription;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbSimplification;
		private System.Windows.Forms.TextBox tbDerivative;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbInput;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TextBox tbIlCode;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox tbDerivativeIlCode;
		private System.Windows.Forms.TextBox tbDerivatives;
		private System.Windows.Forms.Button btnRebuildDerivatives;
		private System.Windows.Forms.CheckBox cbRealTimeUpdate;
		private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.TextBox tbVar;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbDerivativeDelta;
        private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;



	}
}

