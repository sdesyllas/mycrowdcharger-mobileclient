using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Locations;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Util;
using MyCrowdCharger.Mobile.Api.Dtos;
using MyCrowdCharger.Mobile.Api.Interfaces;
using MyCrowdCharger.Mobile.Api.Services;

namespace MyCrowdCharger.Mobile.Client
{
    [Activity(Label = "MyCrowdCharger.Mobile.Client", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, ILocationListener 
    {
        private ILog _logService;
        private ICrowdService _crowdService;

        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;

        private TextView deviceInfo;
        private TextView locationText;
        private TextView batteryText;
        private TextView contributionsText;
        private TextView locationInfoText;

        private Device _currentDevice;

        private Device[] availableDevices;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _logService = new XamarinLogService();
            _crowdService = new CrowdService(_logService);

            InitializeLocationManager();

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.findDevices);
            Button deleteButton = FindViewById<Button>(Resource.Id.deleteDevice);

            batteryText = FindViewById<TextView>(Resource.Id.batteryText);
            contributionsText = FindViewById<TextView>(Resource.Id.contributionsText);
            locationInfoText = FindViewById<TextView>(Resource.Id.locationInfoText);

            button.Click += FindAvailableDevices;
            deleteButton.Click += DeleteDevice;

            deviceInfo = FindViewById<TextView>(Resource.Id.deviceInfo);
            locationText = FindViewById<TextView>(Resource.Id.locationText);
            
            var androidId = Settings.Secure.GetString(ContentResolver, Settings.Secure.AndroidId);
            _currentDevice = _crowdService.GetDeviceByName(androidId);
            
            if (_currentDevice == null)
            {
                _logService.Info($"Device with id:{androidId} does not exist");
                StartActivity(typeof(RegisterDeviceActivity));
            }
            else
            {
                this.Title = $"{_currentDevice.Nickname} - battery: {_currentDevice.BatteryLevel}%";
                batteryText.Text = $"Battery: {_currentDevice.BatteryLevel}%";
                contributionsText.Text = $"contributed: {_currentDevice.Contributions}";
               
                deviceInfo.Text = _currentDevice.ToString();
            }
        }

        protected Address ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10).Result;

            Address address = addressList.FirstOrDefault();
            return address;
        }

        protected void DisplayBatteryLevel()
        {
            _currentDevice = _crowdService.GetDeviceByName(Settings.Secure.GetString(ContentResolver, Settings.Secure.AndroidId));
            if (_currentDevice != null)
            {
                batteryText.Text = $"Battery: {_currentDevice.BatteryLevel}%";
                this.Title = $"{_currentDevice.Nickname} - battery: {_currentDevice.BatteryLevel}%";
            }
        }

        protected void DeleteDevice(object sender, EventArgs e)
        {
            var isDeleted = _crowdService.DeleteDeviceByName(_currentDevice.Name);
            if (isDeleted)
            {
                StartActivity(typeof(MainActivity));
            }
        }

        protected void FindAvailableDevices(object sender, EventArgs e)
        {
            StartActivity(typeof(AvailableDevices));
        }

        protected override void OnResume()
        {
            base.OnResume();
            DisplayBatteryLevel();
            _locationManager.RequestLocationUpdates(_locationProvider, 10000, 1, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _logService.Warning("Unable to determine your location. Try again in a short while.");
            }
            else
            {
                _logService.Debug($"{_currentLocation.Latitude:f6},{_currentLocation.Longitude:f6}");
                locationText.Text = $"long:{_currentLocation.Longitude}, lat:{_currentLocation.Latitude}";
                _currentDevice.Location[0] = location.Longitude;
                _currentDevice.Location[1] = location.Latitude;
                _currentDevice = _crowdService.RefreshDevice(_currentDevice);
                var address = ReverseGeocodeCurrentLocation();
                if (address != null)
                {
                    locationInfoText.Text = $"{address.CountryName}, {address.PostalCode}, {address.Locality}, {address.SubAdminArea}";
                }
                Toast.MakeText(this, $"{_currentDevice.Nickname} location refreshed", ToastLength.Short);
            }
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {

        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug("LocationManager", "Using " + _locationProvider + ".");
        }
    }
}

