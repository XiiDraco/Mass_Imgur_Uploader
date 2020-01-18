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
        }

        public Program()
        {
            Application.EnableVisualStyles();
            Application.Run((Form)new Form1());
        }
    }
}
