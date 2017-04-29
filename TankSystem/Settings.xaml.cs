using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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

namespace TankSystem
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        int tempDb = 0;
        string username;
        public Settings(string username)
        {
            InitializeComponent();
            this.username = username;

            tempDb = service.m1_max;
            m1_max.Text =  service.m1_max + "";
            m1_min.Text = service.m1_min + "";
            m2_max.Text = service.m2_max + "";
            m2_min.Text = service.m2_min + "";
            Tank1Height.Text = service.tank1_height + "";
            Tank2Height.Text = service.tank2_height + "";
            Tank1Capecity.Text = service.tank1_capacity + "";
            Tank2Capecity.Text = service.tank2_capacity + "";
        }

        int firstuser = 0;
        public Settings(int temp,string uname)
        {
            InitializeComponent();
            firstuser = temp;
            username = uname;
        }

        private void setting_save(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);

            if (firstuser == 1)
            {
                SqlCommand com1 = new SqlCommand("insert into settings values (1, @m1min , @m1max , @m2min , @m2max , @t1height ,  @t1capecity , @t2height , @t2capecity );", conn);
                com1.Parameters.AddWithValue("m1min", m1_min.Text);
                com1.Parameters.AddWithValue("m2min", m2_min.Text);
                com1.Parameters.AddWithValue("m2max", m2_max.Text);
                com1.Parameters.AddWithValue("m1max", m1_max.Text);
                com1.Parameters.AddWithValue("t1capecity", Tank1Capecity.Text);
                com1.Parameters.AddWithValue("t2capecity", Tank2Capecity.Text);
                com1.Parameters.AddWithValue("t2height", Tank2Height.Text);
                com1.Parameters.AddWithValue("t1height", Tank1Height.Text);
                com1.Parameters.AddWithValue("m1maxs", tempDb);

                conn.Open();
                com1.ExecuteNonQuery();
                conn.Close();

            }
            else
            {
                SqlCommand com = new SqlCommand("update settings set m1min = @m1min , m1max = @m1max , m2min = @m2min , m2max = @m2max , t1height = @t1height , t1capecity =  @t1capecity , t2height = @t2height , t2capecity = @t2capecity where id = 1", conn);
                com.Parameters.AddWithValue("m1min", m1_min.Text);
                com.Parameters.AddWithValue("m2min", m2_min.Text);
                com.Parameters.AddWithValue("m2max", m2_max.Text);
                com.Parameters.AddWithValue("m1max", m1_max.Text);
                com.Parameters.AddWithValue("t1capecity", Tank1Capecity.Text);
                com.Parameters.AddWithValue("t2capecity", Tank2Capecity.Text);
                com.Parameters.AddWithValue("t2height", Tank2Height.Text);
                com.Parameters.AddWithValue("t1height", Tank1Height.Text);
                com.Parameters.AddWithValue("m1maxs", tempDb);

                conn.Open();
                com.ExecuteNonQuery();
                conn.Close();
            }
            service.m1_max = Convert.ToInt32(m1_max.Text);
            service.m1_min = Convert.ToInt32(m1_min.Text);
            service.m2_max = Convert.ToInt32(m2_max.Text);
            service.m2_min = Convert.ToInt32(m2_min.Text);

            service.tank1_capacity = Convert.ToInt32(Tank1Capecity.Text);
            service.tank2_capacity = Convert.ToInt32(Tank2Capecity.Text);
            service.tank1_height = Convert.ToInt32(Tank1Height.Text);
            service.tank2_height = Convert.ToInt32(Tank2Height.Text);

            if (firstuser == 1)
            {
                SignInPage sip = new SignInPage();
                sip.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("changes is affect after restart");
                this.Close();
            }

        }

        private void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            SignUpGrid.Width = new GridLength(400); 
        }

        private void SingUPDetails_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);
            SqlCommand com1 = new SqlCommand("insert into adminuser values ( @username , @password , @firstname , @lastname);", conn);
            com1.Parameters.AddWithValue("username",UserNameTxt.Text);
            com1.Parameters.AddWithValue("password", PasswordTxt.Password);
            com1.Parameters.AddWithValue("firstname", FirstNameTxt.Text);
            com1.Parameters.AddWithValue("lastname", LastNameTxt.Text);
            conn.Open();
            com1.ExecuteNonQuery();
            conn.Close();
            MessageBox.Show("User Add Successfully");
            SignUpGrid.Width = new GridLength(0);
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            SignUpGrid.Width = new GridLength(0);
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }
    }
}
