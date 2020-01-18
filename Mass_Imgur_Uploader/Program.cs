using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mass_Imgur_Uploader
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            new Program();
            System.Threading.Thread.Sleep(1);
        }

        

        public Program()
        {
            Application.EnableVisualStyles();
            Application.Run((Form)new Form1());
        }
    }
}
