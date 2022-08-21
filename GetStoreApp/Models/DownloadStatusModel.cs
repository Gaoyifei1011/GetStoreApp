using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class DownloadStatusModel : DependencyObject
    {
        // 下载任务的GID信息
        public string GID
        {
            get { return (string)GetValue(GIDProperty); }
            set { SetValue(GIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GIDProperty =
            DependencyProperty.Register("GID", typeof(string), typeof(DownloadStatusModel), new PropertyMetadata(""));

        /// <summary>
        /// 下载任务状态信息
        /// </summary>
        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Status.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(DownloadStatusModel), new PropertyMetadata(""));

        /// <summary>
        /// 下载任务项目的总长度
        /// </summary>
        public int TotalLength
        {
            get { return (int)GetValue(TotalLengthProperty); }
            set { SetValue(TotalLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalLengthProperty =
            DependencyProperty.Register("TotalLength", typeof(int), typeof(DownloadStatusModel), new PropertyMetadata(0));

        /// <summary>
        /// 下载任务项目的已完成长度
        /// </summary>
        public int CompletedLength
        {
            get { return (int)GetValue(CompletedLengthProperty); }
            set { SetValue(CompletedLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CompletedLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompletedLengthProperty =
            DependencyProperty.Register("CompletedLength", typeof(int), typeof(DownloadStatusModel), new PropertyMetadata(0));

        /// <summary>
        /// 下载任务项目的即时速度
        /// </summary>
        public int DownloadSpeed
        {
            get { return (int)GetValue(DownloadSpeedProperty); }
            set { SetValue(DownloadSpeedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DownloadSpeed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DownloadSpeedProperty =
            DependencyProperty.Register("DownloadSpeed", typeof(int), typeof(DownloadStatusModel), new PropertyMetadata(0));
    }
}
