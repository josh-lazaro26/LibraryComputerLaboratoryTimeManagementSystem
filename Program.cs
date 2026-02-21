using LibraryComputerLaboratoryTimeManagementSystem.FORMS;
using LibraryComputerLaboratoryTimeManagementSystem.Frontend.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var dummyTime = new Dictionary<int, TimeSpan>()
            {
                { 1, TimeSpan.FromMinutes(120) } // key = 1, value = 2 hours
            };

            Application.Run(new MainForm());

        }
    }
}
