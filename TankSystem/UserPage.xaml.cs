using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TankSystem
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Window
    {
        SerialPort serialPort1 = new SerialPort();

        string username;
        public UserPage(string username)
        {
            InitializeComponent();
            this.username = username;
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);
            SqlCommand com = new SqlCommand("select firstname,lastname from adminuser where username = @username", conn);
            com.Parameters.AddWithValue("username", username);
            conn.Open();
            SqlDataReader dr = com.ExecuteReader();

            string FullName = "";

            while (dr.Read())
            {
                FullName = dr.GetString(0) + " " + dr.GetString(1);
            }

            dr.Close();
            com = new SqlCommand("select * from settings", conn);

            dr = com.ExecuteReader();

            while (dr.Read())
            {
                service.m1_max = (int)dr["m1max"];
                service.m2_max = (int)dr["m2max"];
                service.m1_min = (int)dr["m1min"];
                service.m2_min = (int)dr["m2min"];
                service.tank1_capacity = (int)dr["t1capecity"];
                service.tank2_capacity = (int)dr["t2capecity"];
                service.tank1_height = (int)dr["t1height"];
                service.tank2_height = (int)dr["t2height"];
            }

            conn.Close();

            WelcomeTxt.Text = "Welcome " + FullName;

            serialPort1.BaudRate = 9600;
            serialPort1.PortName = "COM4";
            serialPort1.DataReceived += SerialPort1_DataReceived;

            try
            {
                serialPort1.Open();
            }
            catch
            {
                MessageBox.Show("Error in connecting to the system");
                // this.Close();
            }


            Prog1.Maximum = service.tank1_capacity;
            Prog1.Minimum = 0;

            Prog2.Minimum = 0;
            Prog2.Maximum = service.tank2_capacity;
        }


        private delegate void UpdateUiTextDelegate(string text);
        string RxString;
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RxString = serialPort1.ReadLine();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DisplayText), RxString);
        }


        string ActiveMode = "Auto";
        string Motor1_ManualMode = "OFF";
        string Motor2_ManualMode = "OFF";


        int sensor1Value = 0;
        int sensor2Value = 0;

        string[] temp;
        string[] temp2;


        Usage usageM1 = new Usage();
        Usage usageM2 = new Usage();
        DAO dao = new DAO();

        int tempAmoutnt1 = 0;
        int tempAmoutnt2 = 0;

        int sensor1Temp = 0;
        int sensor2Temp = 0;

        int sen1, sen2;
        private void DisplayText(string rxstring)
        {
            try
            {
                temp2 = rxstring.Split('\r');
                temp = temp2[0].Split('-');

                string value = temp[1];

                if (rxstring.StartsWith("S1"))
                {
                    if (sen1 <= service.tank1_height && sen1 <= service.tank1_height)
                    {
                        sen1 = service.tank1_height - Conversion.stringToint(temp[1]);
                    }

                }
                if (rxstring.StartsWith("S2"))
                {
                    if (sen2 <= service.tank1_height)
                    {
                        sen2 = service.tank2_height - Conversion.stringToint(temp[1]);
                    }


                }

                double sen1val = sen1 * (service.tank1_capacity / service.tank1_height);
                double sen2val = sen2 * (service.tank2_capacity / service.tank2_height);


                if (sen1val <= service.tank1_capacity * 0.1)
                {
                    Prog1.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (sen1val <= service.tank1_capacity * 0.5 && sen1val >= service.tank1_capacity * 0.1)
                {
                    Prog1.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else if (sen1val <= service.tank1_capacity && sen1val >= service.tank1_capacity * 0.5)
                {
                    Prog1.Foreground = new SolidColorBrush(Colors.Green);
                }


                if (sen2val <= service.tank2_capacity * 0.1)
                {
                    Prog2.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (sen2val <= service.tank2_capacity * 0.5 && sen2val >= service.tank2_capacity * 0.1)
                {
                    Prog2.Foreground = new SolidColorBrush(Colors.Yellow);
                }
                else if (sen2val <= service.tank2_capacity && sen2val >= service.tank2_capacity * 0.5)
                {
                    Prog2.Foreground = new SolidColorBrush(Colors.Green);
                }


                Prog1.Value = sen1val;
                Prog2.Value = sen2val;

                tank1_liter.Text = sen1val + " Liter";
                tank2_liter.Text = sen2val + " Liter";

                if (ActiveMode == "Auto")
                {
                    if (sen1val <= service.m1_min)
                    {
                        if (sensor1Temp == 1)
                        {
                            serialPort1.Write("M11");
                            //  MessageBox.Show("m11");
                            tank1_lastupdate.Text = DateTime.Now.ToString();
                            usageM1.startdate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                            usageM1.starttime = DateTime.Now.ToString("hh:mm:ss");
                            tempAmoutnt1 = sensor1Value;
                            sensor1Temp = 0;
                        }
                    }

                    if (sen1val >= service.m1_max)
                    {
                        if (sensor1Temp == 0)
                        {
                            serialPort1.Write("M10");
                            //   MessageBox.Show("m10");
                            usageM1.enddate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                            usageM1.endtime = DateTime.Now.ToString("hh:mm:ss");
                            usageM1.motorid = "M1";
                            tempAmoutnt1 = sensor1Value - tempAmoutnt1;
                            usageM1.amountleter = tempAmoutnt1;
                            dao.addData(usageM1);
                            sensor1Temp = 1;
                        }
                    }

                    if (sen2val <= service.m2_min)
                    {
                        if (sensor2Temp == 1)
                        {
                            serialPort1.Write("M21");
                            tank2_lastupdate.Text = DateTime.Now.ToString();

                            tank2_lastupdate.Text = DateTime.Now.ToString();
                            usageM2.startdate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                            usageM2.starttime = DateTime.Now.ToString("hh:mm:ss");
                            tempAmoutnt2 = sensor2Value;
                            sensor2Temp = 0;
                        }
                    }

                    if (sen2val >= service.m2_max)
                    {
                        if (sensor2Temp == 0)
                        {
                            serialPort1.Write("M20");
                            usageM2.enddate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                            usageM2.endtime = DateTime.Now.ToString("hh:mm:ss");
                            usageM2.motorid = "M2";
                            tempAmoutnt2 = sensor2Value - tempAmoutnt2;
                            usageM2.amountleter = tempAmoutnt2;
                            dao.addData(usageM2);
                            sensor2Temp = 1;
                        }
                    }
                }


                if (ActiveMode == "Manual")
                {

                    if (Motor1LiterTxt.Text == "0")
                    {
                        if (Motor1_ManualMode == "ON")
                        {

                            if (sensor1Temp == 1)
                            {
                                serialPort1.Write("M11");
                                tank1_lastupdate.Text = DateTime.Now.ToString();
                                tank1_lastupdate.Text = DateTime.Now.ToString();
                                usageM1.startdate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                                usageM1.starttime = DateTime.Now.ToString("hh:mm:ss");
                                tempAmoutnt1 = sensor1Value;
                                sensor1Temp = 0;
                            }
                        }
                        else if (Motor1_ManualMode == "OFF")
                        {
                            if (sensor1Temp == 0)
                            {
                                serialPort1.Write("M10");
                                usageM1.enddate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                                usageM1.endtime = DateTime.Now.ToString("hh:mm:ss");
                                usageM1.motorid = "M1";
                                tempAmoutnt1 = sensor1Value - tempAmoutnt1;
                                usageM1.amountleter = tempAmoutnt1;
                                dao.addData(usageM1);
                                sensor1Temp = 1;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(Motor1LiterTxt.Text) > Convert.ToInt32(sen1val) && (Motor1_ManualMode == "ON"))
                        {
                            if (sensor1Temp == 1)
                            {
                                serialPort1.Write("M11");
                                tank1_lastupdate.Text = DateTime.Now.ToString();
                                tank1_lastupdate.Text = DateTime.Now.ToString();
                                tank1_lastupdate.Text = DateTime.Now.ToString();
                                usageM1.startdate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                                usageM1.starttime = DateTime.Now.ToString("hh:mm:ss");
                                tempAmoutnt1 = sensor1Value;
                                sensor1Temp = 0;
                            }
                        }
                        else
                        {
                            if (sensor1Temp == 0)
                            {
                                serialPort1.Write("M10");
                                usageM1.enddate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                                usageM1.endtime = DateTime.Now.ToString("hh:mm:ss");
                                usageM1.motorid = "M1";
                                tempAmoutnt1 = sensor1Value - tempAmoutnt1;
                                usageM1.amountleter = tempAmoutnt1;
                                dao.addData(usageM1);
                                sensor1Temp = 1;
                            }
                        }
                    }

                    if (Motor2LiterTxt.Text == "0")
                    {
                        if (Motor2_ManualMode == "ON")
                        {
                            if (sensor2Temp == 1)
                            {
                                serialPort1.Write("M21");
                                tank2_lastupdate.Text = DateTime.Now.ToString();
                                tank2_lastupdate.Text = DateTime.Now.ToString();
                                usageM2.startdate = DateTime.Today.ToString("dd-MM-yyyy").Substring(0, 10);
                                usageM2.starttime = DateTime.Now.ToString("hh:mm:ss");
                                tempAmoutnt2 = sensor2Value;
                                sensor2Temp = 0;
                            }
                        }
                        else if (Motor2_ManualMode == "OFF")
                        {
                            if (sensor2Temp == 0)
                            {

                                serialPort1.Write("M20");
                                usageM2.enddate = DateTime.Now.ToString("dd-mm-yyyy").Substring(0, 10);
                                usageM2.endtime = DateTime.Now.ToString("hh:mm:ss");
                                usageM2.motorid = "M2";
                                tempAmoutnt2 = sensor2Value - tempAmoutnt2;
                                usageM2.amountleter = tempAmoutnt2;
                                dao.addData(usageM2);
                                sensor2Temp = 1;
                            }
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(Motor2LiterTxt.Text) > Convert.ToInt32(sen2val) && (Motor2_ManualMode == "ON"))
                        {
                            if (sensor2Temp == 1)
                            {
                                serialPort1.Write("M21");
                                tank2_lastupdate.Text = DateTime.Now.ToString();
                                tank2_lastupdate.Text = DateTime.Now.ToString();
                                usageM2.startdate = DateTime.Now.ToString("dd-mm-yyyy").Substring(0, 10);
                                usageM2.starttime = DateTime.Now.ToString("hh:mm:ss");
                                tempAmoutnt2 = sensor2Value;
                                sensor2Temp = 0;
                            }
                        }
                        else
                        {
                            if (sensor2Temp == 0)
                            {

                                serialPort1.Write("M20");
                                usageM2.enddate = DateTime.Now.ToString("dd-mm-yyyy").Substring(0, 10);
                                usageM2.endtime = DateTime.Now.ToString("hh:mm:ss");
                                usageM2.motorid = "M2";
                                tempAmoutnt2 = sensor2Value - tempAmoutnt2;
                                usageM2.amountleter = tempAmoutnt2;
                                dao.addData(usageM2);
                                sensor2Temp = 1;
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {

            }
        }



        private void ModeGraphics()
        {
            AutometicMode.Foreground = new SolidColorBrush(Colors.Black);
            AutometicMode.Background = new SolidColorBrush(Colors.Transparent);
            ManualMode.Foreground = new SolidColorBrush(Colors.Black);
            ManualMode.Background = new SolidColorBrush(Colors.Transparent);
            MaintenanceMode.Foreground = new SolidColorBrush(Colors.Black);
            MaintenanceMode.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void AutometicMode_Click(object sender, RoutedEventArgs e)
        {
            ModeGraphics();
            ModeImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/Autometic.PNG", UriKind.Absolute));
            ModeMessageTxt.Text = "system is in auto mode.";
            AutometicMode.Foreground = new SolidColorBrush(Colors.White);
            AutometicMode.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
            AutoModeGrid.Height = 200;
            ModeGrid.Height = 0;
            ActiveMode = "Auto";
        }



        private void ManualMode_Click(object sender, RoutedEventArgs e)
        {
            ModeGraphics();
            ManualMode.Foreground = new SolidColorBrush(Colors.White);
            ManualMode.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
            AutoModeGrid.Height = 0;
            ModeGrid.Height = 200;
            ActiveMode = "Manual";
        }

        private void MaintenanceMode_Click(object sender, RoutedEventArgs e)
        {
            ModeGraphics();
            ModeImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/Maintenence.PNG", UriKind.Absolute));
            ModeMessageTxt.Text = "system is in maintenance mode.";
            MaintenanceMode.Foreground = new SolidColorBrush(Colors.White);
            MaintenanceMode.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
            AutoModeGrid.Height = 200;
            ModeGrid.Height = 0;
            ActiveMode = "Maintenance";
            try
            {
                serialPort1.Write("M10");
                serialPort1.Write("M20");
            }
            catch
            {
                MessageBox.Show("Error in connection to system");
            }
        }



        private void Motor1Graphics()
        {
            Motor1OFFBtn.Foreground = new SolidColorBrush(Colors.Black);
            Motor1OFFBtn.Background = new SolidColorBrush(Colors.Transparent);
            Motor1ONBtn.Foreground = new SolidColorBrush(Colors.Black);
            Motor1ONBtn.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Motor2Graphics()
        {
            Motor2OFFBtn.Foreground = new SolidColorBrush(Colors.Black);
            Motor2OFFBtn.Background = new SolidColorBrush(Colors.Transparent);
            Motor2ONBtn.Foreground = new SolidColorBrush(Colors.Black);
            Motor2ONBtn.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Motor1ONBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(Motor1LiterTxt.Text) <= service.tank1_capacity && Convert.ToInt32(Motor1LiterTxt.Text) >= 0)
            {
                Motor1Graphics();
                Motor1ONBtn.Foreground = new SolidColorBrush(Colors.White);
                Motor1ONBtn.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
                Motor1_ManualMode = "ON";
            }
            else
            {
                //  MessageBox.Show("Capacity is not greater then tank capacity");
                Motor1Graphics();
                Motor1OFFBtn.Foreground = new SolidColorBrush(Colors.White);
                Motor1OFFBtn.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
                Motor1_ManualMode = "OFF";
            }

        }



        private void Motor1OFFBtn_Click(object sender, RoutedEventArgs e)
        {
            Motor1Graphics();
            Motor1OFFBtn.Foreground = new SolidColorBrush(Colors.White);
            Motor1OFFBtn.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
            Motor1_ManualMode = "OFF";
        }

        private void Motor2ONBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(Motor2LiterTxt.Text) <= service.tank2_capacity && Convert.ToInt32(Motor2LiterTxt.Text) >= 0)
            {
                Motor2_ManualMode = "ON";
                Motor2Graphics();
                Motor2ONBtn.Foreground = new SolidColorBrush(Colors.White);
                Motor2ONBtn.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
            }
            else
            {
                MessageBox.Show("Capacity is not greater then tank capacity");
                Motor2OFFBtn.Foreground = new SolidColorBrush(Colors.White);
                Motor2OFFBtn.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
                Motor2_ManualMode = "OFF";
            }

        }

        private void Motor2OFFBtn_Click(object sender, RoutedEventArgs e)
        {
            Motor2Graphics();
            Motor2OFFBtn.Foreground = new SolidColorBrush(Colors.White);
            Motor2OFFBtn.Background = (Brush)new BrushConverter().ConvertFrom("#FF0E5BA8");
            Motor2_ManualMode = "OFF";
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            ReportsPage rp = new ReportsPage();
            rp.Show();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            Settings settingPage = new Settings(username);
            settingPage.Show();
        }
    }
}
