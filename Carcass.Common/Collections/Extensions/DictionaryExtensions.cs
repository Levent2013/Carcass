using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Carcass.Common.Collections.Extensions
{
    /// <summary>
    /// Set of common logic to work with <c>IDictionary&lt;string, object&gt;</c>
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Default serialization format for Float Number
        /// </summary>
        public const string SerializationFloatNumberFormat = "G";

        /// <summary>
        /// Default serialization format for Datetime
        /// </summary>
        public const string SerializationDateFormat = "u";
        
        private static ITypeDescriptorContext _commonTypeDescriptorInstance = new CommonTypeDescriptorContext();

        /// <summary>
        /// Load object of defined type from IDictionary&lt;string, object&gt;
        /// </summary>
        /// <typeparam name="T">Defined object type</typeparam>
        /// <param name="dict">Dictionary to load</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Loaded object</returns>
        public static T Get<T>(this IDictionary<string, object> dict, string attributeName) where T : class
        {
            return dict.ContainsKey(attributeName) ? ExtractClassValue<T>(dict[attributeName]) : null;
        }

        /// <summary>
        /// Load array of value type objects from IDictionary&lt;string, object&gt;
        /// </summary>
        /// <typeparam name="T">Defined object type</typeparam>
        /// <param name="dict">Dictionary to load</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Loaded object</returns>
        public static T[] GetArray<T>(this IDictionary<string, object> dict, string attributeName) where T : struct
        {
            return dict.ContainsKey(attributeName) ? ExtractArray<T>(dict[attributeName]) : null;
        }

        /// <summary>Load value of defined type from IDictionary&lt;string, object&gt;</summary>
        /// <typeparam name="T">Defined object type</typeparam>
        /// <param name="dict">Dictionary to load</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Loaded object or default value</returns>
        public static T Get<T>(this IDictionary<string, object> dict, string attributeName, T defaultValue)
        {
            if (dict.ContainsKey(attributeName))
            {
                var value = dict[attributeName];
                if (value != null)
                {
                    if (value is T)
                    {
                        return (T)value;
                    }

                    var conv = TypeDescriptor.GetConverter(typeof(T));
                    if (conv == null)
                    {
                        return defaultValue;
                    }

                    var converted = conv.ConvertFrom(value);
                    if (converted != null)
                    {
                        return (T)converted;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>Set value of defined type to IDictionary&lt;string, object&gt; under desired name</summary>
        /// <typeparam name="T">Defined object type</typeparam>
        /// <param name="dict">Dictionary to update</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">The value.</param>
        public static void Set<T>(this IDictionary<string, T> dict, string attributeName, T value)
        {
            if (dict.ContainsKey(attributeName))
            {
                dict[attributeName] = value;
            }
            else
            {
                dict.Add(attributeName, value);
            }
        }

        public static TVal Get<TKey, TVal>(this IDictionary<TKey, TVal> dict, TKey key, TVal defaultValue)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }

            return defaultValue;
        }

        /// <summary>
        /// Load object of defined type from IDictionary&lt;string, string&gt;
        /// </summary>
        /// <typeparam name="T">Defined object type</typeparam>
        /// <param name="dict">Dictionary to load</param>
        /// <param name="attributeName">Attribute name</param>
        /// <returns>Loaded object</returns>
        public static T Get<T>(this IDictionary<string, string> dict, string attributeName) where T : class
        {
            if (dict.ContainsKey(attributeName))
            {
                return ExtractClassValue<T>(dict[attributeName]);
            }
            
            return null;
        }

        /// <summary>
        /// Load value object of defined type from IDictionary&lt;string, object&gt;
        /// </summary>
        /// <typeparam name="T">Defined object type</typeparam>
        /// <param name="dict">Dictionary to load</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Loaded object or default value</returns>
        public static T Get<T>(this IDictionary<string, string> dict, string attributeName, T defaultValue) where T : struct
        {
            if (dict.ContainsKey(attributeName))
            {
                var value = dict[attributeName];

                if (value == null)
                {
                    return defaultValue;
                }

                var conv = TypeDescriptor.GetConverter(typeof(T));
                if (conv == null)
                {
                    return defaultValue;
                }

                if (conv.CanConvertFrom(value.GetType()))
                {
                    try
                    {
                        var converted = conv.ConvertFrom(value);
                        if (converted != null)
                        {
                            return (T)converted;
                        }    
                    }
                    catch (Exception)
                    {
                        return defaultValue;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>Load boolean value from <c>IDictionary&lt;string, string&gt;</c></summary>
        /// <param name="dict">The dictionary</param>
        /// <param name="attributeName">The attribute name.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Parsed boolean</returns>
        public static bool Get(this Dictionary<string, string> dict, string attributeName, bool defaultValue)
        {
            if (dict.ContainsKey(attributeName))
            {
                var value = dict[attributeName];
                if (value == null)
                {
                    return defaultValue;
                }

                return String.Compare(value, Boolean.TrueString, true) == 0;
            }

            return defaultValue;
        }

        /// <summary>Extract object with conversion if possible</summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T">Output object type</typeparam>
        /// <returns>Extracted object</returns>
        private static T ExtractClassValue<T>(object value) 
            where T : class
        {
            if (value == null)
            {
                return null;
            }

            if (value is T)
            {
                return value as T;
            }

            var conv = TypeDescriptor.GetConverter(typeof(T));
            if (conv == null)
            {
                return null;
            }

            if (conv.CanConvertFrom(value.GetType()))
            {
                return (T)conv.ConvertFrom(_commonTypeDescriptorInstance, CultureInfo.InvariantCulture, value);
            }

            return null;
        }

        /// <summary>Extract object with conversion if possible</summary>
        /// <param name="value">The value.</param>
        /// <typeparam name="T">Output object type</typeparam>
        /// <returns>Extracted object</returns>
        private static T[] ExtractArray<T>(object value)
            where T : struct
        {
            if (value == null)
            {
                return null;
            }

            if (value is T[])
            {
                return value as T[];
            }

            if (value is object[])
            {
                var elemConv = TypeDescriptor.GetConverter(typeof(T));
                if (elemConv == null)
                {
                    return null;
                }

                var converted = Array.ConvertAll<object, T>(
                                value as object[],
                                p => (T)elemConv.ConvertFrom(_commonTypeDescriptorInstance, CultureInfo.InvariantCulture, p));

                return converted;
            }

            return null;
        }

        /// <summary>Extract string value</summary>
        /// <param name="value">The value.</param>
        /// <returns>String value</returns>
        private static string ExtractStringValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string)
            {
                return value as string;
            }
            
            if (value is byte)
            {
                return ((byte)value).ToString(NumberFormatInfo.InvariantInfo);
            }

            if (value is short)
            {
                return ((short)value).ToString(NumberFormatInfo.InvariantInfo);
            }

            if (value is int)
            {
                return ((int)value).ToString(NumberFormatInfo.InvariantInfo);
            }

            if (value is long)
            {
                return ((long)value).ToString(NumberFormatInfo.InvariantInfo);
            }

            if (value is double)
            {
                return ((double)value).ToString(SerializationFloatNumberFormat, NumberFormatInfo.InvariantInfo);
            }

            if (value is DateTime)
            {
                return ((DateTime)value).ToString(SerializationDateFormat, CultureInfo.InvariantCulture);
            }

            return value.ToString();
        }

        private class CommonTypeDescriptorContext : ITypeDescriptorContext
        {
            #region Implementation of IServiceProvider

            /// <summary>
            /// Gets the service object of the specified type.
            /// </summary>
            /// <returns>
            /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
            /// </returns>
            /// <param name="serviceType">An object that specifies the type of service object to get. </param><filterpriority>2</filterpriority>
            public object GetService(Type serviceType)
            {
                return null;
            }

            #endregion

            #region Implementation of ITypeDescriptorContext

            /// <summary>
            /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanging"/> event.
            /// </summary>
            /// <returns>
            /// true if this object can be changed; otherwise, false.
            /// </returns>
            public bool OnComponentChanging()
            {
                return true;
            }

            /// <summary>
            /// Raises the <see cref="E:System.ComponentModel.Design.IComponentChangeService.ComponentChanged"/> event.
            /// </summary>
            public void OnComponentChanged()
            {
            }

            /// <summary>
            /// Gets the container representing this <see cref="T:System.ComponentModel.TypeDescriptor"/> request.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.ComponentModel.IContainer"/> with the set of objects for this <see cref="T:System.ComponentModel.TypeDescriptor"/>; otherwise, null if there is no container or if the <see cref="T:System.ComponentModel.TypeDescriptor"/> does not use outside objects.
            /// </returns>
            public IContainer Container
            {
                get { return null; }
            }

            /// <summary>
            /// Gets the object that is connected with this type descriptor request.
            /// </summary>
            /// <returns>
            /// The object that invokes the method on the <see cref="T:System.ComponentModel.TypeDescriptor"/>; otherwise, null if there is no object responsible for the call.
            /// </returns>
            public object Instance
            {
                get { return null;  }
            }

            /// <summary>
            /// Gets the <see cref="T:System.ComponentModel.PropertyDescriptor"/> that is associated with the given context item.
            /// </summary>
            /// <returns>
            /// The <see cref="T:System.ComponentModel.PropertyDescriptor"/> that describes the given context item; otherwise, null if there is no <see cref="T:System.ComponentModel.PropertyDescriptor"/> responsible for the call.
            /// </returns>
            public PropertyDescriptor PropertyDescriptor
            {
                get { return null; }
            }

            #endregion
        }
    }
}
