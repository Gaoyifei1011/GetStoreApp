namespace GetStoreAppInstaller.UI.Backdrop
{
    /// <summary>
    /// 抽象类：系统背景色
    /// </summary>
    public abstract class SystemBackdrop(DesktopWindowTarget target) : IDisposable
    {
        public abstract float LightTintOpacity { get; set; }

        public abstract float LightLuminosityOpacity { get; set; }

        public abstract float DarkTintOpacity { get; set; }

        public abstract float DarkLuminosityOpacity { get; set; }

        public abstract Color LightTintColor { get; set; }

        public abstract Color LightFallbackColor { get; set; }

        public abstract Color DarkTintColor { get; set; }

        public abstract Color DarkFallbackColor { get; set; }

        public abstract ElementTheme RequestedTheme { get; set; }

        public abstract bool IsInputActive { get; set; }

        public DesktopWindowTarget DesktopWindowTarget { get; } = target;

        public abstract void InitializeBackdrop();

        public abstract void ResetProperties();

        public abstract void Dispose();
    }
}
