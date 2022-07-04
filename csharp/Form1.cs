using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        DataTable table;
        string filename;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // string contents = File.ReadAllText(@"\\88888-PC\data\op.txt");
            //MessageBox.Show(contents);

             filename = @"\\88888-PC\data\op.txt";

  

             table = new DataTable();
            table.Columns.Add("symbol");
            table.Columns.Add("open");
            table.Columns.Add("high");
            table.Columns.Add("low");
            table.Columns.Add("last");
            table.Columns.Add("vol");

            timer1_Tick(null, null);
            //MyGridView.DataBind();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime modification = File.GetLastWriteTime(filename);
            this.Text = modification.ToString();

            table.Clear();

            using (StreamReader sr = new StreamReader(filename))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string[] parts = sr.ReadLine().Split('\t');
                    table.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);
                }
            }
            MyGridView.DataSource = table;
        }
    }
}
