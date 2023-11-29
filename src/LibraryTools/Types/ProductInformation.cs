using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTools.Types
{
    public class ProductInformation
    {
        public ProductInformation()
        {
            var assembly = Assembly.GetEntryAssembly();

            var version = assembly?.GetName()?.Version?.ToString();
            var informationVersion = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var productName = assembly?.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

            Version = version;
            InformationalVersion = informationVersion;
            Name = productName;
        }

        public string Version { get; set; }
        public string InformationalVersion { get; set; }
        public string Name { get; set; }
    }
}
