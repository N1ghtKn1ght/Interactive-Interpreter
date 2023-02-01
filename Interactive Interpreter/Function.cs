using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InteractiveInterpreter
{
    public class Function
    {
        private string _command;
        private string[] _namesOfVars;
        public int CountVars
        {
            get => _namesOfVars.Length;
        }

        private Function(string command, string[] namesOfVars)
        {
            if (command.Length == 0)
                throw new ArgumentNullException();
            this._command = command;
            this._namesOfVars = namesOfVars;
        }

        public static Function CreateFunction(string input, out string name)
        {
            string match = Regex.Match(input, @"fn(.*)=>").Value;
            if (match.CheckValid(new Regex(@"fn([^fn])=>")))
                throw new ArgumentException("Function's or vars' name can't be fn");
            
            string[] _params = Regex.Split(Regex.Replace(match, @"fn[\s+]|[\s+]=>", ""), @"\s+");
            name = _params[0];
            
            string[] vars = _params.Skip(1).ToArray();
            if (vars.Distinct().Count() != vars.Length)
                throw new ArgumentException("Vars' names can't be same");

            string command = input.Replace(match, "");
            return TestCommand(command, vars);
        }

        private static Function TestCommand(string command, string[] namesOfVars)
        {
            Interpreter interpreter = new Interpreter();
            for (int i = 0; i < namesOfVars.Length; i++)
                interpreter.Input($"{namesOfVars[i]}=1");
            interpreter.Input(command);
            return new Function(command, namesOfVars);
        }

        public double? Call(List<string> vars, Dictionary<string, Function> functions)
        {
            if (vars.Count != _namesOfVars.Length)
                throw new ArgumentException("vars invalid");
            Interpreter interpreter = new Interpreter(functions);
            for (int i = 0; i < vars.Count; i++)
                interpreter.Input($"{_namesOfVars[i]}={vars[i]}");
            return interpreter.Input(_command);
        }
    }
}
