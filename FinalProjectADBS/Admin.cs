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
    public partial class Admin : Form
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        public Admin(string username)
        {
            InitializeComponent();
            lblUser.Text = username;
        }

        private void AddUserControl(UserControl uc)
        {
            ucPanel.Controls.Clear();
            ucPanel.Controls.Add(uc);
        }


        private void lblAddRecords_Click(object sender, EventArgs e)
        {
            AddRecords ucAddRecords = new AddRecords();
            AddUserControl(ucAddRecords);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Display a confirmation dialog box to the user
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Login login = new Login();
                login.Show();
                this.Close();
            }
        }

        private void btnAddRecords_Click(object sender, EventArgs e)
        {
            AddRecords ucAddRecords = new AddRecords();
            AddUserControl(ucAddRecords);
        }

        private void btnAddUsers_Click(object sender, EventArgs e)
        {
            AddUsers user = new AddUsers();
            AddUserControl(user);
        }

        private void btnUserLogs_Click(object sender, EventArgs e)
        {
            UserLogs logs = new UserLogs();
            AddUserControl(logs);
        }

        private void btnPatientInfo_Click(object sender, EventArgs e)
        {
            PatientInformation info = new PatientInformation();
            AddUserControl(info);
        }

        private void btnCheckup_Click(object sender, EventArgs e)
        {
            CheckUp checkUp = new CheckUp();
            AddUserControl(checkUp);
        }

        private void btnDiagnosis_Click(object sender, EventArgs e)
        {
            Diagnosis diagnosis = new Diagnosis(lblUser.Text);
            AddUserControl(diagnosis);
        }

        private void btnMonthly_Click(object sender, EventArgs e)
        {
            Monthly monthly = new Monthly();
            AddUserControl(monthly);
        }

        private void btnYearly_Click(object sender, EventArgs e)
        {
            Yearly yearly = new Yearly();
            AddUserControl(yearly);
        }
    }
}
