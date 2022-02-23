using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using PropertyChanged;
using DashboardControl.Models;

namespace DashboardControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        public string[] SerialPortEntries { get; set; }
        public SerialPort SerialPortConnection { get; set; } = new();
        public string SerialMessage { get; set; } = string.Empty;

        private Timer? _sendTimer;
        private Timer? _turnIndicatorTimer;

        public TelemetrieData Telemetrie { get; set; }

        public bool IsConnected { get; set; }
        public bool TurnIndicatorLeftChecked { get; set; }
        public bool TurnIndicatorRightChecked { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Telemetrie = new TelemetrieData();
            DataContext = this;

            SerialPortEntries = SerialPort.GetPortNames();
            SerialPortConnection.ReadTimeout = 5000;
            SerialPortConnection.WriteTimeout = 5000;
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SerialPortConnection.Open();
            }
            catch (Exception ex)
            {
                Reset();
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Could not open connection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            IsConnected = true;

            _sendTimer = new(100);
            /*
             * {Rpms&SpeedKmh&Fuel_Percent&WaterTemperature&TurnIndicatorLeft&TurnIndicatorRight&Handbrake&OilTemperature}
             */
            _sendTimer.Elapsed += /*async*/ (_, _) =>
            {
                SerialMessage = FormattableString.Invariant($"{{{Telemetrie.RoundsPerMinute:0}&{Telemetrie.SpeedKmh:0}&{Telemetrie.FuelPercent:0}&{Telemetrie.WaterTemperatureDegC:0.00}&{(Telemetrie.TurnIndicatorLeft ? 1 : 0)}&{(Telemetrie.TurnIndicatorRight ? 1 : 0)}&{(Telemetrie.ParkingBrake ? 1 : 0)}&{Telemetrie.OilTemperatureDegC:0.00}}}");
                Trace.WriteLine($"SerialMessage: {SerialMessage}");
                try
                {
                    SerialPortConnection.WriteLine(SerialMessage);
                }
                catch (Exception ex)
                {
                    Reset();
                    MessageBox.Show("A handled exception just occurred: " + ex.Message, "Could not send data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            };
            _sendTimer.Start();
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SerialPortConnection.Close();
            }
            catch (Exception ex)
            {
                Reset();
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Could not close connection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            IsConnected = false;

            _sendTimer?.Stop();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.AddedItems[0];
            if (selectedItem != null)
            {
                string? choosenPort = selectedItem.ToString();
                if (choosenPort != null)
                {
                    SerialPortConnection = new SerialPort(choosenPort, 115200);
                }
            }
        }

        private void CheckBoxTurnIndicatorLeft_Clicked(object sender, RoutedEventArgs e)
        {
            TurnIndicatorRightChecked = false;
            Telemetrie.TurnIndicatorRight = false;

            CheckBox cb = (CheckBox)sender;
            string checkState = "Indeterminate";

            if (cb.IsChecked == true)
            {
                checkState = "Checked";
                if (_turnIndicatorTimer != null)
                {
                    if (_turnIndicatorTimer.Enabled)
                    {
                        _turnIndicatorTimer.Dispose();
                    }
                }
                _turnIndicatorTimer = new(600);

                _turnIndicatorTimer.Elapsed += /*async*/ (_, _) =>
                {
                    Telemetrie.TurnIndicatorLeft = !Telemetrie.TurnIndicatorLeft;
                    Trace.WriteLine($"{nameof(Telemetrie.TurnIndicatorLeft)}: {Telemetrie.TurnIndicatorLeft}");
                };

                _turnIndicatorTimer.Start();
            }
            else
            {
                checkState = "Unchecked";
                _turnIndicatorTimer?.Dispose();

                Telemetrie.TurnIndicatorLeft = false;
            }
            Trace.WriteLine($"CheckState {nameof(Telemetrie.TurnIndicatorRight)}: {checkState}");
        }

        private void CheckBoxTurnIndicatorRight_Clicked(object sender, RoutedEventArgs e)
        {
            TurnIndicatorLeftChecked = false;
            Telemetrie.TurnIndicatorLeft = false;

            CheckBox cb = (CheckBox)sender;
            string checkState = "Indeterminate";

            if (cb.IsChecked == true)
            {
                checkState = "Checked";
                if (_turnIndicatorTimer != null)
                {
                    if (_turnIndicatorTimer.Enabled)
                    {
                        _turnIndicatorTimer.Dispose();
                    }
                }
                _turnIndicatorTimer = new(600);

                _turnIndicatorTimer.Elapsed += /*async*/ (_, _) =>
                {
                    Telemetrie.TurnIndicatorRight = !Telemetrie.TurnIndicatorRight;
                    Trace.WriteLine($"{nameof(Telemetrie.TurnIndicatorRight)}: {Telemetrie.TurnIndicatorRight}");
                };

                _turnIndicatorTimer.Start();
            }
            else
            {
                checkState = "Unchecked";
                _turnIndicatorTimer?.Dispose();

                Telemetrie.TurnIndicatorRight = false;
            }
            Trace.WriteLine($"CheckState {nameof(Telemetrie.TurnIndicatorRight)}: {checkState}");
        }

        private void Reset()
        {
            if (_sendTimer != null)
            {
                if (_sendTimer.Enabled)
                {
                    _sendTimer.Dispose();
                }
            }

            if (_turnIndicatorTimer != null)
            {
                if (_turnIndicatorTimer.Enabled)
                {
                    _turnIndicatorTimer.Dispose();
                }
            }

            IsConnected = false;
            TurnIndicatorLeftChecked = false;
            Telemetrie.TurnIndicatorLeft = false;
            TurnIndicatorRightChecked = false;
            Telemetrie.TurnIndicatorRight = false;

            SerialPortEntries = SerialPort.GetPortNames();
        }
    }
}
