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
 *      Description: An attribute to describe each option that an application can expect 
 *                   on it's command line.
 *      Version:     1.0
 *      
 */

namespace PXitCore.CommandLineParsing
{
    using System;
    using System.Collections.Generic;    
    
    /// <summary>
    /// An attribute to describe each option that an application can expect on it's command line
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class OptionAttribute : Attribute, IOption
    {
        #region OptionAttribute Declarations
        List<string> _values = new List<string>();
        string _name = string.Empty;
        string _shortDescription = string.Empty;
        string _longDescription = string.Empty;
        string _formatPattern = string.Empty;
        string _formatDisplay = string.Empty;
        string _group = string.Empty;
        OptionAttributeValuePresence _valuePresence = OptionAttributeValuePresence.MayHaveValue;
        bool _isOptional = true;
        bool _isAnonymous = false;
        bool _processed = false;
        int _order = 99;
        bool _allowMultiple = false;
        #endregion OptionAttribute Declarations

        #region OptionAttribute Constructors
        /// <summary>
        /// Create a new OptionAttribute instance. Each OptionAttribute instance represents a single command line option.
        /// </summary>
        /// <param name="name">The name of the option. This is the string that you'll type onto the command line.</param>
        public OptionAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Create a new OptionAttribute instance. Each OptionAttribute instance represents a single command line option.
        /// </summary>
        /// <param name="name">The name of the option. This is the string that you'll type onto the command line.</param>
        /// <param name="value">The default value of the option.</param>
        public OptionAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
        #endregion OptionAttribute Constructors

        #region OptionAttribute Methods
        internal protected void AddValue(string value)
        {
            _values.Add(value);
        }
        #endregion OptionAttribute Methods

        #region OptionAttribute Properties
        /// <summary>
        /// Get or set the OptionAttribute Name.
        /// </summary>
        /// <remarks>
        /// This will be the text you enter on the command line. For example, /XD. If you've
        /// set IsAnonymous to true, then you must not enter the name on the command line. Anonymous options
        /// are typically used to enter filenames or paths on a command line.
        /// </remarks>
        public string Name { get { return _name; } set { _name = value.Trim(); } }
        /// <summary>
        /// Get or set the OptionAttribute value.
        /// </summary>
        /// <remarks>        
        /// Not all options have values. You can configure this with the ValuePresence property. The value is entered 
        /// on the command line after the name, for example, /XD:true. If IsAnonymous is set to true, then the value 
        /// is entered directly without the name. This is useful for entering filenames or paths on the command line.
        /// </remarks>
        public string Value 
        {
            get { return (_values.Count == 0) ? string.Empty : _values[0]; } 
            set 
            {
                if (_values.Count == 0)
                    _values.Add(value.Trim());
                else
                    _values[0] = value.Trim();                
            } 
        }
        /// <summary>
        /// Get or set the OptionAttribute values.
        /// </summary>
        /// <remarks>        
        /// Not all options have values. You can configure this with the ValuePresence property. The value is entered 
        /// on the command line after the name, for example, /XD:true. If IsAnonymous is set to true, then the value 
        /// is entered directly without the name. This is useful for entering filenames or paths on the command line.
        /// </remarks>
        public string[] Values 
        { 
            get { return _values.ToArray(); }
            // TODO: Should probably do a Trim() on each element when added
            set { _values = (value == null) ? new List<string>() : new List<string>(value); } 
        }
        /// <summary>
        /// Get or set the short description
        /// </summary>
        /// <remarks>This is printed next to the option name on the help screen.</remarks>
        public string ShortDescription { get { return _shortDescription; } set { _shortDescription = value.Trim(); } }
        /// <summary>
        /// Get or set the long description
        /// </summary>
        /// <remarks>This is printed next to the option name on the help screen.</remarks>
        public string LongDescription { get { return _longDescription; } set { _longDescription = value.Trim(); } }
        /// <summary>
        /// Get or set the regular expression pattern to validate the value.
        /// </summary>
        /// <remarks>The pattern, if given, will be matched against the value as it's parsed from the command line.
        /// A value that cannot be matched will result in a CommandLineParsingException exception being thrown. If you
        /// add a pattern, be sure to provide a matching value for the FormatDisplay property, so that meaningful
        /// error messages can be written to the console.</remarks>
        public string FormatPattern { get { return _formatPattern; } set { _formatPattern = value.Trim(); } }
        /// <summary>
        /// Get or set the require value display pattern.
        /// </summary>
        /// <remarks>This is used very clsoely with the FormatPattern property, and these two properties should 
        /// always match. The FormatDisplay is shown on the help screen, and in the error message when 
        /// validation fails against the FormatPattern pattern.</remarks>
        public string FormatDisplay { get { return _formatDisplay; } set { _formatDisplay = value.Trim(); } }
        /// <summary>
        /// Get or set the name of the group to which this OptionAttribute belongs.
        /// </summary>
        public string Group { get { return _group; } set { _group = value.Trim(); } }
        /// <summary>
        /// Get or set whether this OptionAttribute is required to appear on the command line.
        /// </summary>
        public bool IsOptional { get { return _isOptional; } set { _isOptional = value; } }
        /// <summary>
        /// Get or set whether this OptionAttribute is entered on the command line with or without it's name 
        /// preceding the value.
        /// </summary>
        /// <remarks>If an option is anonymous, it has to have a value (set with the ValuePresence property).</remarks>
        public bool IsAnonymous { get { return _isAnonymous; } set { _isAnonymous = value; } }
        /// <summary>
        /// Get or set whether a value is required, optional, or must not appear.
        /// </summary>
        public OptionAttributeValuePresence ValuePresence { get { return _valuePresence; } set { _valuePresence = value; } }
        /// <summary>
        /// Get whether this option has a value.
        /// </summary>
        public bool HasValue { get { return (Value != string.Empty); } }
        /// <summary>
        /// Get the order in which this OptionAttribute should be expected on the command line.
        /// </summary>
        public int Order { get { return _order; } set { _order = value; } }
        /// <summary>
        /// Get whether this option appeared on the command line
        /// </summary>
        public bool IsPresent { get { return _processed; } }
        /// <summary>
        /// Get or set whether multiple instances of this option can appear on the command line
        /// </summary>
        public bool AllowMultiple { get { return _allowMultiple; } set { _allowMultiple = value; } }

        internal bool MustHaveValue { get { return (_valuePresence == OptionAttributeValuePresence.MustHaveValue); } }
        internal bool MustNotHaveValue { get { return (_valuePresence == OptionAttributeValuePresence.MustNotHaveValue); } }
        internal bool MayHaveValue { get { return (_valuePresence == OptionAttributeValuePresence.MayHaveValue); } }
        internal bool Processed { get { return _processed; } set { _processed = value; } }
        #endregion OptionAttribute Properties
    }    
    
    /// <summary>
    /// Describe whether or not to expect a value to be given for an OptionAttribute.
    /// </summary>
    public enum OptionAttributeValuePresence
    {
        /// <summary>
        /// No value must be given.
        /// </summary>
        MustNotHaveValue,
        /// <summary>
        /// A value or or may not be given. The value is optional.
        /// </summary>
        MayHaveValue,
        /// <summary>
        /// A value must not be given.
        /// </summary>
        MustHaveValue
    }    
}
