using System.Reflection;
using System.Threading;
using QuizApp;
using Xunit;

namespace UnitTestProject1
{
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


    }
}
