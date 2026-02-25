namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    partial class NotificationModalForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NotificationModalForm));
            this.NotificationModalFormElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.NotificationTitlePanel = new System.Windows.Forms.Panel();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.NotificationTitleLabel = new System.Windows.Forms.Label();
            this.NotificationMessageLabel = new System.Windows.Forms.Label();
            this.NotificationTitlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NotificationModalFormElipse
            // 
            this.NotificationModalFormElipse.ElipseRadius = 10;
            this.NotificationModalFormElipse.TargetControl = this;
            // 
            // NotificationTitlePanel
            // 
            this.NotificationTitlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(80)))), ((int)(((byte)(60)))));
            this.NotificationTitlePanel.Controls.Add(this.CloseBtn);
            this.NotificationTitlePanel.Controls.Add(this.NotificationTitleLabel);
            this.NotificationTitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NotificationTitlePanel.Location = new System.Drawing.Point(0, 0);
            this.NotificationTitlePanel.Name = "NotificationTitlePanel";
            this.NotificationTitlePanel.Size = new System.Drawing.Size(365, 44);
            this.NotificationTitlePanel.TabIndex = 0;
            // 
            // CloseBtn
            // 
            this.CloseBtn.FlatAppearance.BorderSize = 0;
            this.CloseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseBtn.Image = ((System.Drawing.Image)(resources.GetObject("CloseBtn.Image")));
            this.CloseBtn.Location = new System.Drawing.Point(323, 2);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(37, 40);
            this.CloseBtn.TabIndex = 29;
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // NotificationTitleLabel
            // 
            this.NotificationTitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.NotificationTitleLabel.Font = new System.Drawing.Font("Roboto Condensed SemiBold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotificationTitleLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.NotificationTitleLabel.Location = new System.Drawing.Point(3, 4);
            this.NotificationTitleLabel.Name = "NotificationTitleLabel";
            this.NotificationTitleLabel.Size = new System.Drawing.Size(359, 40);
            this.NotificationTitleLabel.TabIndex = 1;
            this.NotificationTitleLabel.Text = "Warning";
            this.NotificationTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NotificationMessageLabel
            // 
            this.NotificationMessageLabel.Font = new System.Drawing.Font("Roboto Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotificationMessageLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.NotificationMessageLabel.Location = new System.Drawing.Point(0, 74);
            this.NotificationMessageLabel.Name = "NotificationMessageLabel";
            this.NotificationMessageLabel.Size = new System.Drawing.Size(362, 77);
            this.NotificationMessageLabel.TabIndex = 1;
            this.NotificationMessageLabel.Text = "This is a warning message.";
            this.NotificationMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NotificationModalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(64)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(365, 210);
            this.Controls.Add(this.NotificationMessageLabel);
            this.Controls.Add(this.NotificationTitlePanel);
            this.Font = new System.Drawing.Font("Roboto", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "NotificationModalForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NotificationModalForm";
            this.NotificationTitlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuElipse NotificationModalFormElipse;
        private System.Windows.Forms.Panel NotificationTitlePanel;
        public System.Windows.Forms.Button CloseBtn;
        private System.Windows.Forms.Label NotificationMessageLabel;
        private System.Windows.Forms.Label NotificationTitleLabel;
    }
}