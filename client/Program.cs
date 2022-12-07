using System;
using GCP.Interfaces;
using System.Threading;
using Gtk;

namespace GCP
{
    static class Program
    {


        public static MainWindow mainWindow ;
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();
            mainWindow = new MainWindow() ;
            Application.Run();
        }
    }
}
