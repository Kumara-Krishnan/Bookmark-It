using BookmarkItLibrary.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark_It
{
    public sealed class AppManager
    {
        public static AppManager Instance { get { return AppManagerSingleton.Instance; } }

        public UserDetails CurrentUser { get; set; }

        private AppManager() { }

        private class AppManagerSingleton
        {
            static AppManagerSingleton() { }

            internal static readonly AppManager Instance = new AppManager();
        }
    }
}
