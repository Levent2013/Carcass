using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Carcass.Resources;

namespace Carcass.Infrastructure
{
    /// <summary>
    /// Carcass configuration section
    /// </summary>
    public class ApplicationConfigurationSection : ConfigurationSection
    {
        private readonly AssemblyName _siteAssemblyName;

        public ApplicationConfigurationSection()
        {
            var assembly = typeof(ApplicationConfigurationSection).Assembly;
            _siteAssemblyName = assembly.GetName();
        }

        /// <summary>
        /// Gets the configuration section instance
        /// </summary>
        public static ApplicationConfigurationSection GetConfig()
        {
            return (ApplicationConfigurationSection)ConfigurationManager.GetSection("application");
        }

        public Version SiteAssemblyVersion
        {
            get { return _siteAssemblyName.Version; }
        }

        [ConfigurationProperty("defaultAdminProvider", IsRequired = true, DefaultValue = "google")]
        public string DefaultAdminProvider
        {
            get { return (string)this["defaultAdminProvider"]; }
            set { this["defaultAdminProvider"] = value; }
        }

        [ConfigurationProperty("defaultAdminUserName", IsRequired = true, DefaultValue = "artem.kustikov@gmail.com")]
        public string DefaultAdminUserName
        {
            get { return (string)this["defaultAdminUserName"]; }
            set { this["defaultAdminUserName"] = value; }
        }

        [ConfigurationProperty("passwordGenerator", IsRequired = true)]
        public PasswordGeneratorElement PasswordGenerator
        {
            get { return (PasswordGeneratorElement)this["passwordGenerator"]; }
            set { this["passwordGenerator"] = value; }
        }
    }

    /// <summary>
    /// Complex element example
    /// </summary>
    public class PasswordGeneratorElement : ConfigurationElement
    {
        [ConfigurationProperty("passwordLength", IsRequired = true, DefaultValue = 1)]
        [IntegerValidator(MinValue = 1, MaxValue = 128)]
        public int PasswordLength
        {
            get { return (int)this["passwordLength"]; }
            set { this["passwordLength"] = value; }
        }

        [ConfigurationProperty("alphabet", IsRequired = true, DefaultValue = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=[{]};:>|./?")]
        public string Alphabet
        {
            get { return (string)this["alphabet"]; }
            set { this["alphabet"] = value; }
        }
    }
}
