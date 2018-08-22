using System;
using System.Linq;
using System.Reflection;

namespace CnSharp.Reflection
{
    /// <summary>
    ///     object loader from assembly by reflection
    /// </summary>
    public class ObjectLoader
    {
        /// <summary>
        ///     * 从程序集生成对象
        /// </summary>
        /// <typeparam name="TObject">返回类型</typeparam>
        /// <returns></returns>
        public static TObject CreateObject<TObject>() where TObject : class
        {
            var type = typeof (TObject);

            return CreateObject<TObject>(type.FullName);
        }

        /// <summary>
        ///     * 根据类型名称生成对象
        /// </summary>
        /// <typeparam name="TObject">类型</typeparam>
        /// <param name="typeName">类型名称</param>
        /// <returns></returns>
        public static TObject CreateObject<TObject>(string typeName) where TObject : class
        {
            TObject res = null;

            res = GetAssemblyByType(typeName).CreateInstance(typeName) as TObject;

            return res;
        }

        /// <summary>
        ///     * 根据类型名称生成对象
        /// </summary>
        /// <typeparam name="TObject">类型</typeparam>
        /// <param name="typeName">类型名称</param>
        /// <param name="parameters">构造函数参数</param>
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
        ///     * 从程序集生成对象
        /// </summary>
        /// <typeparam name="TObject">返回类型</typeparam>
        /// <param name="typeName">反射类型名称</param>
        /// <param name="assemblyName">程序集名称</param>
        /// <returns></returns>
        public static TObject CreateObject<TObject>(string typeName, string assemblyName) where TObject : class
        {
            TObject res = null;

            res = GetAssembly(assemblyName).CreateInstance(typeName) as TObject;

            return res;
        }

        /// <summary>
        ///     * 根据类型获得程序集
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <returns></returns>
        public static Assembly GetAssemblyByType(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(v => v.GetType(typeName) != null);
        }

        /// <summary>
        ///     * 获得程序集
        /// </summary>
        /// <param name="assemblyName">程序集名称</param>
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
        ///     * 获得一个类型
        /// </summary>
        /// <param name="typeName">类型名称</param>
        /// <param name="assemblyName">类型所在程序集</param>
        /// <returns></returns>
        public static Type GetType(string typeName, string assemblyName)
        {
            return GetAssembly(assemblyName).GetType(typeName);
        }
    }
}