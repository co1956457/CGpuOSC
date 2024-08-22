using System;                       // default
using System.Collections.Generic;   // default
using System.ComponentModel;        // default
using System.Data;                  // default
using System.Drawing;               // default
using System.Linq;                  // default
using System.Text;                  // default
using System.Threading.Tasks;       // default
using System.Windows.Forms;         // default

//
// LibreHardwareMonitorLib.dll
// https://github.com/LibreHardwareMonitor/LibreHardwareMonitor
//
using LibreHardwareMonitor.Hardware;

namespace CGpuOSC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0; // 停止
        }

        // バージョン情報（読み取り専用）
        private readonly string version = "v1.0";

        // 温度取得間隔 0:停止
        int interval = 0;

        // System.Windows.Forms.Timer を使用
        // async wait は停止のタイミングに難あり
        private Timer _timer;

        //
        // 温度取得間隔が変更されたとき
        //
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Timer が動いていたら
            if (_timer != null)
            {
                _timer.Stop();
            }

            // 温度取得間隔: ミリ秒
            interval = Convert.ToInt32(comboBox1.SelectedItem) * 1000;

            // 0 は停止
            if (interval == 0)
            {
                label2.Text = "停止 / Stop\nCPU: --℃ (Core Max)\nGPU: --℃ (Hot Spot)";

                // OSC で 停止（CPU と GPU の温度 = 0）を送信
                SendOscTemperatures(0, 0);
            }
            else
            {
                // バージョンをOSCで送る
                SendOscVersion(version); 

                // Timer 設定
                _timer = new Timer();
                _timer.Interval = interval;
                _timer.Tick += OnTimerTick;
                _timer.Start();

                // Timer 設定完了直後に一度実行
                OnTimerTick(this, EventArgs.Empty); 
            }
        }

        //
        //CPU と GPU の温度を取得して OSC で送信
        //
        private void OnTimerTick(object sender, EventArgs e)
        {
            // CPU と GPU の温度を取得
            var (cpuTemperature, gpuTemperature) = GetCGpuTemperatures();

            // 0: 値がとれなかったら「--」
            string cpuText = "--";
            string gpuText = "--";
            if (cpuTemperature > 0)
            {
                cpuText = cpuTemperature.ToString();
            }
            if (gpuTemperature > 0)
            {
                gpuText = gpuTemperature.ToString();
            }

            // ラベルのテキストを更新
            label2.Text = DateTime.Now.ToString() + "\nCPU: " + cpuText + "℃ (Core Max)" + "\nGPU: " + gpuText + "℃ (Hot Spot)";
            
            // OSC で CPU と GPU の温度を送信
            SendOscTemperatures(cpuTemperature, gpuTemperature);
        }

        //
        // CPU と GPU の温度を取得して返す
        //
        private (int, int) GetCGpuTemperatures()
        {
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
            };

            computer.Open();
            computer.Accept(new UpdateVisitor());

            int cpuTemperature = 0;
            int gpuTemperature = 0;

            foreach (IHardware hardware in computer.Hardware)
            {
                foreach (ISensor sensor in hardware.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                    {
                        if (sensor.Name == "Core Max")
                        {
                            var value = sensor.Value;
                            if (value.HasValue)
                            {
                                cpuTemperature = Convert.ToInt32(value.Value);
                            }
                        }
                        else if (sensor.Name == "GPU Hot Spot")
                        {
                            var value = sensor.Value;
                            if (value.HasValue)
                            {
                                gpuTemperature = Convert.ToInt32(value.Value);
                            }
                        }
                    }
                }
            }
            computer.Close();
            return (cpuTemperature, gpuTemperature);
        }

        // OSC で送信
        // SharpOSC (MIT license) を部分利用
        // https://github.com/ValdemarOrn/SharpOSC
        //
        // CPU と GPU の温度を OSC で送信
        //
        static void SendOscTemperatures(int cpuTemperature, int gpuTemperature)
        {
            try
            {
                int int_cpuTemperature = cpuTemperature;
                int int_gpuTemperature = gpuTemperature;

                var sender = new UDPSender("127.0.0.1", 19100);
                var message = new OscMessage("/Taki/CGpuTemp/temperatures", int_cpuTemperature, int_gpuTemperature);

                sender.Send(message);
            }
            // for debugging purposes
            //catch (Exception error)
            catch (Exception)
            {
                // for debugging purposes
                // MessageBox.Show(error.ToString());
            }
        }


        // OSC で送信
        // SharpOSC (MIT license) を部分利用
        // https://github.com/ValdemarOrn/SharpOSC
        //
        // Version を送信
        //
        static void SendOscVersion(string version)
        {
            try
            {
                Encoding utf8 = Encoding.UTF8;
                byte[] blob_version = utf8.GetBytes(version);

                var sender = new UDPSender("127.0.0.1", 19100);
                var message = new OscMessage("/Taki/CGpuTemp/version", blob_version);

                sender.Send(message);
            }
            // for debugging purposes
            //catch (Exception error)
            catch (Exception)
            {
                // for debugging purposes
                // MessageBox.Show(error.ToString());
            }
        }
    }
}
