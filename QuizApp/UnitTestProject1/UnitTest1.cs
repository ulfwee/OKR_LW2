using Xunit;
using QuizApp;
using System.Threading;

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

    }
}
