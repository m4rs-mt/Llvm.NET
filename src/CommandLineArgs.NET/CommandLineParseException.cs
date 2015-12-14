// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CommandLineArgs.NET
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CommandLineParseException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CommandLineParseException( )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public CommandLineParseException( string message )
            : base( message )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public CommandLineParseException( string message, Exception inner )
            : base( message, inner )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formatProvider"></param>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        public CommandLineParseException( IFormatProvider formatProvider, string fmt, params object[] args )
            : base( string.Format( formatProvider, fmt, args ) )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inner"></param>
        /// <param name="formatProvider"></param>
        /// <param name="fmt"></param>
        /// <param name="args"></param>
        public CommandLineParseException( Exception inner
                                        , IFormatProvider formatProvider
                                        , string fmt
                                        , params object[] args
                                        )
            : base( string.Format( formatProvider, fmt, args ), inner )
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected CommandLineParseException( SerializationInfo info
                                           , StreamingContext context
                                           )
            : base( info, context )
        {
        }
    }
}