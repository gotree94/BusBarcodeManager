using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ExamMySQL
{
    public partial class Form1 : Form
    {
        string _server = "localhost";
        int _port = 3308;
        string _database = "new_schema";
        string _id = "root";
        string _pw = "root";
        string _connectionAddress = "";
        int index_sel = 0;

        public Form1()
        {
            InitializeComponent();

            _connectionAddress = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}", _server, _port, _database, _id, _pw);
            selectTable();

            textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            KeyPreview = true;

            // 리스트를 가장 최신값으로 보여주기
            // 아래의 2가지 방법중에서 2번 방법이 동작됨.
            // 1.
            //listViewPhoneBook.EnsureVisible(listViewPhoneBook.Items.Count - 1);
            // 2.
            listViewPhoneBook.TopItem = listViewPhoneBook.Items[listViewPhoneBook.Items.Count - 1];
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                    mysql.Open();
                    string insertQuery = string.Format("INSERT INTO accounts_table (id, date, barcode, name) VALUES ('{0}', '{1}', '{2}', '{3}');", index_sel+1, textBoxDate.Text, textBoxBarcode.Text, textBoxWorker.Text);

                    MySqlCommand command = new MySqlCommand(insertQuery, mysql);
                    if (command.ExecuteNonQuery() != 1)
                        MessageBox.Show("Failed to insert data.");

                    textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    textBoxBarcode.Text = "";
                    //textBoxWorker.Text = "";

                    selectTable();

                    listViewPhoneBook.TopItem = listViewPhoneBook.Items[listViewPhoneBook.Items.Count - 1];
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                    mysql.Open();
                    int pos = listViewPhoneBook.SelectedItems[0].Index;
                    int index = Convert.ToInt32(listViewPhoneBook.Items[pos].Text);
                    string updateQuery = string.Format("UPDATE accounts_table SET date = '{1}', barcode = '{2}', name = '{3}' WHERE id={0};", index, textBoxDate.Text, textBoxBarcode.Text, textBoxWorker.Text);

                    MySqlCommand command = new MySqlCommand(updateQuery, mysql);
                    if (command.ExecuteNonQuery() != 1)
                        MessageBox.Show("Failed to delete data.");

                    textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    textBoxBarcode.Text = "";
                    //textBoxWorker.Text = "";

                    selectTable();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    int pos = listViewPhoneBook.SelectedItems[0].Index;
                    int index = Convert.ToInt32(listViewPhoneBook.Items[pos].Text);
                    string deleteQuery = string.Format("DELETE FROM accounts_table WHERE id={0};", index);

                    MySqlCommand command = new MySqlCommand(deleteQuery, mysql);
                    if (command.ExecuteNonQuery() != 1)
                        MessageBox.Show("Failed to delete data.");

                    textBoxDate.Text = "";
                    textBoxBarcode.Text = "";
                    //textBoxWorker.Text = "";

                    selectTable();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void buttonSelect_Click(object sender, EventArgs e)
        {
            selectTable();
        }

        private void selectTable()
        {
            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format("SELECT * FROM accounts_table");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    MySqlDataReader table = command.ExecuteReader();

                    listViewPhoneBook.Items.Clear();

                    while (table.Read())
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = table["id"].ToString();
                        item.SubItems.Add(table["date"].ToString());
                        item.SubItems.Add(table["barcode"].ToString());
                        item.SubItems.Add(table["name"].ToString());

                        listViewPhoneBook.Items.Add(item);
                    }

                    table.Close();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void listViewPhoneBook_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView listview = sender as ListView;

            int index = listview.FocusedItem.Index;
            index_sel = index;
            textBoxDate.Text = listview.Items[index].SubItems[1].Text;
            textBoxBarcode.Text = listview.Items[index].SubItems[2].Text;
            textBoxWorker.Text = listview.Items[index].SubItems[3].Text;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            textBoxBarcode.Text = "";
            //textBoxWorker.Text = "";
            Num_wash.Text = "0";
        }

        private void buttonCount_Click(object sender, EventArgs e)
        {
            textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

            try
            {
                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
                {
                    mysql.Open();
                    string selectQuery = string.Format("SELECT COUNT(*) FROM accounts_table WHERE barcode = '{0}';", textBoxBarcode.Text);
                    //string selectQuery = string.Format("SELECT COUNT(*) FROM accounts_table WHERE barcode = 'BINX71A8239';");

                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
                    int totalCount = Convert.ToInt32(command.ExecuteScalar());

                    Num_wash.Text = Convert.ToString(totalCount);

                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        private void button_Export_Click(object sender, EventArgs e)
        {
            // Getting the location and file name of the excel to save from user
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.AddExtension = true;
            saveFileDialog.ValidateNames = true;
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            saveFileDialog.DefaultExt = ".xls";
            saveFileDialog.Filter = "Microsoft Excel Workbook (*.xls)|*.xlsx";
            saveFileDialog.FileName = "StudentData".ToString();
        }

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        //{
        //    Keys key = keyData & ~(Keys.Shift | Keys.Control);

        //    switch (key)
        //    {
        //        //case Keys.F10:// 조합키 사용 시
        //        //    if ((keyData & Keys.Control) != 0)
        //        //    {
        //        //        MessageBox.Show("Ctrl+F");
        //        //    }
        //        //    break;
        //        case Keys.F1:
        //            MessageBox.Show("A");
        //            break;
        //        case Keys.F2:
        //            MessageBox.Show("S");
        //            break;
        //        case Keys.F3:
        //            MessageBox.Show("D");
        //            break;
        //        case Keys.F4:
        //            selectTable();
        //            break;
        //        case Keys.F5:
        //            textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        //            textBoxBarcode.Text = "";
        //            //textBoxWorker.Text = "";
        //            Num_wash.Text = "0";
        //            break;
        //        case Keys.F6:
        //            textBoxDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        //            try
        //            {
        //                using (MySqlConnection mysql = new MySqlConnection(_connectionAddress))
        //                {
        //                    mysql.Open();
        //                    string selectQuery = string.Format("SELECT COUNT(*) FROM accounts_table WHERE barcode = '{0}';", textBoxBarcode.Text);
        //                    //string selectQuery = string.Format("SELECT COUNT(*) FROM accounts_table WHERE barcode = 'BINX71A8239';");

        //                    MySqlCommand command = new MySqlCommand(selectQuery, mysql);
        //                    int totalCount = Convert.ToInt32(command.ExecuteScalar());

        //                    Num_wash.Text = Convert.ToString(totalCount);

        //                }
        //            }
        //            catch (Exception exc)
        //            {
        //                MessageBox.Show(exc.Message);
        //            }
        //            break;
        //        default:
        //            //MessageBox.Show("지정되지 않은 키입니다.");
        //            //return base.ProcessCmdKey(ref msg, keyData);
        //            break;
        //    }
        //    return true;
        //}
    }
}
