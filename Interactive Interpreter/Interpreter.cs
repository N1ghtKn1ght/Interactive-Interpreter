using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace InteractiveInterpreter
{
    public class Interpreter
    {
        private Regex _regexBrackets = new Regex(@"\(([^()]*)\)");
        private Regex _regexFunctions = new Regex(@"fn[\s\w\s]*=>");
        private Dictionary<string, double> _vars = new Dictionary<string, double>();
        private Dictionary<string, Function> _functions;
        private char[] _operators = new[]
        {
            '/', '=', '*', '+', '-', '%'
        };

        public Interpreter(Dictionary<string, Function> functions = null)
        {
            if (functions != null)
                this._functions = new Dictionary<string, Function>(functions);
            else
                this._functions = new Dictionary<string, Function>();
        }

        public double? Input(string input)
        {
            input = Regex.Replace(input, @"\s+", " ");

            if (input.Contains("=>") || input.Contains("fn"))
            {
                if (!input.CheckValid(_regexFunctions))
                    throw new ArgumentException("Function isn't full");

                Function function = Function.CreateFunction(input, out string name);
                _functions[name] = function;
                return null;
            }

            string nameOfFunction = input.Split(' ').FirstOrDefault(_functions.Keys.Contains);
            if (nameOfFunction.CheckValid())
            {
                string _params = input.Substring(input.IndexOf(nameOfFunction) + nameOfFunction.Length);
                string[] vars = Regex.Split(_params, @"\s").Where(x => !string.IsNullOrEmpty(x.Replace(" ", ""))).ToArray();
                List<string> _vars = new List<string>();
                for (int i = 0; i < vars.Length; i++)
                {
                    string var = vars[i];
                    if (_functions.TryGetValue(var, out Function function))
                    {
                        _vars.Add(string.Join(" ", vars.Skip(i).Take(function.CountVars + 1)));
                        i += function.CountVars;
                        continue;
                    }
                    _vars.Add(var);
                }
                return _functions[nameOfFunction].Call(_vars, _functions);
            }
            var valid = Regex.Split(input, @"\s+").Where(x => !string.IsNullOrEmpty(x.Replace(" ", ""))).ToArray();
            if (valid.Length > 1 && !input.Any(_operators.Contains))
                return null;
            if (input.Contains('='))
            {
                string[] parts = input.Split('=');
                while (input.Contains("("))
                {
                    var array = _regexBrackets.Matches(input);
                    foreach (Match item in array)
                    {
                        string expr = item.Value.Substring(1, item.Value.Length - 2);
                        double? _value = this.Input(expr);
                        input = input.Replace(item.Value, $"{_value}");
                    }
                }
                double? value = this.Input(input.Substring(input.IndexOf("=") + 1));
                if (value == null)
                    return null;
                _vars[Regex.Replace(parts[0], @"\s+", "")] = (double)value;
                return value;
            }

            return Calculate(input);
        }

        private double GetValue(string str) => double.TryParse(str, out double value) ? value : _vars[str];

        private double Calculate(string s)
        {
            s = Regex.Replace(s, @"\s+", "");
            while (s.Contains("("))
            {
                var array = _regexBrackets.Matches(s);
                foreach (Match item in array)
                {
                    string expr = item.Value.Substring(1, item.Value.Length - 2);
                    double value = Calculate(expr);
                    s = s.Replace(item.Value, $"{value}");
                }
            }
            string[] split = Regex.Split(Regex.Replace(s, @"(?<=[\d])-", "+-"), @"[+]");
            List<double> sum = new List<double>();
            foreach (string item in split)
            {
                if (item.Contains("*"))
                {
                    double value = item.Split('*').Select(x => Calculate(x)).Aggregate((a, x) => a * x);
                    sum.Add(value);
                    continue;
                }
                if (item.Contains("/"))
                {
                    double value = item.Split('/').Select(x => Calculate(x)).Aggregate((a, x) => a / x);
                    sum.Add(value);
                    continue;
                }
                if (item.Contains("%"))
                {
                    double value = item.Split('%').Select(x => Calculate(x)).Aggregate((a, x) => a % x);
                    sum.Add(value);
                    continue;
                }
                sum.Add(GetValue(item));
            }
            return sum.Sum();
        }
    }
}
