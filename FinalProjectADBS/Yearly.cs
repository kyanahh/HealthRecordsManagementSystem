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
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts.WinForms;
using System.Globalization;

namespace FinalProjectADBS
{
    public partial class Yearly : UserControl
    {
        MySqlConnection con = new MySqlConnection("datasource=localhost;username=root;password=;database=putatan;");
        MySqlCommand cmd = new MySqlCommand();
        MySqlDataReader dtr;

        public Yearly()
        {
            InitializeComponent();
        }

        private void Yearly_Load(object sender, EventArgs e)
        {
            LoadYearlyRecords();
        }

        private void LoadYearlyRecords()
        {
            try
            {
                con.Open();

                // Get the yearly records count
                string sql = "SELECT YEAR(cudate) AS Year, COUNT(*) AS RecordCount " +
                             "FROM healthrecords " +
                             "GROUP BY YEAR(cudate)";

                MySqlCommand cmd = new MySqlCommand(sql, con);
                MySqlDataReader reader = cmd.ExecuteReader();

                // Create the series collection for the pie chart
                SeriesCollection seriesCollection = new SeriesCollection();

                // Loop through the data reader and create a PieSeries for each year
                while (reader.Read())
                {
                    // Get the year and record count
                    int year = reader.GetInt32("Year");
                    int recordCount = reader.GetInt32("RecordCount");

                    // Create a PieSeries for the year
                    PieSeries pieSeries = new PieSeries
                    {
                        Title = year.ToString(),
                        Values = new ChartValues<int> { recordCount },
                        DataLabels = true
                    };

                    // Add the PieSeries to the series collection
                    seriesCollection.Add(pieSeries);
                }

                // Create the chart
                LiveCharts.WinForms.PieChart chart = new LiveCharts.WinForms.PieChart
                {
                    Series = seriesCollection,
                    LegendLocation = LegendLocation.Right,
                    Dock = DockStyle.Fill
                };

                // Set the chart size and location
                chart.Size = new Size(1058, 564);
                chart.Location = new Point(152, 93);

                // Add the chart to the form
                Controls.Add(chart);

                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
