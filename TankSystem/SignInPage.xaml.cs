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
    /// Interaction logic for SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Window
    {
        public SignInPage()
        {
            InitializeComponent();

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);
            SqlCommand com = new SqlCommand("select COUNT(*) from adminuser", conn);
            conn.Open();
            if ((int)com.ExecuteScalar() == 0)
            {
                conn.Close();
                AddAdminPage adp = new AddAdminPage();
                adp.Show();
                this.Close();
            }
        }


        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);
            SqlCommand com = new SqlCommand("select * from adminuser where username = @username and password = @password;", conn);
            conn.Open();
            com.Parameters.AddWithValue("username", UserNameInput.Text);
            com.Parameters.AddWithValue("password", PasswordInput.Password);
            SqlDataReader dr = com.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    UserPage usr = new UserPage(dr.GetString(1));
                    usr.Show();
                }
                conn.Close();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invelid Username or Password");
                conn.Close();
            }

        }


    }
}
