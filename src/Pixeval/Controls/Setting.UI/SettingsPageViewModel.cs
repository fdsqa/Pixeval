﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Pixeval.CoreApi.Global.Enum;
using Pixeval.Options;

namespace Pixeval.Controls.Setting.UI
{
    public class SettingsPageViewModel : ObservableObject
    {
        private readonly AppSetting _appSetting;

        public static readonly IEnumerable<string> AvailableFonts = new InstalledFontCollection().Families.Select(f => f.Name);

        public SettingsPageViewModel(AppSetting appSetting)
        {
            _appSetting = appSetting;
        }

        public string GetLastUpdateCheckDisplayString(DateTimeOffset lastChecked)
        {
            return $"{SettingsPageResources.LastCheckedPrefix}{lastChecked.ToString(CultureInfo.CurrentUICulture)}";
        }

        public ApplicationTheme Theme
        {
            get => _appSetting.Theme;
            set => SetProperty(_appSetting.Theme, value, _appSetting, (setting, value) => setting.Theme = value);
        }

        public bool FiltrateRestrictedContent
        {
            get => _appSetting.FiltrateRestrictedContent;
            set => SetProperty(_appSetting.FiltrateRestrictedContent, value, _appSetting, (setting, value) => setting.FiltrateRestrictedContent = value);
        }

        public ObservableCollection<string> ExcludeTags
        {
            get => _appSetting.ExcludeTags;
            set => SetProperty(_appSetting.ExcludeTags, value, _appSetting, (setting, value) => setting.ExcludeTags = value);
        }

        public bool DisableDomainFronting
        {
            get => _appSetting.DisableDomainFronting;
            set => SetProperty(_appSetting.DisableDomainFronting, value, _appSetting, (setting, value) =>
            {
                setting.DisableDomainFronting = value;
                App.AppViewModel.MakoClient.Configuration.Bypass = !value;
            });
        }

        public IllustrationSortOption DefaultSortOption
        {
            get => _appSetting.DefaultSortOption;
            set => SetProperty(_appSetting.DefaultSortOption, value, _appSetting, (setting, value) => setting.DefaultSortOption = value);
        }

        public SearchTagMatchOption TagMatchOption
        {
            get => _appSetting.TagMatchOption;
            set => SetProperty(_appSetting.TagMatchOption, value, _appSetting, (setting, value) => setting.TagMatchOption = value);
        }

        public TargetFilter TargetFilter
        {
            get => _appSetting.TargetFilter;
            set => SetProperty(_appSetting.TargetFilter, value, _appSetting, (setting, value) => setting.TargetFilter = value);
        }

        public int PreLoadRows
        {
            get => _appSetting.PreLoadRows;
            set => SetProperty(_appSetting.PreLoadRows, value, _appSetting, (setting, value) => setting.PreLoadRows = value);
        }

        public int PageLimitForKeywordSearch
        {
            get => _appSetting.PageLimitForKeywordSearch;
            set => SetProperty(_appSetting.PageLimitForKeywordSearch, value, _appSetting, (setting, value) => setting.PageLimitForKeywordSearch = value);
        }

        public int SearchStartingFromPageNumber
        {
            get => _appSetting.SearchStartingFromPageNumber;
            set => SetProperty(_appSetting.SearchStartingFromPageNumber, value, _appSetting, (setting, value) => setting.SearchStartingFromPageNumber = value);
        }

        public int PageLimitForSpotlight
        {
            get => _appSetting.PageLimitForSpotlight;
            set => SetProperty(_appSetting.PageLimitForSpotlight, value, _appSetting, (setting, value) => setting.PageLimitForSpotlight = value);
        }

        public string? MirrorHost
        {
            get => _appSetting.MirrorHost;
            set => SetProperty(_appSetting.MirrorHost, value, _appSetting, (setting, value) =>
            {
                setting.MirrorHost = value;
                App.AppViewModel.MakoClient.Configuration.MirrorHost = value;
            });
        }

        public int MaxDownloadTaskConcurrencyLevel
        {
            get => _appSetting.MaxDownloadTaskConcurrencyLevel;
            set => SetProperty(_appSetting.MaxDownloadTaskConcurrencyLevel, value, _appSetting, (setting, value) => setting.MaxDownloadTaskConcurrencyLevel = value);
        }

        public bool DisplayTeachingTipWhenGeneratingAppLink
        {
            get => _appSetting.DisplayTeachingTipWhenGeneratingAppLink;
            set => SetProperty(_appSetting.DisplayTeachingTipWhenGeneratingAppLink, value, _appSetting, (setting, value) => setting.DisplayTeachingTipWhenGeneratingAppLink = value);
        }

        public int ItemsNumberLimitForDailyRecommendations
        {
            get => _appSetting.ItemsNumberLimitForDailyRecommendations;
            set => SetProperty(_appSetting.ItemsNumberLimitForDailyRecommendations, value, _appSetting, (settings, value) => settings.ItemsNumberLimitForDailyRecommendations = value);
        }

        public bool UseFileCache
        {
            get => _appSetting.UseFileCache;
            set => SetProperty(_appSetting.UseFileCache, value, _appSetting, (settings, value) => settings.UseFileCache = value);
        }

        public ThumbnailDirection ThumbnailDirection
        {
            get => _appSetting.ThumbnailDirection;
            set => SetProperty(_appSetting.ThumbnailDirection, value, _appSetting, (settings, value) => settings.ThumbnailDirection = value);
        }

        public DateTimeOffset LastCheckedUpdate
        {
            get => _appSetting.LastCheckedUpdate;
            set => SetProperty(_appSetting.LastCheckedUpdate, value, _appSetting, (settings, value) => settings.LastCheckedUpdate = value);
        }

        public bool DownloadUpdateAutomatically
        {
            get => _appSetting.DownloadUpdateAutomatically;
            set => SetProperty(_appSetting.DownloadUpdateAutomatically, value, _appSetting, (settings, value) => settings.DownloadUpdateAutomatically = value);
        }

        public string AppFontFamilyName
        {
            get => _appSetting.AppFontFamilyName;
            set => SetProperty(_appSetting.AppFontFamilyName, value, _appSetting, (settings, value) => settings.AppFontFamilyName = value);
        }
    }
}