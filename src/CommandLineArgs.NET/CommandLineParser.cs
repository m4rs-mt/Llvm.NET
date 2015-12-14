// -----------------------------------------------------------------------
// <copyright company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLineArgs.NET.Properties;

namespace CommandLineArgs.NET
{
    /// <summary>Class for parsing command line arguments</summary>
    public class CommandLineArgsParser
    {
        /// <summary>Create a new class for the arguments and parse the command line into it</summary>
        /// <typeparam name="T">Type of the parsed arguments data structure</typeparam>
        /// <param name="args">Command line args array</param>
        /// <param name="parsedArgs">Parsed arguments instance to fill in</param>
        /// <remarks>
        /// <para>Switches are matched against properties of type <typeparamref name="T"/>
        /// If no property name matches the command line switch, then a property
        /// with a <see cref="CommandLineArgAttribute"/> ShortName or LongName
        /// matching the switch is used.</para>
        /// <para>Property values are set by using a <see cref="TypeConverter"/>
        /// associated with the property. Using converters allows for a class to 
        /// support custom parsing of values (e.g. GUID values can be parsed directly to 
        /// a GUID type). Custom types with a defined TypeConverter will support parsing
        /// to custom types.</para>
        /// <para>If a command line argument does not contain a switch it is considered
        /// as a potentially positional argument. To accommodate space separated switch
        /// and parameter syntaxes, any switch found without a value whose matching property
        /// of <typeparamref name="T"/> is not a Boolean is potentially a space separated
        /// switch. Thus, when a potentially positional argument is encountered a look back
        /// to the previous argument is performed to see if it was a potentially space separated
        /// switch. If a valid switch was found then the argument is applied to that
        /// switch.</para>
        /// <para>If a potentially positional argument is found but a potentially space 
        /// separated switch wasn't found then the argument is considered positional.
        /// Positional arguments are stored into a collection of <see cref="IList{T}"/>.
        /// The positional list must be designated with the <see cref="DefaultPropertyAttribute"/>
        /// to support positional arguments. If no default property or an unsupported type
        /// exists for the property and positional arguments are provided an exception is thrown</para>
        /// </remarks>
        public void Parse< T >( IList< string > args, T parsedArgs )
            where T : class
        {
            if( args == null )
                throw new ArgumentNullException( nameof( args ) );

            if( parsedArgs == null )
                throw new ArgumentNullException( nameof( parsedArgs ) );

            Props = TypeDescriptor.GetProperties( typeof( T ) );
            PropertyDescriptor pendingFlag = null;

            // note: this is allowed to be null, it is checked in SetPositional()
            var positionalArgs = GetPositionalArgList( parsedArgs );

            foreach( var arg in args )
            {
                var match = ArgDetailsRegex.Match( arg );
                if( !match.Success )
                {
                    SetPositional( parsedArgs, positionalArgs, pendingFlag, arg );
                    pendingFlag = null;
                }
                else
                {
                    // if a potentially space delimited arg was pending a value
                    // this is an error, since a new switch isn't a value
                    if( pendingFlag != null )
                    {
                        throw new CommandLineParseException( CultureInfo.CurrentUICulture
                                                           , Resources.MissingValueForOptionFmt
                                                           , pendingFlag.Name
                                                           );
                    }

                    pendingFlag = SetProperty( parsedArgs, null, match );
                }
            }
        }

        // sets a value of a property on an object
        private static void SetValue( object obj, PropertyDescriptor prop, string value )
        {
            var propList = prop.GetValue( obj ) as IList;
            if( propList != null )
                propList.Add( value );
            else
            {
                object convertedValue = value;
                if( prop.Converter != null )
                    convertedValue = prop.Converter.ConvertFromString( value ?? string.Empty );

                // ReSharper disable once AssignNullToNotNullAttribute 
                prop.SetValue( obj, convertedValue );
            }
        }

        // finds the default property with a string list type
        private static IList<string> GetPositionalArgList<T>( T obj )
        {
            var p = TypeDescriptor.GetDefaultProperty( typeof( T ) );
            if( p == null )
                return null;

            return p.GetValue( obj ) as IList<string>;
        }

        // sets a potentially positional arg as either a value or positional argument
        private static void SetPositional( object value
                                         , ICollection< string > positionalArgs
                                         , PropertyDescriptor pendingFlag
                                         , string arg
                                         )
        {
            // if a potentially space delimited switch is pending
            // treat the arg as the value for the switch
            if( pendingFlag != null )
                SetValue( value, pendingFlag, arg );
            else
            {
                if( positionalArgs == null )
                    throw new CommandLineParseException( CultureInfo.CurrentUICulture, Resources.UnknownOptionFmt, arg );

                positionalArgs.Add( arg );
            }
        }

        // set a property value on retVal based on the regex match
        private PropertyDescriptor SetProperty( object retVal, PropertyDescriptor pendingFlag, Match match )
        {
            var name = match.Groups[ "switch" ].Value;
            var val = match.Groups[ "value" ].Value;

            var prop = GetPropertyForArg( name );
            if( prop == null )
                throw new CommandLineParseException( CultureInfo.CurrentUICulture, Resources.UnknownOptionFmt, name );

            // handle boolean flags, might not have a value to convert
            if( prop.PropertyType == typeof( bool ) )
            {
                // assume true unless a value is specified
                var boolVal = true;

                // If there's a value attached, see if it can be converted to a boolean
                if( !string.IsNullOrWhiteSpace( val ) )
                    boolVal = Convert.ToBoolean( val, CultureInfo.CurrentCulture );

                prop.SetValue( retVal, boolVal );
            }
            else
            {
                // if value is missing mark it as a pending potentially space delimited arg
                var hasValue = !string.IsNullOrWhiteSpace( val );
                pendingFlag = hasValue ? null : prop;
                if( hasValue )
                    SetValue( retVal, prop, val );
            }

            return pendingFlag;
        }

        // finds an appropriate descriptor for the given property name
        private PropertyDescriptor GetPropertyForArg( string name )
        {
            foreach( PropertyDescriptor prop in Props )
            {
                // try with a direct matching name first
                if( 0 == string.Compare( prop.Name, name, true, CultureInfo.InvariantCulture ) )
                    return prop;

                // Check the short and long names of the CommandLineArgAttribute, if there is one
                foreach( var cla in prop.Attributes.OfType< CommandLineArgAttribute >( ) )
                {
                    if( 0 == string.Compare( cla.ShortName, name, true, CultureInfo.InvariantCulture ) )
                        return prop;

                    if( 0 == string.Compare( cla.LongName, name, true, CultureInfo.InvariantCulture ) )
                        return prop;
                }
            }

            // property for the arg wasn't found
            return null;
        }

        // <switch><name><valuesep><value> - ignoring delimiters in quotes
        [SuppressMessage( "StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "More readable this way" )]
        private static readonly Regex ArgDetailsRegex =
            new Regex( "(?:^-{1,2}|^/)(?<switch>[^\\:=]*)(?:(?:\\:|=)(?<qo>[\"'])?(?<value>[^'\"]*)(?<qc-qo>[\"'])?)?$"
                     , RegexOptions.Compiled
                       | RegexOptions.IgnoreCase
                       | RegexOptions.ExplicitCapture
                     );

        private PropertyDescriptorCollection Props;
    }
}