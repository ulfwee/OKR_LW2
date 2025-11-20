using System.Text.Json;

namespace QuizApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected virtual Form CreateForm2() => new Form2();

        private void button1_Click(object sender, EventArgs e)
        {
            var f = CreateForm2();
            f.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var results = LoadResults();
            Form3 form3 = new Form3(results);
            form3.Show();
            this.Hide();
        }

        private List<QuizResult> LoadResults()
        {
            string filePath = Path.Combine(Application.StartupPath, "results.json");
            if (!File.Exists(filePath)) return new List<QuizResult>();

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<List<QuizResult>>(json) ?? new List<QuizResult>();
            }
            catch
            {
                return new List<QuizResult>();
            }
        }

       

        private void button3_Click_1(object sender, EventArgs e)
        {
Application.Exit();
        }
    }
}
