// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CommandLineArgs.NET
{
    using System;

    /// <summary>Allows accessing a property via the command line with a name that is different from the property name</summary>
    [AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
    public sealed class CommandLineArgAttribute
        : Attribute
    {
        /// <summary>Construct a new <see cref="CommandLineArgAttribute"/></summary>
        /// <param name="shortName">Short name for the argument</param>
        public CommandLineArgAttribute( string shortName )
        {
            ShortName = shortName;
        }

        /// <summary>Gets short form name for the command line argument</summary>
        public string ShortName { get; }

        /// <summary>Gets or sets long form name for the command line argument</summary>
        public string LongName { get; set; }
    }
}