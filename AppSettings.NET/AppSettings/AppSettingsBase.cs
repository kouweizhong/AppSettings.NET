﻿using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Caching;

namespace AppSettings.NET
{
    internal abstract class AppSettingsBase
    {
        private string IsCachekey = "APPSETTINGSBASE_IsCache";

        protected string XmlPath
        {
            get
            {
                string file = ConfigurationManager.AppSettings["AppSettingsPath"];
                if (string.IsNullOrEmpty(file))
                {
                    throw new ConfigurationErrorsException("自定义配置文件配置AppSettingsPath未找到");
                }
                if (!File.Exists(file))
                {
                    throw new ConfigurationErrorsException("自定义配置文件不存在");
                }
                return file; 
            }
        }

        protected bool IsCache()
        {
            if (HttpRuntime.Cache[IsCachekey] == null)
            {
                var doc = XDocument.Load(XmlPath);
                var appSettings = doc.Elements().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase("AppSettings"));

                var isCache = false;
                if (appSettings != null)
                {
                    var attributes = appSettings.Attributes().FirstOrDefault(s => s.Name.LocalName.EqualsIgnoreCase("Cache"));
                    if (attributes != null)
                    {
                        bool.TryParse(attributes.Value, out isCache);
                    }
                }

                var cdd = new CacheDependency(XmlPath); 
                HttpRuntime.Cache.Insert(IsCachekey, isCache, cdd, DateTime.MaxValue, System.Web.Caching.Cache.NoSlidingExpiration);

                return isCache;
            }
            return (bool)HttpRuntime.Cache[IsCachekey];
        }
    }
}
