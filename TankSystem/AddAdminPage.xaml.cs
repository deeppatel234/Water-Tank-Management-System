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
    /// Interaction logic for AddAdminPage.xaml
    /// </summary>
    public partial class AddAdminPage : Window
    {
        public AddAdminPage()
        {
            InitializeComponent();
        }

        private void AddAdminBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (productkeyinput.Text == "R2FW6-NQKRV-FRVBD-QR6BJ-9TKKV")
                {
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);
                    SqlCommand com = new SqlCommand("insert into adminuser values (@username,@password,@firstname,@lastname)", conn);
                    com.Parameters.AddWithValue("username", adminUserNameInput.Text);
                    com.Parameters.AddWithValue("password", adminPasswordInput.Password);
                    com.Parameters.AddWithValue("firstname", firstnameinput.Text);
                    com.Parameters.AddWithValue("lastname", lastnameinput.Text);
                    conn.Open();
                    com.ExecuteNonQuery();
                    MessageBox.Show("Admin Add Successfully");
                    conn.Close();
                    Settings stg = new Settings(1,adminUserNameInput.Text);
                    stg.Show();
                    stg.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Invelid Product Key.", "Warning");
                }
            }
            catch
            {
                MessageBox.Show("Please Enter the Values.", "Warning");
            }
        }
    }
}
