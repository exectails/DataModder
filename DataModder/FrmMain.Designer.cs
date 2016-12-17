namespace DataModder
{
	partial class FrmMain
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
			this.TxtTrace = new System.Windows.Forms.TextBox();
			this.BtnCreateData = new System.Windows.Forms.Button();
			this.BtnModify = new System.Windows.Forms.Button();
			this.BtnRemovMods = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TxtTrace
			// 
			this.TxtTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TxtTrace.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TxtTrace.Location = new System.Drawing.Point(12, 12);
			this.TxtTrace.Multiline = true;
			this.TxtTrace.Name = "TxtTrace";
			this.TxtTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TxtTrace.Size = new System.Drawing.Size(621, 397);
			this.TxtTrace.TabIndex = 0;
			this.TxtTrace.TabStop = false;
			this.TxtTrace.WordWrap = false;
			// 
			// BtnCreateData
			// 
			this.BtnCreateData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnCreateData.Location = new System.Drawing.Point(12, 415);
			this.BtnCreateData.Name = "BtnCreateData";
			this.BtnCreateData.Size = new System.Drawing.Size(125, 23);
			this.BtnCreateData.TabIndex = 1;
			this.BtnCreateData.Text = "Create data folder";
			this.BtnCreateData.UseVisualStyleBackColor = true;
			this.BtnCreateData.Click += new System.EventHandler(this.BtnCreateData_Click);
			// 
			// BtnModify
			// 
			this.BtnModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnModify.Location = new System.Drawing.Point(407, 415);
			this.BtnModify.Name = "BtnModify";
			this.BtnModify.Size = new System.Drawing.Size(110, 23);
			this.BtnModify.TabIndex = 2;
			this.BtnModify.Text = "Modify";
			this.BtnModify.UseVisualStyleBackColor = true;
			this.BtnModify.Click += new System.EventHandler(this.BtnEnableDataFiles_Click);
			// 
			// BtnRemovMods
			// 
			this.BtnRemovMods.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnRemovMods.Location = new System.Drawing.Point(523, 415);
			this.BtnRemovMods.Name = "BtnRemovMods";
			this.BtnRemovMods.Size = new System.Drawing.Size(110, 23);
			this.BtnRemovMods.TabIndex = 3;
			this.BtnRemovMods.Text = "Remove Mods";
			this.BtnRemovMods.UseVisualStyleBackColor = true;
			this.BtnRemovMods.Click += new System.EventHandler(this.BtnRemovMods_Click);
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(645, 450);
			this.Controls.Add(this.BtnRemovMods);
			this.Controls.Add(this.BtnModify);
			this.Controls.Add(this.BtnCreateData);
			this.Controls.Add(this.TxtTrace);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FrmMain";
			this.Text = "DataModder";
			this.Load += new System.EventHandler(this.FrmMain_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox TxtTrace;
		private System.Windows.Forms.Button BtnCreateData;
		private System.Windows.Forms.Button BtnModify;
		private System.Windows.Forms.Button BtnRemovMods;
	}
}