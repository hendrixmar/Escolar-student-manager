using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
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
        Dictionary<string, List<Tuple<String, String>>> comboBoxes = new Dictionary<string, List< Tuple<String, String>>>();
        

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

            try
            {
                
                String sqlQuery = @"select nivel from grados group by nivel";
                SqlCommand  cmd = new SqlCommand(sqlQuery, dataBaseConnection);
                reader = cmd.ExecuteReader();

                
                while (reader.Read())
                {
                    comboBox1.Items.Add( reader["nivel"].ToString() );
                }
                
                reader.Close();

                
                sqlQuery = @"select grado from grados";
                cmd = new SqlCommand(sqlQuery, dataBaseConnection);
                reader = cmd.ExecuteReader();

                
                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["grado"].ToString());
                }
                
                reader.Close();

                da = new SqlDataAdapter();
                ds = new DataSet();

            //Depending the level permission the sql query will be modified
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

                comboBoxes.Add("ASIGNATURA", SelectDistinct(ds.Tables["getTemas"], "ASIGNATURA"));
                comboBoxes.Add("BLOQUE", SelectDistinct(ds.Tables["getTemas"], "BLOQUE"));
                comboBoxes.Add("GRADO", SelectDistinct(ds.Tables["getTemas"], "GRADO"));
                /*
                Console.WriteLine("---- ASGINATURA ----");
                foreach (String element in comboBoxes["ASIGNATURA"])
                {
                    Console.WriteLine($"<<  {element}  >>");
                }

                Console.WriteLine("---- BLOQUE ----");
                foreach (String element in comboBoxes["BLOQUE"])
                {
                    Console.WriteLine($"<<  {element}  >>");
                }

                Console.WriteLine("---- GRADO ----");
                foreach (String element in comboBoxes["GRADO"])
                {
                    Console.WriteLine($"<<  {element}  >>");
                }
                */
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
                

                var strExpr = $"ID_GRADO = {year}";
                
                var dv = ds.Tables[0].DefaultView;
                dv.RowFilter = strExpr;
                var newDS = new DataSet();
                var newDT = dv.ToTable();
                newDS.Tables.Add(newDT);

                dataGridView1.DataSource = newDS.Tables[0];
                dataGridView1.Refresh();

                    
             
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

            Form3 addUserView = new Form3(comboBoxes);
            
           

            

            //addUserView.tempDataSet = ds.Tables["getTemas"].NewRow(); //empty row
            addUserView.ShowDialog();

             if (addUserView.formResult.Count == 0)
            {
                return;
            }

          

            Tuple< String, String > id_grado = comboBoxes["GRADO"].Find(r => r.Item1 == addUserView.formResult["GRADO"]);
            Tuple< String, String > id_asignatura = comboBoxes["BLOQUE"].Find(r => r.Item1 == addUserView.formResult["BLOQUE"]);
            Tuple< String, String > id_bloque = comboBoxes["ASIGNATURA"].Find(r => r.Item1 == addUserView.formResult["ASIGNATURA"]);
            String tema = addUserView.formResult["TEMA"];

            Console.WriteLine(id_grado);
            Console.WriteLine(id_bloque);
            Console.WriteLine(id_asignatura);
            /*
            Console.WriteLine($"valor obtenido es: {dataGridView1.CurrentCell.RowIndex}   {row.GetType() }");
            int i = 0;
            while (i < row.Table.Columns.Count)
            {
                Console.WriteLine(row.Table.Columns[i].ColumnName);
                i++;
            }
            */

            string insertion = $@"insert into gettemas(
                          ID_ASIGNATURA,ASIGNATURA,
                          ID_GRADO,GRADO,
                          ID_BLOQUE,BLOQUE,
                          TEMA)
                          values(
                            {id_asignatura.Item2}, '{id_asignatura.Item1}',
                            {id_grado.Item2}, '{id_grado.Item1}',
                            {id_bloque.Item2},'{id_bloque.Item1}',
                          '{tema}')";
            try
            {
                
                Console.WriteLine(insertion);
                SqlCommand cmd = new SqlCommand(insertion, dataBaseConnection);
                da.InsertCommand = cmd;
                da.InsertCommand.ExecuteNonQuery();

                MessageBox.Show("Nuevo dato agregado con exito a la base de datos");

                DataRow temp = ds.Tables["getTemas"].NewRow();
                //da.Update(ds, "getTemas");

                temp["ID_ASIGNATURA"] = id_asignatura.Item2;
                temp["ASIGNATURA"] = id_asignatura.Item1;
                temp["ID_GRADO"] = id_grado.Item2;
                temp["GRADO"] = id_grado.Item1;
                temp["TEMA"] = tema;
                temp["ID_BLOQUE"] = id_bloque.Item2;
                temp["BLOQUE"] = id_bloque.Item1;

                ds.Tables[0].Rows.Add(temp);
                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Refresh();


            }
            catch(Exception p)
            {
                Console.WriteLine(p);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentCell.RowIndex;


            string deleteQuery = $@"delete from gettemas where
                        id_grado = {ds.Tables[0].Rows[index]["ID_GRADO"]} 
                        and id_asignatura = {ds.Tables[0].Rows[index]["ID_ASIGNATURA"]}
                        and id_bloque = {ds.Tables[0].Rows[index]["ID_BLOQUE"]}";

            
            
            
            if(!String.IsNullOrEmpty(ds.Tables[0].Rows[index]["ID_TEMA"].ToString()))
            {
                deleteQuery += $"and id_tema = {ds.Tables[0].Rows[index]["ID_TEMA"]}";
            }

            
            MessageBox.Show($"Se ha borrado el tema {ds.Tables[0].Rows[index]["TEMA"]}");
            SqlCommand cmd = new SqlCommand(deleteQuery, dataBaseConnection);
            da.InsertCommand = cmd;
            da.InsertCommand.ExecuteNonQuery();

            ds.Tables[0].Rows[index].Delete();
            ds.Tables[0].AcceptChanges();
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.Refresh();
           

        
        }

        private void Button4_Click(object sender, EventArgs e)
        {

            int index = dataGridView1.CurrentCell.RowIndex;
            Form3 addUserView = new Form3(comboBoxes, ds.Tables[0].Rows[index]);





            //addUserView.tempDataSet = ds.Tables["getTemas"].NewRow(); //empty row
            addUserView.ShowDialog();

            if (addUserView.formResult.Count == 0)
            {
                return;
            }
            Tuple<String, String> id_grado = comboBoxes["GRADO"].Find(r => r.Item1 == addUserView.formResult["GRADO"]);
            Tuple<String, String> id_asignatura = comboBoxes["BLOQUE"].Find(r => r.Item1 == addUserView.formResult["BLOQUE"]);
            Tuple<String, String> id_bloque = comboBoxes["ASIGNATURA"].Find(r => r.Item1 == addUserView.formResult["ASIGNATURA"]);
            String tema = addUserView.formResult["TEMA"];


            Console.WriteLine(id_grado);
            Console.WriteLine(id_bloque);
            Console.WriteLine(id_asignatura);


            string updateQuery = $@"UPDATE getTemas set 
                           GRADO = '{id_grado.Item1}',
                           ASIGNATURA = '{id_asignatura.Item1}',
                           BLOQUE = '{id_bloque.Item1}',
                           TEMA = '{tema}'
                           WHERE
                           ID_GRADO = {id_grado.Item2}
                        and ID_ASIGNATURA = {id_asignatura.Item2}
                        and ID_BLOQUE = {id_bloque.Item2}";

            if (!String.IsNullOrEmpty(ds.Tables[0].Rows[index]["ID_TEMA"].ToString()))
            {
                updateQuery += $" and id_tema = {ds.Tables[0].Rows[index]["ID_TEMA"]}";
            }

            Console.WriteLine(updateQuery);
            
            try
            {
                Console.WriteLine(updateQuery);
                SqlCommand cmd = new SqlCommand(updateQuery, dataBaseConnection);
                da.UpdateCommand = cmd;
                da.UpdateCommand.ExecuteNonQuery();

                MessageBox.Show("Nuevo dato actualizado con exito a la base de datos");

                
                //da.Update(ds, "getTemas");

                ds.Tables[0].Rows[index]["ID_ASIGNATURA"] = id_asignatura.Item2;
                ds.Tables[0].Rows[index]["ASIGNATURA"] = id_asignatura.Item1;
                ds.Tables[0].Rows[index]["ID_GRADO"] = id_grado.Item2;
                ds.Tables[0].Rows[index]["GRADO"] = id_grado.Item1;
                ds.Tables[0].Rows[index]["TEMA"] = tema;
                ds.Tables[0].Rows[index]["ID_BLOQUE"] = id_bloque.Item2;
                ds.Tables[0].Rows[index]["BLOQUE"] = id_bloque.Item1;

                dataGridView1.DataSource = ds.Tables[0];
                dataGridView1.Refresh();
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
            //MessageBox.Show($"{dataGridView1.CurrentCell.RowIndex }"); 
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void OnRowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //MessageBox.Show($"You clicked row {dataGridView1.CurrentCell.RowIndex }");
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
          
        }
   
        public List<Tuple<String, String>> SelectDistinct(DataTable SourceTable, string FieldName)
        {
            // Create a Datatable – datatype same as FieldName
            HashSet<String> checkingUniquesValues = new HashSet<String>();

            List<Tuple<String, String>> output = new List<Tuple<String, String>>();

            foreach (DataRow row in SourceTable.Rows)
            {
            
                String item = row[FieldName].ToString();
                String id = row[$"ID_{FieldName}"].ToString();

                if (!checkingUniquesValues.Contains(item))
                {
                    output.Add(new Tuple<String, String>(item, id));
                    checkingUniquesValues.Add(item);
                }
                
            }

            return output;
        
        }
    }
}
