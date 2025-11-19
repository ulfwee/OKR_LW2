using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using QuizApp;
using Xunit;

namespace UnitTestProject1
{
    public class TestableForm2 : Form2
    {
        public List<Quiz> Questions => baseQuestions;

        public void InvokeLoadQuestions(string path)
        {
            LoadQuestions(path);
        }

        private List<Quiz> baseQuestions
        {
            get
            {
                var field = typeof(Form2).GetField("questions",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                return (List<Quiz>)field.GetValue(this);
            }
        }
    }

    public class TestableForm3 : Form3
    {
        public TestableForm3() : base(new List<QuizResult>()) { }

        public void InvokeLoadResults(List<QuizResult> results)
        {
            LoadResults(results);
        }

        public DataGridView Grid => ResultsGrid;
    }



    public class UnitTest1
    {
        [Fact]
        public void Form1_Should_Open_Without_Exception()
        {
            var exception = Record.Exception(() =>
            {
                var form = new Form1();
                form.Dispose();
            });

            Assert.Null(exception);
        }

        [Fact]
        public void Form2_Should_Open_Without_Exception()
        {
            var exception = Record.Exception(() =>
            {
                var form = new Form2();
                form.Dispose();
            });

            Assert.Null(exception);
        }

        [Fact]
        public void Form1_Should_Open_Form2()
        {
            var form = new TestForm1();

            var method = typeof(Form1)
                .GetMethod("button1_Click", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(form, new object[] { null, EventArgs.Empty });

            Assert.True(form.CreatedForm2);
        }

        [Fact]
        public void Form2_Should_Call_Quit()
        {
            var form = new TestForm2();

            var method = typeof(Form2)
                .GetMethod("button1_Click", BindingFlags.NonPublic | BindingFlags.Instance);

            method.Invoke(form, new object[] { null, EventArgs.Empty });

            Assert.True(form.QuitCalled);
        }

        [Fact]
        public void LoadQuestions_ShouldLoad_FromValidJson()
        {
            // Arrange: create temp JSON file
            string tempFile = Path.GetTempFileName();
            string json = @"[
            {
                ""Question"": ""What is 2+2?"",
                ""Options"": [""3"", ""4"", ""5"", ""6""],
                ""Correct"": ""4""
            }
        ]";
            File.WriteAllText(tempFile, json);

            var form = new TestableForm2(); // subclass to expose questions list

            // Act
            form.InvokeLoadQuestions(tempFile);

            // Assert
            Assert.Single(form.Questions);
            Assert.Equal("What is 2+2?", form.Questions[0].Question);
            Assert.Equal(4, form.Questions[0].Options.Count);
        }

        [Fact]
        public void LoadQuestions_EmptyJson_ShouldReturnEmptyList()
        {
            string tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "[]");

            var form = new TestableForm2();

            form.InvokeLoadQuestions(tempFile);

            Assert.Empty(form.Questions);
        }

        [Fact]
        public void LoadQuestions_FileMissing_ShouldNotThrow()
        {
            var form = new TestableForm2();

            form.InvokeLoadQuestions("this_file_does_not_exist.json");

            Assert.Empty(form.Questions);
        }

        [Fact]
        public void LoadResults_ShouldFillDataGrid_WithCorrectRows()
        {
            // Arrange
            var results = new List<QuizResult>
    {
        new QuizResult { Date = "2025-01-01", CorrectAnswers = 4, TotalQuestions = 5 },
        new QuizResult { Date = "2025-01-02", CorrectAnswers = 3, TotalQuestions = 5 }
    };

            var form = new TestableForm3();

            // Act
            form.InvokeLoadResults(results);

            // Assert
            Assert.Equal(2, form.Grid.Rows.Count);  // two results loaded

            Assert.Equal("2025-01-01", form.Grid.Rows[0].Cells[0].Value);
            Assert.Equal(4, form.Grid.Rows[0].Cells[1].Value);
            Assert.Equal(5, form.Grid.Rows[0].Cells[2].Value);

            Assert.Equal("2025-01-02", form.Grid.Rows[1].Cells[0].Value);
        }

        [Fact]
        public void LoadResults_ShouldCreateCorrectColumns()
        {
            var form = new TestableForm3();

            form.InvokeLoadResults(new List<QuizResult>());

            Assert.Equal(3, form.Grid.Columns.Count);
            Assert.Equal("Date & Time", form.Grid.Columns[0].HeaderText);
            Assert.Equal("Correct", form.Grid.Columns[1].HeaderText);
            Assert.Equal("Total", form.Grid.Columns[2].HeaderText);
        }

        [Fact]
        public void LoadResults_ShouldSetGridToReadOnly()
        {
            var form = new TestableForm3();

            form.InvokeLoadResults(new List<QuizResult>());

            Assert.True(form.Grid.ReadOnly);
        }


    }
}
