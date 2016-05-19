namespace Shuttle.TenPinBowling.Shell
{
	partial class Frame
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.TitleDisplay = new System.Windows.Forms.Label();
            this.Roll1Pins = new System.Windows.Forms.Label();
            this.Roll2Pins = new System.Windows.Forms.Label();
            this.Roll3Pins = new System.Windows.Forms.Label();
            this.ScoreDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TitleDisplay
            // 
            this.TitleDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TitleDisplay.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitleDisplay.Location = new System.Drawing.Point(2, 2);
            this.TitleDisplay.Name = "TitleDisplay";
            this.TitleDisplay.Padding = new System.Windows.Forms.Padding(5);
            this.TitleDisplay.Size = new System.Drawing.Size(153, 31);
            this.TitleDisplay.TabIndex = 0;
            this.TitleDisplay.Text = "Frame";
            this.TitleDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Roll1Pins
            // 
            this.Roll1Pins.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Roll1Pins.Location = new System.Drawing.Point(2, 42);
            this.Roll1Pins.Name = "Roll1Pins";
            this.Roll1Pins.Size = new System.Drawing.Size(47, 32);
            this.Roll1Pins.TabIndex = 1;
            this.Roll1Pins.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Roll2Pins
            // 
            this.Roll2Pins.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Roll2Pins.Location = new System.Drawing.Point(55, 42);
            this.Roll2Pins.Name = "Roll2Pins";
            this.Roll2Pins.Size = new System.Drawing.Size(47, 32);
            this.Roll2Pins.TabIndex = 2;
            this.Roll2Pins.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Roll3Pins
            // 
            this.Roll3Pins.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Roll3Pins.Location = new System.Drawing.Point(108, 42);
            this.Roll3Pins.Name = "Roll3Pins";
            this.Roll3Pins.Size = new System.Drawing.Size(47, 32);
            this.Roll3Pins.TabIndex = 3;
            this.Roll3Pins.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Roll3Pins.Visible = false;
            // 
            // ScoreDisplay
            // 
            this.ScoreDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScoreDisplay.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ScoreDisplay.Location = new System.Drawing.Point(2, 82);
            this.ScoreDisplay.Name = "ScoreDisplay";
            this.ScoreDisplay.Padding = new System.Windows.Forms.Padding(5);
            this.ScoreDisplay.Size = new System.Drawing.Size(153, 31);
            this.ScoreDisplay.TabIndex = 4;
            this.ScoreDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Frame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.ScoreDisplay);
            this.Controls.Add(this.Roll3Pins);
            this.Controls.Add(this.Roll2Pins);
            this.Controls.Add(this.Roll1Pins);
            this.Controls.Add(this.TitleDisplay);
            this.Name = "Frame";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Size = new System.Drawing.Size(157, 115);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label TitleDisplay;
		private System.Windows.Forms.Label Roll1Pins;
		private System.Windows.Forms.Label Roll2Pins;
		private System.Windows.Forms.Label Roll3Pins;
		private System.Windows.Forms.Label ScoreDisplay;
	}
}
