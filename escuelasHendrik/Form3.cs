using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace escuelasHendrik
{
    public partial class Form3 : Form
    {

        public int level;
        public Form3(SqlConnection dataBaseConnection, String userName, int level)
        {
            InitializeComponent();
            this.level = level;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
    // return 123;

        }
    }
}
