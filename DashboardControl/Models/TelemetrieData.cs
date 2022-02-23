using System.ComponentModel;

namespace DashboardControl.Models
{
    public class TelemetrieData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public double RoundsPerMinute { get; set; }
        public double SpeedKmh { get; set; }
        public double FuelPercent { get; set; }
        public double WaterTemperatureDegC { get; set; }
        public bool TurnIndicatorLeft { get; set; }
        public bool TurnIndicatorRight { get; set; }
        public bool ParkingBrake { get; set; }
        public double OilTemperatureDegC { get; set; }
    }
}

