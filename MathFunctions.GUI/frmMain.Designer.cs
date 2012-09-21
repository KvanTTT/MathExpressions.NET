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
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.label4 = new System.Windows.Forms.Label();
			this.dgvErrors = new System.Windows.Forms.DataGridView();
			this.clnPos = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.clnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.tbIlCode = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tbSimplification = new System.Windows.Forms.TextBox();
			this.tbDerivative = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.tbInput = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
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
			this.splitContainer3.Size = new System.Drawing.Size(952, 586);
			this.splitContainer3.SplitterDistance = 419;
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
			this.splitContainer2.Size = new System.Drawing.Size(952, 419);
			this.splitContainer2.SplitterDistance = 571;
			this.splitContainer2.TabIndex = 11;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 15);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(44, 13);
			this.label4.TabIndex = 11;
			this.label4.Text = "IL Code";
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
			this.dgvErrors.Size = new System.Drawing.Size(952, 163);
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
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.label4);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.tbIlCode);
			this.splitContainer4.Size = new System.Drawing.Size(377, 419);
			this.splitContainer4.SplitterDistance = 51;
			this.splitContainer4.TabIndex = 12;
			// 
			// tbIlCode
			// 
			this.tbIlCode.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbIlCode.Location = new System.Drawing.Point(0, 0);
			this.tbIlCode.Multiline = true;
			this.tbIlCode.Name = "tbIlCode";
			this.tbIlCode.ReadOnly = true;
			this.tbIlCode.Size = new System.Drawing.Size(377, 364);
			this.tbIlCode.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(20, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 22;
			this.label2.Text = "Simplification";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(20, 57);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 21;
			this.label1.Text = "Derivative";
			// 
			// tbSimplification
			// 
			this.tbSimplification.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSimplification.Location = new System.Drawing.Point(94, 83);
			this.tbSimplification.Name = "tbSimplification";
			this.tbSimplification.ReadOnly = true;
			this.tbSimplification.Size = new System.Drawing.Size(457, 20);
			this.tbSimplification.TabIndex = 20;
			// 
			// tbDerivative
			// 
			this.tbDerivative.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDerivative.Location = new System.Drawing.Point(94, 57);
			this.tbDerivative.Name = "tbDerivative";
			this.tbDerivative.ReadOnly = true;
			this.tbDerivative.Size = new System.Drawing.Size(457, 20);
			this.tbDerivative.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(20, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(31, 13);
			this.label3.TabIndex = 18;
			this.label3.Text = "Input";
			// 
			// tbInput
			// 
			this.tbInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbInput.Location = new System.Drawing.Point(57, 24);
			this.tbInput.Name = "tbInput";
			this.tbInput.Size = new System.Drawing.Size(494, 20);
			this.tbInput.TabIndex = 17;
			this.tbInput.Text = "2 + 2 * 2";
			this.tbInput.TextChanged += new System.EventHandler(this.tbInput_TextChanged);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(952, 586);
			this.Controls.Add(this.splitContainer3);
			this.Name = "frmMain";
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
			((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel1.PerformLayout();
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
			this.splitContainer4.ResumeLayout(false);
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
		private System.Windows.Forms.TextBox tbIlCode;



	}
}

