using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuizApp
{
    public partial class Form3 : Form
    {
        protected DataGridView ResultsGrid => dataGridView1;

        public Form3(List<QuizResult> results)
        {
            InitializeComponent();
            LoadResults(results);
        }

        protected void LoadResults(List<QuizResult> results)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date & Time", typeof(string));
            dt.Columns.Add("Correct", typeof(int));
            dt.Columns.Add("Total", typeof(int));

            foreach (var r in results)
            {
                dt.Rows.Add(r.Date, r.CorrectAnswers, r.TotalQuestions);
            }

            dataGridView1.DataSource = dt;

            dataGridView1.ReadOnly = true;

        }
        private void label2_Click(object sender, EventArgs e)
        {
            new Form1().Show();
            this.Hide();
        }
    }
}
