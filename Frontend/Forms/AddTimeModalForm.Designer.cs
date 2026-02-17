namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    partial class AddTimeModalForm
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
            this.AddTimePanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DurationLabel = new System.Windows.Forms.Label();
            this.CancelSessionBtn = new System.Windows.Forms.Button();
            this.UpdateSessionBtn = new System.Windows.Forms.Button();
            this.DurationPanel = new System.Windows.Forms.Panel();
            this.DurationTb = new System.Windows.Forms.TextBox();
            this.MainAddModalPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddTimeBtn = new System.Windows.Forms.Button();
            this.AddTimePanel.SuspendLayout();
            this.DurationPanel.SuspendLayout();
            this.MainAddModalPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddTimePanel
            // 
            this.AddTimePanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AddTimePanel.Controls.Add(this.panel2);
            this.AddTimePanel.Controls.Add(this.DurationLabel);
            this.AddTimePanel.Controls.Add(this.CancelSessionBtn);
            this.AddTimePanel.Controls.Add(this.UpdateSessionBtn);
            this.AddTimePanel.Controls.Add(this.DurationPanel);
            this.AddTimePanel.Location = new System.Drawing.Point(0, 1);
            this.AddTimePanel.Name = "AddTimePanel";
            this.AddTimePanel.Size = new System.Drawing.Size(532, 336);
            this.AddTimePanel.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(532, 49);
            this.panel2.TabIndex = 10;
            // 
            // DurationLabel
            // 
            this.DurationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.DurationLabel.AutoSize = true;
            this.DurationLabel.Location = new System.Drawing.Point(137, 91);
            this.DurationLabel.Name = "DurationLabel";
            this.DurationLabel.Size = new System.Drawing.Size(92, 25);
            this.DurationLabel.TabIndex = 9;
            this.DurationLabel.Text = "Duration";
            // 
            // CancelSessionBtn
            // 
            this.CancelSessionBtn.Location = new System.Drawing.Point(97, 234);
            this.CancelSessionBtn.Name = "CancelSessionBtn";
            this.CancelSessionBtn.Size = new System.Drawing.Size(132, 57);
            this.CancelSessionBtn.TabIndex = 8;
            this.CancelSessionBtn.Text = "Cancel";
            this.CancelSessionBtn.UseVisualStyleBackColor = true;
            this.CancelSessionBtn.Click += new System.EventHandler(this.CancelSessionBtn_Click_2);
            // 
            // UpdateSessionBtn
            // 
            this.UpdateSessionBtn.Location = new System.Drawing.Point(283, 234);
            this.UpdateSessionBtn.Name = "UpdateSessionBtn";
            this.UpdateSessionBtn.Size = new System.Drawing.Size(132, 57);
            this.UpdateSessionBtn.TabIndex = 7;
            this.UpdateSessionBtn.Text = "Update";
            this.UpdateSessionBtn.UseVisualStyleBackColor = true;
            this.UpdateSessionBtn.Click += new System.EventHandler(this.UpdateSessionBtn_Click);
            // 
            // DurationPanel
            // 
            this.DurationPanel.Controls.Add(this.DurationTb);
            this.DurationPanel.Location = new System.Drawing.Point(97, 139);
            this.DurationPanel.Name = "DurationPanel";
            this.DurationPanel.Size = new System.Drawing.Size(318, 69);
            this.DurationPanel.TabIndex = 6;
            // 
            // DurationTb
            // 
            this.DurationTb.Font = new System.Drawing.Font("Roboto", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DurationTb.Location = new System.Drawing.Point(6, 15);
            this.DurationTb.Margin = new System.Windows.Forms.Padding(6);
            this.DurationTb.Name = "DurationTb";
            this.DurationTb.Size = new System.Drawing.Size(306, 42);
            this.DurationTb.TabIndex = 0;
            // 
            // MainAddModalPanel
            // 
            this.MainAddModalPanel.Controls.Add(this.panel1);
            this.MainAddModalPanel.Controls.Add(this.AddTimeBtn);
            this.MainAddModalPanel.Location = new System.Drawing.Point(1, 0);
            this.MainAddModalPanel.Name = "MainAddModalPanel";
            this.MainAddModalPanel.Size = new System.Drawing.Size(530, 336);
            this.MainAddModalPanel.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(530, 47);
            this.panel1.TabIndex = 2;
            // 
            // AddTimeBtn
            // 
            this.AddTimeBtn.Location = new System.Drawing.Point(177, 120);
            this.AddTimeBtn.Name = "AddTimeBtn";
            this.AddTimeBtn.Size = new System.Drawing.Size(159, 77);
            this.AddTimeBtn.TabIndex = 1;
            this.AddTimeBtn.Text = "Add Time";
            this.AddTimeBtn.UseVisualStyleBackColor = true;
            this.AddTimeBtn.Click += new System.EventHandler(this.AddTimeBtn_Click);
            // 
            // AddTimeModalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 336);
            this.Controls.Add(this.AddTimePanel);
            this.Controls.Add(this.MainAddModalPanel);
            this.Font = new System.Drawing.Font("Roboto", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "AddTimeModalForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddTimeModalForm_FormClosed);
            this.AddTimePanel.ResumeLayout(false);
            this.AddTimePanel.PerformLayout();
            this.DurationPanel.ResumeLayout(false);
            this.DurationPanel.PerformLayout();
            this.MainAddModalPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel AddTimePanel;
        private System.Windows.Forms.Label DurationLabel;
        private System.Windows.Forms.Button CancelSessionBtn;
        private System.Windows.Forms.Button UpdateSessionBtn;
        private System.Windows.Forms.Panel DurationPanel;
        private System.Windows.Forms.TextBox DurationTb;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel MainAddModalPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button AddTimeBtn;
    }
}