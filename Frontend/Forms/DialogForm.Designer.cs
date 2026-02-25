namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    partial class DialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogForm));
            this.NotificationTitlePanel = new System.Windows.Forms.Panel();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.DialogTitleLabel = new System.Windows.Forms.Label();
            this.DialogMessageLabel = new System.Windows.Forms.Label();
            this.DialogConfirmBtn = new System.Windows.Forms.Button();
            this.DialogCancelBtn = new System.Windows.Forms.Button();
            this.DialogFormElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.ConfirmBtnElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.CancelBtnElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.NotificationTitlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // NotificationTitlePanel
            // 
            this.NotificationTitlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(80)))), ((int)(((byte)(60)))));
            this.NotificationTitlePanel.Controls.Add(this.CloseBtn);
            this.NotificationTitlePanel.Controls.Add(this.DialogTitleLabel);
            this.NotificationTitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NotificationTitlePanel.Location = new System.Drawing.Point(0, 0);
            this.NotificationTitlePanel.Name = "NotificationTitlePanel";
            this.NotificationTitlePanel.Size = new System.Drawing.Size(365, 45);
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
            this.CloseBtn.TabIndex = 28;
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // DialogTitleLabel
            // 
            this.DialogTitleLabel.Font = new System.Drawing.Font("Roboto Condensed", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DialogTitleLabel.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.DialogTitleLabel.Location = new System.Drawing.Point(0, 6);
            this.DialogTitleLabel.Name = "DialogTitleLabel";
            this.DialogTitleLabel.Size = new System.Drawing.Size(363, 30);
            this.DialogTitleLabel.TabIndex = 0;
            this.DialogTitleLabel.Text = "Are you sure?";
            this.DialogTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DialogMessageLabel
            // 
            this.DialogMessageLabel.Font = new System.Drawing.Font("Roboto Condensed", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DialogMessageLabel.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.DialogMessageLabel.Location = new System.Drawing.Point(0, 65);
            this.DialogMessageLabel.Name = "DialogMessageLabel";
            this.DialogMessageLabel.Size = new System.Drawing.Size(363, 75);
            this.DialogMessageLabel.TabIndex = 1;
            this.DialogMessageLabel.Text = "MessageLabel";
            this.DialogMessageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DialogConfirmBtn
            // 
            this.DialogConfirmBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.DialogConfirmBtn.FlatAppearance.BorderSize = 0;
            this.DialogConfirmBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DialogConfirmBtn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.DialogConfirmBtn.Location = new System.Drawing.Point(58, 153);
            this.DialogConfirmBtn.Name = "DialogConfirmBtn";
            this.DialogConfirmBtn.Size = new System.Drawing.Size(95, 45);
            this.DialogConfirmBtn.TabIndex = 2;
            this.DialogConfirmBtn.Text = "Confirm";
            this.DialogConfirmBtn.UseVisualStyleBackColor = false;
            this.DialogConfirmBtn.Click += new System.EventHandler(this.DialogConfirmBtn_Click);
            // 
            // DialogCancelBtn
            // 
            this.DialogCancelBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.DialogCancelBtn.FlatAppearance.BorderSize = 0;
            this.DialogCancelBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DialogCancelBtn.Location = new System.Drawing.Point(212, 153);
            this.DialogCancelBtn.Name = "DialogCancelBtn";
            this.DialogCancelBtn.Size = new System.Drawing.Size(95, 45);
            this.DialogCancelBtn.TabIndex = 3;
            this.DialogCancelBtn.Text = "Cancel";
            this.DialogCancelBtn.UseVisualStyleBackColor = false;
            this.DialogCancelBtn.Click += new System.EventHandler(this.DialogCancelBtn_Click);
            // 
            // DialogFormElipse
            // 
            this.DialogFormElipse.ElipseRadius = 10;
            this.DialogFormElipse.TargetControl = this;
            // 
            // ConfirmBtnElipse
            // 
            this.ConfirmBtnElipse.ElipseRadius = 5;
            this.ConfirmBtnElipse.TargetControl = this.DialogConfirmBtn;
            // 
            // CancelBtnElipse
            // 
            this.CancelBtnElipse.ElipseRadius = 5;
            this.CancelBtnElipse.TargetControl = this.DialogCancelBtn;
            // 
            // DialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(64)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(365, 210);
            this.Controls.Add(this.DialogCancelBtn);
            this.Controls.Add(this.DialogConfirmBtn);
            this.Controls.Add(this.DialogMessageLabel);
            this.Controls.Add(this.NotificationTitlePanel);
            this.Font = new System.Drawing.Font("Roboto", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DialogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DialogForm";
            this.NotificationTitlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel NotificationTitlePanel;
        private System.Windows.Forms.Label DialogTitleLabel;
        private System.Windows.Forms.Label DialogMessageLabel;
        private System.Windows.Forms.Button DialogConfirmBtn;
        private System.Windows.Forms.Button DialogCancelBtn;
        private Bunifu.Framework.UI.BunifuElipse DialogFormElipse;
        private Bunifu.Framework.UI.BunifuElipse ConfirmBtnElipse;
        private Bunifu.Framework.UI.BunifuElipse CancelBtnElipse;
        public System.Windows.Forms.Button CloseBtn;
    }
}