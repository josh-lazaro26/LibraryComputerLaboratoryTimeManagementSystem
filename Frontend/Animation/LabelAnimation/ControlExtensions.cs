using System.Reflection;
using System.Windows.Forms;

namespace LibraryComputerLaboratoryTimeManagementSystem
{
    public static class ControlExtensions
    {
        public static void SetDoubleBuffered(this Control control, bool enable)
        {
            var prop = typeof(Control).GetProperty(
                "DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);

            prop?.SetValue(control, enable, null);
        }
    }
}
