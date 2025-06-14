﻿using System;
using System.ComponentModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GetStoreApp.Models
{
    /// <summary>
    /// 商店应用更新数据模型
    /// </summary>
    public sealed partial class AppUpdateModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 应用图标
        /// </summary>
        private Uri _logoImage;

        public Uri LogoImage
        {
            get { return _logoImage; }

            set
            {
                if (!Equals(_logoImage, value))
                {
                    _logoImage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogoImage)));
                }
            }
        }

        /// <summary>
        /// 标志应用是否处于升级状态
        /// </summary>
        private bool _isUpdating;

        public bool IsUpdating
        {
            get { return _isUpdating; }

            set
            {
                if (!Equals(_isUpdating, value))
                {
                    _isUpdating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsUpdating)));
                }
            }
        }

        private bool _isOperating;

        public bool IsOperating
        {
            get { return _isOperating; }

            set
            {
                if (!Equals(_isOperating, value))
                {
                    _isOperating = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOperating)));
                }
            }
        }

        /// <summary>
        /// 应用显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        public string PublisherDisplayName { get; set; }

        /// <summary>
        /// 应用的包系列名称
        /// </summary>
        public string PackageFamilyName { get; set; }

        /// <summary>
        /// 安装信息
        /// </summary>
        private string _installInformation = string.Empty;

        public string InstallInformation
        {
            get { return _installInformation; }

            set
            {
                if (!string.Equals(_installInformation, value))
                {
                    _installInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallInformation)));
                }
            }
        }

        private string _installSubInformation = string.Empty;

        public string InstallSubInformation
        {
            get { return _installSubInformation; }

            set
            {
                if (!string.Equals(_installSubInformation, value))
                {
                    _installSubInformation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstallSubInformation)));
                }
            }
        }

        /// <summary>
        /// 应用的产品 ID
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// 当前应用的安装状态
        /// </summary>
        private AppInstallState _appInstallState;

        public AppInstallState AppInstallState
        {
            get { return _appInstallState; }

            set
            {
                if (!Equals(_appInstallState, value))
                {
                    _appInstallState = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AppInstallState)));
                }
            }
        }

        /// <summary>
        /// 当前应用的安装完成百分比
        /// </summary>
        private double _percentComplete;

        public double PercentComplete
        {
            get { return _percentComplete; }

            set
            {
                if (!Equals(_percentComplete, value))
                {
                    _percentComplete = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PercentComplete)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
