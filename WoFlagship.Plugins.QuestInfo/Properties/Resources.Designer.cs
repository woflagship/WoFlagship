﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WoFlagship.Plugins.QuestInfo.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WoFlagship.Plugins.QuestInfo.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   使用此强类型资源类，为所有资源查找
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 S胜利6次, BOSS战胜利12次, 进行BOSS战24次, 出击36次(一次出击多次战斗算一次) 的本地化字符串。
        /// </summary>
        internal static string agou_format {
            get {
                return ResourceManager.GetString("agou_format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {Ship:Ship()} {Select:Select()};  的本地化字符串。
        /// </summary>
        internal static string group_format {
            get {
                return ResourceManager.GetString("group_format", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 获得A胜或以上 的本地化字符串。
        /// </summary>
        internal static string result_a {
            get {
                return ResourceManager.GetString("result_a", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 获得B胜或以上 的本地化字符串。
        /// </summary>
        internal static string result_b {
            get {
                return ResourceManager.GetString("result_b", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 获得C败或以上 的本地化字符串。
        /// </summary>
        internal static string result_c {
            get {
                return ResourceManager.GetString("result_c", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 完成全图 的本地化字符串。
        /// </summary>
        internal static string result_clear {
            get {
                return ResourceManager.GetString("result_clear", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 获得S胜 的本地化字符串。
        /// </summary>
        internal static string result_s {
            get {
                return ResourceManager.GetString("result_s", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 任意舰 的本地化字符串。
        /// </summary>
        internal static string ship_any {
            get {
                return ResourceManager.GetString("ship_any", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 未知舰种 的本地化字符串。
        /// </summary>
        internal static string ship_unknown {
            get {
                return ResourceManager.GetString("ship_unknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 {MapStr}{Boss:Boss战|出击}{Result:Result()}{Times}次; {Groups:Group()}} 的本地化字符串。
        /// </summary>
        internal static string sortie_format {
            get {
                return ResourceManager.GetString("sortie_format", resourceCulture);
            }
        }
    }
}
