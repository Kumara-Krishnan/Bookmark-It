using Bookmark_It.ViewModel.Contract;
using BookmarkItCommonLibrary.Data.Handler.Contract;
using BookmarkItCommonLibrary.DI;
using BookmarkItCommonLibrary.Domain;
using BookmarkItCommonLibrary.Model;
using BookmarkItCommonLibrary.Model.Entity;
//using BookmarkItSyncLibrary.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extension;
using Utilities.UseCase;
using UWPUtilities.Extension;
using Windows.Security.Authentication.Web;
using Windows.Storage;

namespace Bookmark_It.ViewModel
{
    internal class MainPageViewModel : MainPageViewModelBase
    {
        public override void GetLoggedInUserDetails()
        {
            var request = new GetCurrentUserDetailsRequest();
            var getLoggedInUserUC = new GetCurrentUserDetails(request, new GetLoggedInUserDetailsCallback(this));
            getLoggedInUserUC.Execute();
        }

        public override void GetRequestToken()
        {
            var request = new GetRequestTokenRequest(WebAuthenticationBroker.GetCurrentApplicationCallbackUri());
            var getRequestTokenUC = new GetRequestToken(request, new GetRequestTokenCallback(this));
            getRequestTokenUC.Execute();
        }

        public override void GetUserDetails(string requestToken)
        {
            var request = new GetUserDetailsRequest(RequestType.Network, userId: default, requestToken, setAsCurrentUser: true);
            var getUserDetailsUC = new GetUserDetails(request, new GetUserDetailsCallback(this));
            getUserDetailsUC.Execute();
        }

        public override void SyncBookmarks(string userId)
        {
            //var request = new SyncBookmarksRequest(userId);
            //var getBookmarksUC = new SyncBookmarks(request, new SyncBookmarksCallback(this));
            //getBookmarksUC.Execute();
        }

        public override async void AddBookmarks(string userId)
        {
            try
            {
                var netHandler = CommonDIServiceProvider.Instance.GetService<INetHandler>();
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/1lacurls.csv"));
                var lines = await FileIO.ReadLinesAsync(file);
                var urls = lines.Select(l => new BookmarkBObj() { Url = $"https://{l.Split(',')[1]}" }).ToList();
                //var jRootBookmarks = JObject.Parse(content);
                //var urls = ExtractUrls(jRootBookmarks).Distinct();                
                //foreach (var url in urls)
                //{
                //    if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri) && uri.Scheme == "http" || uri.Scheme == "https")
                //    {
                //        var response = await netHandler.AddBookmarkAsync(userId, uri.ToString());
                //    }
                //    else
                //    {

                //    }
                //}
                var skip = 0;
                var limit = 100;
                do
                {
                    try
                    {
                        var batch = urls.Skip(skip).Take(limit);
                        var response = await netHandler.AddBookmarksAsync(userId, batch);
                        skip += limit;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                while (skip <= urls.Count);

            }
            catch (Exception ex)
            {

            }
        }

        private IEnumerable<string> ExtractUrls(JToken jRootBookmarks)
        {
            List<string> urls = new List<string>();
            if (jRootBookmarks is JObject jObj)
            {
                if (jObj.ContainsKey("children"))
                {
                    urls.AddRange(ExtractUrls(jRootBookmarks.GetJArray("children")));
                }
                else if (jObj.ContainsKey("uri"))
                {
                    urls.Add(jObj.GetString("uri"));
                }
            }
            else if (jRootBookmarks is JArray jArr)
            {
                foreach (var jElement in jArr)
                {
                    urls.AddRange(ExtractUrls(jElement));
                }
            }
            return urls;
        }

        class GetLoggedInUserDetailsCallback : IGetCurrentUserDetailsPresenterCallback
        {
            private readonly MainPageViewModel Presenter;

            public GetLoggedInUserDetailsCallback(MainPageViewModel presenter)
            {
                Presenter = presenter;
            }

            public void OnCanceled(IUseCaseResponse<GetCurrentUserDetailsResponse> response)
            {

            }

            public void OnError(UseCaseError error)
            {
                //TODO: Error handling
            }

            public void OnFailed(IUseCaseResponse<GetCurrentUserDetailsResponse> response)
            {
                Presenter.GetRequestToken();
            }

            public void OnSuccess(IUseCaseResponse<GetCurrentUserDetailsResponse> response)
            {
                _ = Presenter.View.RunOnUIThread(() =>
                {
                    Presenter.View.OnUserAuthenticationSuccess(response.Data.User);
                });
            }
        }

        class GetRequestTokenCallback : IGetRequestTokenPresenterCallback
        {
            private readonly MainPageViewModel Presenter;

            public GetRequestTokenCallback(MainPageViewModel presenter)
            {
                Presenter = presenter;
            }

            public void OnCanceled(IUseCaseResponse<GetRequestTokenResponse> response)
            {

            }

            public void OnError(UseCaseError error)
            {

            }

            public void OnFailed(IUseCaseResponse<GetRequestTokenResponse> response)
            {

            }

            public void OnSuccess(IUseCaseResponse<GetRequestTokenResponse> response)
            {
                _ = Presenter.View.RunOnUIThread(() =>
                {
                    Presenter.View.AuthenticateUser(response.Data.RequestToken);
                });
            }
        }

        class GetUserDetailsCallback : IGetUserDetailsPresenterCallback
        {
            private readonly MainPageViewModel Presenter;

            public GetUserDetailsCallback(MainPageViewModel presenter)
            {
                Presenter = presenter;
            }

            public void OnCanceled(IUseCaseResponse<GetUserDetailsResponse> response)
            {

            }

            public void OnError(UseCaseError error)
            {

            }

            public void OnFailed(IUseCaseResponse<GetUserDetailsResponse> response)
            {

            }

            public void OnSuccess(IUseCaseResponse<GetUserDetailsResponse> response)
            {
                _ = Presenter.View.RunOnUIThread(() =>
                {
                    Presenter.View.OnUserAuthenticationSuccess(response.Data.User);
                });
            }
        }

        //class SyncBookmarksCallback : ISyncBookmarksPresenterCallback
        //{
        //    private readonly MainPageViewModel Presenter;

        //    public SyncBookmarksCallback(MainPageViewModel presenter)
        //    {
        //        Presenter = presenter;
        //    }

        //    public void OnCanceled(IUseCaseResponse<SyncBookmarksResponse> response)
        //    {

        //    }

        //    public void OnError(UseCaseError error)
        //    {

        //    }

        //    public void OnFailed(IUseCaseResponse<SyncBookmarksResponse> response)
        //    {

        //    }

        //    public void OnProgress(IUseCaseResponse<SyncBookmarksResponse> response)
        //    {
        //        _ = Presenter.View.RunOnUIThread(() =>
        //        {
        //            Presenter.View.UpdateUserSyncStatus(response.Data.User);
        //        });
        //    }

        //    public void OnSuccess(IUseCaseResponse<SyncBookmarksResponse> response)
        //    {
        //        _ = Presenter.View.RunOnUIThread(() =>
        //        {
        //            Presenter.View.UpdateUserSyncStatus(response.Data.User);
        //        });
        //    }
        //}
    }
}
