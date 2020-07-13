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
    public partial class Form1 : Form
    {
        SqlConnection conexion = null;
        string conexionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hendr\Desktop\REACTIVOS.mdf;Integrated Security=True;Connect Timeout=30";
        private String userName = "";
        private String passWord = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Label2_Click(object sender, EventArgs e)
        {
        }
        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            userName = textBox1.Text;
            passWord = textBox2.Text;
            try
            {
                //Abrir conexión
                conexion = new SqlConnection(conexionString);
                conexion.Open();

                String sqlQuery = $"select * from Usuarios where NombreUsuario = '{userName}' and Clave = '{passWord}'";
                
                SqlCommand cmd = new SqlCommand(sqlQuery, conexion);
                SqlDataReader reader = cmd.ExecuteReader();
                
                if (reader.HasRows)
                {
                    reader.Read();
                    String permisos = reader["Permisos"].ToString();
                    reader.Close();
                    MessageBox.Show($"Bienvenido {userName}");
                    this.Hide();
                    using (Form2 frm1 = new Form2(conexion, userName, permisos))
                    {
                        frm1.ShowDialog();
                    }
                    
                    Application.Exit();

                }
                else
                {
                    MessageBox.Show("Error: El usuario no se encuentra registrado","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch( Exception error)
            {
                Console.WriteLine($"Error en la conexion {error.Message}");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

   
}
