using Microsoft.UI.Xaml;

namespace GetStoreApp.Models
{
    public class GeographicalLocation : DependencyObject
    {
        public string Nation
        {
            get { return (string)GetValue(NationProperty); }
            set { SetValue(NationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Nation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NationProperty =
            DependencyProperty.Register("Nation", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string Latitude
        {
            get { return (string)GetValue(LatitudeProperty); }
            set { SetValue(LatitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Latitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string Longitude
        {
            get { return (string)GetValue(LongitudeProperty); }
            set { SetValue(LongitudeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Longitude.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string ISO2
        {
            get { return (string)GetValue(ISO2Property); }
            set { SetValue(ISO2Property, value); }
        }

        // Using a DependencyProperty as the backing store for ISO2.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ISO2Property =
            DependencyProperty.Register("ISO2", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string ISO3
        {
            get { return (string)GetValue(ISO3Property); }
            set { SetValue(ISO3Property, value); }
        }

        // Using a DependencyProperty as the backing store for ISO3.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ISO3Property =
            DependencyProperty.Register("ISO2", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string Rfc1766
        {
            get { return (string)GetValue(Rfc1766Property); }
            set { SetValue(Rfc1766Property, value); }
        }

        // Using a DependencyProperty as the backing store for Rfc1766.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Rfc1766Property =
            DependencyProperty.Register("Rfc1766", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string Lcid
        {
            get { return (string)GetValue(LcidProperty); }
            set { SetValue(LcidProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lcid.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LcidProperty =
            DependencyProperty.Register("Lcid", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string FriendlyName
        {
            get { return (string)GetValue(FriendlyNameProperty); }
            set { SetValue(FriendlyNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FriendlyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FriendlyNameProperty =
            DependencyProperty.Register("FriendlyName", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string OfficialName
        {
            get { return (string)GetValue(OfficialNameProperty); }
            set { SetValue(OfficialNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OfficialName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OfficialNameProperty =
            DependencyProperty.Register("OfficialName", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string TimeZones
        {
            get { return (string)GetValue(TimeZonesProperty); }
            set { SetValue(TimeZonesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeZones.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeZonesProperty =
            DependencyProperty.Register("TimeZones", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));

        public string OfficialLanguages
        {
            get { return (string)GetValue(OfficialLanguagesProperty); }
            set { SetValue(OfficialLanguagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OfficialLanguages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OfficialLanguagesProperty =
            DependencyProperty.Register("OfficialLanguages", typeof(string), typeof(GeographicalLocation), new PropertyMetadata(""));
    }
}
