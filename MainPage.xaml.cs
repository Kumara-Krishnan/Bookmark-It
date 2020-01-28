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
using Windows.UI.Xaml;
using Windows.System;
using Windows.Web.Http.Filters;

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
            //VM.GetLoggedInUserDetails();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        public async Task AuthenticateUser(string requestToken)
        {
            await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.SilentMode, new Uri("https://getpocket.com/lo"), WebAuthenticationBroker.GetCurrentApplicationCallbackUri());
            var requestUri = LibUtil.GetSignInUrl(requestToken, isWebAuthenticationBroker: true);
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, requestUri);
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                VM.GetUserDetails(requestToken);
            }
        }

        public void OnUserAuthenticationSuccess(UserDetails userDetails)
        {
            AppManager.Instance.CurrentUser = userDetails;
            UserName.Text = userDetails.UserName;
            VM.GetBookmarks(userDetails);
        }

        public void OnUserAuthenticationFailed()
        {

        }

        private void AuthenticationWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.AbsoluteUri.ToString() == "https://twitter.com/login")
            {
                var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
                var cookies = httpBaseProtocolFilter.CookieManager.GetCookies(new Uri("https://twitter.com"));
                foreach (var cookie in cookies)
                {
                    System.Diagnostics.Debug.WriteLine($"{cookie.Name}: {cookie.Value}");
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            AuthenticationWebView.WebResourceRequested += OnWebResourceRequested;
            AuthenticationWebView.Navigate(new Uri("https://twitter.com/login"));
        }

        private void OnWebResourceRequested(WebView sender, WebViewWebResourceRequestedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Request.RequestUri.ToString());
        }

        private void AuthenticationWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine(args.Uri.ToString());
        }
    }
}
