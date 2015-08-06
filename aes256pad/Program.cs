using System;
using System.IO;
using System.Windows.Forms;

namespace aes256pad
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string filepath = null;
            try
            {
                if (args.Length > 0 && File.Exists(args[0]))
                {
                    filepath = args[0];
                }
            }
            catch (Exception)
            {
            }
            Application.Run(new MainForm(filepath));
        }
    }
}
