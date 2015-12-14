using Llvm.NET;
using CommandLineArgs.NET;

namespace opt_net
{
    class Program
    {
        /// <summary>This is a test app that uses Llvm.NET to replace the standard llvm opt.exe application</summary>
        /// <param name="args">Command line arguments</param>
        /// <remarks>
        /// This is a test application that is intended to operate as a substitue for the official opt.exe.
        /// While this adds no new functionality to the bit code optimizer it shows/verifies that all of the
        /// optimization support made available through opt.exe is also availabel through Llvm.NET.
        /// </remarks>
        static void Main( string[ ] args )
        {
            using( var context = new Context( ) )
            {
                StaticState.RegisterAll( );
                GlobalPassRegistry.InitializeAll( );
                
                // get full args - handling quoted strings with trailing path seperators
                var unparsedArgs = Environment.GetCommandLineArgs( System.Environment.CommandLine, false );
                StaticState.ParseCommandLineOptions( unparsedArgs, "opt-net test app for optimizations in Llvm.NET" );

            }
        }
    }
}
