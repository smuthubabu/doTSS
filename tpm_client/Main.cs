using System;
using Iaik.Connection.ClientConnections;
using Iaik.Tc.Tpm.Context;
using System.Threading;
using Iaik.Tc.Tpm.library.common;
using System.IO;
using Iaik.Utils;
using System.Reflection;

namespace Iaik.Tc.Tpm
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			log4net.Appender.DebugAppender appender = new log4net.Appender.DebugAppender ();
			appender.Name = "DebugAppender";
			appender.Layout = new log4net.Layout.PatternLayout ("[%date{dd.MM.yyyy HH:mm:ss,fff}]-%-5level-[%type]: %message%newline");
			log4net.Config.BasicConfigurator.Configure (appender);

			CommandLineHandler cmdlHandler = new CommandLineHandler ();
			cmdlHandler.RegisterCallback ("help", CmdCallback_Help);
			cmdlHandler.RegisterCallback ("help_scripts", CmdCallback_HelpScripts);
			
			string scriptFile = null;
			cmdlHandler.RegisterCallback ("script", delegate(CommandLineHandler.CommandOption cmdOption)
			{
				if (cmdOption.Arguments != null && cmdOption.Arguments.Length > 0)
					scriptFile = cmdOption.Arguments[0];
			});
			
			cmdlHandler.Parse (args);
			
			if (scriptFile == null)
				new TPMConsole ().Run ();
			else
            	new TPMConsole().RunScriptFile(scriptFile);
		}
				
		
		private static void CmdCallback_Help (CommandLineHandler.CommandOption cmdOption)
		{
			DateTime buildDate = new DateTime (2000, 1, 1);
			buildDate = buildDate.AddDays (Assembly.GetEntryAssembly ().GetName ().Version.Build);
			buildDate = buildDate.AddSeconds (Assembly.GetEntryAssembly ().GetName ().Version.Revision * 2);

			
			Console.WriteLine ("TPM_csharp framework tester v.{0} Built on: {1}\n", Assembly.GetCallingAssembly().GetName().Version, buildDate.ToShortDateString());
			Console.WriteLine ("Start with no arguments and type 'help' for a list of supported commands\n\n");
			
			Console.WriteLine ("--script=/path/to/script\tExecutes the commands defined in the scripts as they where entered on the commandline");
			Console.WriteLine ("Start with --help_scripts for more information on scripting");
			Environment.Exit(0);
		}
		
		
		private static void CmdCallback_HelpScripts (CommandLineHandler.CommandOption cmdOption)
		{
			DateTime buildDate = new DateTime (2000, 1, 1);
			buildDate = buildDate.AddDays (Assembly.GetEntryAssembly ().GetName ().Version.Build);
			buildDate = buildDate.AddSeconds (Assembly.GetEntryAssembly ().GetName ().Version.Revision * 2);
			
			
			Console.WriteLine ("TPM_csharp framework tester v.{0} Built on: {1}\n", Assembly.GetCallingAssembly ().GetName ().Version, buildDate.ToShortDateString ());
			
			Console.WriteLine(@"
Commands in script files are interpreted line by line (as they are entered on the command line)
Lines starting with '#' are interpreted as comments.

Commands starting with '@' are special commands and are only available in script execution environment.
The following is a list of available special commands and their effects:

@exit_on_error [0|1] (Default: 0) 
    Instructs the command interpreter to exit command execution if an error occurs.
    Keep in mind, if the execution of a special command fails the script execution is always stopped.

@exit_on_finish [0|1] (Default: 1)
	Instructs the command interpreter to exit if all commands in the script file have been processed
    Set to 0 if you want the script to just initialize an environment

@include [filename]
	Includes the specified file (another script file). Relative paths should start with './' or '.\'.
    If the specified path is a relative path the base path is the path where the including script
    file is located.
");

			Environment.Exit (0);
		}
	}
}

