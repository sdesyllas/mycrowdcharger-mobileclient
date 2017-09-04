using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MyCrowdCharger.Mobile.Api.Dtos;
using MyCrowdCharger.Mobile.Api.Interfaces;
using MyCrowdCharger.Mobile.Api.Services;

namespace MyCrowdCharger.Mobile.Client
{
    [Activity(Label = "RegisterDeviceActivity")]
    public class RegisterDeviceActivity : Activity, ILocationListener
    {
        private ILog _logService;
        private ICrowdService _crowdService;

        Location _currentLocation;
        LocationManager _locationManager;
        string _locationProvider;

        private TextView _deviceInfo;
        private TextView _deviceAddressInfo;
        private EditText _nickName;
        private Button _registerButton;
        private Button _goBackButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _logService = new XamarinLogService();
            _crowdService = new CrowdService(_logService);
            InitializeLocationManager();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.RegisterDevice);
            // Create your application here

            _deviceInfo = FindViewById<TextView>(Resource.Id.deviceInfo);
            _deviceAddressInfo = FindViewById<TextView>(Resource.Id.deviceAddressInfo);
            _nickName = FindViewById<EditText>(Resource.Id.nickName);

            _registerButton = FindViewById<Button>(Resource.Id.registerButton);
            _goBackButton = FindViewById<Button>(Resource.Id.goBackButton);

            _registerButton.Click += RegisterButton_OnClick;
            _goBackButton.Click += GoBack_OnClick;
        }

        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 10000, 1, this);
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
        }

        public void GoBack_OnClick(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
        }

        public void RegisterButton_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_nickName.Text))
            {
                _deviceAddressInfo.Text = "Nickname cannot be empty, please provide a nickname for your device!";
                return;
            }
            if (_currentLocation == null)
            {
                _deviceAddressInfo.Text = "Can't determine the current address. Try again in a few minutes.";
                return;
            }

            var createdDevice = CreateDevice(_nickName.Text, _currentLocation);
            _deviceInfo.Text = createdDevice.ToString();
            if (createdDevice.Nickname != _nickName.Text) return;
            _registerButton.Visibility = ViewStates.Invisible;
            _goBackButton.Visibility = ViewStates.Visible;
            _nickName.Visibility = ViewStates.Invisible;
        }

        protected Device CreateDevice(string nickName, Location currentLocation)
        {
            var androidId = Settings.Secure.GetString(ContentResolver, Settings.Secure.AndroidId);

            var device = new Device
            {
                Name = androidId,
                Nickname = nickName,
                Contributions = 0,
                BatteryLevel = GetCurrentBatteryLevel(),
                Location = new double[2],
            };
            device.Location[0] = currentLocation.Longitude;
            device.Location[1] = currentLocation.Latitude;

            var createdDevice = _crowdService.RegisterDevice(device);
            return createdDevice;
        }

        protected int GetCurrentBatteryLevel()
        {
            var filter = new IntentFilter(Intent.ActionBatteryChanged);
            var battery = RegisterReceiver(null, filter);
            return Utilities.BatteryManager.GetCurrentBatteryLevel(battery);
        }

        private void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            _locationProvider = acceptableLocationProviders.Any() ? acceptableLocationProviders.First() : string.Empty;
            Log.Debug("LocationManager", "Using " + _locationProvider + ".");
        }

        public async void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _logService.Warning("Unable to determine your location. Try again in a short while.");
            }
            else
            {
                _logService.Debug($"{_currentLocation.Latitude:f6},{_currentLocation.Longitude:f6}");
                _deviceAddressInfo.Text = $"long:{_currentLocation.Longitude}, lat:{_currentLocation.Latitude}";
            }
        }

        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }
    }
}