namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    partial class UpdateSettingProgressBarForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateSettingProgressBarForm));
            this.UpdateSettingProgressBar = new System.Windows.Forms.ProgressBar();
            this.ProgressBarLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UpdateSettingProgressBar
            // 
            this.UpdateSettingProgressBar.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.UpdateSettingProgressBar.Location = new System.Drawing.Point(34, 163);
            this.UpdateSettingProgressBar.Name = "UpdateSettingProgressBar";
            this.UpdateSettingProgressBar.Size = new System.Drawing.Size(375, 29);
            this.UpdateSettingProgressBar.TabIndex = 0;
            // 
            // ProgressBarLabel
            // 
            this.ProgressBarLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ProgressBarLabel.Font = new System.Drawing.Font("Roboto Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgressBarLabel.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.ProgressBarLabel.Location = new System.Drawing.Point(0, 85);
            this.ProgressBarLabel.Name = "ProgressBarLabel";
            this.ProgressBarLabel.Size = new System.Drawing.Size(450, 47);
            this.ProgressBarLabel.TabIndex = 1;
            this.ProgressBarLabel.Text = "ProgressLabel";
            this.ProgressBarLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateSettingProgressBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(64)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(450, 250);
            this.Controls.Add(this.ProgressBarLabel);
            this.Controls.Add(this.UpdateSettingProgressBar);
            this.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UpdateSettingProgressBarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar UpdateSettingProgressBar;
        private System.Windows.Forms.Label ProgressBarLabel;
    }
}