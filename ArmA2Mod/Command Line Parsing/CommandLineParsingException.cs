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
 *      Description: An exception thrown when parsing an application's command line options. 
 *      Version:     1.0
 *      
 */

namespace PXitCore.CommandLineParsing
{
    using System;

    /// <summary>
    /// An exception thrown when parsing an application's command line options. 
    /// </summary>
    [Serializable]
    public class CommandLineParsingException : ApplicationException
    {
        #region CommandLineParsingException Contructors
        /// <summary>
        /// Create a new CommandLineParsingException instance.
        /// </summary>
        public CommandLineParsingException() : base() { }

        /// <summary>
        /// Create a new instance of CommandLineParsingException and provide a message.
        /// </summary>
        /// <param name="message">A description of the cause of the exception.</param>
        public CommandLineParsingException(string message)
            : base(message) { }

        /// <summary>
        /// Create a new instance of CommandLineParsingException and provide a message.
        /// </summary>
        /// <param name="format">The format string for the message.</param>
        /// <param name="args">The arguments of the message format string.</param>
        public CommandLineParsingException(string format, params object[] args)
            : base(string.Format(format, args)) { }

        /// <summary>
        /// Create a new instance of CommandLineParsingException, based on some other Exception.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public CommandLineParsingException(Exception innerException)
            : base((innerException == null) ? string.Empty : innerException.Message, innerException) { }

        /// <summary>
        /// Create a new instance of CommandLineParsingException, based on some other Exception.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="message">A description of the cause of the exception.</param>
        public CommandLineParsingException(Exception innerException, string message)
            : base(message, innerException) { }

        /// <summary>
        /// Create a new instance of CommandLineParsingException, based on some other Exception.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="format">The format string for the message.</param>
        /// <param name="args">The arguments of the message format string.</param>
        public CommandLineParsingException(Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException) { }

        #endregion CommandLineParsingException Contructors
    }    
}