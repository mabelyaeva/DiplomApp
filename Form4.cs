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
    public partial class Form4 : Form
    {
        DataBase dataBase = new DataBase();

        public Form4()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataBase.openConnection();

            var type = textBox3.Text;
            var count = textBox4.Text;
            var postav = textBox5.Text;
            int price;

            if (int.TryParse(textBox6.Text, out price))
            {
                var addQuery = $"insert into diplom_belyaeva (type_of, count_of, postavka, price) values ('{type}', '{count}', '{postav}', '{price}')";

                var command = new SqlCommand(addQuery, dataBase.getConnection());
                command.ExecuteNonQuery();

                MessageBox.Show("Запись успешно создана!", "Успешно!");
            }
            else 
            {
                MessageBox.Show("Запись не создана!", "Ошибка!");
            }
            dataBase.closeConnection();


        }

        private void button1_Click(object sender, EventArgs e)
        {
         
        }
    }
}
