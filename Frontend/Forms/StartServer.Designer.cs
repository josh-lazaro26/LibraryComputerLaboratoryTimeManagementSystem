namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    partial class StartServer
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
            this.StartServerBtn = new System.Windows.Forms.Button();
            this.StartServerElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.StartServerBtnElipse = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.SuspendLayout();
            // 
            // StartServerBtn
            // 
            this.StartServerBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StartServerBtn.BackColor = System.Drawing.Color.ForestGreen;
            this.StartServerBtn.FlatAppearance.BorderSize = 0;
            this.StartServerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartServerBtn.Font = new System.Drawing.Font("Inter SemiBold", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartServerBtn.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.StartServerBtn.Location = new System.Drawing.Point(10, 68);
            this.StartServerBtn.Name = "StartServerBtn";
            this.StartServerBtn.Size = new System.Drawing.Size(571, 352);
            this.StartServerBtn.TabIndex = 0;
            this.StartServerBtn.Text = "Start Server";
            this.StartServerBtn.UseVisualStyleBackColor = false;
            this.StartServerBtn.Click += new System.EventHandler(this.StartServerBtn_Click);
            // 
            // StartServerElipse
            // 
            this.StartServerElipse.ElipseRadius = 20;
            this.StartServerElipse.TargetControl = this;
            // 
            // StartServerBtnElipse
            // 
            this.StartServerBtnElipse.ElipseRadius = 15;
            this.StartServerBtnElipse.TargetControl = this.StartServerBtn;
            // 
            // StartServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(64)))), ((int)(((byte)(43)))));
            this.ClientSize = new System.Drawing.Size(595, 432);
            this.Controls.Add(this.StartServerBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "StartServer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartServerBtn;
        private Bunifu.Framework.UI.BunifuElipse StartServerElipse;
        private Bunifu.Framework.UI.BunifuElipse StartServerBtnElipse;
    }
}