using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TankSystem
{
    class DAO
    {
        public void addData(Usage usage)
        {
            try
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);
                SqlCommand com = new SqlCommand("insert into usage (starttime,endtime,startdate,enddate,motor,amountleter) values (@starttime,@endtime,@startdate,@enddate,@motor,@amountleter)", conn);
                conn.Open();

                com.Parameters.AddWithValue("starttime", usage.starttime);
                com.Parameters.AddWithValue("endtime", usage.endtime);
                com.Parameters.AddWithValue("startdate", usage.startdate);
                com.Parameters.AddWithValue("enddate", usage.enddate);
                com.Parameters.AddWithValue("motor", usage.motorid);
                com.Parameters.AddWithValue("amountleter", usage.amountleter);

                com.ExecuteNonQuery();

                conn.Close();
            }
            catch
            {
                MessageBox.Show("Something Went Wrong");
            }
        }
    }
}
