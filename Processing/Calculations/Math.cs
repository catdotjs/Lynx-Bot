using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace Lynx_Bot.Processing.Calculations {
    static class CustomMath {
        public static Dictionary<string, Func<float,float,float>> Functions = new Dictionary<string, Func<float, float, float>>() {
            { "pow", (a,b)=>MathF.Pow(a,b)},    // Power
            { "sqrt", (a,b)=>MathF.Sqrt(a)},  // Square-Root
            { "root", (a,b)=>MathF.Pow(a,1f/b)},  // Root
            { "exp", (a,b)=>MathF.Pow(MathF.E,a)},  // Exponential
            { "log", (a,b)=>MathF.Log(a,b)},   // Log
            { "ln", (a,b)=>MathF.Log(a,MathF.E)},    // Natural-Log
            { "sin", (a,b)=>MathF.Sin(a)},   // Sine
            { "cos", (a,b)=>MathF.Cos(a)},   // Cosine
            { "tan", (a,b)=>MathF.Tan(a)},   // Tan
        };

        public static Dictionary<string, double> Symbols = new Dictionary<string, double>() {
            { "pi", Math.PI},    // Pi
            { "e", Math.E},      // E
        };

        public static EmbedBuilder HelpEmbed = new EmbedBuilder() {
            Title="Cheatsheet for evaluate",
            Fields= new List<EmbedFieldBuilder>() {
                new EmbedFieldBuilder() {
                    Name="__**Exponents and Roots**__",
                    Value="-> pow(a,b) = Returns a^b\n-> sqrt(a) = Returns square-root of a\n-> root(a,b) = get a's root of b",
                    IsInline=true,
                },
                new EmbedFieldBuilder() {
                    Name="__**Trigonometric**__",
                    Value="-> sin(a) = Returns a's sin value(a is considered radian)\n-> cos(a) = Returns a's cos value(a is considered radian)\n-> tan(a) = Returns a's tan value(a is considered radian)",
                    IsInline=true,
                },
                new EmbedFieldBuilder() {
                    Name="__**Logarithm**__",
                    Value="-> log(a,b) = Returns a log b\n-> ln(a) = Returns natural-log of a\n-> exp(a) = Returns e^a",
                    IsInline=true,
                }
            }
        };
        public static float Evaluate(string expression) {
            // Some things are not supported by damn datatables, welp time to do it by hand :shrug:
            string SimplifiedExpression = expression.ToLower().Trim();
            // Example : (5*pow(2,pi))/2 needs to be converted to (5*pow(2,3.1415))/2
            /*foreach(var symbol in Symbols) {
                Regex GetSymbol = new Regex(@$"\b({symbol.Key})\b");
                foreach(Match sym in GetSymbol.Matches(SimplifiedExpression)) {
                    SimplifiedExpression=SimplifiedExpression.Remove(sym.Index, sym.Length)
                        .Insert(sym.Index, (symbol.Value+"").Replace(',', '.'));
                }
            }*/

            // Then : (5*pow(2,3.1415))/2 needs to be converted to (5*8.8250)/2
            Regex GetNumbers = new Regex(@"(\d*\.\d*)|(\d*)");
            foreach(var func in Functions) {

                // This regex gets the values inside each function only
                Regex GetFunction = new Regex($@"\b({func.Key}).*\b\)");

                // Get the functions inside string
                // I know this is a chaos but if you take the time to analyze it
                // It is not as bad - things I should do : use structs, use less select :skull:
                List<(List<float> values,int position,int length)> variables = GetFunction.Matches(SimplifiedExpression) // Get functions in string
                    .Select(x => (GetNumbers.Matches(x.Value),x.Index,x.Length)) // Get values of functions
                    .Select(f => (f.Item1.Where(s=>s.Value!="") // Some fucking reason there are empty text?!?!?!?
                                    .Select(v=>Convert.ToSingle(v.Value)) // Make values into floats
                                    .ToList(), // ToList those values
                                f.Index, // Where function starts
                                f.Length // Length of the function
                    )).ToList();

                // Run those functions
                foreach(var InnerFunction in variables) {
                    // We need to make sure there is always 2 values supplied. Why? because I am a bad programmer and didn't study enough
                    if(InnerFunction.values.Count<2) {
                        InnerFunction.values.Add(0); // A=some value, B=0
                    }

                    // Do math bbg
                    float result = func.Value.Invoke(InnerFunction.values[0], InnerFunction.values[1]);
                    
                    // Just makes it easier to deal with
                    result = MathF.Round(result,4);

                    // Add result to the function
                    SimplifiedExpression = SimplifiedExpression.Remove(InnerFunction.position, InnerFunction.length)
                        .Insert(InnerFunction.position,(result+"").Replace(',','.'));
                }

            }

            // Evaluate
            return Convert.ToSingle(new DataTable().Compute(SimplifiedExpression, ""));
        }
    }
}
