using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuizApp
{
    public partial class Form2 : Form
    {
        private List<Quiz> questions = new List<Quiz>();
        private int currentIndex = 0;
        private readonly Dictionary<int, string> userAnswers = new Dictionary<int, string>();

        private string selectedFilePath = "";


        public Form2()
        {
            InitializeComponent();
            label1.BackColor = Color.Transparent;
            groupBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.Transparent;


            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

        }

        protected virtual void Quit() => Application.Exit();

        private void button1_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select quiz file";
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    LoadQuestions(selectedFilePath);

                    if (questions.Count > 0)
                    {
                        //SetProgressBarMax();
                        DisplayQuestion();

                        radioButton1.CheckedChanged += RadioButton_CheckedChanged;
                        radioButton2.CheckedChanged += RadioButton_CheckedChanged;
                        radioButton3.CheckedChanged += RadioButton_CheckedChanged;
                        radioButton4.CheckedChanged += RadioButton_CheckedChanged;
                    }
                    else
                    {
                        MessageBox.Show("The selected file has no valid questions.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            }

        }

        protected void LoadQuestions(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    questions = JsonSerializer.Deserialize<List<Quiz>>(jsonContent) ?? new List<Quiz>();

                    if (questions.Count == 0)
                        MessageBox.Show("JSON file is empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading JSON: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("File not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DisplayQuestion()
        {
            foreach (RadioButton rb in groupBox1.Controls.OfType<RadioButton>())
                rb.Checked = false;

            if (questions.Count == 0) return;

            var q = questions[currentIndex];
            label1.Text = $"{currentIndex + 1}. {q.Question}";

            radioButton1.Text = q.Options[0];
            radioButton2.Text = q.Options[1];
            radioButton3.Text = q.Options[2];
            radioButton4.Text = q.Options[3];

            if (userAnswers.TryGetValue(currentIndex, out string saved))
            {
                var rb = groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Text == saved);
                if (rb != null) rb.Checked = true;
            }

            button3.Enabled = currentIndex > 0;
            UpdateNextButton();
            // UpdateProgressBar();
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;

            userAnswers[currentIndex] = rb.Text;
            UpdateNextButton();
        }
        private void UpdateNextButton()
        {
            bool hasAnswer = groupBox1.Controls.OfType<RadioButton>().Any(r => r.Checked);

            if (currentIndex == questions.Count - 1)
                button2.Text = "End";


            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
