using System.Diagnostics;
namespace Kq1Pathing
{
    internal static class Program
    {
        static void Main()
        {
            RunInterface();
        }

        [STAThread]
        static void RunInterface()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new TestForm());
        }
    }
}
