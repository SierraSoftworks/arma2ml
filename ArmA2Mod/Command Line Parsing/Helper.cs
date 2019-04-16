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
 *      29 August 2008 
 *      mailto:fletcher.keith@gmail.com
 *   
 *      Description: General helper methods.
 *      Version:     1.0
 *      
 */

namespace PXitCore.CommandLineParsing
{
    using System;
    using System.Reflection;

    internal static class Helper
    {
        #region Reflection helpers
        internal static Version GetAssemblyFileVersion(Assembly assembly)
        {
            if (assembly == null)
                return new Version();

            AssemblyFileVersionAttribute attribute = GetCustomAttribute<AssemblyFileVersionAttribute>(assembly);
            if (attribute == null)
                return new Version();
            else
                return new Version(attribute.Version);
        }

        internal static Version GetAssemblyVersion(Assembly assembly)
        {
            if (assembly == null)
                return new Version();
            else
                return assembly.GetName().Version;
        }

        internal static string GetAssemblyCopyright(Assembly assembly)
        {
            if (assembly == null)
                return string.Empty;

            AssemblyCopyrightAttribute attribute = GetCustomAttribute<AssemblyCopyrightAttribute>(assembly);
            if (attribute == null)
                return string.Empty;
            else
                return attribute.Copyright;
        }

        internal static string GetAssemblyDescription(Assembly assembly)
        {
            if (assembly == null)
                return string.Empty;

            AssemblyDescriptionAttribute attribute = GetCustomAttribute<AssemblyDescriptionAttribute>(assembly);
            if (attribute == null)
                return string.Empty;
            else
                return attribute.Description;
        }

        internal static string GetAssemblyTitle(Assembly assembly)
        {
            if (assembly == null)
                return string.Empty;

            AssemblyTitleAttribute attribute = GetCustomAttribute<AssemblyTitleAttribute>(assembly);
            if (attribute == null)
                return string.Empty;
            else
                return attribute.Title;
        }

        internal static string GetAssemblyCompany(Assembly assembly)
        {
            if (assembly == null)
                return string.Empty;

            AssemblyCompanyAttribute attribute = GetCustomAttribute<AssemblyCompanyAttribute>(assembly);
            if (attribute == null)
                return string.Empty;
            else
                return attribute.Company;
        }

        internal static T GetCustomAttribute<T>(Assembly assembly) where T : Attribute
        {
            if (assembly == null) 
                return null;

            T[] customAttributes = (assembly.GetCustomAttributes(typeof(T), false)) as T[];
            if ((customAttributes == null) || (customAttributes.Length == 0))
                return null;
            else
                return customAttributes[0];
        }

        internal static T[] GetCustomAttributes<T>(MemberInfo memberInfo) where T : Attribute
        {
            if (memberInfo == null)
                return null;
            else
                return (memberInfo.GetCustomAttributes(typeof(T), true)) as T[];
        }
        #endregion Reflection helpers
    }
}
