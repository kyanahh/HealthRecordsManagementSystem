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
    public partial class Diagnosis : UserControl
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        public Diagnosis(string username)
        {
            InitializeComponent();

            lblUser.Text = username;
        }

        private void Diagnosis_Load(object sender, EventArgs e)
        {
            loadData();
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow row = dgvData.Rows[index];
            txtDiagnosisID.Text = row.Cells[0].Value.ToString();
            txtRecordID.Text = row.Cells[1].Value.ToString();
            txtDiagnosis.Text = row.Cells[7].Value.ToString();
        }

        public void loadData()
        {
            try
            {
                con.Open();

                string sql = "call viewDiagnosis()";

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
            txtDiagnosisID.Clear();
            txtRecordID.Clear();
            txtDiagnosis.Clear();
            txtSearch.Clear();
            cbSearch.SelectedIndex = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
            loadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();

                // Check if all required fields are filled
                if (string.IsNullOrEmpty(txtRecordID.Text) || string.IsNullOrEmpty(txtDiagnosis.Text))
                {
                    MessageBox.Show("Please fill in all the required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method without proceeding further
                }

                string reserverQuery = "SELECT userid FROM user WHERE username = '" + lblUser.Text + "'";
                MySqlCommand reserverCmd = new MySqlCommand(reserverQuery, con);
                int userid = Convert.ToInt32(reserverCmd.ExecuteScalar());

                if (MessageBox.Show("Are you sure you want to add this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {

                    string sql = "INSERT INTO diagnosis(recordid, diagnosis, userid)" +
                        "VALUES ('" + txtRecordID.Text + "', '" + txtDiagnosis.Text + "', " + userid + ")";
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

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if all required fields are filled
                if (string.IsNullOrEmpty(txtRecordID.Text) || string.IsNullOrEmpty(txtDiagnosis.Text))
                {
                    MessageBox.Show("Please fill in all the required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit the method without proceeding further
                }


                if (MessageBox.Show("Are you sure you want to update this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();
                    string sql = "UPDATE diagnosis SET recordid = '" + txtRecordID.Text + "', diagnosis = '" + txtDiagnosis.Text + "'" +
                        " WHERE diagnosisid = " + txtDiagnosisID.Text + ";";
                    cmd = new MySqlCommand(sql, con);
                    dtr = cmd.ExecuteReader();

                    MessageBox.Show("Record Updated Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to permanently delete this record?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    con.Open();
                    string sql = "DELETE FROM diagnosis WHERE diagnosisid = '" + txtDiagnosisID.Text + "' ";
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

                if (cbSearch.Text == "Diagnosis ID")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE diagnosis.diagnosisid = " + txtSearch.Text + ";";
                }
                else if (cbSearch.Text == "Record ID")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE healthrecords.recordid = " + txtSearch.Text + ";";
                }
                else if (cbSearch.Text == "Family Number")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE records.familynumber = " + txtSearch.Text + " ";
                }
                else if (cbSearch.Text == "Patient ID")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE records.patientid = " + txtSearch.Text + " ";
                }
                else if (cbSearch.Text == "Last Name")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE LOWER(records.lastname) = '" + searchValue + "'" +
                    "OR LOWER(records.lastname) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "First Name")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE LOWER(records.firstname) = LOWER('" + searchValue + "')" +
                    "OR LOWER(records.firstname) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Check Up Details")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE LOWER(healthrecords.checkup) = LOWER('" + searchValue + "')" +
                    "OR LOWER(healthrecords.checkup) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Diagnosis")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE LOWER(diagnosis.diagnosis) = LOWER('" + searchValue + "')" +
                    "OR LOWER(diagnosis.diagnosis) LIKE '%" + searchValue + "%' ";
                }
                else if (cbSearch.Text == "Check Up Date")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                "WHERE LOWER(healthrecords.cudate) = STR_TO_DATE('" + searchValue + "', '%Y-%m-%d') " +
                "OR LOWER(healthrecords.cudate) LIKE '%" + searchValue + "%' " +
                "OR LOWER(healthrecords.cudate) = '" + searchValue + "' " +
                "OR STR_TO_DATE(healthrecords.cudate, '%M %d, %Y') = STR_TO_DATE('" + searchValue + "', '%M %d, %Y')" +
                "OR LOWER(date_format(healthrecords.cudate, '%M %d, %Y')) = '" + searchValue + "'" +
                "OR LOWER(date_format(healthrecords.cudate, '%M %d, %Y')) LIKE '%" + searchValue + "%'";
                }
                else if (cbSearch.Text == "Recorder")
                {
                    sql = "SELECT diagnosis.diagnosisid as DiagnosisID, healthrecords.recordid as RecordID, " +
                        "records.familynumber as FamilyNumber, records.patientid as PatientID, records.lastname as LastName, " +
                        "records.firstname as FirstName, healthrecords.checkup as CheckUp_Details, diagnosis.diagnosis as Diagnosis, " +
                        "date_format(healthrecords.cudate, '%M %d, %Y') as CheckUp_Date, user.username as Recorder " +
                        "FROM (((diagnosis INNER JOIN healthrecords on diagnosis.recordid = healthrecords.recordid) " +
                        "INNER JOIN records on healthrecords.patientid = records.patientid) " +
                        "INNER JOIN user on diagnosis.userid = user.userid) " +
                        "WHERE LOWER(user.username) = LOWER('" + searchValue + "')" +
                    "OR LOWER(user.username) LIKE '%" + searchValue + "%' ";
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
