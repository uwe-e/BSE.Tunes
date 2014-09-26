using BSE.Tunes.StoreApp.Services;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSE.Tunes.StoreApp.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IResourceService, ResourceService>();
            SimpleIoc.Default.Register<IDataService, DataService>();
            SimpleIoc.Default.Register<IHostsettingsService, HostsettingsService>();
            SimpleIoc.Default.Register<IAccountService, AccountService>();
            SimpleIoc.Default.Register<IPlayerService, PlayerService>();
            SimpleIoc.Default.Register<ICacheableBitmapService, CacheableBitmapService>();

            SimpleIoc.Default.Register<BSE.Tunes.StoreApp.Managers.PlayerManager>();
            SimpleIoc.Default.Register<PlayerUserControlViewModel>();
            SimpleIoc.Default.Register<PlayerBarUserControlViewModel>();
            SimpleIoc.Default.Register<AlbumsPageViewModel>();
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<PlaylistDetailPageViewModel>();
            SimpleIoc.Default.Register<AlbumDetailPageViewModel>();
            SimpleIoc.Default.Register<AlbumsGroupUserControlViewModel>();
            SimpleIoc.Default.Register<HostSettingsPageViewModel>();
            SimpleIoc.Default.Register<SignInSettingsPageViewModel>();
            SimpleIoc.Default.Register<TopAppBarUserControlViewModel>();
            SimpleIoc.Default.Register<RandomPlayerUserControlViewModel>();
            SimpleIoc.Default.Register<PlaylistGroupUserControlViewModel>();
            SimpleIoc.Default.Register<SearchUserControlViewModel>();
            SimpleIoc.Default.Register<SearchResultPageViewModel>();
            SimpleIoc.Default.Register<SearchCategoryTracksPageViewModel>();
            SimpleIoc.Default.Register<SearchCategoryAlbumsPageViewModel>();
            SimpleIoc.Default.Register<PlaylistPageViewModel>();
            SimpleIoc.Default.Register<HostSettingsRedirectorUserControlViewModel>();
            SimpleIoc.Default.Register<SignInSettingsRedirectorUserControlViewModel>();
        }
        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }
        public AlbumDetailPageViewModel AlbumDetailPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlbumDetailPageViewModel>();
            }
        }
        public AlbumsGroupUserControlViewModel AlbumsGroupViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlbumsGroupUserControlViewModel>();
            }
        }
        public PlaylistDetailPageViewModel PlaylistDetailPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlaylistDetailPageViewModel>();
            }
        }
        public AlbumsPageViewModel AlbumsPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AlbumsPageViewModel>();
            }
        }
        public PlayerUserControlViewModel PlayerViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlayerUserControlViewModel>();
            }
        }
        public HostSettingsPageViewModel HostSettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HostSettingsPageViewModel>();
            }
        }

        public HostSettingsRedirectorUserControlViewModel HostSettingsRedirectorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<HostSettingsRedirectorUserControlViewModel>();
            }
        }
        public SignInSettingsPageViewModel SignInSettingsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SignInSettingsPageViewModel>();
            }
        }
        public SignInSettingsRedirectorUserControlViewModel SignInSettingsRedirectorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SignInSettingsRedirectorUserControlViewModel>();
            }
        }
        public TopAppBarUserControlViewModel TopAppBarViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<TopAppBarUserControlViewModel>();
            }
        }
        public RandomPlayerUserControlViewModel RandomPlayerViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RandomPlayerUserControlViewModel>();
            }
        }
        public PlaylistGroupUserControlViewModel PlaylistGroupViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlaylistGroupUserControlViewModel>();
            }
        }
        public SearchUserControlViewModel SearchViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchUserControlViewModel>();
            }
        }
        public SearchResultPageViewModel SearchResultPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchResultPageViewModel>();
            }
        }
        public SearchCategoryTracksPageViewModel SearchCategoryTracksPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchCategoryTracksPageViewModel>();
            }
        }
        public SearchCategoryAlbumsPageViewModel SearchCategoryAlbumsPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchCategoryAlbumsPageViewModel>();
            }
        }
        public PlaylistPageViewModel PlaylistPageViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PlaylistPageViewModel>();
            }
        }
    }
}
