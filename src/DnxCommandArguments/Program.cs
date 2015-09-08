using Microsoft.Dnx.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Dnx.Runtime.Common.CommandLine;

namespace DnxCommandArguments
{
    public class Program
    {
        private readonly IRuntimeEnvironment _runtimeEnv;

        public Program(IRuntimeEnvironment runtimeEnv)
        {
            _runtimeEnv = runtimeEnv;
        }

        public int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "sample-fu",
                Description = "Runs different methods as dnx commands",
                FullName = "Sample-Fu - Your Do-nothing dnx Commandifier"
            };

            // the "echo" command, named display instead because different
            app.Command("display", c =>
            {
                c.Description = "Displays a message of your choosing to console.";

                var reverseOption = c.Option("-r|--reverse", "Display the message in reverse", CommandOptionType.NoValue);
                var messageArg = c.Argument("[message]", "The message you wish to display");
                c.HelpOption("-?|-h|--help");

                c.OnExecute(() =>
                {
                    var message = messageArg.Value;
                    if (reverseOption.HasValue())
                    {
                        message = new string(message.ToCharArray().Reverse().ToArray());
                    }
                    Console.WriteLine(message);
                    return 0;
                });
            });

            //  the "calc" command
            app.Command("calc", c =>
            {
                c.Description = "Evaluates arguments with the operation specified.";

                var operationOption = c.Option("-o|--operation <OPERATION>", "You can add or multiply the terms specified using 'add' or 'mul'.", CommandOptionType.SingleValue);
                var termsArg = c.Argument("[terms]", "The numbers to use as a term", true);
                c.HelpOption("-?|-h|--help");

                c.OnExecute(() =>
                {
                    // check to see if we got what we were expecting
                    if (!operationOption.HasValue())
                    {
                        Console.WriteLine("No operation specified.");
                        return 1;
                    }
                    if (termsArg.Values.Count != 2)
                    {
                        Console.WriteLine("You must specify exactly 2 terms.");
                        return 1;
                    }

                    // perform the operation
                    var operation = operationOption.Value();
                    var term1 = int.Parse(termsArg.Values[0]);
                    var term2 = int.Parse(termsArg.Values[1]);
                    if (operation.ToLower() == "mul")
                    {
                        var result = term1 * term2;
                        Console.WriteLine($" {term1} x {term2} = {result}");
                    }
                    else
                    {
                        var result = term1 + term2;
                        Console.WriteLine($" {term1} + {term2} = {result}");
                    }
                    return 0;
                });
            });

            // show the help for the application
            app.OnExecute(() =>
            {
                app.ShowHelp();
                return 2;
            });

            return app.Execute(args);
        }
    }
}
