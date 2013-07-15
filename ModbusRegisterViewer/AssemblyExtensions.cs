using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModbusRegisterViewer
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns the content of the specified resource stream.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ReadManifestResourceStream(this Assembly assembly, string name)
        {
            using (var file = assembly.GetManifestResourceStream(name))
            {
                if (file == null)
                    throw new FileNotFoundException(string.Format("Unable to find manifest resource stream '{0}'", name), name);

                var reader = new StreamReader(file);

                return reader.ReadToEnd();
            }
        }
    }
}
