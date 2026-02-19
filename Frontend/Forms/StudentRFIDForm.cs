using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms
{
    public partial class StudentRFIDForm : Form
    {
        public string ScannedRfid { get; private set; }

        public StudentRFIDForm()
        {
            InitializeComponent();
            this.Shown += (s, e) => RFIDTextBox.Focus();
            RFIDTextBox.KeyDown += RFIDTextBox_KeyDown;
        }

        private void RFIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ScannedRfid = RFIDTextBox.Text.Trim(); // or from serial, etc.
                DialogResult = DialogResult.OK;        // signal success
                Close();
                e.Handled = true;
            }
        }

        private void StudentRfidCloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

}
