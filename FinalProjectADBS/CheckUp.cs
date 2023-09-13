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
    public partial class CheckUp : UserControl
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        public CheckUp()
        {
            InitializeComponent();
            cbSearch.SelectedIndex = 0;
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow row = dgvData.Rows[index];
            txtRecordID.Text = row.Cells[0].Value.ToString();
            txtPatientID.Text = row.Cells[2].Value.ToString();
            txtAge.Text = row.Cells[5].Value.ToString();
            txtWeight.Text = row.Cells[6].Value.ToString();
            txtHeight.Text = row.Cells[7].Value.ToString();
            txtCheckUp.Text = row.Cells[8].Value.ToString();
        }

        public void loadData()
        {
            try
            {
                con.Open();

                string sql = "call viewCheckUp()";

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

        private void CheckUp_Load(object sender, EventArgs e)
        {
            loadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if all required fields are filled
                if (string.IsNullOrEmpty(txtPatientID.Text) || string.IsNullOrEmpty(txtAge.Text) || string.IsNullOrEmpty(txtHeight.Text) ||
                string.IsNullOrEmpty(txtWeight.Text) || string.IsNullOrEmpty(txtCheckUp.Text))
                {
                    MessageBox.Show("Please fill in all the required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method without proceeding further
                }

                DateTime now = DateTime.Now;

                if (MessageBox.Show("Are you sure you want to add this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();

                    string sql = "INSERT INTO healthrecords(patientid, age, weight, height, checkup," +
                        "cudate)" +
                        "VALUES ('" + txtPatientID.Text + "', '" + txtAge.Text + "', '" + txtWeight.Text + "'," +
                        "'" + txtHeight.Text + "', '" + txtCheckUp.Text + "', '" + now.ToString("yyyy-MM-dd hh:mm:ss tt") + "')";
                    MySqlCommand cmd2 = new MySqlCommand(sql, con);
                    cmd2.ExecuteNonQuery();

                    MessageBox.Show("Record Added Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

            loadData();

        }

        public void ClearAll()
        {
            txtRecordID.Clear();
            txtPatientID.Clear();
            txtAge.Clear();
            txtWeight.Clear();
            txtHeight.Clear();
            txtCheckUp.Clear();
            txtSearch.Clear();
            cbSearch.SelectedIndex = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
            loadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if all required fields are filled
                if (string.IsNullOrEmpty(txtPatientID.Text) || string.IsNullOrEmpty(txtAge.Text) || string.IsNullOrEmpty(txtHeight.Text) ||
                string.IsNullOrEmpty(txtWeight.Text) || string.IsNullOrEmpty(txtCheckUp.Text))
                {
                    MessageBox.Show("Please fill in all the required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method without proceeding further
                }


                if (MessageBox.Show("Are you sure you want to update this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();
                    string sql = "UPDATE healthrecords SET age = '" + txtAge.Text + "', weight = '" + txtWeight.Text + "'," +
                        " height = '" + txtHeight.Text + "', checkup = '" + txtCheckUp.Text + "' " +
                        "WHERE recordid = " + txtRecordID.Text + ";";
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to permanently delete this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();
                    string sql = "DELETE FROM healthrecords WHERE recordid = '" + txtRecordID.Text + "' ";
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "";
                string searchValue = txtSearch.Text.ToLower();  // Convert search term to lowercase
                con.Open();

                if (cbSearch.Text == "Record ID")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE healthrecords.recordid = " + txtSearch.Text + ";";
                }
                else if (cbSearch.Text == "Family Number")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE records.familynumber = " + txtSearch.Text + " ";
                }
                else if (cbSearch.Text == "Patient ID")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE records.patientid = " + txtSearch.Text + " ";
                }
                else if (cbSearch.Text == "Last Name")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE LOWER(records.lastname) = '" + searchValue + "'" +
                    "OR LOWER(records.lastname) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "First Name")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE LOWER(records.firstname) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.firstname) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Age")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE LOWER(healthrecords.age) = LOWER('" + searchValue + "')" +
                    "OR LOWER(healthrecords.age) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Weight")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE LOWER(healthrecords.weight) = LOWER('" + searchValue + "')" +
                    "OR LOWER(healthrecords.weight) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Height")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE LOWER(healthrecords.height) = LOWER('" + searchValue + "')" +
                    "OR LOWER(healthrecords.height) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Check Up Details")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid WHERE LOWER(healthrecords.checkup) = LOWER('" + searchValue + "')" +
                    "OR LOWER(healthrecords.checkup) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Check Up Date")
                {
                    sql = "SELECT healthrecords.recordid as RecordID, records.familynumber as FamilyNumber, " +
                    "healthrecords.patientid as PatientID, records.lastname as LastName, records.FirstName," +
                    "healthrecords.age as Age, healthrecords.weight as Weight, healthrecords.height as Height, " +
                    "healthrecords.checkup as CheckUp_Details, date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date " +
                    "FROM healthrecords INNER JOIN records on healthrecords.patientid = records.patientid " +
                "WHERE LOWER(healthrecords.cudate) = STR_TO_DATE('" + searchValue + "', '%Y-%m-%d') " +
                "OR LOWER(healthrecords.cudate) LIKE '%" + searchValue + "%' " +
                "OR LOWER(healthrecords.cudate) = '" + searchValue + "' " +
                "OR STR_TO_DATE(healthrecords.cudate, '%M %d, %Y') = STR_TO_DATE('" + searchValue + "', '%M %d, %Y')" +
                "OR LOWER(date_format(healthrecords.cudate, '%M %d, %Y')) = '" + searchValue + "'" +
                "OR LOWER(date_format(healthrecords.cudate, '%M %d, %Y')) LIKE '%" + searchValue + "%'";
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
