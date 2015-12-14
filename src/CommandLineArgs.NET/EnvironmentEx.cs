// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommandLineArgs.NET
{
    /// <summary>Provides extended command line environment capabilities</summary>
    /// <remarks>
    /// Default argument parsing in .NET has a "feature" whereby command line
    /// strings can include special character escaping to allow double quoting.
    /// While this seems like a good idea, it isn't without some issues. In
    /// particular the world of command line usage has been around for far longer
    /// than .NET and there are established patterns of usage where that causes
    /// serious issues. In particular, a trailing slash on a quoted parameter 
    /// i.e. "c:\foo\" will interpret the trailing \" as an escape code for the
    /// string and thus the argument value in the string[] passed to main will
    /// not include the trailing slash but will include an extra " character on
    /// the end of the string instead. Sadly, there is no way to disable that
    /// "feature", so the methods in this class do the job of handling it.
    /// </remarks>
    public static class Environment
    {
        // regex to find non whitespace string with possible embedded strings containing whitespace
        [SuppressMessage( "StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "More readable this way" )]
        private static readonly Regex ArgSplitter =
            new Regex( "(?:[^\"'\\s]*)(?<qo>[\"'])?(?<qval>[^'\"]*)(?<qc-qo>[\"'])(?:[^\"'\\s]*)|[^\"'\\s]*"
                     , RegexOptions.Compiled
                       | RegexOptions.IgnoreCase
                       | RegexOptions.ExplicitCapture
                     );

        /// <summary>General command line argument splitting</summary>
        /// <param name="cmdLine">Full unparsed command line (usually from <see cref="System.Environment.CommandLine"/>)</param>
        /// <param name="skipModule">Flag to indicate if the module name should be skipped</param>
        /// <returns>Individual arguments split out from a command line</returns>
        public static string[] GetCommandLineArgs( string cmdLine, bool skipModule = true )
        {
            var matches = ArgSplitter.Matches( cmdLine );
            var filter = from Match match in matches
                         where match.Success && !string.IsNullOrWhiteSpace( match.Value )
                         select match.Value;
            
            if( skipModule )
            {
                return filter.Skip( 1 )
                             .ToArray( );
            }

            return filter.ToArray( );
        }
    }
}