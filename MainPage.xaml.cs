using Bookmark_It.DI;
using Bookmark_It.View.Contract;
using Bookmark_It.ViewModel.Contract;
using BookmarkItLibrary.Data.Handler.Contract;
using BookmarkItLibrary.Data.Parser;
using BookmarkItLibrary.Model.Entity;
using BookmarkItLibrary.Util;
using System;
using System.Threading.Tasks;
using UWPUtilities.Util;
using Windows.Security.Authentication.Web;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bookmark_It
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IMainPageView
    {
        private readonly MainPageViewModelBase VM;
        public MainPage()
        {
            VM = DIServiceProvider.Instance.GetService<MainPageViewModelBase>();
            this.InitializeComponent();
            VM.View = this;
        }

        public void Dispose()
        {
            VM.Dispose();
        }

        private void Page_Loading(Windows.UI.Xaml.FrameworkElement sender, object args)
        {
            VM.View = this;
            VM.GetLoggedInUserDetails();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        public async Task AuthenticateUser(string requestToken)
        {
            var requestUri = LibUtil.GetSignInUrl(requestToken);
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, requestUri);
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                VM.GetUserDetails(requestToken);
            }
        }

        public void OnUserAuthenticationSuccess(UserDetails userDetails)
        {
            UserName.Text = userDetails.UserName;
        }

        public void OnUserAuthenticationFailed()
        {

        }
    }
}
