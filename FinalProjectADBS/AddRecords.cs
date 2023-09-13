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
    public partial class AddRecords : UserControl
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        int sex = 0; 
        int status = 0;

        public AddRecords()
        {
            InitializeComponent();

            cbStatus.SelectedIndex = 0;
        }

        public void Reset()
        {
            txtLast.Clear();
            txtFN.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            { 

                DialogResult result = MessageBox.Show("Are you sure you want to generate this family number?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    con.Open();
                    string sql = "INSERT INTO family(lastname) VALUES('" + txtLast.Text + "')";
                    cmd = new MySqlCommand(sql, con);
                    dtr = cmd.ExecuteReader();

                    // Close the data reader before executing the SELECT statement
                    if (dtr != null)
                    {
                        dtr.Close();
                    }

                    // Get the last inserted family number
                    string getLastInserted = "SELECT familynumber FROM family ORDER BY familynumber DESC LIMIT 1";
                    cmd = new MySqlCommand(getLastInserted, con);
                    string lastInsertedFamilyNumber = cmd.ExecuteScalar().ToString();
                    txtFN.Text = lastInsertedFamilyNumber;

                    lblWrong.Text = "Family Number added successfully";

                    txtLast.Clear();


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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        public void ClearAll()
        {
            txtFamilyNumber.Clear();
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            dtBday.ResetText();
            txtBirthPlace.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            rbMale.Checked = false;
            rbFemale.Checked = false;
            cbStatus.SelectedIndex = 0;
        }

        public void Sex()
        {
            if (rbMale.Checked)
                sex = 1;
            else if (rbFemale.Checked)
                sex = 2;
            else
                sex = 0;
        }

        
        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to add this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();

                    Sex();

                    string sql = "INSERT INTO records(familynumber, lastname, firstname, middlename, birthdate," +
                        "birthplace, statusNo, sexNo, address, phone, email)" +
                        "VALUES ('"+ txtFamilyNumber.Text + "', '" + txtLastName.Text + "', '" + txtFirstName.Text +"'," +
                        "'" + txtMiddleName.Text + "', '" + dtBday.Value.ToString("yyyy-MM-dd") + "', '" + txtBirthPlace.Text +"'," +
                        "" + status + ", " + sex + ", '" + txtAddress.Text + "', '" + txtPhone.Text + "', '" + txtEmail.Text +"')";
                    MySqlCommand cmd2 = new MySqlCommand(sql, con);
                    cmd2.ExecuteNonQuery();

                }


                MessageBox.Show("Record Added Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void cbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = cbStatus.SelectedItem.ToString();

            if (selectedItem == "Single")
                status = 1;
            else if (selectedItem == "Married")
                status = 2;
            else if (selectedItem == "Separated")
                status = 3;
            else if (selectedItem == "Divorced")
                status = 4;
            else if (selectedItem == "Widowed")
                status = 5;
            else
                status = 0;
        }
    }
}
