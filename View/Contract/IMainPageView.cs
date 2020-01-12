using BookmarkItLibrary.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPUtilities.View.Contract;

namespace Bookmark_It.View.Contract
{
    internal interface IMainPageView : IView
    {
        Task AuthenticateUser(string requestToken);

        void OnUserAuthenticationSuccess(UserDetails userDetails);

        void OnUserAuthenticationFailed();
    }
}
