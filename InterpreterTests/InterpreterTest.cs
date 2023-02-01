namespace InterpreterTests
{
    [TestFixture]
    public class InterpreterTest
    {
        private static void Check(ref Interpreter interpret, string inp, double? res)
        {
            double? result;
            try 
            { 
                result = interpret.Input(inp); 
            } 
            catch (Exception ex) 
            { 
                result = null;
                Console.WriteLine(ex.Message);
            }
            if (result != res) 
                Assert.Fail("input(\"" + inp + "\") == <" + res + "> and not <" + result + "> => wrong solution, aborted!"); 
            else 
                Console.WriteLine("input(\"" + inp + "\") == <" + res + "> was ok");
        }

        [Test]
        public void BasicArithmeticTests()
        {
            Interpreter interpret = new Interpreter();
            Check(ref interpret, "", null);
            Check(ref interpret, "     ", null);
            Check(ref interpret, "1 + 1", 2);
            Check(ref interpret, "2 - 1", 1);
            Check(ref interpret, "2 * 3", 6);
            Check(ref interpret, "8 / 4", 2);
            Check(ref interpret, "7 % 4", 3);
        }


        [Test]
        public void VariablesTests()
        {
            Interpreter interpret = new Interpreter();
            Check(ref interpret, "x = 1", 1);
            Check(ref interpret, "x", 1);
            Check(ref interpret, "x + 3", 4);
            Check(ref interpret, "y", null);
            Check(ref interpret, "x = y = 713", 713);
            Check(ref interpret, "x = 29 + (y = 11)", 40);
            Check(ref interpret, "y", 11);
        }

        [Test]
        public void FunctionsTests()
        {
            Interpreter interpret = new Interpreter();
            Check(ref interpret, "fn one => 1", null);
            Check(ref interpret, "fn avg x y => (x + y) / 2", null);
            Check(ref interpret, "fn echo x => x", null);
            Check(ref interpret, "fn add x y => x + z", null);
            Check(ref interpret, "fn add x x => x + x", null);
            Check(ref interpret, "(fn f => 1)", null);
            Check(ref interpret, "avg echo 4 echo 2", 3);
            Check(ref interpret, "fn f a b => a * b", null);
            Check(ref interpret, "fn g a b c => a * b * c", null);
            Check(ref interpret, "g g 1 2 3 f 4 5 f 6 7", 5040);
        }

        [Test]
        public void ConflictsTests()
        {
            Interpreter interpret = new Interpreter();
            Check(ref interpret, "fn x x => 0", null);
            Check(ref interpret, "fn avg => 0", null);
            Check(ref interpret, "avg = 5", null);
        }

        [Test]
        public void ComplexExpressions()
        {
            Interpreter interpret = new Interpreter();
            Check(ref interpret, "4 + 2 * 3", 10);
            Check(ref interpret, "4 / 2 * 3", 6);
            Check(ref interpret, "7 % 2 * 8", 8);
            Check(ref interpret, "2 * 3", 6);
            Check(ref interpret, "8 / 4", 2);
            Check(ref interpret, "(7 + 3) / (2 * 2 + 1)", 2);
            Check(ref interpret, "(10 / (8 - (4 + 2))) * 3", 15);
        }
    }
}