using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace BanID
{
    public partial class CreateRequestWindow : Window
    {
        private User user;
        private string connectionString = "Data Source=DESKTOP-KIU2UL0\\SQLEXPRESS;Initial Catalog = уп задание 2;Integrated Security = True";

        public CreateRequestWindow(User u)
        {
            InitializeComponent();
            user = u;
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Requests (UserId, RequestType, RequestData, Status) VALUES (@uid, @type, @data, 'New')";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", user.Id);
                    cmd.Parameters.AddWithValue("@type", (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString());
                    cmd.Parameters.AddWithValue("@data", txtData.Text);
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Заявка отправлена");
            this.Close();
        }
    }
}