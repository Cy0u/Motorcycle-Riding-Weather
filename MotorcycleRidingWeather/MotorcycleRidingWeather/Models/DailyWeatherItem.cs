﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using DarkSkyApi;
using MotorcycleRidingWeather.Constants;
using MotorcycleRidingWeather.Services;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using Prism.Mvvm;
using Xamarin.Forms;

namespace MotorcycleRidingWeather.Models
{


    public class DailyWeatherItem : BindableBase
    {
        readonly Dictionary<string, string> WeatherIconDictionary = new Dictionary<string, string>
        {
            {"clear-day", "dayclear.png"},
            {"clear-night", "nighthalfmoonclear.png"},
            {"rain", "rain.png"},
            {"snow", "snow.png"},
            {"sleet", "sleet.png"},
            {"wind", "windIcon.png"},
            {"fog", "fog.png"},
            {"cloudy", "daypartialcloud.png"},
            {"partly-cloudy-day", "daypartialcloud.png"},
            {"partly-cloudy-night", "nightfullmoonpartialcloud.png"},
        };

        //clear-day, clear-night, rain, snow, sleet, wind, fog, cloudy, partly-cloudy-day, or partly-cloudy-night

        private static ISettings AppSettings => CrossSettings.Current;

        public bool IsGoodRidingDay
        {
            get
            {
                if(RainAccummulationCalculatedByDaily <= 0.00999)
                {
                    RainAccummulationCalculatedByDaily = 0;
                }

                return HighTemperature <= SessionData.CurrentUserPreferences.MaxRidingTemp
                        && LowTemperature >= SessionData.CurrentUserPreferences.MinRidingTemp
                        && PrecipitationProbability * 100 <= SessionData.CurrentUserPreferences.MaxRainPercentage
                        && (RainAccummulationCalculatedByDaily <= SessionData.CurrentUserPreferences.MaxRainAccumulation);
            }
        }

        public bool IsBadRidingDay
        {
            get
            {
                return !IsGoodRidingDay;
            }
        }

        public string WeatherIcon
        {
            get
            {
                return WeatherIconDictionary[Icon];
            }
        }

        public Color BackgroundColorBasedOnWeather
        {
            get
            {
                if (IsGoodRidingDay)
                {
                    return Color.FromHex("#607d8b");
                }

                return Color.FromHex("#607d8b");
            }
        }

        public double RainAccummulationCalculatedByDaily { get; set; }


        public string PrecipitationString
        {
            get
            {
                return $"{PrecipitationProbability * 100}%";
            }
        }

        /// <summary>
        /// Unix time at which this data point applies.
        /// </summary>
        [DataMember]
        private int time;

        /// <summary>
        /// Unix time of the last sunrise before the solar noon closest to local noon on
        /// the given day.
        /// </summary>
        [DataMember]
        private int sunriseTime;

        /// <summary>
        /// Unix time of the first sunset after the solar noon closest to local noon on
        /// the given day.
        /// </summary>
        [DataMember]
        private int sunsetTime;

        /// <summary>
        /// Unix time at which the maximum expected precipitation occurs.
        /// </summary>
        [DataMember]
        private int precipIntensityMaxTime;

        /// <summary>
        /// Unix time at which the maximum temperature occurs.
        /// </summary>
        [Obsolete]
        [DataMember]
        private int temperatureMaxTime;

        /// <summary>
        /// Unix time at which the lowest temperature occurs.
        /// </summary>
        [Obsolete]
        [DataMember]
        private int temperatureMinTime;

        /// <summary>
        /// Unix time at which the apparent minimum temperature occurs.
        /// </summary>
        [Obsolete]
        [DataMember]
        private int apparentTemperatureMinTime;

        /// <summary>
        /// Unix time at which the apparent maximum temperature occurs.
        /// </summary>
        [Obsolete]
        [DataMember]
        private int apparentTemperatureMaxTime;

        /// <summary>
        /// Unix time at which the overnight low apparent temperature occurs.
        /// </summary>
        [DataMember]
        private int apparentTemperatureLowTime;

        /// <summary>
        /// Unix time at which the daytime high apparent temperature occurs.
        /// </summary>
        [DataMember]
        private int apparentTemperatureHighTime;

        /// <summary>
        /// Unix time at which the overnight low temperature occurs.
        /// </summary>
        [DataMember]
        private int temperatureLowTime;

        /// <summary>
        /// Unix time at which the daytime high temperature occurs.
        /// </summary>
        [DataMember]
        private int temperatureHighTime;

        /// <summary>
        /// Unix time at which the maximum UV index occurs.
        /// </summary>
        [DataMember]
        private int uvIndexTime;

        /// <summary>
        /// Unix time at which the wind gust speed occurs.
        /// </summary>
        [DataMember]
        private int windGustTime;

        /// <summary>
        /// Gets or sets the time of this data point.
        /// </summary>
        public DateTimeOffset Time
        {
            get
            {
                return time.ToDateTimeOffset();
            }

            set
            {
                time = value.ToUnixTime();
            }
        }

        public string StructuredTime
        {
            get
            {
                return $"{Time.DayOfWeek.ToString().Substring(0,3)}";
            }
        }

        public string CurrentDayLabelText
        {
            get
            {
                return $"{Time.DayOfWeek}";
            }
        }

        /// <summary>
        /// Gets or sets a human-readable summary of this data point.
        /// </summary>
        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets machine-readable text that can be used to select an icon to display.
        /// </summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; }

        /// <summary>
        /// Gets or sets the time of the first sunset after the solar noon closest to local noon
        /// on the given day.
        /// </summary>
        public DateTimeOffset SunsetTime
        {
            get
            {
                return sunsetTime.ToDateTimeOffset();
            }

            set
            {
                sunsetTime = value.ToUnixTime();
            }
        }

        public string SunsetTimeOnly
        {
            get
            {
                return SunsetTime.LocalDateTime.ToString("h:mm tt");
            }
        }

        public string SunriseTimeOnly
        {
            get
            {
                return SunriseTime.LocalDateTime.ToString("h:mm tt");
            }
        }

        /// <summary>
        /// Gets or sets the time of the last sunrise before the solar noon closest to local noon
        /// on the given day.
        /// </summary>
        public DateTimeOffset SunriseTime
        {
            get
            {
                return sunriseTime.ToDateTimeOffset();
            }

            set
            {
                sunriseTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets a value representing the fractional part of the lunation number
        /// of the given day. Can be thought of as the "percentage complete" of the current
        /// lunar month.
        /// </summary>
        [DataMember(Name = "moonPhase")]
        public float MoonPhase { get; set; }

        /// <summary>
        /// Gets or sets the average expected precipitation assuming any precipitation occurs.
        /// </summary>
        [DataMember(Name = "precipIntensity")]
        public float PrecipitationIntensity { get; set; }

        /// <summary>
        /// Gets or sets the maximum expected precipitation intensity.
        /// </summary>
        [DataMember(Name = "precipIntensityMax")]
        public float MaxPrecipitationIntensity { get; set; }

        /// <summary>
        /// Gets or sets the type of precipitation.
        /// </summary>
        [DataMember(Name = "precipType")]
        public string PrecipitationType { get; set; }

        /// <summary>
        /// Gets or sets the time at which the maximum expected precipitation intensity occurs.
        /// </summary>
        public DateTimeOffset MaxPrecipitationIntensityTime
        {
            get
            {
                return precipIntensityMaxTime.ToDateTimeOffset();
            }

            set
            {
                precipIntensityMaxTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the probability of precipitation (from 0 to 1).
        /// </summary>
        [DataMember(Name = "precipProbability")]
        public float PrecipitationProbability { get; set; }

        /// <summary>
        /// Gets or sets the amount of snowfall accumulation expected to occur.
        /// </summary>
        [DataMember(Name = "precipAccumulation")]
        public float PrecipitationAccumulation { get; set; }

        /// <summary>
        /// Gets or sets the overnight apparent ("feels like") low temperature.
        /// </summary>
        [DataMember(Name = "apparentTemperatureLow")]
        public float ApparentLowTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the overnight apparent low temperature occurs.
        /// </summary>
        public DateTimeOffset ApparentLowTemperatureTime
        {
            get
            {
                return apparentTemperatureLowTime.ToDateTimeOffset();
            }

            set
            {
                apparentTemperatureLowTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the daytime apparent ("feels like") high temperature.
        /// </summary>
        [DataMember(Name = "apparentTemperatureHigh")]
        public float ApparentHighTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the daytime apparent high temperature occurs.
        /// </summary>
        public DateTimeOffset ApparentHighTemperatureTime
        {
            get
            {
                return apparentTemperatureHighTime.ToDateTimeOffset();
            }

            set
            {
                apparentTemperatureHighTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the overnight low temperature.
        /// </summary>
        [DataMember(Name = "temperatureLow")]
        public float LowTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the overnight low temperature occurs.
        /// </summary>
        public DateTimeOffset LowTemperatureTime
        {
            get
            {
                return temperatureLowTime.ToDateTimeOffset();
            }

            set
            {
                temperatureLowTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the daytime high temperature.
        /// </summary>
        [DataMember(Name = "temperatureHigh")]
        public float HighTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the daytime high temperature occurs.
        /// </summary>
        public DateTimeOffset HighTemperatureTime
        {
            get
            {
                return temperatureHighTime.ToDateTimeOffset();
            }

            set
            {
                temperatureHighTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the air temperature.
        /// </summary>
        [DataMember(Name = "temperature")]
        public float Temperature { get; set; }

        /// <summary>
        /// Gets or sets the minimum (lowest) temperature for the day.
        /// </summary>
        [Obsolete("Deprecated - consider using LowTemperature instead.")]
        [DataMember(Name = "temperatureMin")]
        public float MinTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the minimum (lowest) temperature occurs.
        /// </summary>
        [Obsolete("Deprecated - consider using LowTemperatureTime instead.")]
        public DateTimeOffset MinTemperatureTime
        {
            get
            {
                return temperatureMinTime.ToDateTimeOffset();
            }

            set
            {
                temperatureMinTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the maximum (highest) temperature for the day.
        /// </summary>
        [Obsolete("Deprecated - consider using HighTemperature instead.")]
        [DataMember(Name = "temperatureMax")]
        public float MaxTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the maximum (highest) temperature occurs.
        /// </summary>
        [Obsolete("Deprecated - consider using HighTemperatureTime instead.")]
        public DateTimeOffset MaxTemperatureTime
        {
            get
            {
                return temperatureMaxTime.ToDateTimeOffset();
            }

            set
            {
                temperatureMaxTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the apparent ("feels like") minimum temperature.
        /// </summary>
        [Obsolete("Deprecated - consider using ApparentLowTemperature instead.")]
        [DataMember(Name = "apparentTemperatureMin")]
        public float ApparentMinTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the apparent minimum temperature occurs.
        /// </summary>
        [Obsolete("Deprecated - consider using ApparentLowTemperatureTime instead.")]
        public DateTimeOffset ApparentMinTemperatureTime
        {
            get
            {
                return apparentTemperatureMinTime.ToDateTimeOffset();
            }

            set
            {
                apparentTemperatureMinTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the apparent ("feels like") maximum temperature.
        /// </summary>
        [Obsolete("Deprecated - consider using ApparentHighTemperature instead.")]
        [DataMember(Name = "apparentTemperatureMax")]
        public float ApparentMaxTemperature { get; set; }

        /// <summary>
        /// Gets or sets the time at which the apparent maximum temperature occurs.
        /// </summary>
        [Obsolete("Deprecated - consider using ApparentHighTemperatureTime instead.")]
        public DateTimeOffset ApparentMaxTemperatureTime
        {
            get
            {
                return apparentTemperatureMaxTime.ToDateTimeOffset();
            }

            set
            {
                apparentTemperatureMaxTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the dew point.
        /// </summary>
        [DataMember(Name = "dewPoint")]
        public float DewPoint { get; set; }

        /// <summary>
        /// Gets or sets the relative humidity (from 0 to 1).
        /// </summary>
        [DataMember(Name = "humidity")]
        public float Humidity { get; set; }

        /// <summary>
        /// Gets or sets the wind speed.
        /// </summary>
        [DataMember(Name = "windSpeed")]
        public float WindSpeed { get; set; }

        /// <summary>
        /// Gets or sets the direction (in degrees) the wind is coming from.
        /// </summary>
        [DataMember(Name = "windBearing")]
        public float WindBearing { get; set; }

        /// <summary>
        /// Gets or sets the wind gust speed.
        /// </summary>
        [DataMember(Name = "windGust")]
        public float WindGust { get; set; }

        /// <summary>
        /// Gets or sets the time at which the wind gust speed occurs.
        /// </summary>
        public DateTimeOffset WindGustTime
        {
            get
            {
                return windGustTime.ToDateTimeOffset();
            }

            set
            {
                windGustTime = value.ToUnixTime();
            }
        }

        /// <summary>
        /// Gets or sets the average visibility (capped at 10 miles).
        /// </summary>
        [DataMember(Name = "visibility")]
        public float Visibility { get; set; }

        /// <summary>
        /// Gets or sets the percentage of cloud cover (from 0 to 1).
        /// </summary>
        [DataMember(Name = "cloudCover")]
        public float CloudCover { get; set; }

        public string CloudCoverString
        {
            get
            {
                return $"{CloudCover * 100}%";
            }
        }

        /// <summary>
        /// Gets or sets the sea-level air pressure.
        /// </summary>
        [DataMember(Name = "pressure")]
        public float Pressure { get; set; }

        /// <summary>
        /// Gets or sets the columnar density of total atmospheric ozone, in Dobson units.
        /// </summary>
        [DataMember(Name = "ozone")]
        public float Ozone { get; set; }

        /// <summary>
        /// Gets or sets the UV index.
        /// </summary>
        [DataMember(Name = "uvIndex")]
        public float UVIndex { get; set; }

        /// <summary>
        /// Gets or sets the time at which the maximum UV index occurs.
        /// </summary>
        public DateTimeOffset UVIndexTime
        {
            get
            {
                return uvIndexTime.ToDateTimeOffset();
            }

            set
            {
                uvIndexTime = value.ToUnixTime();
            }
        }

        public string HighLowFormattedText
        {
            get
            {
                return $"{(int)HighTemperature}/{(int)LowTemperature} °F";
            }
        }
    }
}


