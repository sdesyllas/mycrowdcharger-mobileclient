using System.Linq;
using Android.App;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using MyCrowdCharger.Mobile.Api.Dtos;
using MyCrowdCharger.Mobile.Api.Interfaces;
using MyCrowdCharger.Mobile.Api.Services;

namespace MyCrowdCharger.Mobile.Client
{
    [Activity(Label = "AvailableDevices")]
    public class AvailableDevices : ListActivity
    {
        private ILog _logService;
        private ICrowdService _crowdService;
        private Device[] nearestDevices;
        private Device _currentDevice;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Title = "Tap to get battery!";

            _logService = new XamarinLogService();
            _crowdService = new CrowdService(_logService);

            PopulateNearestDevices();
        
            ListView.TextFilterEnabled = true;
        }

        protected void PopulateNearestDevices()
        {
            var androidId = Settings.Secure.GetString(ContentResolver, Settings.Secure.AndroidId);
            nearestDevices = _crowdService.GetNearestDevicesToDeviceLocation(androidId).ToArray();

            ListAdapter = new DevicesAdapter(this, nearestDevices.ToArray());
            _currentDevice = _crowdService.GetDeviceByName(androidId);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var chosenDevice = nearestDevices[position];

            BatterySend batterySend = new BatterySend
            {
                RecipientUser = new BatterySend.Recipient()
                {
                    Name = _currentDevice.Name
                },
                SenderUser = new BatterySend.Sender()
                {
                    Name = chosenDevice.Name,
                    Battery = 1
                }
            };
            var batterySent = _crowdService.SendBattery(batterySend);
            if (batterySent)
            {
                Toast.MakeText(this, batterySend.ToString(), ToastLength.Short).Show();
                PopulateNearestDevices();
                this.Title = $"{_currentDevice.Nickname} - battery: {_currentDevice.BatteryLevel}%";
            }
            else
            {
                Toast.MakeText(this, "Already full or insufficient recipient levels", ToastLength.Short).Show();
            }
        }

        public class DevicesAdapter : BaseAdapter<Device>
        {
            Device[] items;
            Activity context;
            public DevicesAdapter(Activity context, Device[] items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override Device this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Length; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view = convertView; // re-use an existing view, if one is available
                if (view == null) // otherwise create a new one
                    view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
                var currentItem = items[position];
                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text =
                    $"{currentItem.Nickname} battery:{currentItem.BatteryLevel}% | {currentItem.Contributions} contributions";
                return view;
            }
        }
    }
}