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

namespace DiplomApp
{
    enum RowState
    {
        Existed,
        New,
        Modified,
        ModifiedNew,
        Deleted
    }

    public partial class Form3 : Form
    {
        private readonly checkUser _user;

        DataBase dataBase = new DataBase();

        int selectedRow;

        public Form3(checkUser user)
        {
            _user = user;
            InitializeComponent();
        }

        private void IsAdmin()
        {
            управлениеToolStripMenuItem.Enabled = _user.IsAdmin;
            button2.Enabled = _user.IsAdmin;
            button3.Enabled = _user.IsAdmin;
            button4.Enabled = _user.IsAdmin;
            button5.Enabled = _user.IsAdmin;
        }

        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id", "№");
            dataGridView1.Columns.Add("type_of", "рейс");
            dataGridView1.Columns.Add("count_of", "билетов");
            dataGridView1.Columns.Add("postavka", "дата");
            dataGridView1.Columns.Add("price", "цена");
            dataGridView1.Columns.Add("isNew", String.Empty);
        }


        private void ClearFields()
        {
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetInt32(2), record.GetString(3), record.GetInt32(4), RowState.ModifiedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string queryString = $"select * from diplom_belyaeva";

            SqlCommand command = new SqlCommand(queryString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = $"{_user.Login}: {_user.Status}";
            IsAdmin();
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox2.Text = row.Cells[0].Value.ToString();
                textBox3.Text = row.Cells[1].Value.ToString();
                textBox4.Text = row.Cells[2].Value.ToString();
                textBox5.Text = row.Cells[3].Value.ToString();
                textBox6.Text = row.Cells[4].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshDataGrid(dataGridView1);
            ClearFields();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form4 frm4 = new Form4();
            frm4.Show();
        }

        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from diplom_belyaeva where concat (id, type_of, count_of, postavka, price) like '%" + textBox1.Text + "%'";

            SqlCommand com = new SqlCommand(searchString, dataBase.getConnection());

            dataBase.openConnection();

            SqlDataReader read = com.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
        }        
        
        private void deleteRow()
        {
            int index = dataGridView1.CurrentCell.RowIndex;

            dataGridView1.Rows[index].Visible = false;

            if (dataGridView1.Rows[index].Cells[0].Value.ToString() == string.Empty)
            {
                dataGridView1.Rows[index].Cells[5].Value = RowState.Deleted;
                return;

            }
           dataGridView1.Rows[index].Cells[5].Value = RowState.Deleted;

        }


        private void Update()
        {
            dataBase.openConnection();

            for(int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var rowState = (RowState)dataGridView1.Rows[index].Cells[5].Value;

                if (rowState == RowState.Existed)
                    continue;

                if(rowState == RowState.Deleted)
                {
                    var id = Convert.ToInt32(dataGridView1.Rows[index].Cells[0].Value);
                    var deleteQuery = $"delete from diplom_belyaeva where id = {id}";

                    var command = new SqlCommand(deleteQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();
                }

                if(rowState == RowState.Modified)
                {
                    var id = dataGridView1.Rows[index].Cells[0].Value.ToString();
                    var type = dataGridView1.Rows[index].Cells[1].Value.ToString();
                    var count = dataGridView1.Rows[index].Cells[2].Value.ToString();
                    var postavka = dataGridView1.Rows[index].Cells[3].Value.ToString();
                    var price = dataGridView1.Rows[index].Cells[4].Value.ToString();

                    var changeQuery = $"update diplom_belyaeva set type_of = '{type}', count_of = '{count}', postavka = '{postavka}', price = '{price}' where id = '{id}'";

                    var command = new SqlCommand(changeQuery, dataBase.getConnection());
                    command.ExecuteNonQuery();

                }
            }
            
            dataBase.closeConnection();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            deleteRow();
            ClearFields();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Update();
           
        }

        private void Change()
        {
            var selectedRoeIndex = dataGridView1.CurrentCell.RowIndex;

            var id = textBox2.Text;
            var type = textBox3.Text;
            var count = textBox4.Text;
            var postav = textBox5.Text;
            int price;

            if(dataGridView1.Rows[selectedRoeIndex].Cells[0].Value.ToString() != string.Empty)
            {
                if(int.TryParse(textBox6.Text, out price))
                {
                    dataGridView1.Rows[selectedRoeIndex].SetValues(id, type, count, postav, price);
                    dataGridView1.Rows[selectedRoeIndex].Cells[5].Value = RowState.Modified;
                }
                else
                {
                    MessageBox.Show("Цена должна иметь числовой формат!");
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Change();
            ClearFields();
        }

        private void управлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Panel_Admin pnladm = new Panel_Admin();
            pnladm.Show();
            
            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            log_in lgn = new log_in();
            lgn.Show();
            this.Hide();
        }
    }
}
