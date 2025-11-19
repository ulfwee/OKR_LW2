using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizApp
{
    public class TestForm1 : Form1
{
    public bool CreatedForm2 { get; private set; }

    protected override Form CreateForm2()
    {
        CreatedForm2 = true;
        return new Form();  // Anything, doesn't matter
    }
}

public class TestForm2 : Form2
{
    public bool QuitCalled { get; private set; }

    protected override void Quit()
    {
        QuitCalled = true;
    }
}

}
