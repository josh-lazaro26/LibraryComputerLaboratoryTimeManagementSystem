
using LibraryComputerLaboratoryTimeManagementSystem.FORMS;
using System;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class StartServer : Form
    {
        public StartServer()
        {
            InitializeComponent();
        }

        private void StartServerBtn_Click(object sender, EventArgs e)
        {
            RFIDForm rfid = new RFIDForm();
            rfid.Show();
            this.Hide();
        }
    }
}
