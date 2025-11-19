using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp
{
    public class Quiz
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
