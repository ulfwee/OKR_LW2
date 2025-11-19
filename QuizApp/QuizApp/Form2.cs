using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Formats.Asn1.AsnWriter;

namespace QuizApp
{
    public partial class Form2 : Form
    {
        private List<Quiz> questions = new List<Quiz>();
        private int currentIndex = 0;
        private readonly Dictionary<int, string> userAnswers = new Dictionary<int, string>();
        private int score = 0;
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
            SaveCurrentAnswer();

            if (currentIndex < questions.Count - 1)
            {
                if (currentIndex > 0) pictureBox2.Enabled = true;

                currentIndex++;
                DisplayQuestion();
            }
            else
            {
                FinishQuiz();
            }
        }
        private void RestartQuiz()
        {
            currentIndex = 0;

            score = 0;

            progressBar1.Value = 0;

            DisplayQuestion();

            pictureBox2.Enabled = false;

        }

        private void SaveCurrentAnswer()
        {
            var checkedRb = groupBox1.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
            if (checkedRb != null)
                userAnswers[currentIndex] = checkedRb.Text;
        }

        private void FinishQuiz()
        {
            int correct = 0;
            for (int i = 0; i < questions.Count; i++)
            {
                if (userAnswers.TryGetValue(i, out string ans) && ans == questions[i].CorrectAnswer)
                    correct++;
            }

            var result = new QuizResult
            {
                Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                CorrectAnswers = correct,
                TotalQuestions = questions.Count
            };

            string filePath = Path.Combine(Application.StartupPath, "results.json");
            var allResults = new List<QuizResult>();

            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    allResults = JsonSerializer.Deserialize<List<QuizResult>>(json) ?? new List<QuizResult>();
                }
                catch { }
            }

            allResults.Add(result);
            File.WriteAllText(filePath, JsonSerializer.Serialize(allResults, new JsonSerializerOptions { WriteIndented = true }));

            Form3 form3 = new Form3(allResults);
            this.Hide();
            form3.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                DisplayQuestion();
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            RestartQuiz();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Graphics mgraphics = e.Graphics;
            Pen pen = new Pen(Color.FromArgb(183, 228, 199));

            Rectangle area = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            LinearGradientBrush lgb = new LinearGradientBrush(area,
                Color.FromArgb(183, 228, 199),
                Color.FromArgb(79, 136, 141),
                LinearGradientMode.BackwardDiagonal);

            mgraphics.FillRectangle(lgb, area);
            mgraphics.DrawRectangle(pen, area);
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = (GroupBox)sender;

            if (box.Parent != null)
            {
                PaintEventArgs pea = new PaintEventArgs(e.Graphics, e.ClipRectangle);
                GraphicsState state = e.Graphics.Save();
                e.Graphics.TranslateTransform(-box.Left, -box.Top);
                InvokePaintBackground(box.Parent, pea);
                InvokePaint(box.Parent, pea);
                e.Graphics.Restore(state);
            }

            Color borderColor = Color.Transparent;
            using (Pen pen = new Pen(borderColor, 2))
            {
                Size textSize = TextRenderer.MeasureText(box.Text, box.Font);
                Rectangle rect = new Rectangle(
                    box.ClientRectangle.X,
                    box.ClientRectangle.Y + (textSize.Height / 2),
                    box.ClientRectangle.Width - 1,
                    box.ClientRectangle.Height - (textSize.Height / 2) - 1);

                e.Graphics.DrawRectangle(pen, rect);

                TextRenderer.DrawText(e.Graphics, box.Text, box.Font,
                    new Point(rect.X + 10, rect.Y - (textSize.Height / 2)),
                    box.ForeColor);
            }
        }
    }
}