using Bookmark_It.ViewModel.Contract;
using BookmarkItLibrary.Data.Handler.Contract;
using BookmarkItLibrary.Domain;
using BookmarkItLibrary.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWPUtilities.Extension;
using UWPUtilities.UseCase;

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
            var request = new GetRequestTokenRequest();
            var getRequestTokenUC = new GetRequestToken(request, new GetRequestTokenCallback(this));
            getRequestTokenUC.Execute();
        }

        public override void GetUserDetails(string requestToken)
        {
            var request = new GetUserDetailsRequest(RequestType.Network, userId: default, requestToken, setAsCurrentUser: true);
            var getUserDetailsUC = new GetUserDetails(request, new GetUserDetailsCallback(this));
            getUserDetailsUC.Execute();
        }

        public override void GetBookmarks(UserDetails user)
        {
            var request = new GetBookmarksRequest(RequestType.Sync, user.LastSyncedTime, user.Id);
            var getBookmarksUC = new GetBookmarks(request, new GetBookmarksCallback(this));
            getBookmarksUC.Execute();
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

        class GetBookmarksCallback : IGetBookmarksPresenterCallback
        {
            private readonly MainPageViewModel Presenter;

            public GetBookmarksCallback(MainPageViewModel presenter)
            {
                Presenter = presenter;
            }

            public void OnCanceled(IUseCaseResponse<GetBookmarksResponse> response)
            {

            }

            public void OnError(UseCaseError error)
            {

            }

            public void OnFailed(IUseCaseResponse<GetBookmarksResponse> response)
            {

            }

            public void OnSuccess(IUseCaseResponse<GetBookmarksResponse> response)
            {
                _ = Presenter.View.RunOnUIThread(() =>
                {
                    if (response.Type == ResponseType.Network || response.Type == ResponseType.Sync)
                    {
                        Presenter.Bookmarks.Clear();
                    }
                    response.Data.Bookmarks.ForEach(Presenter.Bookmarks.Add);
                });
            }
        }
    }
}
