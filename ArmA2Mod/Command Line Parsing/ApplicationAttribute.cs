/*
 * This work is licenced under the Creative Commons Attribution 3.0 Unported License. 
 * To view a copy of this licence, visit http://creativecommons.org/licenses/by/3.0/ 
 * or send a letter to Creative Commons, 171 Second Street, Suite 300, San Francisco, 
 * California 94105, USA
 * 
 * This license lets you distribute, remix, tweak, and build upon this work, even 
 * commercially, as long as you credit me for the original creation. You can do this
 * by leaving this comment block intact.
 * 
 *      Keith Fletcher
 *      29 Aug 2008 
 *      mailto:fletcher.keith@gmail.com
 *   
 *      Description: An attribute to describe how the application processes 
 *                   the command line options.
 *      Version:     1.0
 *      
 */

namespace PXitCore.CommandLineParsing
{
    using System;

    /// <summary>
    /// An attribute to describe how the application processes the command line options.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ApplicationAttribute : Attribute
    {
        #region ApplicationAttribute Declarations
        static readonly string[] DEFAULT_OPTION_PREFIXES = new string[] { "/", "-" };
        static readonly string[] DEFAULT_HELP_OPTIONS = new string[] { "?", "help" };
        static readonly char[] DEFAULT_OPTION_SPLITTERS = new char[] { ' ',':'};


        string[] _optionPrefixes = DEFAULT_OPTION_PREFIXES;
        string[] _helpOptions = DEFAULT_HELP_OPTIONS;
        char[] _optionSplitters = DEFAULT_OPTION_SPLITTERS;
        bool _throwParsingExceptions = true;
        #endregion ApplicationAttribute Declarations

        #region ApplicationAttribute Constructors
        /// <summary>
        /// Create a new instance of ApplicationAttribute.
        /// </summary>
        public ApplicationAttribute() { }
        #endregion ApplicationAttribute Constructors

        #region ApplicationAttribute Properties
        /// <summary>
        /// Gets or sets the characters which are used to split between parameter identifiers and their values.
        /// The defaults are ' ' and ':'.
        /// </summary>
        public char[] OptionValueSplitters { get { return _optionSplitters; } set { _optionSplitters = value; } }
        /// <summary>
        /// Get or set the list of option prefixes. The defaults are "/" and "-".
        /// </summary>
        public string[] OptionPrefixes { get { return _optionPrefixes; } set { _optionPrefixes = value; } }
        /// <summary>
        /// Get or set the list of switches used to invoke the help screen. The defaults "?" and "help".
        /// </summary>
        public string[] HelpOptions { get { return _helpOptions; } set { _helpOptions = value; } }
        /// <summary>
        /// Get or set how parsing exceptions are handled. If set to False, no exceptions will the thrown. Error 
        /// messages will be written to the console only. If set to True, messages will be written to the console, 
        /// and a CommandLineParsingException exception will be thrown. The default is True.
        /// </summary>        
        public bool ThrowParsingExceptions { get { return _throwParsingExceptions; } set { _throwParsingExceptions = value; } }
        #endregion ApplicationAttribute Properties
    }    
}
