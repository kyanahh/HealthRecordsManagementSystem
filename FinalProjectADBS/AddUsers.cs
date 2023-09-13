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
using BCrypt.Net;



namespace FinalProjectADBS
{
    public partial class AddUsers : UserControl
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        public AddUsers()
        {
            InitializeComponent();
            cmbType.SelectedIndex = 0;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if username already exists
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM user WHERE username = @username";
                cmd = new MySqlCommand(checkSql, con);
                cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

                if (count > 0)
                {
                    lblWrong.Text = "Username already exists!";
                    return;
                }

                if (cmbType.SelectedIndex == 0)
                {
                    MessageBox.Show("Please select a type!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("Are you sure you want to add this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    con.Open();

                    // Generate a salt and hash the password
                    string salt = BCrypt.Net.BCrypt.GenerateSalt();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text, salt);

                    string sql = "INSERT INTO user(username, password, type) VALUES(@username, @password, @type)";
                    cmd = new MySqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@type", cmbType.SelectedIndex);

                    dtr = cmd.ExecuteReader();

                    lblWrong.Text = "Record added successfully";
                }

                ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void ClearAll()
        {
            cmbType.SelectedIndex = 0;
            txtUsername.Clear();
            txtPassword.Clear();
        }
    }
}
