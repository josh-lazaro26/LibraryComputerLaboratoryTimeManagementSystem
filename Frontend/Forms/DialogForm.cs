using System;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class DialogForm : Form
    {
        public bool IsConfirmed { get; private set; } = false;

        public DialogForm(string title, string message)
        {
            InitializeComponent();
            DialogTitleLabel.Text = title;
            DialogMessageLabel.Text = message;
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.Close();
        }

        private void DialogConfirmBtn_Click(object sender, EventArgs e)
        {
            IsConfirmed = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DialogCancelBtn_Click(object sender, EventArgs e)
        {
            IsConfirmed = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}