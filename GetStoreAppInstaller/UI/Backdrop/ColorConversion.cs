namespace GetStoreAppInstaller.UI.Backdrop
{
    public static class ColorConversion
    {
        public static Color GetEffectiveTintColor(Color tintColor, double tintOpacity, double? luminosityOpacity)
        {
            // Update tintColor's alpha with the combined opacity value
            // If LuminosityOpacity was specified, we don't intervene into users parameters
            if (luminosityOpacity.HasValue)
            {
                tintColor.A = (byte)Math.Round(tintColor.A * tintOpacity);
            }
            else
            {
                double tintOpacityModifier = GetTintOpacityModifier(tintColor);
                tintColor.A = (byte)Math.Round(tintColor.A * tintOpacity * tintOpacityModifier);
            }

            return tintColor;
        }

        public static double GetTintOpacityModifier(Color tintColor)
        {
            // TintOpacityModifier affects the 19H1+ Luminosity-based recipe only
            //if (!SharedHelpers::Is19H1OrHigher())
            //{
            //    return 1.0;
            //}

            // This method supresses the maximum allowable tint opacity depending on the luminosity and saturation of a color by
            // compressing the range of allowable values - for example, a user-defined value of 100% will be mapped to 45% for pure
            // white (100% luminosity), 85% for pure black (0% luminosity), and 90% for pure gray (50% luminosity).  The intensity of
            // the effect increases linearly as luminosity deviates from 50%.  After this effect is calculated, we cancel it out
            // linearly as saturation increases from zero.

            const double midPoint = 0.50; // Mid point of HsvV range that these calculations are based on. This is here for easy tuning.

            const double whiteMaxOpacity = 0.45; // 100% luminosity
            const double midPointMaxOpacity = 0.90; // 50% luminosity
            const double blackMaxOpacity = 0.85; // 0% luminosity

            (double, double, double) rgb = (tintColor.R / 255.0, tintColor.G / 255.0, tintColor.B / 255.0);
            (double _, double s, double v) = RgbToHsv(rgb);

            double opacityModifier = midPointMaxOpacity;

            if (v != midPoint)
            {
                // Determine maximum suppression amount
                double lowestMaxOpacity = midPointMaxOpacity;
                double maxDeviation = midPoint;

                if (v > midPoint)
                {
                    lowestMaxOpacity = whiteMaxOpacity; // At white (100% hsvV)
                    maxDeviation = 1 - maxDeviation;
                }
                else if (v < midPoint)
                {
                    lowestMaxOpacity = blackMaxOpacity; // At black (0% hsvV)
                }

                double maxOpacitySuppression = midPointMaxOpacity - lowestMaxOpacity;

                // Determine normalized deviation from the midpoint
                double deviation = Math.Abs(v - midPoint);
                double normalizedDeviation = deviation / maxDeviation;

                // If we have saturation, reduce opacity suppression to allow that color to come through more
                if (s > 0)
                {
                    // Dampen opacity suppression based on how much saturation there is
                    maxOpacitySuppression *= Math.Max(1 - (s * 2), 0.0);
                }

                double opacitySuppression = maxOpacitySuppression * normalizedDeviation;

                opacityModifier = midPointMaxOpacity - opacitySuppression;
            }

            return opacityModifier;
        }

        public static Color GetEffectiveLuminosityColor(Color tintColor, double tintOpacity, double? luminosityOpacity)
        {
            // Purposely leaving out tint opacity modifier here because GetLuminosityColor needs the *original* tint opacity set by the user.
            tintColor.A = (byte)Math.Round(tintColor.A * tintOpacity);

            return GetLuminosityColor(tintColor, luminosityOpacity);
        }

        public static Color GetLuminosityColor(Color tintColor, double? luminosityOpacity)
        {
            (double r, double g, double b) rgbTintColor = (tintColor.R / 255.0, tintColor.G / 255.0, tintColor.B / 255.0);

            // If luminosity opacity is specified, just use the values as is
            if (luminosityOpacity.HasValue)
            {
                return Color.FromArgb((byte)(Math.Min(luminosityOpacity.Value, 1.0) * 255),
                    (byte)(rgbTintColor.r * 255),
                    (byte)(rgbTintColor.g * 255),
                    (byte)(rgbTintColor.b * 255));
            }
            else
            {
                // To create the Luminosity blend input color without luminosity opacity,
                // we're taking the TintColor input, converting to HSV, and clamping the V between these values
                const double minHsvV = 0.125;
                const double maxHsvV = 0.965;

                (double h, double s, double v) = RgbToHsv(rgbTintColor);

                double clampedHsvV = Clamp(v, minHsvV, maxHsvV);

                (double h, double s, double clampedHsvV) hsvLuminosityColor = (h, s, clampedHsvV);
                (double r, double g, double b) = HsvToRgb(hsvLuminosityColor);

                // Now figure out luminosity opacity
                // Map original *tint* opacity to this range
                double minLuminosityOpacity = 0.15;
                double maxLuminosityOpacity = 1.03;

                double luminosityOpacityRangeMax = maxLuminosityOpacity - minLuminosityOpacity;
                double mappedTintOpacity = ((tintColor.A / 255.0) * luminosityOpacityRangeMax) + minLuminosityOpacity;

                // Finally, combine the luminosity opacity and the HsvV-clamped tint color
                return Color.FromArgb((byte)(Math.Min(mappedTintOpacity, 1.0) * 255),
                    (byte)(r * 255),
                    (byte)(g * 255),
                    (byte)(b * 255));
            }
        }

        public static (double h, double s, double v) RgbToHsv((double r, double g, double b) rgb)
        {
            double max = rgb.r >= rgb.g ? (rgb.r >= rgb.b ? rgb.r : rgb.b) : (rgb.g >= rgb.b ? rgb.g : rgb.b);
            double min = rgb.r <= rgb.g ? (rgb.r <= rgb.b ? rgb.r : rgb.b) : (rgb.g <= rgb.b ? rgb.g : rgb.b);

            // The value, a number between 0 and 1, is the largest of R, G, and B (divided by 255).
            // Conceptually speaking, it represents how much color is present.
            // If at least one of R, G, B is 255, then there exists as much color as there can be.
            // If RGB = (0, 0, 0), then there exists no color at all - a value of zero corresponds
            // to black (i.e., the absence of any color).
            double value = max;

            // The "chroma" of the color is a value directly proportional to the extent to which
            // the color diverges from greyscale.  If, for example, we have RGB = (255, 255, 0),
            // then the chroma is maximized - this is a pure yellow, no grey of any kind.
            // On the other hand, if we have RGB = (128, 128, 128), then the chroma being zero
            // implies that this color is pure greyscale, with no actual hue to be found.
            double chroma = max - min;

            double hue;
            double saturation;
            // If the chrome is zero, then hue is technically undefined - a greyscale color
            // has no hue.  For the sake of convenience, we'll just set hue to zero, since
            // it will be unused in this circumstance.  Since the color is purely grey,
            // saturation is also equal to zero - you can think of saturation as basically
            // a measure of hue intensity, such that no hue at all corresponds to a
            // nonexistent intensity.
            if (chroma is 0)
            {
                hue = 0.0;
                saturation = 0.0;
            }
            else
            {
                // In this block, hue is properly defined, so we'll extract both hue
                // and saturation information from the RGB color.

                // Hue can be thought of as a cyclical thing, between 0 degrees and 360 degrees.
                // A hue of 0 degrees is red; 120 degrees is green; 240 degrees is blue; and 360 is back to red.
                // Every other hue is somewhere between either red and green, green and blue, and blue and red,
                // so every other hue can be thought of as an angle on this color wheel.
                // These if/else statements determines where on this color wheel our color lies.
                if (rgb.r == max)
                {
                    // If the red channel is the most pronounced channel, then we exist
                    // somewhere between (-60, 60) on the color wheel - i.e., the section around 0 degrees
                    // where red dominates.  We figure out where in that section we are exactly
                    // by considering whether the green or the blue channel is greater - by subtracting green from blue,
                    // then if green is greater, we'll nudge ourselves closer to 60, whereas if blue is greater, then
                    // we'll nudge ourselves closer to -60.  We then divide by chroma (which will actually make the result larger,
                    // since chroma is a value between 0 and 1) to normalize the value to ensure that we get the right hue
                    // even if we're very close to greyscale.
                    hue = 60 * (rgb.g - rgb.b) / chroma;
                }
                else if (rgb.g == max)
                {
                    // We do the exact same for the case where the green channel is the most pronounced channel,
                    // only this time we want to see if we should tilt towards the blue direction or the red direction.
                    // We add 120 to center our value in the green third of the color wheel.
                    hue = 120 + 60 * (rgb.b - rgb.r) / chroma;
                }
                else // rgb.b == max
                {
                    // And we also do the exact same for the case where the blue channel is the most pronounced channel,
                    // only this time we want to see if we should tilt towards the red direction or the green direction.
                    // We add 240 to center our value in the blue third of the color wheel.
                    hue = 240 + 60 * (rgb.r - rgb.g) / chroma;
                }

                // Since we want to work within the range [0, 360), we'll add 360 to any value less than zero -
                // this will bump red values from within -60 to -1 to 300 to 359.  The hue is the same at both values.
                if (hue < 0.0)
                {
                    hue += 360.0;
                }

                // The saturation, our final HSV axis, can be thought of as a value between 0 and 1 indicating how intense our color is.
                // To find it, we divide the chroma - the distance between the minimum and the maximum RGB channels - by the maximum channel (i.e., the value).
                // This effectively normalizes the chroma - if the maximum is 0.5 and the minimum is 0, the saturation will be (0.5 - 0) / 0.5 = 1,
                // meaning that although this color is not as bright as it can be, the dark color is as intense as it possibly could be.
                // If, on the other hand, the maximum is 0.5 and the minimum is 0.25, then the saturation will be (0.5 - 0.25) / 0.5 = 0.5,
                // meaning that this color is partially washed out.
                // A saturation value of 0 corresponds to a greyscale color, one in which the color is *completely* washed out and there is no actual hue.
                saturation = chroma / value;
            }

            return (hue, saturation, value);
        }

        public static (double r, double g, double b) HsvToRgb((double h, double s, double v) hsv)
        {
            double hue = hsv.h;
            double saturation = hsv.s;
            double value = hsv.v;

            // We want the hue to be between 0 and 359,
            // so we first ensure that that's the case.
            while (hue >= 360.0)
            {
                hue -= 360.0;
            }

            while (hue < 0.0)
            {
                hue += 360.0;
            }

            // We similarly clamp saturation and value between 0 and 1.
            saturation = saturation < 0.0 ? 0.0 : saturation;
            saturation = saturation > 1.0 ? 1.0 : saturation;

            value = value < 0.0 ? 0.0 : value;
            value = value > 1.0 ? 1.0 : value;

            // The first thing that we need to do is to determine the chroma (see above for its definition).
            // Remember from above that:
            //
            // 1. The chroma is the difference between the maximum and the minimum of the RGB channels,
            // 2. The value is the maximum of the RGB channels, and
            // 3. The saturation comes from dividing the chroma by the maximum of the RGB channels (i.e., the value).
            //
            // From these facts, you can see that we can retrieve the chroma by simply multiplying the saturation and the value,
            // and we can retrieve the minimum of the RGB channels by subtracting the chroma from the value.
            double chroma = saturation * value;
            double min = value - chroma;

            // If the chroma is zero, then we have a greyscale color.  In that case, the maximum and the minimum RGB channels
            // have the same value (and, indeed, all of the RGB channels are the same), so we can just immediately return
            // the minimum value as the value of all the channels.
            if (chroma is 0)
            {
                return (min, min, min);
            }

            // If the chroma is not zero, then we need to continue.  The first step is to figure out
            // what section of the color wheel we're located in.  In order to do that, we'll divide the hue by 60.
            // The resulting value means we're in one of the following locations:
            //
            // 0 - Between red and yellow.
            // 1 - Between yellow and green.
            // 2 - Between green and cyan.
            // 3 - Between cyan and blue.
            // 4 - Between blue and purple.
            // 5 - Between purple and red.
            //
            // In each of these sextants, one of the RGB channels is completely present, one is partially present, and one is not present.
            // For example, as we transition between red and yellow, red is completely present, green is becoming increasingly present, and blue is not present.
            // Then, as we transition from yellow and green, green is now completely present, red is becoming decreasingly present, and blue is still not present.
            // As we transition from green to cyan, green is still completely present, blue is becoming increasingly present, and red is no longer present.  And so on.
            //
            // To convert from hue to RGB value, we first need to figure out which of the three channels is in which configuration
            // in the sextant that we're located in.  Next, we figure out what value the completely-present color should have.
            // We know that chroma = (max - min), and we know that this color is the max color, so to find its value we simply add
            // min to chroma to retrieve max.  Finally, we consider how far we've transitioned from the pure form of that color
            // to the next color (e.g., how far we are from pure red towards yellow), and give a value to the partially present channel
            // equal to the minimum plus the chroma (i.e., the max minus the min), multiplied by the percentage towards the new color.
            // This gets us a value between the maximum and the minimum representing the partially present channel.
            // Finally, the not-present color must be equal to the minimum value, since it is the one least participating in the overall color.
            int sextant = (int)(hue / 60);
            double intermediateColorPercentage = hue / 60 - sextant;
            double max = chroma + min;

            double r = 0;
            double g = 0;
            double b = 0;

            switch (sextant)
            {
                case 0:
                    r = max;
                    g = min + chroma * intermediateColorPercentage;
                    b = min;
                    break;

                case 1:
                    r = min + chroma * (1 - intermediateColorPercentage);
                    g = max;
                    b = min;
                    break;

                case 2:
                    r = min;
                    g = max;
                    b = min + chroma * intermediateColorPercentage;
                    break;

                case 3:
                    r = min;
                    g = min + chroma * (1 - intermediateColorPercentage);
                    b = max;
                    break;

                case 4:
                    r = min + chroma * intermediateColorPercentage;
                    g = min;
                    b = max;
                    break;

                case 5:
                    r = max;
                    g = min;
                    b = min + chroma * (1 - intermediateColorPercentage);
                    break;
            }

            return (r, g, b);
        }

        public static double Clamp(double value, double min, double max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format("最小值不能超过最大值，最小值 {0}，最大值 {1}", min, max));
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;
        }
    }
}
