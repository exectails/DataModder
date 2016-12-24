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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.BtnAllInOne = new System.Windows.Forms.Button();
			this.LblPatchingNote = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// TxtTrace
			// 
			this.TxtTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TxtTrace.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TxtTrace.Location = new System.Drawing.Point(12, 212);
			this.TxtTrace.Multiline = true;
			this.TxtTrace.Name = "TxtTrace";
			this.TxtTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.TxtTrace.Size = new System.Drawing.Size(412, 236);
			this.TxtTrace.TabIndex = 0;
			this.TxtTrace.TabStop = false;
			this.TxtTrace.WordWrap = false;
			// 
			// BtnCreateData
			// 
			this.BtnCreateData.Location = new System.Drawing.Point(29, 132);
			this.BtnCreateData.Name = "BtnCreateData";
			this.BtnCreateData.Size = new System.Drawing.Size(208, 23);
			this.BtnCreateData.TabIndex = 1;
			this.BtnCreateData.Text = "Create data folder";
			this.BtnCreateData.UseVisualStyleBackColor = true;
			this.BtnCreateData.Click += new System.EventHandler(this.BtnCreateData_Click);
			// 
			// BtnModify
			// 
			this.BtnModify.Location = new System.Drawing.Point(136, 103);
			this.BtnModify.Name = "BtnModify";
			this.BtnModify.Size = new System.Drawing.Size(101, 23);
			this.BtnModify.TabIndex = 2;
			this.BtnModify.Text = "Modify";
			this.BtnModify.UseVisualStyleBackColor = true;
			this.BtnModify.Click += new System.EventHandler(this.BtnModify_Click);
			// 
			// BtnRemovMods
			// 
			this.BtnRemovMods.Location = new System.Drawing.Point(29, 103);
			this.BtnRemovMods.Name = "BtnRemovMods";
			this.BtnRemovMods.Size = new System.Drawing.Size(101, 23);
			this.BtnRemovMods.TabIndex = 3;
			this.BtnRemovMods.Text = "Remove Mods";
			this.BtnRemovMods.UseVisualStyleBackColor = true;
			this.BtnRemovMods.Click += new System.EventHandler(this.BtnRemovMods_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Gray;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(386, 73);
			this.label1.TabIndex = 5;
			this.label1.Text = "DataModder";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.DimGray;
			this.label2.Location = new System.Drawing.Point(245, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(178, 42);
			this.label2.TabIndex = 6;
			this.label2.Text = "Prototype";
			// 
			// BtnAllInOne
			// 
			this.BtnAllInOne.Location = new System.Drawing.Point(29, 161);
			this.BtnAllInOne.Name = "BtnAllInOne";
			this.BtnAllInOne.Size = new System.Drawing.Size(208, 23);
			this.BtnAllInOne.TabIndex = 7;
			this.BtnAllInOne.Text = "All in one";
			this.BtnAllInOne.UseVisualStyleBackColor = true;
			this.BtnAllInOne.Click += new System.EventHandler(this.BtnAllInOne_Click);
			// 
			// LblPatchingNote
			// 
			this.LblPatchingNote.AutoSize = true;
			this.LblPatchingNote.Location = new System.Drawing.Point(249, 129);
			this.LblPatchingNote.Name = "LblPatchingNote";
			this.LblPatchingNote.Size = new System.Drawing.Size(163, 52);
			this.LblPatchingNote.TabIndex = 8;
			this.LblPatchingNote.Text = "Move DataModder to your non\r\nDATA packer modded Mabinogi\r\nfolder to enable patchi" +
    "ng of pack\r\nfiles.";
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(436, 460);
			this.Controls.Add(this.LblPatchingNote);
			this.Controls.Add(this.BtnAllInOne);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.BtnRemovMods);
			this.Controls.Add(this.BtnModify);
			this.Controls.Add(this.BtnCreateData);
			this.Controls.Add(this.TxtTrace);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FrmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button BtnAllInOne;
		private System.Windows.Forms.Label LblPatchingNote;
	}
}