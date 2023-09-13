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
    public partial class UserLogs : UserControl
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        public UserLogs()
        {
            InitializeComponent();
        }

        private void UserLogs_Load(object sender, EventArgs e)
        {
            loadData();
        }

        public void loadData()
        {
            try
            {
                con.Open();

                string sql = "call viewLogs()";
                cmd = new MySqlCommand(sql, con);
                dtr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dtr);
                dgvUserLogs.DataSource = dt;

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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                con.Open();

                if (cbSearch.Text == "User ID")
                {
                    sql = "SELECT logtime.logid as LogTimeID, user.userid as UserID, log_time as Login_Time FROM user " +
                        "INNER JOIN logtime on user.userid = logtime.userid " +
                        "WHERE user.userid = " + txtSearch.Text + " ";
                }

                cmd = new MySqlCommand(sql, con);
                dtr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dtr);
                dgvUserLogs.DataSource = dt;
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
