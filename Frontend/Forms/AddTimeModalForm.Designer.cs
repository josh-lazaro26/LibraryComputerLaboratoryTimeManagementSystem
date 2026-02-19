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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddTimeModalForm));
            this.AddTimePanel = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.HeaderLabel = new System.Windows.Forms.Label();
            this.UpdateStudentTimeCloseBtn = new System.Windows.Forms.Button();
            this.DurationLabel = new System.Windows.Forms.Label();
            this.UpdateSessionBtn = new System.Windows.Forms.Button();
            this.DurationPanel = new System.Windows.Forms.Panel();
            this.HrAndMinPanelElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.HrAndMinPanel = new System.Windows.Forms.Panel();
            this.DurationTb = new System.Windows.Forms.TextBox();
            this.HourLabel = new System.Windows.Forms.Label();
            this.MinutesLabel = new System.Windows.Forms.Label();
            this.UpdateStudentTimeCloseBtnElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.DurationPanelElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.UpdateSessionBtnElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.AddTimeModalFormElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.AddTimePanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.DurationPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AddTimePanel
            // 
            this.AddTimePanel.Controls.Add(this.HourLabel);
            this.AddTimePanel.Controls.Add(this.HrAndMinPanel);
            this.AddTimePanel.Controls.Add(this.DurationLabel);
            this.AddTimePanel.Controls.Add(this.UpdateSessionBtn);
            this.AddTimePanel.Controls.Add(this.DurationPanel);
            this.AddTimePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddTimePanel.Location = new System.Drawing.Point(0, 0);
            this.AddTimePanel.Name = "AddTimePanel";
            this.AddTimePanel.Size = new System.Drawing.Size(532, 336);
            this.AddTimePanel.TabIndex = 6;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(64)))), ((int)(((byte)(43)))));
            this.panel2.Controls.Add(this.HeaderLabel);
            this.panel2.Controls.Add(this.UpdateStudentTimeCloseBtn);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(532, 61);
            this.panel2.TabIndex = 10;
            // 
            // HeaderLabel
            // 
            this.HeaderLabel.AutoSize = true;
            this.HeaderLabel.Font = new System.Drawing.Font("Roboto Condensed Light", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeaderLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.HeaderLabel.Location = new System.Drawing.Point(143, 9);
            this.HeaderLabel.Name = "HeaderLabel";
            this.HeaderLabel.Size = new System.Drawing.Size(250, 35);
            this.HeaderLabel.TabIndex = 11;
            this.HeaderLabel.Text = "Update Student Time";
            // 
            // UpdateStudentTimeCloseBtn
            // 
            this.UpdateStudentTimeCloseBtn.FlatAppearance.BorderSize = 0;
            this.UpdateStudentTimeCloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UpdateStudentTimeCloseBtn.Image = ((System.Drawing.Image)(resources.GetObject("UpdateStudentTimeCloseBtn.Image")));
            this.UpdateStudentTimeCloseBtn.Location = new System.Drawing.Point(481, 2);
            this.UpdateStudentTimeCloseBtn.Name = "UpdateStudentTimeCloseBtn";
            this.UpdateStudentTimeCloseBtn.Size = new System.Drawing.Size(49, 58);
            this.UpdateStudentTimeCloseBtn.TabIndex = 28;
            this.UpdateStudentTimeCloseBtn.UseVisualStyleBackColor = true;
            this.UpdateStudentTimeCloseBtn.Click += new System.EventHandler(this.UpdateStudentTimeCloseBtn_Click);
            // 
            // DurationLabel
            // 
            this.DurationLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.DurationLabel.Font = new System.Drawing.Font("Roboto Condensed Medium", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DurationLabel.Location = new System.Drawing.Point(0, 67);
            this.DurationLabel.Name = "DurationLabel";
            this.DurationLabel.Size = new System.Drawing.Size(532, 49);
            this.DurationLabel.TabIndex = 9;
            this.DurationLabel.Text = "Duration";
            this.DurationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateSessionBtn
            // 
            this.UpdateSessionBtn.Font = new System.Drawing.Font("Roboto", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateSessionBtn.Location = new System.Drawing.Point(140, 238);
            this.UpdateSessionBtn.Name = "UpdateSessionBtn";
            this.UpdateSessionBtn.Size = new System.Drawing.Size(244, 72);
            this.UpdateSessionBtn.TabIndex = 7;
            this.UpdateSessionBtn.Text = "Update";
            this.UpdateSessionBtn.UseVisualStyleBackColor = true;
            this.UpdateSessionBtn.Click += new System.EventHandler(this.UpdateSessionBtn_Click);
            // 
            // DurationPanel
            // 
            this.DurationPanel.BackColor = System.Drawing.SystemColors.Window;
            this.DurationPanel.Controls.Add(this.DurationTb);
            this.DurationPanel.Location = new System.Drawing.Point(149, 123);
            this.DurationPanel.Name = "DurationPanel";
            this.DurationPanel.Size = new System.Drawing.Size(108, 69);
            this.DurationPanel.TabIndex = 6;
            // 
            // HrAndMinPanelElipse
            // 
            this.HrAndMinPanelElipse.ElipseRadius = 10;
            this.HrAndMinPanelElipse.TargetControl = this.HrAndMinPanel;
            // 
            // HrAndMinPanel
            // 
            this.HrAndMinPanel.BackColor = System.Drawing.SystemColors.Window;
            this.HrAndMinPanel.Location = new System.Drawing.Point(276, 123);
            this.HrAndMinPanel.Name = "HrAndMinPanel";
            this.HrAndMinPanel.Size = new System.Drawing.Size(108, 69);
            this.HrAndMinPanel.TabIndex = 7;
            // 
            // DurationTb
            // 
            this.DurationTb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DurationTb.Font = new System.Drawing.Font("Roboto Condensed", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DurationTb.Location = new System.Drawing.Point(7, 15);
            this.DurationTb.Margin = new System.Windows.Forms.Padding(6);
            this.DurationTb.Name = "DurationTb";
            this.DurationTb.Size = new System.Drawing.Size(95, 39);
            this.DurationTb.TabIndex = 0;
            this.DurationTb.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DurationTb.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DurationTb_KeyPress);
            // 
            // HourLabel
            // 
            this.HourLabel.AutoSize = true;
            this.HourLabel.BackColor = System.Drawing.SystemColors.Window;
            this.HourLabel.Font = new System.Drawing.Font("Roboto Condensed", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HourLabel.Location = new System.Drawing.Point(290, 139);
            this.HourLabel.Name = "HourLabel";
            this.HourLabel.Size = new System.Drawing.Size(57, 38);
            this.HourLabel.TabIndex = 10;
            this.HourLabel.Text = "hrs";
            this.HourLabel.Click += new System.EventHandler(this.HourLabel_Click);
            // 
            // MinutesLabel
            // 
            this.MinutesLabel.AutoSize = true;
            this.MinutesLabel.BackColor = System.Drawing.Color.Transparent;
            this.MinutesLabel.Font = new System.Drawing.Font("Roboto Condensed", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinutesLabel.Location = new System.Drawing.Point(290, 197);
            this.MinutesLabel.Name = "MinutesLabel";
            this.MinutesLabel.Size = new System.Drawing.Size(79, 38);
            this.MinutesLabel.TabIndex = 11;
            this.MinutesLabel.Text = "mins";
            this.MinutesLabel.Click += new System.EventHandler(this.MinutesLabel_Click);
            // 
            // UpdateStudentTimeCloseBtnElipse
            // 
            this.UpdateStudentTimeCloseBtnElipse.ElipseRadius = 5;
            this.UpdateStudentTimeCloseBtnElipse.TargetControl = this.UpdateStudentTimeCloseBtn;
            // 
            // DurationPanelElipse
            // 
            this.DurationPanelElipse.ElipseRadius = 10;
            this.DurationPanelElipse.TargetControl = this.DurationPanel;
            // 
            // UpdateSessionBtnElipse
            // 
            this.UpdateSessionBtnElipse.ElipseRadius = 5;
            this.UpdateSessionBtnElipse.TargetControl = this.UpdateSessionBtn;
            // 
            // AddTimeModalFormElipse
            // 
            this.AddTimeModalFormElipse.ElipseRadius = 20;
            this.AddTimeModalFormElipse.TargetControl = this;
            // 
            // AddTimeModalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 336);
            this.Controls.Add(this.MinutesLabel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.AddTimePanel);
            this.Font = new System.Drawing.Font("Roboto", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "AddTimeModalForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddTimeModalForm_FormClosed);
            this.AddTimePanel.ResumeLayout(false);
            this.AddTimePanel.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.DurationPanel.ResumeLayout(false);
            this.DurationPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel AddTimePanel;
        private System.Windows.Forms.Label DurationLabel;
        private System.Windows.Forms.Button UpdateSessionBtn;
        private System.Windows.Forms.Panel DurationPanel;
        private System.Windows.Forms.Panel panel2;
        private Bunifu.Framework.UI.BunifuElipse HrAndMinPanelElipse;
        private System.Windows.Forms.Button UpdateStudentTimeCloseBtn;
        private System.Windows.Forms.Label HeaderLabel;
        private System.Windows.Forms.Panel HrAndMinPanel;
        private System.Windows.Forms.Label HourLabel;
        private System.Windows.Forms.TextBox DurationTb;
        private System.Windows.Forms.Label MinutesLabel;
        private Bunifu.Framework.UI.BunifuElipse UpdateStudentTimeCloseBtnElipse;
        private Bunifu.Framework.UI.BunifuElipse DurationPanelElipse;
        private Bunifu.Framework.UI.BunifuElipse UpdateSessionBtnElipse;
        private Bunifu.Framework.UI.BunifuElipse AddTimeModalFormElipse;
    }
}