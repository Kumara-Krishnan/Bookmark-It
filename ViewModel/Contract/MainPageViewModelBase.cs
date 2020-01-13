using Bookmark_It.View.Contract;
using BookmarkItLibrary.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPUtilities.View.Contract;

namespace Bookmark_It.ViewModel.Contract
{
    internal abstract class MainPageViewModelBase : ViewModelBase<IMainPageView>
    {
        public MainPageViewModelBase() : base() { }

        public abstract void GetLoggedInUserDetails();

        public abstract void GetRequestToken();

        public abstract void GetUserDetails(string requestToken);

        public abstract void GetBookmarks(UserDetails user);
    }
}
