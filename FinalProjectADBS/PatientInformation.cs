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
    public partial class PatientInformation : UserControl
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        int status = 0;

        public PatientInformation()
        {
            InitializeComponent();

            cbStatus.SelectedIndex = 0;
            cbStatus.SelectedIndex = 0;
        }

        private void PatientInformation_Load(object sender, EventArgs e)
        {
            loadData();
        }

        public void loadData()
        {
            try
            {
                con.Open();

                string sql = "call viewPatient()";

                cmd = new MySqlCommand(sql, con);
                dtr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dtr);
                dgvData.DataSource = dt;

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
            txtPatientID.Clear();
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
            loadData();
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow row = dgvData.Rows[index];
            txtPatientID.Text = row.Cells[0].Value.ToString();
            txtFamilyNumber.Text = row.Cells[1].Value.ToString();
            txtLastName.Text = row.Cells[2].Value.ToString();
            txtFirstName.Text = row.Cells[3].Value.ToString();
            txtMiddleName.Text = row.Cells[4].Value.ToString();
            dtBday.Value = DateTime.Parse(row.Cells[5].Value.ToString());
            txtBirthPlace.Text = row.Cells[6].Value.ToString();

            string civilstatus = row.Cells[7].Value.ToString();

            if (civilstatus == "Single")
            {
                cbStatus.SelectedIndex = 0;
            }
            else if (civilstatus == "Married")
            {
                cbStatus.SelectedIndex = 1;
            }
            else if (civilstatus == "Separated")
            {
                cbStatus.SelectedIndex = 2;
            }
            else if (civilstatus == "Divorced")
            {
                cbStatus.SelectedIndex = 3;
            }
            else if (civilstatus == "Widowed")
            {
                cbStatus.SelectedIndex = 4;
            }

            string gender = row.Cells[8].Value.ToString();

            if(gender == "Male")
            {
                rbMale.Checked = true;
            }else if(gender == "Female")
            {
                rbFemale.Checked = true;
            }

            txtAddress.Text = row.Cells[9].Value.ToString();
            txtPhone.Text = row.Cells[10].Value.ToString();
            txtEmail.Text = row.Cells[11].Value.ToString();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to permanently delete this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();
                    string sql = "DELETE FROM records WHERE patientid = '" + txtPatientID.Text + "' ";
                    cmd = new MySqlCommand(sql, con);
                    dtr = cmd.ExecuteReader();

                    MessageBox.Show("Record Deleted Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    return;
                };

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

            loadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if all required fields are filled
                if (string.IsNullOrEmpty(txtLastName.Text) || string.IsNullOrEmpty(txtAddress.Text) || status == 0)
                {
                    MessageBox.Show("Please fill in all the required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method without proceeding further
                }

                if (MessageBox.Show("Are you sure you want to update this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();
                    string sql = "UPDATE records SET lastname = '"+ txtLastName.Text + "', middlename = '" + txtMiddleName.Text +"'," +
                        " statusNo = " + status + ", address = '" + txtAddress.Text + "', phone = '" + txtPhone.Text + "', " +
                        " email = '" + txtEmail.Text +"'" +
                        "WHERE patientid = "+ txtPatientID.Text +";";
                    cmd = new MySqlCommand(sql, con);
                    dtr = cmd.ExecuteReader();

                    MessageBox.Show("Record Updated Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    return;
                };


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            loadData();
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                string searchValue = txtSearch.Text.ToLower();  // Convert search term to lowercase
                con.Open();

                if (cbSearch.Text == "Patient ID")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE records.patientid = " + txtSearch.Text + ";";
                }
                else if (cbSearch.Text == "Family Number")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE records.familynumber = " + txtSearch.Text + " ";
                }
                else if (cbSearch.Text == "Last Name")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(records.lastname) = '" + searchValue + "'" +
                    "OR LOWER(records.lastname) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "First Name")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(records.firstname) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.firstname) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Middle Name")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(records.middlename) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.middlename) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Birthday")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) " +
                "WHERE LOWER(records.birthdate) = STR_TO_DATE('" + searchValue + "', '%Y-%m-%d') " +
                "OR LOWER(records.birthdate) LIKE '%" + searchValue + "%' " +
                "OR LOWER(records.birthdate) = '" + searchValue + "' " +
                "OR STR_TO_DATE(records.birthdate, '%M %d, %Y') = STR_TO_DATE('" + searchValue + "', '%M %d, %Y')" +
                "OR LOWER(date_format(records.birthdate, '%M %d, %Y')) = '" + searchValue + "'" +
                "OR LOWER(date_format(records.birthdate, '%M %d, %Y')) LIKE '%" + searchValue + "%'";
                }
                else if (cbSearch.Text == "Birth Place")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(records.birthplace) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.birthplace) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Sex")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(sex.sex) = LOWER('" + searchValue + "')" +
                    "OR LOWER(sex.sex) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Civil Status")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(civilstatus.status) = LOWER('" + searchValue + "')" +
                    "OR LOWER(civilstatus.status) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Address")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(records.address) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.address) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Phone")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(records.phone) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.phone) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Email")
                {
                    sql = "SELECT records.patientid as PatientID, records.familynumber as FamilyNumber, records.lastname as LastName, " +
                    "records.firstname as FirstName, records.middlename as MiddleName, date_format(records.birthdate, '%M %d, %Y') as Birthday, " +
                    "records.birthplace as BirthPlace, civilstatus.status as Civil_Status, sex.sex as Gender, records.address as Address, " +
                    "records.phone as Phone, records.email as Email " +
                    "FROM (( records INNER JOIN civilstatus on records.statusNo = civilstatus.statusNo) " +
                    "INNER JOIN sex on records.sexNo = sex.sexNo) WHERE LOWER(records.email) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.email) LIKE '%" + searchValue + "%' ";
                }

                cmd = new MySqlCommand(sql, con);
                dtr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dtr);
                dgvData.DataSource = dt;

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
