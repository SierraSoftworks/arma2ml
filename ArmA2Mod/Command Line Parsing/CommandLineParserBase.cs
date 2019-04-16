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
 *      Description: A base class that performs command line processing. The main 
 *                   class of this library.
 *      Version:     1.0
 *   
 */

namespace PXitCore.CommandLineParsing
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    
 

    /// <summary>
    /// A base class that performs command line processing.
    /// </summary>
    public abstract class CommandLineBase
    {
        #region CommandLineBase Declarations
        bool _showHelp = false;
        OptionAttribute[] _attributes = new OptionAttribute[0];
        bool _throwParsingExceptions = true;
        Regex _argRegex = new Regex("^([a-z0-9]*)(?:[:=](.*))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        TextWriter _textWriter = null;
        int _writerLineLength;
        
        string _preferredPrefix = "/";
        string[] _helpOptions = new string[] { "?","help" };
        bool _parseErrors = false;
        string[] _optionPrefixes = new string[] { "/","-" };
        char[] _optionSplitters = new char[] { ' ', ':' };
        string[] _commandLineArgs = new string[0];
        OptionCollection _options = null;
        string _startupPath = null;
        #endregion CommandLineBase Declarations

        #region CommandLineBase Constructors
        /// <summary>
        /// Create a new CommandLineBase instance. All output is sent to the console.
        /// </summary>
        public CommandLineBase()
        {
            Construct(null, -1, new string[0]);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and parse the command line arguments given.
        /// All output is sent to the console.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public CommandLineBase(string[] args)
        {
            Construct(null, -1, args);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and parse the command line arguments given.
        /// All output is sent to the console.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public CommandLineBase(string args)
        {
            Construct(null, -1, args);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and output all errors and messages to the
        /// TextWriter provided.
        /// </summary>
        /// <param name="textWriter">The TextWriter to handle all output messages.</param>
        public CommandLineBase(TextWriter textWriter)
        {
            Construct(textWriter, -1, new string[0]);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and output all errors and messages to the
        /// TextWriter provided.
        /// </summary>
        /// <param name="textWriter">The TextWriter to handle all output messages.</param>
        /// <param name="lineLength">The line length for this TextWriter.</param>
        public CommandLineBase(TextWriter textWriter, int lineLength)
        {
            Construct(textWriter, lineLength, new string[0]);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and output all errors and messages to the
        /// TextWriter provided. Parse the command line arguments given.
        /// </summary>
        /// <param name="textWriter">The TextWriter to handle all output messages.</param>
        /// <param name="args">The command line arguments.</param>
        public CommandLineBase(TextWriter textWriter, string[] args)
        {
            Construct(textWriter, -1, args);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and output all errors and messages to the
        /// TextWriter provided. Parse the command line arguments given.
        /// </summary>
        /// <param name="textWriter">The TextWriter to handle all output messages.</param>
        /// <param name="args">The command line arguments.</param>
        public CommandLineBase(TextWriter textWriter, string args)
        {
            Construct(textWriter, -1, args);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and output all errors and messages to the
        /// TextWriter provided. Parse the command line arguments given.
        /// </summary>
        /// <param name="textWriter">The TextWriter to handle all output messages.</param>
        /// <param name="lineLength">The line length for this TextWriter.</param>
        /// <param name="args">The command line arguments.</param>
        public CommandLineBase(TextWriter textWriter, int lineLength, string[] args)
        {
            Construct(textWriter, lineLength, args);
        }

        /// <summary>
        /// Create a new CommandLineBase instance and output all errors and messages to the
        /// TextWriter provided. Parse the command line arguments given.
        /// </summary>
        /// <param name="textWriter">The TextWriter to handle all output messages.</param>
        /// <param name="lineLength">The line length for this TextWriter.</param>
        /// <param name="args">The command line arguments.</param>
        public CommandLineBase(TextWriter textWriter, int lineLength, string args)
        {
            Construct(textWriter, lineLength, args);
        }

        private void Construct(TextWriter textWriter, int lineLength, string[] args)
        {
            if (textWriter == null)
            {
                // Set the Console as our output writer. Try get the line length.
                try
                {
                    _textWriter = Console.Out;
                    if (lineLength == -1)
                        try
                        {
                            _writerLineLength = Console.WindowWidth;
                        }
                        catch (Exception)
                        {
                            //System.Diagnostics.Debug.WriteLine(exception.Message);
                            _writerLineLength = 80;
                        }
                }
                catch (Exception)
                {
                    //System.Diagnostics.Debug.WriteLine(exception.Message);
                    _textWriter = null;
                }
            }
            else
            {
                // Use the TextWriter for output. Default the line length if not given.
                _textWriter = textWriter;
                if (lineLength == -1)
                    lineLength = 1024;
            }                      

            CollectAttributes();
            if (args != null)
                ParseCommandLine(args);

            if (_textWriter != null)
                _textWriter.Flush();
        }

        private void Construct(TextWriter textWriter, int lineLength, string args)
        {
            if (textWriter == null)
            {
                // Set the Console as our output writer. Try get the line length.
                try
                {
                    _textWriter = Console.Out;
                    if (lineLength == -1)
                        try
                        {
                            _writerLineLength = Console.WindowWidth;
                        }
                        catch (Exception)
                        {
                            //System.Diagnostics.Debug.WriteLine(exception.Message);
                            _writerLineLength = 80;
                        }
                }
                catch (Exception)
                {
                    //System.Diagnostics.Debug.WriteLine(exception.Message);
                    _textWriter = null;
                }
            }
            else
            {
                // Use the TextWriter for output. Default the line length if not given.
                _textWriter = textWriter;
                if (lineLength == -1)
                    lineLength = 1024;
            }

            CollectAttributes();
            if (args != null)
                ParseCommandLineRegex(args);

            if (_textWriter != null)
                _textWriter.Flush();
        }
        #endregion CommandLineBase Constructors

        private string EscapeRegex(string text)
        {
            return text
                .Replace(":", "\\:")
                .Replace("\"", "\\\"")
                .Replace("*", "\\*")
                .Replace("+", "\\+")
                .Replace("?", "\\?")
                .Replace("-", "\\-")
                .Replace(".","\\.")
                .Replace("\n","\\n")
                .Replace("\t","\\t")
                .Replace("\r","\\r")
                .Replace("/","\\/")
                .Replace("<","\\<")
                .Replace(">","\\>")
                .Replace(" ","\\ ");
        }

        #region CommandLineBase Methods
        /// <summary>
        /// Parse the command line arguments given.
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        /// <returns>Returns true if no errors are encountered, false otherwise.</returns>
        public bool ParseCommandLine(string[] args)
        {
            IOption anonymousOption = null;
            _commandLineArgs = args;
            OptionEnumerator anonymousOptions = _options.GetAnonymousEnumerator() as OptionEnumerator;


            _parseErrors = false;
            if (_options == null)
                return true;

            try
            {
                _options.ClearProcessedFlags();

                for (int argNo = 0; argNo < args.Length; argNo++)
                {
                    string optionName = string.Empty;
                    string optionValue = string.Empty;
                    string argument = "";

                    // Check if we have an option prefix. If not, we'll assume that it's
                    // an anonymous option (just a value), else we'll need to try split
                    // the option name and value
                    if (Array.IndexOf(_optionPrefixes, args[argNo][0].ToString()) == -1)
                    {
                        optionValue = args[argNo];
                        argument = args[argNo];
                    }
                    else
                    {
                        foreach(string prefix in _optionPrefixes)
                            if( args[argNo].Trim().StartsWith(prefix))
                                argument =  args[argNo].Trim().Remove(0,prefix.Length);
                        if (!GetNameAndValue(argument, ref optionName, ref optionValue))
                        {
                            if (!IsHelpOption(argument))
                                throw new CommandLineParsingException(
                                    "Could not parse command line option {0}{1}.",
                                    _preferredPrefix, argument);

                            _showHelp = true;
                            break;
                        }
                    }

                    // Try find this option in our collection. If we don't, then
                    // try check to see if it's a help option.
                    IOption optionTemp = null;
                    if (!_options.TryGetOption(optionName, out optionTemp))
                    {
                        if (IsHelpOption(optionName.ToLower()))
                        {
                            _showHelp = true;
                            break;
                        }

                        // Check for anonymous options...
                        if ((anonymousOption != null) &&
                            (anonymousOption.AllowMultiple))
                            optionTemp = anonymousOption;

                        else if (!anonymousOptions.MoveNext())
                            throw new CommandLineParsingException(
                                "Invalid command line argument {0}.",
                                argument);
                        else
                        {
                            anonymousOption = anonymousOptions.Current;
                            optionTemp = anonymousOption;
                        }
                    }

                    OptionAttribute option = optionTemp as OptionAttribute;

                    // Just make sure...
                    if (option == null)
                        throw new CommandLineParsingException(
                            "Invalid command line option {0}{1}.",
                            _preferredPrefix, optionName);

                    // Get the name of the anonymous option
                    if (option.IsAnonymous)
                        optionName = option.Name;          

                    // Validate the value format, if a pattern was given
                    if (option.FormatPattern != string.Empty)
                    {
                        Regex regex = null;
                        try
                        {
                            regex = new Regex(option.FormatPattern, RegexOptions.IgnoreCase);
                        }
                        catch (ArgumentException exception)
                        {
                            throw new CommandLineParsingException(
                                exception,
                                "Could not parse the format pattern. The pattern given was {0}.",
                                option.FormatPattern);
                        }

                        if (!regex.IsMatch(optionValue))
                            throw new CommandLineParsingException(
                                "The value given for option {0}{1} was invalid.{2}",                                
                                ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName,
                                (option.FormatDisplay == string.Empty) ? string.Empty :
                                    string.Format("It should be formatted as as '{0}'.", option.FormatDisplay));
                    }



                    if (option.AllowMultiple)
                        option.AddValue(optionValue);
                    else
                        option.Value = optionValue;
                    option.Processed = true;                    


                    // Test the value presence
                    if ((option.MustHaveValue) &&
                        (!option.HasValue))
                        throw new CommandLineParsingException(
                            "Was expecting a value for option {0}{1} and didn't detect one.",
                            ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName);
                    if ((option.MustNotHaveValue) &&
                        (option.HasValue))
                        throw new CommandLineParsingException(
                            "Was not expecting a value for option {0}{1} and one was detected.",
                            ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName);
                }


                if (_showHelp)
                {
                    // TODO: Must be a better way to do this. We should capture the
                    // 'help' option up front before we go to the trouble of parsing 
                    // everything else on the command line
                    ShowHelp();
                }
                else
                    foreach(IOption option in _options)
                        if ((!option.IsPresent) &&
                            (!option.IsOptional))
                            throw new CommandLineParsingException(
                                "Option {0}{1} is mandatory, but was not provided.",
                                ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), option.Name);
            }
            catch (CommandLineParsingException exception)
            {
                WriteLine("ERROR: {0}", exception.Message);
                _parseErrors = true;
                if (_throwParsingExceptions)
                    throw exception;
            }

            return !_parseErrors;
        }

        /// <summary>
        /// Parse the command line arguments given using a RegEx
        /// </summary>
        /// <param name="args">An array of command line arguments.</param>
        /// <returns>Returns true if no errors are encountered, false otherwise.</returns>
        public bool ParseCommandLineRegex(string args)
        {

            string regexOptionPrefixes = "";
            string regexOptionPrefixesContained = "";
            foreach (string id in _optionPrefixes)
            {
                regexOptionPrefixes += EscapeRegex(id) + "|";
                regexOptionPrefixesContained += EscapeRegex(id);
            }
            regexOptionPrefixes = regexOptionPrefixes.Remove(regexOptionPrefixes.Length - 1);

            string regexOptionSplit = "\\ ";
            string regexOptionSplitContained = "\\ ";
            foreach(char id in _optionSplitters)
                if(id != ' ')
                {
                    regexOptionSplit += "|" + EscapeRegex(id.ToString());
                    regexOptionSplitContained += EscapeRegex(id.ToString());
                }


            Regex cmdParser = new Regex(
                /*
            #region "Identifiers and Values"
                #region "Quoted Values"
                    "(?<Argument>" +
                    "(?:"+regexOptionPrefixes+")" + //Option Prefixes
                    "(?<Identifier>[^"+regexOptionSplitContained+"]+)" + //Identifiers
                    "(?:"+regexOptionSplit+")" + //Option/Value spliters
                    "\\\"" +
                    "(?<Value>.+)" +
                    "\\\"" +
                    ")" +
                #endregion
                "|" +
                #region "UnQuoted Values"
                    "(?<Argument>" +
                    "(?:"+regexOptionPrefixes+")" + //Option Prefixes
                    "(?<Identifier>[^"+regexOptionSplitContained+"]+)" + //Identifiers
                    "(?:"+regexOptionSplit+")" + //Option/Value spliters
                    "\\\"" +
                    "(?<Value>[^"+regexOptionSplitContained + regexOptionPrefixesContained+"]+)" +
                    "\\\"" +
                    ")" +
                #endregion      
                "|" +
            #endregion

            #region "Identifiers"
                "(?<Argument>" +
                "(?:" + regexOptionPrefixes + ")" +
                "(?<Identifier>[^" + regexOptionSplitContained + "]+)" +
                ")" +
            #endregion

            #region "Values"
                #region "Quoted"
                    "(?<Argument>" +                    
                    "(?<Value>\\\".+\\\")" +
                    ")" +
                #endregion
                #region "UnQuoted"
                    "(?<Argument>" +                    
                    "(?<Value>\\\"[^\\ ]+\\\")" +
                    ")"
                #endregion
            #endregion
                */

                "(?<Argument>\r\n(?:" + regexOptionPrefixes + ")\r\n(?<Identifier>[^" + regexOptionSplitContained + "]+)\r\n(?:\\=" +
      "|\\ |\\:)\r\n\\\"\r\n(?<Value>[^\\\"]+)\r\n\\\"\r\n)\r\n|\r\n(?<Argument>\r\n(?:" +
      "" + regexOptionPrefixes + ")\r\n(?<Identifier>[^" + regexOptionSplitContained + "]+)\r\n(?:\\=|\\ |\\:)\r\n(?<Val" +
      "ue>[^" + regexOptionSplitContained + regexOptionPrefixesContained + "]+)\r\n)\r\n|\r\n(?<Argument>\r\n(?:" + regexOptionPrefixes + ")\r\n(?<Iden" +
      "tifier>[^" + regexOptionSplitContained + "]+)\r\n)\r\n|\r\n(?<Argument>\r\n\\\"(?<Value>[^\\\"]+)\\\"" +
      "\r\n)\r\n|\r\n(?<Argument>\r\n(?<Value>[^\\ ]+)\r\n)",
    RegexOptions.CultureInvariant
    | RegexOptions.IgnorePatternWhitespace
    | RegexOptions.Compiled);


            IOption anonymousOption = null;
            OptionEnumerator anonymousOptions = _options.GetAnonymousEnumerator() as OptionEnumerator;


            _parseErrors = false;
            if (_options == null)
                return true;

            try
            {
                _options.ClearProcessedFlags();

                bool sysPath = false;

                foreach (Match regMatch in cmdParser.Matches(args))
                {
                    string optionName = string.Empty;
                    string optionValue = string.Empty;
                    string argument;

                    

                    if (string.IsNullOrEmpty(regMatch.Groups["Identifier"].Value))
                    {
                        //Anonymous Option
                        optionValue = regMatch.Groups["Value"].Value;
                        argument = regMatch.Value;
                    }
                    else if (string.IsNullOrEmpty(regMatch.Groups["Value"].Value))
                    {
                        //No value
                        optionName = regMatch.Groups["Identifier"].Value;
                        argument = regMatch.Value;
                    }
                    else
                    {
                        //Full values
                        optionName = regMatch.Groups["Identifier"].Value;
                        optionValue = regMatch.Groups["Value"].Value;
                        argument = regMatch.Value;
                    }

                    if (!sysPath)
                    {
                        if (optionName == "")
                        {
                            sysPath = true;
                            _startupPath = optionValue;
                            continue;
                        }
                    }


                    IOption optionTemp = null;
                    if (!_options.TryGetOption(optionName, out optionTemp))
                    {
                        if (IsHelpOption(optionName.ToLower()))
                        {
                            _showHelp = true;
                            break;
                        }

                        // Check for anonymous options...
                        if ((anonymousOption != null) &&
                            (anonymousOption.AllowMultiple))
                            optionTemp = anonymousOption;

                        else if (!anonymousOptions.MoveNext())
                            throw new CommandLineParsingException(
                                "Invalid command line argument {0}.",
                                argument);
                        else
                        {
                            anonymousOption = anonymousOptions.Current;
                            optionTemp = anonymousOption;
                        }
                    }


                    OptionAttribute option = optionTemp as OptionAttribute;

                    // Just make sure...
                    if (option == null)
                        throw new CommandLineParsingException(
                            "Invalid command line option {0}{1}.",
                            _preferredPrefix, optionName);

                    // Get the name of the anonymous option
                    if (option.IsAnonymous)
                        optionName = option.Name;
                                       

                    // Validate the value format, if a pattern was given
                    if (option.FormatPattern != string.Empty)
                    {
                        Regex regex = null;
                        try
                        {
                            regex = new Regex(option.FormatPattern, RegexOptions.IgnoreCase);
                        }
                        catch (ArgumentException exception)
                        {
                            throw new CommandLineParsingException(
                                exception,
                                "Could not parse the format pattern. The pattern given was {0}.",
                                option.FormatPattern);
                        }

                        if (!regex.IsMatch(optionValue))
                            throw new CommandLineParsingException(
                                "The value given for option {0}{1} was invalid.{2}",
                                ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName,
                                (option.FormatDisplay == string.Empty) ? string.Empty :
                                    string.Format("It should be formatted as as '{0}'.", option.FormatDisplay));
                    }



                    if (option.AllowMultiple)
                        option.AddValue(optionValue);
                    else
                        option.Value = optionValue;
                    option.Processed = true;


                    // Test the value presence
                    if ((option.MustHaveValue) &&
                        (!option.HasValue))
                        throw new CommandLineParsingException(
                            "Was expecting a value for option {0}{1} and didn't detect one.",
                            ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName);
                    if ((option.MustNotHaveValue) &&
                        (option.HasValue))
                    {
                        //This should actually process an anon value then
                        // Check for anonymous options...
                        if ((anonymousOption != null) &&
                            (anonymousOption.AllowMultiple))
                            optionTemp = anonymousOption;

                        else if (!anonymousOptions.MoveNext())
                            throw new CommandLineParsingException(
                           "Was not expecting a value for option {0}{1} and one was detected.",
                           ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName);
                        else
                        {
                            anonymousOption = anonymousOptions.Current;
                            optionTemp = anonymousOption;
                        }

                        option = optionTemp as OptionAttribute;

                        // Just make sure...
                        if (option == null)
                            throw new CommandLineParsingException(
                                "Invalid command line option {0}{1}.",
                                _preferredPrefix, optionName);

                        // Get the name of the anonymous option
                        if (option.IsAnonymous)
                            optionName = option.Name;


                        // Validate the value format, if a pattern was given
                        if (option.FormatPattern != string.Empty)
                        {
                            Regex regex = null;
                            try
                            {
                                regex = new Regex(option.FormatPattern, RegexOptions.IgnoreCase);
                            }
                            catch (ArgumentException exception)
                            {
                                throw new CommandLineParsingException(
                                    exception,
                                    "Could not parse the format pattern. The pattern given was {0}.",
                                    option.FormatPattern);
                            }

                            if (!regex.IsMatch(optionValue))
                                throw new CommandLineParsingException(
                                    "The value given for option {0}{1} was invalid.{2}",
                                    ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName,
                                    (option.FormatDisplay == string.Empty) ? string.Empty :
                                        string.Format("It should be formatted as as '{0}'.", option.FormatDisplay));
                        }



                        if (option.AllowMultiple)
                            option.AddValue(optionValue);
                        else
                            option.Value = optionValue;
                        option.Processed = true;
                    }
                }


                if (_showHelp)
                {
                    // TODO: Must be a better way to do this. We should capture the
                    // 'help' option up front before we go to the trouble of parsing 
                    // everything else on the command line
                    ShowHelp();
                }
                else
                    foreach (IOption option in _options)
                        if ((!option.IsPresent) &&
                            (!option.IsOptional))
                            throw new CommandLineParsingException(
                                "Option {0}{1} is mandatory, but was not provided.",
                                ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), option.Name);
            }
            catch (CommandLineParsingException exception)
            {
                WriteLine("ERROR: {0}", exception.Message);
                _parseErrors = true;
                if (_throwParsingExceptions)
                    throw exception;
            }

            return !_parseErrors;

        }

        public bool ParseCommandLineNew(string[] args)
        {
            IOption anonymousOption = null;
            _commandLineArgs = args;
            OptionEnumerator anonymousOptions = _options.GetAnonymousEnumerator() as OptionEnumerator;


            _parseErrors = false;
            if (_options == null)
                return true;

            try
            {
                _options.ClearProcessedFlags();

                Regex Spliter = new Regex(@"^-{1,2}|^/|=|:",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

                Regex Remover = new Regex(@"^['""]?(.*?)['""]?$",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                string Parameter = null;
                string[] Parts;



                foreach (string txt in args)
                {
                    Parts = Spliter.Split(txt,3);
                    string optionName = string.Empty;
                    string optionValue = string.Empty;
                    string argument = string.Empty;

                    int tempLength = Parts.Length;
                    if(Parts[0] == "")
                        tempLength --;

                switch(tempLength)
                {
                // Found a value (for the last parameter 

                // found (space separator))                        

                case 1:
                        if (Parameter != null)
                        {
                            //switch
                            optionName = Parts[0];
                            argument = txt;

                            processOptions(anonymousOptions, anonymousOption, optionName, optionValue, argument);

                            optionName = string.Empty;
                            optionValue = string.Empty;
                            argument = string.Empty;

                            Parameter = null;
                        }

                        else
                        {
                            //anonymous parameter
                            optionValue = Parts[0];
                            argument = txt;

                            processOptions(anonymousOptions, anonymousOption, optionName, optionValue, argument);

                            optionName = string.Empty;
                            optionValue = string.Empty;
                            argument = string.Empty;

                            Parameter = null;
                        }

                        break;

                case 2:

                    if(Parameter!=null)
                    {
                        //Anonymous Value
                        optionName = Parameter;
                        argument = txt;

                        processOptions(anonymousOptions, anonymousOption, optionName, optionValue, argument);
                        optionName = string.Empty;
                        optionValue = string.Empty;
                        argument = string.Empty;

                    }
                    Parameter=Parts[1];
                    break;


                case 3:
                    // The last parameter is still waiting. 

                    // With no value, set it to true.

                    if(Parameter != null)
                    {
                        //Anonymous Value
                        optionName = Parameter;
                        argument = txt;

                        processOptions(anonymousOptions, anonymousOption, optionName, optionValue, argument);
                        optionName = string.Empty;
                        optionValue = string.Empty;
                        argument = string.Empty;

                    }

                    Parameter = Parts[1];

                    // Remove possible enclosing characters (",')

                    
                    Parts[2] = Remover.Replace(Parts[2], "$1");
                    optionName = Parameter;
                    optionValue = Parts[2];
                    argument = txt;

                    processOptions(anonymousOptions, anonymousOption, optionName, optionValue, argument);
                            optionName = string.Empty;
                            optionValue = string.Empty;
                            argument = string.Empty;


                    Parameter=null;
                    break;
                }


            // In case a parameter is still waiting

            if(Parameter != null)
            {
                //Anonymous Option
                optionValue = Parameter;
                argument = txt;

                processOptions(anonymousOptions, anonymousOption, optionName, optionValue, argument);
                optionName = string.Empty;
                optionValue = string.Empty;
                argument = string.Empty;

            }

            }

                
        


                if (_showHelp)
                {
                    // TODO: Must be a better way to do this. We should capture the
                    // 'help' option up front before we go to the trouble of parsing 
                    // everything else on the command line
                    ShowHelp();
                }
                else
                    foreach (IOption option in _options)
                        if ((!option.IsPresent) &&
                            (!option.IsOptional))
                            throw new CommandLineParsingException(
                                "Option {0}{1} is mandatory, but was not provided.",
                                ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), option.Name);
        
        
            }
            catch (CommandLineParsingException exception)
            {
                WriteLine("ERROR: {0}", exception.Message);
                _parseErrors = true;
                if (_throwParsingExceptions)
                    throw exception;
            }
        

            return !_parseErrors;
        
        }

        private OptionAttribute processOptions(OptionEnumerator anonymousOptions, IOption anonymousOption, string optionName, string optionValue, string argument)
        {
            IOption optionTemp = null;
            if (!_options.TryGetOption(optionName, out optionTemp))
            {
                if (IsHelpOption(optionName.ToLower()))
                {
                    _showHelp = true;
                    return null;
                }

                // Check for anonymous options...
                if ((anonymousOption != null) &&
                    (anonymousOption.AllowMultiple))
                    optionTemp = anonymousOption;

                else if (!anonymousOptions.MoveNext())
                    throw new CommandLineParsingException(
                        "Invalid command line argument {0}.",
                        argument);
                else
                {
                    anonymousOption = anonymousOptions.Current;
                    optionTemp = anonymousOption;
                }
            }


            OptionAttribute option = optionTemp as OptionAttribute;

            // Just make sure...
            if (option == null)
                throw new CommandLineParsingException(
                    "Invalid command line option {0}{1}.",
                    _preferredPrefix, optionName);

            // Get the name of the anonymous option
            if (option.IsAnonymous)
                optionName = option.Name;

            // Validate the value format, if a pattern was given
            if (option.FormatPattern != string.Empty)
            {
                Regex regex = null;
                try
                {
                    regex = new Regex(option.FormatPattern, RegexOptions.IgnoreCase);
                }
                catch (ArgumentException exception)
                {
                    throw new CommandLineParsingException(
                        exception,
                        "Could not parse the format pattern. The pattern given was {0}.",
                        option.FormatPattern);
                }

                if (!regex.IsMatch(optionValue))
                    throw new CommandLineParsingException(
                        "The value given for option {0}{1} was invalid.{2}",
                        ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName,
                        (option.FormatDisplay == string.Empty) ? string.Empty :
                            string.Format("It should be formatted as as '{0}'.", option.FormatDisplay));
            }



            if (option.AllowMultiple)
                option.AddValue(optionValue);
            else
                option.Value = optionValue;
            option.Processed = true;


            // Test the value presence
            if ((option.MustHaveValue) &&
                (!option.HasValue))
                throw new CommandLineParsingException(
                    "Was expecting a value for option {0}{1} and didn't detect one.",
                    ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName);
            if ((option.MustNotHaveValue) &&
                (option.HasValue))
                throw new CommandLineParsingException(
                    "Was not expecting a value for option {0}{1} and one was detected.",
                    ((option.IsAnonymous) ? string.Empty : _preferredPrefix.ToString()), optionName);

            return option;
        }

        /// <summary>
        /// Show the basic application information, and is also shown at the top of the help screen. 
        /// This method can be overriden to generate custom header screens.
        /// </summary>
        public virtual void ShowHeader()
        {
            // Collect the various bits of global information we need
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            string appName = Path.GetFileName(entryAssembly.CodeBase);
            string company = Helper.GetAssemblyCompany(entryAssembly).Replace("®", "(R)").Replace("©", "(C)");
            string title = Helper.GetAssemblyTitle(entryAssembly).Replace("®", "(R)").Replace("©", "(C)");
            string description = Helper.GetAssemblyDescription(entryAssembly).Replace("®", "(R)").Replace("©", "(C)");
            string copyright = Helper.GetAssemblyCopyright(entryAssembly).Replace("®", "(R)").Replace("©", "(C)");

            // Print it all out
            if (company != string.Empty)
                Write(company + "   ",ConsoleColor.Blue);

            if (title != string.Empty)
                Write(title + "   ",ConsoleColor.Green);

            WriteLine("\r\n{0}: {1},  {2}: {3}",
                "Assembly version",                
                Helper.GetAssemblyVersion(entryAssembly),
                "File version",
                Helper.GetAssemblyFileVersion(entryAssembly));

            if (description != string.Empty)
                WriteLine(description);

            if (copyright != string.Empty)
                WriteLine(copyright);

            WriteLine(2);
        }

        /// <summary>
        /// Force the help screen to be printed. This method can be overriden to generate custom help screens.
        /// </summary>
        public virtual void ShowHelp()
        {
            const int PAD_WIDTH = 30;
            const string VALUE_PLACEHOLDER = "xxx";
            const int SYNTAX_PAD_WIDTH = 6;

            StringBuilder syntaxBuilder = new StringBuilder();            
            StringBuilder detailBuilder = new StringBuilder();

            
            // Collect the various bits of global information we need
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            syntaxBuilder.AppendFormat("    {0}", 
                Path.GetFileName(entryAssembly.CodeBase));
            int lineLen = syntaxBuilder.Length;

            // Loop through each option and collect the option information
            foreach (IOption option in _options)
            {
                string optionName;
                if (option.IsAnonymous)
                    optionName = (option.FormatDisplay == string.Empty) ? VALUE_PLACEHOLDER : option.FormatDisplay;

                else
                {
                    switch (option.ValuePresence)
                    {
                        case OptionAttributeValuePresence.MustNotHaveValue:
                            optionName = string.Format("{0}{1}",
                                _preferredPrefix, option.Name);
                            break;
                        case OptionAttributeValuePresence.MayHaveValue:
                            optionName = string.Format("{0}{1}[:{2}]",
                                _preferredPrefix, option.Name, (option.FormatDisplay == string.Empty) ? VALUE_PLACEHOLDER : option.FormatDisplay);
                            break;
                        default:
                            optionName = string.Format("{0}{1}:{2}",
                                _preferredPrefix, option.Name, (option.FormatDisplay == string.Empty) ? VALUE_PLACEHOLDER : option.FormatDisplay);
                            break;
                    }
                }


                

                string syntaxWord = string.Format(" {1}{3}{0}{2}{4}",
                    option.Name,
                    (option.IsOptional ? "[" : string.Empty),
                    (option.IsOptional ? "]" : string.Empty),
                    (option.IsAnonymous ? string.Empty : _preferredPrefix.ToString()),
                    (option.AllowMultiple ? "*" : string.Empty));

                if ((lineLen + syntaxWord.Length) > _writerLineLength)
                {
                    syntaxBuilder.AppendFormat("\r\n{0}", new string(' ', SYNTAX_PAD_WIDTH));
                    lineLen = SYNTAX_PAD_WIDTH;
                }

                syntaxBuilder.Append(syntaxWord);
                lineLen += syntaxWord.Length;


                detailBuilder.AppendFormat("{0} {1}\r\n",
                    optionName.PadRight(PAD_WIDTH - 1),
                    PadAndTab(string.Format("{0}{1}",
                    ((option.IsAnonymous) ? option.Name + ": " : string.Empty),
                    option.ShortDescription), PAD_WIDTH, _writerLineLength));
            }

            // Print the header information
            ShowHeader();

            // Print the syntax
            Write(" {0}:\r\n{1}",
                "Usage", 
                syntaxBuilder.ToString());

            // Print the command line details
            WriteLine(4);
            WriteLine("{1}{0}\r\n\r\n{2}",                
                "Command Line Options",
                new string(' ', PAD_WIDTH / 2), 
                detailBuilder.ToString());

        }

        private string PadAndTab(string source, int startPos, int lineLength)
        {
            return PadAndTab(source, startPos, lineLength, new char[] { ' ' });
        }

        private string PadAndTab(string source, int startPos, int lineLength, char[] delimiters)
        {
            StringBuilder builder = new StringBuilder();
            int currentLength = startPos;
            int originalIndex = 0;
            foreach (string word in source.Split(delimiters))
            {
                if ((currentLength + word.Length + 1) >= lineLength)
                {
                    builder.AppendFormat("\r\n{0}", new string(' ', startPos + 2));
                    currentLength = startPos + 2;
                    originalIndex++;
                }

                builder.AppendFormat("{0} ", word);
                currentLength += word.Length + 1;
            }
            return builder.ToString();
        }

        private void CollectAttributes()
        {   
            MemberInfo memberInfo = this.GetType();
            _options = new OptionCollection(memberInfo);
            if (_options == null)
            {
                _helpOptions = new string[] { "?" };
                _preferredPrefix = "/";
                _optionPrefixes = new string[] { "/" };
                _throwParsingExceptions = true;
                _attributes = new OptionAttribute[0];
            }
            else
            {
                _helpOptions = _options.HelpOptions;
                _preferredPrefix = _options.PreferredPrefix;
                _optionPrefixes = _options.OptionPrefixes;
                _throwParsingExceptions = _options.ThrowParsingExceptions;
                _attributes = _options.OptionAttributes;
                _optionSplitters = _options.OptionSplitters;
            }
        }

        private bool IsHelpOption(string optionName)
        {
            // Try to determine whether a 'show help' option was given.
            for (int index = 0; index < _helpOptions.Length; index++)
                if (string.Compare(optionName, _helpOptions[index], true) == 0)
                    return true;
            return false;
        }

        private bool GetNameAndValue(string commandOption, ref string name, ref string value)
        {
            Match match = _argRegex.Match(commandOption);
            if (match == null)
                return false;

            if (match.Groups.Count != 3)
                return false;

            name = match.Groups[1].Value.Trim();
            value = match.Groups[2].Value.Trim();
            return true;
        }

        /// <summary>
        /// Write a message to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="value">The message to write</param>
        public void Write(string value)
        {
            if (_textWriter != null)
                _textWriter.Write(value);
        }

        /// <summary>
        /// Write a message to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="value">The message to write</param>
        public void Write(string value,ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (_textWriter != null)
                _textWriter.Write(value);
            Console.ResetColor();
        }

        /// <summary>
        /// Write a format string to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="format">The format string to write</param>
        /// <param name="args">The format string arguments</param>
        public void Write(string format, params object[] args)
        {
            if (_textWriter != null)
                _textWriter.Write(format, args);
        }

        /// <summary>
        /// Write a format string to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="format">The format string to write</param>
        /// <param name="args">The format string arguments</param>
        public void Write(string format,ConsoleColor color, params object[] args)
        {
            Console.ForegroundColor = color;
            if (_textWriter != null)
                _textWriter.Write(format, args);
            Console.ResetColor();
        }

        /// <summary>
        /// Write a blank line to either the console or the provided TextWriter.
        /// </summary>
        public void WriteLine()
        {
            if (_textWriter != null)
                _textWriter.WriteLine();
        }

        /// <summary>
        /// Write the stated number of blank lines to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="noOfBlankLines">The number of blank lines to write</param>
        public void WriteLine(int noOfBlankLines)
        {
            if (_textWriter != null)
                for (int count = 1; count < noOfBlankLines; count++)
                    _textWriter.WriteLine();
        }

        /// <summary>
        /// Write a message with a line termination to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="value">The message to write</param>
        public void WriteLine(string value)
        {
            if (_textWriter != null)
                _textWriter.WriteLine(value);
        }

        /// <summary>
        /// Write a format string with a line termination to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="format">The format string to write</param>
        /// <param name="args">The format string arguments</param>
        public void WriteLine(string format, params object[] args)
        {
            if (_textWriter != null)
                _textWriter.WriteLine(format, args);
        }

        /// <summary>
        /// Write a message with a line termination to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="value">The message to write</param>
        /// <param name="color">The <see cref="ConsoleColor"/> that should be used on the console display</param>
        public void WriteLine(string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (_textWriter != null)
                _textWriter.WriteLine(value);
            Console.ResetColor();

        }

        /// <summary>
        /// Write a message with a line termination to either the console or the provided TextWriter.
        /// </summary>
        /// <param name="format">The format string to write</param>
        /// <param name="color">The <see cref="ConsoleColor"/> that should be used on the console display</param>
        /// <param name="args">The format string arguments</param>
        public void WriteLine(string format, ConsoleColor color,params object[] args)
        {
            Console.ForegroundColor = color;
            if (_textWriter != null)
                _textWriter.WriteLine(format,args);
            Console.ResetColor();

        }
        #endregion CommandLineBase Methods

        #region CommandLineBase Properties
        /// <summary>
        /// Gets whether errors were encountered during command line parsing.
        /// </summary>
        public bool ParseErrors { get { return _parseErrors; } }
        /// <summary>
        /// Gets whether the help screen was invoked, either programmatically 
        /// or through a command line option.
        /// </summary>
        public bool ShowingHelp { get { return _showHelp; } }
        /// <summary>
        /// Gets a collection of IOptions on the superclass.
        /// </summary>        
        public OptionCollection Options { get { return _options; } }
        /// <summary>
        /// Get the command line arguments.
        /// </summary>
        public string[] CommandLineArgs { get { return _commandLineArgs; } }

        public string StartupPath
        {
            get { return _startupPath; }
        }
        #endregion CommandLineBase Properties
    }    
}
