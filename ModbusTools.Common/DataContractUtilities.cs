using System.IO;
using System.Runtime.Serialization;

namespace ModbusTools.Common
{
    /// <summary>
    /// Utilties for dealing with data contracts.
    /// </summary>
    public static class DataContractUtilities
    {
        /// <summary>
        /// Serializes a data contract to disk.
        /// </summary>
        /// <typeparam name="TDataContract"></typeparam>
        /// <param name="path"></param>
        /// <param name="graph"></param>
        public static void ToFile<TDataContract>(string path, TDataContract graph) 
            where TDataContract : class 
        {
            using (var stream = File.Create(path))
            {
                ToStream(stream, graph);
            }
        }

        /// <summary>
        /// Serializes a data contract to a stream.
        /// </summary>
        /// <typeparam name="TDataContract"></typeparam>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        public static void ToStream<TDataContract>(Stream stream, TDataContract graph)
             where TDataContract : class 
        {
            var serializer = new DataContractSerializer(typeof(TDataContract));

            serializer.WriteObject(stream, graph);
        }

        /// <summary>
        /// Reads a data contract from a file.
        /// </summary>
        /// <typeparam name="TDataContract"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static TDataContract FromFile<TDataContract>(string path)
            where TDataContract : class 
        {
            using (var stream = File.OpenRead(path))
            {
                return FromStream<TDataContract>(stream);
            }
        }

        /// <summary>
        /// Reads a data contract from a stream.
        /// </summary>
        /// <typeparam name="TDataContract"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static TDataContract FromStream<TDataContract>(Stream stream)
            where TDataContract : class 
        {
            var serializer = new DataContractSerializer(typeof(TDataContract));

            return (TDataContract)serializer.ReadObject(stream);
        }

    }
}
