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
 *      Description: Enumerator to support simple iteration over the OptionCollection class.
 *      Version:     1.0
 * 
 */

namespace PXitCore.CommandLineParsing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Supports a simple iteration over the OptionCollection class.
    /// </summary>
    public sealed class OptionEnumerator : IEnumerator<IOption>
    {
        #region OptionEnumerator Declarations
        int _index = -1;
        bool _anonymousOptionsOnly = false;
        OptionCollection _options = null;
        #endregion OptionEnumerator Declarations

        #region OptionEnumerator Constructors
        internal OptionEnumerator(OptionCollection options)
        {
            _options = options;
            _anonymousOptionsOnly = false;
        }

        internal OptionEnumerator(OptionCollection options, bool anonymousOptionsOnly)
        {
            _options = options;
            _anonymousOptionsOnly = anonymousOptionsOnly;
        }
        #endregion OptionEnumerator Constructors

        #region OptionEnumerator Methods
                /// <summary>
        /// Dispose this enumerator
        /// </summary>
        public void Dispose()
        {            
            _options = null;
        }   

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; 
        /// false if the enumerator has passed the end of the collection. </returns>
        public bool MoveNext()
        {   
            return (++_index < ((_anonymousOptionsOnly) ? _options.AnonymousCount : _options.Count));
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _index = -1;
        }
        #endregion OptionEnumerator Methods
        
        #region OptionEnumerator Properties
        /// <summary>
        /// Gets the current element in the collection. 
        /// </summary>
        public IOption Current
        {
            get 
            {
                if ((_options == null) ||
                    (_index == -1) ||
                    (_index >= ((_anonymousOptionsOnly) ? _options.AnonymousCount : _options.Count)))
                    throw new InvalidOperationException();

                return _options[_index, _anonymousOptionsOnly];
            }
        }

        /// <summary>
        /// Gets the current element in the collection. 
        /// </summary>
        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }
        #endregion OptionEnumerator Properties
    }    
}
