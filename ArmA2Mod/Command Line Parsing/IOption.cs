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
 *      Description: An interface for the object that describes each option that an 
 *                   application can expect on it's command line.
 *      Version:     1.0
 *      
 */

namespace PXitCore.CommandLineParsing
{
    /// <summary>
    /// An interface for the object that describes each option that an application can 
    /// expect on it's command line.
    /// </summary>    
    public interface IOption
    {
        #region IOption Properties
        /// <summary>
        /// Get the IOption Name.
        /// </summary>
        /// <remarks>
        /// This will be the text you enter on the command line. For example, /XD. If you've
        /// set IsAnonymous to true, then you must not enter the name on the command line. Anonymous options
        /// are typically used to enter filenames or paths on a command line.
        /// </remarks>
        string Name { get; }
        /// <summary>
        /// Get or set the IOption value.
        /// </summary>
        /// <remarks>        
        /// Not all options have values. You can configure this with the ValuePresence property. The value is entered 
        /// on the command line after the name, for example, /XD:true. If IsAnonymous is set to true, then the value 
        /// is entered directly without the name. This is useful for entering filenames or paths on the command line.
        /// If this option has multiple values, this property will return the first one.
        /// </remarks>
        string Value { get; set; }
        /// <summary>
        /// Get or set the IOption values.
        /// </summary>
        /// <remarks>        
        /// Not all options have values. You can configure this with the ValuePresence property. The value is entered 
        /// on the command line after the name, for example, /XD:true. If IsAnonymous is set to true, then the value 
        /// is entered directly without the name. This is useful for entering filenames or paths on the command line.
        /// Multiple values are entered by adding the full option more than once on the command line.
        /// </remarks>
        string[] Values { get; }
        /// <summary>
        /// Get the short description
        /// </summary>
        /// <remarks>This is printed next to the option name on the help screen.</remarks>
        string ShortDescription { get; }
        /// <summary>
        /// Get the long description
        /// </summary>
        /// <remarks>This is printed next to the option name on the help screen.</remarks>
        string LongDescription { get; }
        /// <summary>
        /// Get the regular expression pattern to validate the value.
        /// </summary>
        /// <remarks>The pattern, if given, will be matched against the value as it's parsed from the command line.
        /// A value that cannot be matched will result in a CommandLineParsingException exception being thrown. If you
        /// add a pattern, be sure to provide a matching value for the FormatDisplay property, so that meaningful
        /// error messages can be written to the console.</remarks>
        string FormatPattern { get; }
        /// <summary>
        /// Get the require value display pattern.
        /// </summary>
        /// <remarks>This performs a case-insenstive pattern match against the value. If a match is not found,
        /// a CommandLineParsingException exception is thrown. This is used very closely with the FormatPattern 
        /// property, and these two properties should always match. The FormatDisplay is shown on the help screen, and in the error message when 
        /// validation fails against the FormatPattern pattern.</remarks>
        string FormatDisplay { get; }
        /// <summary>
        /// Get the group to which this IOption belongs.
        /// </summary>
        string Group { get; }
        /// <summary>
        /// Get whether this IOption is required to appear on the command line.
        /// </summary>
        bool IsOptional { get; }
        /// <summary>
        /// Get whether this IOption is entered on the command line with or without it's name 
        /// preceding the value.
        /// </summary>
        /// <remarks>If an option is anonymous, it has to have a value (set with the ValuePresence property).</remarks>
        bool IsAnonymous { get; }
        /// <summary>
        /// Get whether a value is required, optional, or must not appear.
        /// </summary>
        OptionAttributeValuePresence ValuePresence { get; }
        /// <summary>
        /// Get whether this option has a value.
        /// </summary>
        bool HasValue { get; }
        /// <summary>
        /// Get the order in which the options must appear on the command line.
        /// </summary>
        int Order { get; }
        /// <summary>
        /// Get whether this option appeared on the command line
        /// </summary>
        /// <remarks>The ordering of option only really applies to anonymous options.</remarks>
        bool IsPresent { get; }
        /// <summary>
        /// Get or set whether multiple instances of this option can appear on the command line.
        /// If AllowMultiple is True, each instance of the option will add it's value to the
        /// Values collection.
        /// </summary>
        bool AllowMultiple { get; }

        #endregion IOption Properties
    }    
}
