using System.Reflection;
using System.Threading;
using QuizApp;
using Xunit;

namespace UnitTestProject1
{
    public class TestableForm2 : Form2
    {
        public List<Quiz> Questions => baseQuestions;

        // Expose protected LoadQuestions for testing
        public void InvokeLoadQuestions(string path)
        {
            LoadQuestions(path);
        }

        // Access private 'questions' field via reflection
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

    }
}
