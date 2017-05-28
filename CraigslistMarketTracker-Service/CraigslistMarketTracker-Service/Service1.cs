using CraigslistMarketTracker_Service.WebpageScraper;
using System.ServiceProcess;
using System.Timers;

namespace CraigslistMarketTracker_Service
{
    public partial class Service1 : ServiceBase
    {
        public Timer _timer { get; set; }
        public GlobalDataLoader _globalDataLoader { get; set; }

        public Service1()
        {
            InitializeComponent();
        }

        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            _globalDataLoader = new GlobalDataLoader();

            _timer = new Timer(300000);
            _timer.Elapsed += new ElapsedEventHandler((sender, e) => {
                new DataScraper(_globalDataLoader, "denver").RetrieveNewData();
                _timer.Start(); // Restart timer
            });
            _timer.AutoReset = false;
            _timer.Start();
        }

        protected override void OnStop()
        {

        }
    }
}
