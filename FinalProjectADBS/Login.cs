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


namespace FinalProjectADBS
{
    public partial class Login : Form
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string sql = "SELECT * FROM user where username = '" + txtUsername.Text + "' AND password = '" + txtPassword.Text + "'";
                cmd = new MySqlCommand(sql, con);
                dtr = cmd.ExecuteReader();


                DateTime now = DateTime.Now;
                string formattedDate = now.ToString("yyyy-MM-dd HH:mm:ss");

                if (dtr.Read())
                {
                    int userId = Convert.ToInt32(dtr.GetValue(0));
                    string type = dtr.GetValue(3).ToString();

                    dtr.Close();

                    string sql2 = "INSERT INTO logtime(log_time, userid) VALUES('" + formattedDate + "', '" + userId + "')";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, con);
                    MySqlDataReader dtr2 = cmd2.ExecuteReader();

                    if (type == "1")
                    {
                        Admin admin = new Admin(txtUsername.Text);
                        admin.Show();
                        this.Visible = false;

                        admin.btnAddUsers.Enabled = true;
                        admin.btnUserLogs.Enabled = true;
                        admin.btnDiagnosis.Enabled = true;
                        admin.btnMonthly.Enabled = true;
                        admin.btnYearly.Enabled = true;
                    }
                    else if(type == "2")
                    {
                        Admin admin = new Admin(txtUsername.Text);
                        admin.Show();
                        this.Visible = false;

                        admin.btnDiagnosis.Enabled = true;
                    }else if(type == "3")
                    {
                        Admin admin = new Admin(txtUsername.Text);
                        admin.Show();
                        this.Visible = false;
                    }else if(type == "4")
                    {
                        Admin admin = new Admin(txtUsername.Text);
                        admin.Show();
                        this.Visible = false;


                    }
                }
                else
                {
                    lblWrong.Text = "The username/password you entered is incorrect";
                }
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
    }
}
