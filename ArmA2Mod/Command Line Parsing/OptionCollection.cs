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
 *      Description: A collection class for IOption instances.
 *      Version:     1.0
 * 
 */

namespace PXitCore.CommandLineParsing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    
    /// <summary>
    /// A collection of IOption instances.
    /// </summary>
    public sealed class OptionCollection : IList<IOption>
    {
        #region OptionCollection Declarations
        MemberInfo _memberInfo;
        ApplicationAttribute _appAttribute = null;
        OptionAttribute[] _attributes = null;
        OptionAttribute[] _anonAttributes = null;
        Dictionary<string, int> _attribDict = null;
        string _preferredPrefix;
        bool _throwParsingExceptions = true;
        #endregion OptionCollection Declarations

        #region OptionCollection Constructors
        /// <summary>
        /// Create a new OptionCollection instance.
        /// </summary>
        /// <param name="memberInfo">The MemberInfo of the class to read the attributes from.</param>
        internal OptionCollection(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
            Build();
        }
        #endregion OptionCollection Constructors
        
        #region OptionCollection Methods
        /// <summary>
        /// Return an IOption from the collection, based on it's Name property.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <returns>An OptionAttribute instance matching the name.</returns>
        public IOption GetOption(string optionName)
        {
            IOption option = null;
            if (!TryGetOption(optionName, out option))
                throw new CommandLineParserSystemException(
                    "Option {0}{1} does not exist.",                    
                    _preferredPrefix, optionName);
            return option;
        }

        /// <summary>
        /// Try return an IOption from the collection, based on it's Name property.
        /// </summary>
        /// <param name="optionName">The name of the option.</param>
        /// <param name="option">The IOption, if one was found.</param>
        /// <returns>true if the option is found; false if not.</returns>
        public bool TryGetOption(string optionName, out IOption option)
        {
            int index = -1;
            option = null;
            string key = optionName.ToLower();
            if (!_attribDict.TryGetValue(key, out index))            
                return false;

            option = _attributes[index];
            return true;
        }

        private void Build()
        {            
            // Collect the application level attribute
            ApplicationAttribute[] appAttributes =
                Helper.GetCustomAttributes<ApplicationAttribute>(_memberInfo);

            if (appAttributes != null)
                if (appAttributes.Length > 0)
                    _appAttribute = appAttributes[0];

            // If we don't find one, create a default one
            if (_appAttribute == null)
                _appAttribute = new ApplicationAttribute();

            // Get our preferred option prefix
            if ((_appAttribute.OptionPrefixes == null) ||                
                (_appAttribute.OptionPrefixes.Length == 0))
                throw new CommandLineParserSystemException(
                    "You must provide at least one option prefix.");

            _preferredPrefix = _appAttribute.OptionPrefixes[0];
            _throwParsingExceptions = _appAttribute.ThrowParsingExceptions;


            // Get and sort the option attributes
            List<OptionAttribute> anonAttribs = new List<OptionAttribute>();
            _attributes = Helper.GetCustomAttributes<OptionAttribute>(_memberInfo);
            int attribCount = _attributes.Length;
            if (attribCount > 1)
                Array.Sort<OptionAttribute>(_attributes, new OptionAttributeComparer());
                
            _attribDict = new Dictionary<string, int>(attribCount);

            // Build our OptionCollection, and do some validation
            for (int index = 0; index < attribCount; index++)
            {
                string key = "";
                foreach(string prefix in _appAttribute.OptionPrefixes)
                    if(_attributes[index].Name.ToLower().StartsWith(prefix))
                        key = _attributes[index].Name.ToLower().Remove(0,prefix.Length);

                if(key == "")
                    key = _attributes[index].Name.ToLower();
                OptionAttribute attribute = _attributes[index];

                if (_attribDict.ContainsKey(key))
                    throw new CommandLineParserSystemException(
                        "The option name {0}{1} appears twice. Names must be unique.",                        
                        _preferredPrefix, attribute.Name);

                _attribDict[key] = index;

                if (attribute.IsAnonymous) 
                {
                    if (!attribute.MustHaveValue)
                        throw new CommandLineParserSystemException(
                            "Error processing option {0}{1}. If an OptionAttribute is anonymous, then ValuePresence must be set to MustHaveValue.",                            
                            _preferredPrefix, attribute.Name);
                    else
                        anonAttribs.Add(attribute);
                }
            }

            // Sort the anonymous attributes
            if (anonAttribs.Count > 0)
            {
                _anonAttributes = anonAttribs.ToArray();
                Array.Sort<OptionAttribute>(_anonAttributes, new OptionAttributeComparer());
            }
            else
                _anonAttributes = new OptionAttribute[0];
        }


        private int GetIndexFromName(string optionName)
        {
            int index = -1;
            _attribDict.TryGetValue(optionName.ToLower(), out index);
            return index;
        }
        
        /// <summary>
        /// Determines the index of a specific item in the OptionCollection, based on the 
        /// name of the item passed in.
        /// </summary>
        /// <param name="item">The IOption to look for. This method will search by name.</param>
        /// <returns>The index of the IOption in the collection, or -1 if not found.</returns>
        public int IndexOf(IOption item)
        {
            return ((item == null) ? -1 : GetIndexFromName(item.Name));
        }

        /// <summary>
        /// This method is unsupported, as the collection is read-only.
        /// </summary>
        /// <param name="index">This method is unsupported, as the collection is read-only.</param>
        /// <param name="item">This method is unsupported, as the collection is read-only.</param>        
        public void Insert(int index, IOption item) { throw new NotSupportedException("This is a read-only collection."); }        
        /// <summary>
        /// This method is unsupported, as the collection is read-only.
        /// </summary>
        /// <param name="index">This method is unsupported, as the collection is read-only.</param>
        public void RemoveAt(int index) { throw new NotSupportedException("This is a read-only collection."); }
        /// <summary>
        /// This method is unsupported, as the collection is read-only.
        /// </summary>
        /// <param name="item">This method is unsupported, as the collection is read-only.</param>
        public void Add(IOption item) { throw new NotSupportedException("This is a read-only collection."); }
        /// <summary>
        /// This method is unsupported, as the collection is read-only.
        /// </summary>
        public void Clear() { throw new NotSupportedException("This is a read-only collection."); }
        /// <summary>
        /// Determines whether the OptionCollection contains a specific IOption that has the same name
        /// as the one provided here as a parameter.
        /// </summary>
        /// <param name="item">The IOption instance to look for. Checking is done by IOption.Name, 
        /// not by instance.</param>
        /// <returns>true if item is found in the OptionCollection; otherwise, false.</returns>
        public bool Contains(IOption item) { return (IndexOf(item) != -1); }
        /// <summary>
        /// Copies the elements of the OptionCollection to an Array, starting at a 
        /// particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of 
        /// the elements copied from OptionCollection. The Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(IOption[] array, int arrayIndex)
        {
            if (_attributes.Length > 0)
                Array.Copy(_attributes, 0, array, arrayIndex, _attributes.Length);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the OptionCollection. 
        /// </summary>
        /// <param name="item">Removes the first occurrence of a specific object from the OptionCollection.</param>
        /// <returns>Removes the first occurrence of a specific object from the OptionCollection.</returns>
        public bool Remove(IOption item) { throw new NotSupportedException("This is a read-only collection."); }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator<IOption> GetEnumerator()
        {
            return new OptionEnumerator(this);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection of anonymous options.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection of anonymous options.</returns>
        public IEnumerator<IOption> GetAnonymousEnumerator()
        {
            return new OptionEnumerator(this, true);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new OptionEnumerator(this);
        }

        internal void ClearProcessedFlags()
        {
            for (int index = 0; index < _attributes.Length; index++)            
                _attributes[index].Processed = false;
        }
        #endregion OptionCollection Methods
        
        #region OptionCollection Properties
        /// <summary>
        /// Get the IOption instance from the collection at the given index. Attempting to set the 
        /// IOption instance will result in a NotSupportedException exception being thrown.
        /// </summary>
        /// <param name="index">The index of the IOption instance being sought.</param>
        /// <returns>The IOption instance at the given index</returns>
        public IOption this[int index] { get { return _attributes[index]; } set { throw new NotSupportedException("This is a read-only collection."); } }
        /// <summary>
        /// Get the IOption instance from the collection at the given index. Attempting to set the 
        /// IOption instance will result in a NotSupportedException exception being thrown.
        /// </summary>
        /// <param name="index">The index of the IOption instance being sought.</param>
        /// <param name="anonymousOptions">false to search through all options, true to only get anonymous options.</param>
        /// <returns>The IOption instance at the given index</returns>
        internal IOption this[int index, bool anonymousOptions] 
        { 
            get { return (anonymousOptions) ? _anonAttributes[index] : _attributes[index]; } 
            set { throw new NotSupportedException("This is a read-only collection."); } 
        }
        /// <summary>
        /// Get the IOption instance from the collection which has the given index. Attempting to set the 
        /// IOption instance will result in a NotSupportedException exception being thrown.
        /// </summary>
        /// <param name="optionName">The name of the IOption instance being sought.</param>
        /// <returns>The named IOption instance.</returns>
        public IOption this[string optionName] 
        { 
            get 
            {
                int index = -1;
                string key = optionName.ToLower();
                if (!_attribDict.TryGetValue(key, out index))
                    throw new CommandLineParserSystemException(
                        "Option {0}{1} does not exist.",
                        _preferredPrefix, optionName);
                return _attributes[index];

            } 
            set { throw new NotSupportedException("This is a read-only collection."); } 
        }

        /// <summary>
        /// Get the number of elements contained in OptionCollection.
        /// </summary>
        public int Count { get { return _attributes.Length; } }

        /// <summary>
        /// Get the number of anonymous options contained in OptionCollection.
        /// </summary>
        internal int AnonymousCount { get { return _anonAttributes.Length; } }

        /// <summary>
        /// Gets a value indicating whether the OptionCollection is read-only. 
        /// </summary>
        public bool IsReadOnly { get { return true; } }
        /// <summary>
        /// Gets the list of characters used to split identifiers and their values.
        /// </summary>
        public char[] OptionSplitters { get { return _appAttribute.OptionValueSplitters; } }
        /// <summary>
        /// Get the list of option prefixes.
        /// </summary>
        public string[] OptionPrefixes { get { return _appAttribute.OptionPrefixes; } }
        /// <summary>
        /// Get the preferred option prefix.
        /// </summary>
        public string PreferredPrefix { get { return _preferredPrefix; } }
        /// <summary>
        /// Get the list of switches used to invoke the help screen.
        /// </summary>
        public string[] HelpOptions { get { return _appAttribute.HelpOptions; } }
        /// <summary>
        /// Get how parsing exceptions are handled. If set to False, no exceptions will the thrown. Error messages
        /// will be written to the console only. If set to True, messages will be written to the console, and a 
        /// CommandLineParsingException exception will be thrown.
        /// </summary>        
        public bool ThrowParsingExceptions { get { return _throwParsingExceptions; } }

        internal OptionAttribute[] OptionAttributes { get { return _attributes; } }
        #endregion OptionCollection Properties

        #region OptionAttributeComparer
        internal class OptionAttributeComparer : IComparer<OptionAttribute>
        {
            public int Compare(OptionAttribute x, OptionAttribute y)
            {
                if ((x == null) && (y == null))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return (x.Order.CompareTo(y.Order));
            }
        }
        #endregion OptionAttributeComparer
    }   
}
