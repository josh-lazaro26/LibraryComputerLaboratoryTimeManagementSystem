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
            this.UpdateSettingProgressBar = new System.Windows.Forms.ProgressBar();
            this.ProgressBarLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UpdateSettingProgressBar
            // 
            this.UpdateSettingProgressBar.Location = new System.Drawing.Point(87, 163);
            this.UpdateSettingProgressBar.Name = "UpdateSettingProgressBar";
            this.UpdateSettingProgressBar.Size = new System.Drawing.Size(375, 23);
            this.UpdateSettingProgressBar.TabIndex = 0;
            // 
            // ProgressBarLabel
            // 
            this.ProgressBarLabel.AutoSize = true;
            this.ProgressBarLabel.Location = new System.Drawing.Point(250, 116);
            this.ProgressBarLabel.Name = "ProgressBarLabel";
            this.ProgressBarLabel.Size = new System.Drawing.Size(42, 18);
            this.ProgressBarLabel.TabIndex = 1;
            this.ProgressBarLabel.Text = "label1";
            // 
            // UpdateSettingProgressBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 249);
            this.Controls.Add(this.ProgressBarLabel);
            this.Controls.Add(this.UpdateSettingProgressBar);
            this.Font = new System.Drawing.Font("Roboto Condensed", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "UpdateSettingProgressBarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar UpdateSettingProgressBar;
        private System.Windows.Forms.Label ProgressBarLabel;
    }
}