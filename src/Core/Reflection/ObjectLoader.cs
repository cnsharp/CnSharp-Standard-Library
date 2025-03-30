using System;
using System.Linq;
using System.Reflection;

namespace CnSharp.Reflection
{
    /// <summary>
    ///     Object loader from assembly by reflection
    /// </summary>
    public class ObjectLoader
    {
        /// <summary>
        ///     Create an object from an assembly
        /// </summary>
        /// <typeparam name="TObject">Return type</typeparam>
        /// <returns></returns>
        public static TObject CreateObject<TObject>() where TObject : class
        {
            var type = typeof(TObject);

            return CreateObject<TObject>(type.FullName);
        }

        /// <summary>
        ///     Create an object by type name
        /// </summary>
        /// <typeparam name="TObject">Type</typeparam>
        /// <param name="typeName">Type name</param>
        /// <returns></returns>
        public static TObject CreateObject<TObject>(string typeName) where TObject : class
        {
            TObject res = null;

            res = GetAssemblyByType(typeName).CreateInstance(typeName) as TObject;

            return res;
        }

        /// <summary>
        ///     Create an object by type name
        /// </summary>
        /// <typeparam name="TObject">Type</typeparam>
        /// <param name="typeName">Type name</param>
        /// <param name="parameters">Constructor parameters</param>
        /// <returns></returns>
        public static TObject CreateObject<TObject>(string typeName, params object[] parameters) where TObject : class
        {
            TObject res = null;

            res =
                GetAssemblyByType(typeName)
                        .CreateInstance(typeName, true, BindingFlags.CreateInstance, null, parameters, null, null) as
                    TObject;

            return res;
        }

        /// <summary>
        ///     Create an object from an assembly
        /// </summary>
        /// <typeparam name="TObject">Return type</typeparam>
        /// <param name="typeName">Reflection type name</param>
        /// <param name="assemblyName">Assembly name</param>
        /// <returns></returns>
        public static TObject CreateObject<TObject>(string typeName, string assemblyName) where TObject : class
        {
            TObject res = null;

            res = GetAssembly(assemblyName).CreateInstance(typeName) as TObject;

            return res;
        }

        /// <summary>
        ///     Get assembly by type name
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByType(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(v => v.GetType(typeName) != null);
        }

        /// <summary>
        ///     Get assembly
        /// </summary>
        /// <param name="assemblyName">Assembly name</param>
        /// <returns></returns>
        public static Assembly GetAssembly(string assemblyName)
        {
            Assembly assembly = null;

            if (assemblyName.ToUpper().EndsWith("DLL") || assemblyName.ToUpper().EndsWith("EXE"))
            {
                try
                {
                    assembly = Assembly.LoadFile(assemblyName);
                }
                catch
                {
                    assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + assemblyName);
                }
            }
            else
            {
                assembly = Assembly.Load(assemblyName);
            }

            return assembly;
        }

        /// <summary>
        ///     Get a type
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <param name="assemblyName">Assembly containing the type</param>
        /// <returns></returns>
        public static Type GetType(string typeName, string assemblyName)
        {
            return GetAssembly(assemblyName).GetType(typeName);
        }
    }
}
