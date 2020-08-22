using Bookmark_It.DI;
using Bookmark_It.View.Contract;
using Bookmark_It.ViewModel.Contract;
using BookmarkItCommonLibrary.Data.Handler.Contract;
using BookmarkItCommonLibrary.Data.Parser;
using BookmarkItCommonLibrary.Model.Entity;
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
using BookmarkItCommonLibrary.Util;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel;

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

        public async Task AuthenticateUser(string requestToken)
        {
            await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.SilentMode,
                new Uri(CommonConstants.PocketLogoutUrl), WebAuthenticationBroker.GetCurrentApplicationCallbackUri());
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
            ProfilePicUri.UriSource = new Uri(userDetails.AvatarUrl, UriKind.RelativeOrAbsolute);
            ProfilePic.DisplayName = userDetails.DisplayName;
            UserName.Text = userDetails.UserName;
            //VM.SyncBookmarks(userDetails.Id);
        }

        public void OnUserAuthenticationFailed()
        {

        }

        public void UpdateUserSyncStatus(UserDetails user)
        {
            if (user.IsInitialFetchComplete)
            {
                UserName.Text = $"{user.UserName} \n Last synced at {DateTimeOffset.FromUnixTimeSeconds(user.LastSyncedTime).DateTime.ToString()}";
            }
            else
            {
                var loadedItems = user.InitialFetchOffset <= user.ItemsCount ? user.InitialFetchOffset : user.ItemsCount;
                UserName.Text = $"{loadedItems} of {user.ItemsCount} items";
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        private async void Page_Loading(FrameworkElement sender, object args)
        {
            VM.View = this;
            VM.GetLoggedInUserDetails();
        }

        private async void OnForceSyncClicked(object sender, RoutedEventArgs e)
        {
            if (AppManager.Instance.CurrentUser == null) { return; }
            if (ApiInformation.IsApiContractPresent(typeof(FullTrustAppContract).FullName, 1, 0))
            {
                if (App.AppServiceConnection == null)
                {
                    await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                }
            }
            await Task.Delay(2000);
            await App.InitiateSync(AppManager.Instance.CurrentUser.Id);
        }
    }
}
