using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWindows
{
    public partial class Demo : Form
    {
        public Demo()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string clientId = "AU1-SGYpYOyn9E2QMwwy-6cQwu0gVIq6aJyvS0uQdlmm06LwPqyAOTyUiWMxNmRRy3Q7kowBnjPgrWtB";
            string secret = "EOPbBeAZzf2RkCqY2pkzvfAQaQbGSsoIIHRN0rf0VmFwnO9xCgTlgDXLvjVUIN8sHZYSEKavIpJ9wLjw";
            byte[] by = Encoding.ASCII.GetBytes(clientId + ":" + secret);
            for (int i = 0; i < by.Length; i++)
            {
                Console.WriteLine(by[i].ToString());
            }
            string basic = "Basic " + System.Convert.ToBase64String(by);
            Console.WriteLine(by);
            Console.WriteLine("basic = " + basic);
            lxCustomTextBox1.AppendText(basic);
        }
    }
}
