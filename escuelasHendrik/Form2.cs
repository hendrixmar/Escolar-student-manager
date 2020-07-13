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
    public partial class Form2 : Form
    {
        SqlConnection dataBaseConnection = null;
        SqlDataReader reader = null;
        SqlDataAdapter da = null;
        DataSet ds = null;
        int permissionlevel;
        String userName;

        public Form2()
        {
            InitializeComponent();
        }
        public Form2(SqlConnection dataBaseConnection, String userName, String level)
        {
            InitializeComponent();
            
            this.permissionlevel = int.Parse(level);
            this.dataBaseConnection = dataBaseConnection;
            this.userName =  userName;
            label1.Text = $"Nombre del usuario: {userName}";
            label2.Text = $"Nivel de permisos: {level}";
            label3.Text = DateTime.Now.ToString(@"MM\/dd\/yyyy h\:mm tt");
            String nivel, grado;
            try
            {
                //AGREGAR ELEMENTOS AL COMBOBOX 'NIVEL'
                String sqlQuery = @"select nivel from grados group by nivel";
                SqlCommand  cmd = new SqlCommand(sqlQuery, dataBaseConnection);
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        comboBox1.Items.Add( reader["nivel"].ToString() );
                    }
                }
                reader.Close();

                //AGREGAR ELEMENTOS AL COMBOBOX 'GRADOS'
                sqlQuery = @"select grado from grados";
                cmd = new SqlCommand(sqlQuery, dataBaseConnection);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        comboBox2.Items.Add(reader["grado"].ToString());
                    }
                }
                reader.Close();

                da = new SqlDataAdapter();
                ds = new DataSet();

                //Tabla que tendra toda la informacion
                switch (permissionlevel)
                {
                    case 1:
                        sqlQuery = $"SELECT * FROM GetTemas WHERE ID_GRADO <= 6";
                        break;
                    case 2:
                        sqlQuery = $"SELECT * FROM GetTemas WHERE ID_GRADO > 6";
                        break;
                    case 3:
                        sqlQuery = $"SELECT * FROM GetTemas";
                        break;
                }
                
                cmd = new SqlCommand(sqlQuery, dataBaseConnection);
                da.SelectCommand = cmd;
                da.Fill(ds, "getTemas");

                //Generar los datos del dataGridView
                dataGridView1.RowHeaderMouseClick += new DataGridViewCellMouseEventHandler(OnRowHeaderMouseClick);
                dataGridView1.DataSource = ds.Tables["getTemas"];
                dataGridView1.Refresh();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            bool consulta = false;
            int level = comboBox1.SelectedIndex;
            int year = comboBox2.SelectedIndex + 1;
            
            //Check Authorization depending the level of the current user

            if( permissionlevel == 1 && level == 0  && year <= 6)
            {
                
                consulta = true;

            }else if (permissionlevel == 2 && level == 1 && 6 < year)
            {
                consulta = true;
                
            }
            else if (permissionlevel == 3)
            {
                consulta = true;
                
            }

                //If the query is feasible it will filter the
                //dataset instead of doing a query directly to the data base
            if (consulta)
            {
                
                try
                {
                    var strExpr = $"ID_GRADO = {year}";
                    var strSort = "ID_GRADO DESC";

                    var dv = ds.Tables[0].DefaultView;
                    dv.RowFilter = strExpr;
                    var newDS = new DataSet();
                    var newDT = dv.ToTable();
                    newDS.Tables.Add(newDT);

                    dataGridView1.DataSource = newDS.Tables[0];
                    dataGridView1.Refresh();

                    
                }
                catch
                {
                    MessageBox.Show("Fallo en la base de datos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show($"No tiene permiso para visualizar ese contenido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
   
            this.Close();
            Application.Exit();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Form3 frm1 = new Form3(
                    this.dataBaseConnection,
                    this.userName,
                    this.permissionlevel+12389);
            
            frm1.ShowDialog();
            
            Console.WriteLine($"conectada ---------------------------------- {frm1.level}");
            

            string ins = @"insert into gettemas(
                          ID_GRADO,GRADO,
                          ID_ASIGNATURA,ASIGNATURA,
                          ID_BLOQUE,BLOQUE,
                          ID_TEMA,TEMA)
                          values(
                          @id_grado,@grado,
                          @id_asignatura,@asignatura,
                          @id_bloque,@bloque,
                          @id_tema,@tema)";
            try
            {
                DataTable dt = ds.Tables["getTemas"];
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[0].Value);
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[1].Value);
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[2].Value);
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[3].Value);
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[4].Value);
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[5].Value);
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[6].Value);
                Console.WriteLine(dataGridView1.SelectedRows[0].Cells[7].Value);

                // add a row
                DataRow newRow = dt.NewRow();
                newRow["id_grado"]      = dataGridView1.SelectedRows[0].Cells[0].Value;
                newRow["grado"]         = dataGridView1.SelectedRows[0].Cells[1].Value;
                newRow["id_asignatura"] = dataGridView1.SelectedRows[0].Cells[2].Value;
                newRow["asignatura"]    = dataGridView1.SelectedRows[0].Cells[3].Value;
                newRow["id_bloque"]     = dataGridView1.SelectedRows[0].Cells[4].Value;
                newRow["bloque"]        = dataGridView1.SelectedRows[0].Cells[5].Value;
                newRow["id_tema"]       = dataGridView1.SelectedRows[0].Cells[6].Value;
                newRow["tema"]          = dataGridView1.SelectedRows[0].Cells[7].Value;
                dt.Rows.Add(newRow);

                SqlCommand cmd = new SqlCommand(ins,dataBaseConnection);

                cmd.Parameters.Add("@id_grado", SqlDbType.Int,8,"ID_GRADO");
                cmd.Parameters.Add("@grado", SqlDbType.NVarChar, 50, "GRADO");

                cmd.Parameters.Add("@id_asignatura", SqlDbType.Int, 8, "ID_ASIGNATURA");
                cmd.Parameters.Add("@asignatura", SqlDbType.NVarChar, 50, "ASIGNATURA");

                cmd.Parameters.Add("@id_bloque", SqlDbType.Int, 8, "ID_BLOQUE");
                cmd.Parameters.Add("@bloque", SqlDbType.NVarChar, 50, "BLOQUE");

                cmd.Parameters.Add("@id_tema", SqlDbType.Int, 8, "ID_TEMA");
                cmd.Parameters.Add("@tema", SqlDbType.NVarChar, 50, "TEMA");

                da.InsertCommand = cmd;
                da.Update(dt);
                Console.WriteLine("Se inserto");
            }
            catch(Exception p)
            {
                Console.WriteLine(p);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Console.WriteLine("---------BORRAR-----------");
           string del = @"delete from gettemas where
                        id_grado = @idGrado 
                        and id_asignatura = @idAsignatura 
                        and id_bloque = @idBloque
                        and id_tema = @idTema";
            try
            {
                DataTable dt = ds.Tables["getTemas"];

                SqlCommand cmd = new SqlCommand(del,dataBaseConnection);

                cmd.Parameters.Add("@idGrado",SqlDbType.Int,4,"id_grado");
                cmd.Parameters.Add("@idAsignatura", SqlDbType.Int, 4, "id_asignatura");
                cmd.Parameters.Add("@idBloque", SqlDbType.Int, 4, "id_bloque");
                cmd.Parameters.Add("@idTema", SqlDbType.Int, 4, "id_tema");
                Console.WriteLine("aqui");
                foreach (DataGridViewRow gridrow in dataGridView1.SelectedRows)
                {
                    string filt = "@idGrado = "             + gridrow.Cells[0].Value
                                  + "and @idAsignatura = "  + gridrow.Cells[2].Value
                                  + "and @idBloque = "      + gridrow.Cells[4].Value
                                  + "and @idTema = "        + gridrow.Cells[6].Value;
                    foreach(DataRow row in dt.Select(filt))
                    {
                        row.Delete();
                    }
                }
                Console.WriteLine("alla");
                da.DeleteCommand = cmd;
                da.Update(ds,"getTemas");
                Console.WriteLine("Se realizo elimino");
            }
            catch(Exception p)
            {
                Console.WriteLine("Error");
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {   
            string upd = @"UPDATE getTemas set 
                           GRADO = @grado,
                           ASIGNATURA = @asignatura,
                           BLOQUE = @bloque,
                           TEMA = @tema
                           WHERE
                           ID_GRADO = @id_grado 
                        and ID_ASIGNATURA = @id_asignatura 
                        and ID_BLOQUE = @id_bloque
                        and ID_TEMA = @id_tema";
            try
            {
                SqlCommand cmd = new SqlCommand(upd,dataBaseConnection);
                cmd.Parameters.Add("@id_grado", SqlDbType.Int, 8, "ID_GRADO");
                cmd.Parameters.Add("@grado", SqlDbType.NVarChar, 50, "GRADO");

                cmd.Parameters.Add("@id_asignatura", SqlDbType.Int, 8, "ID_ASIGNATURA");
                cmd.Parameters.Add("@asignatura", SqlDbType.NVarChar, 50, "ASIGNATURA");

                cmd.Parameters.Add("@id_bloque", SqlDbType.Int, 8, "ID_BLOQUE");
                cmd.Parameters.Add("@bloque", SqlDbType.NVarChar, 50, "BLOQUE");

                cmd.Parameters.Add("@id_tema", SqlDbType.Int, 8, "ID_TEMA");
                cmd.Parameters.Add("@tema", SqlDbType.NVarChar, 50, "TEMA");

                da.UpdateCommand = cmd;
                da.Update(ds,"getTemas");

                MessageBox.Show("Actualizacion lista");
            }
            catch(Exception p)
            {
                Console.WriteLine(p);
                Console.WriteLine("error");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show($"{dataGridView1.CurrentCell.RowIndex }"); 
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void OnRowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            MessageBox.Show($"You clicked row {dataGridView1.CurrentCell.RowIndex }");
            int current = dataGridView1.CurrentCell.RowIndex;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            DataRow row = ds.Tables["getTemas"].NewRow();//empty row
            Console.WriteLine($"valor obtenido es: {dataGridView1.CurrentCell.RowIndex}   {row.GetType() }");
            int i = 0;
            while (i < row.Table.Columns.Count)
            {
                Console.WriteLine(row.Table.Columns[i].ColumnName);
                i++;
            }
            /*
            row["column1"] = "a3";
            row["column2"] = "b3";
            ds.Tables["myTable"].Rows.Add(row);
            */
        }
    }
}
