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

        public Dictionary<String, String> formResult = new Dictionary<String, String>();

        public Form3(Dictionary<string, List<Tuple<String, String>>> comboBoxes)
        {
            InitializeComponent();


            foreach (Tuple<String, String> element in comboBoxes["ASIGNATURA"])
            {
                comboBox1.Items.Add(element.Item1);
            }

            foreach (Tuple<String, String> element in comboBoxes["BLOQUE"])
            {
                comboBox2.Items.Add(element.Item1);
            }

            foreach (Tuple<String, String> element in comboBoxes["GRADO"])
            {
                comboBox3.Items.Add(element.Item1);
            }

            

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (formIsEmpty())
            {
                MessageBox.Show("Porfavor rellene todos los campos requeridos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                

                formResult.Add("TEMA", textBox2.Text);
                formResult.Add("ASIGNATURA", comboBox1.Text);
                formResult.Add("BLOQUE", comboBox2.Text);
                formResult.Add("GRADO", comboBox3.Text);

                this.Close();
            }

           

        }
        private bool formIsEmpty()
        {
            bool temp = false;

         
            temp |= ( String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrWhiteSpace(textBox2.Text) );
            temp |= ( String.IsNullOrEmpty(comboBox1.Text) || String.IsNullOrWhiteSpace(comboBox1.Text) );
            temp |= ( String.IsNullOrEmpty(comboBox2.Text) || String.IsNullOrWhiteSpace(comboBox2.Text) );
            temp |= ( String.IsNullOrEmpty(comboBox3.Text) || String.IsNullOrWhiteSpace(comboBox3.Text) );

            return temp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
