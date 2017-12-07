using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace DataSpecTester
{
    /// <summary>
    /// 测试插件配置辅助
    /// </summary>
    public class TesterPlugConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TesterPlugConfig"/> class.
        /// </summary>
        private TesterPlugConfig()
        {
            PlugInDirectory = "PlugIn";
        }

        private static TesterPlugConfig _config = null;

        public static TesterPlugConfig Instance
        {
            get
            {
                if (_config == null)
                {
                    _config = new TesterPlugConfig();
                }
                return _config;
            }
        }

        internal List<Type> PlugTypes = new List<Type>();

        List<Assembly> avalibleAsm = new List<Assembly>();

        /// <summary>
        /// 加载插件配置
        /// </summary>
        /// <returns></returns>
        public TesterPlugConfig Config()
        {
            string[] pluginFIles = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PlugInDirectory),
                "*.dll", SearchOption.TopDirectoryOnly);

            foreach (string plug in pluginFIles)
            {
                //Trace.Write(plug);
                byte[] asmBytes = File.ReadAllBytes(plug);
                Assembly asm = null;

                try
                {
                    string pdbFile = Path.ChangeExtension(plug, ".pdb");
                    if (File.Exists(pdbFile))
                    {
                        asm = Assembly.Load(asmBytes, File.ReadAllBytes(pdbFile));
                    }
                    else
                    {
                        asm = Assembly.Load(asmBytes);
                    }
                }
                catch(Exception) { }

                if (asm != null)
                {
                    foreach (Type innerType in asm.GetTypes())
                    {
                        if (innerType.IsPublic && !innerType.IsAbstract && innerType.GetInterface(typeof(ITesterPlug).FullName, false) != null)
                        {
                            PlugTypes.Add(innerType);
                            if (_currentPlug == null)
                            {
                                _currentPlug = Activator.CreateInstance(innerType) as ITesterPlug;
                            }
                        }
                    }
                   avalibleAsm.Add(asm);
                }

            }
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            return this;
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //System.Windows.Forms.MessageBox.Show(args.Name + "@" + avalibleAsm.Count);
            //Gwsoft.EaseMode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
            return avalibleAsm.Find(m => {
                return m.FullName.Equals(args.Name);
            });
        }

        /// <summary>
        /// 重新配置
        /// </summary>
        /// <returns></returns>
        public TesterPlugConfig Reload()
        {
            return Config();
        }


        private ITesterPlug _currentPlug = null;
        /// <summary>
        /// 当前解析与测试插件
        /// </summary>
        public ITesterPlug CurrentPlug
        {
            get
            {
                return _currentPlug;
            }
        }

        /// <summary>
        /// 插件目录
        /// </summary>
        public string PlugInDirectory
        {
            get;
            private set;
        }

    }
}
