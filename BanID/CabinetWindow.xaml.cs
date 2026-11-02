using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using BanID.Models;

namespace BanID
{
    public partial class CabinetWindow : Window
    {
        private User currentUser;
        private string connectionString = "Data Source=DESKTOP-KIU2UL0\\SQLEXPRESS;Initial Catalog = уп задание 2;Integrated Security = True";
        public CabinetWindow(User user)
        {
            InitializeComponent();
            currentUser = user;
            LoadUserData();
            LoadAccounts();
        }

        private void LoadUserData()
        {
            string decrypted = Helpers.EncryptionHelper.Decrypt(currentUser.EncryptedPersonalData);
            var data = PersonalData.FromDataString(decrypted);
            txtFullName.Text = data.FullName;
            txtPassport.Text = data.Passport;
            txtAddress.Text = data.Address;
            txtPhone.Text = data.Phone;
        }

        private void LoadAccounts()
        {
            var accounts = new ObservableCollection<Account>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, AccountNumber, Balance, Currency FROM Accounts WHERE UserId = @userId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", currentUser.Id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account
                            {
                                Id = reader.GetInt32(0),
                                AccountNumber = reader.GetString(1),
                                Balance = reader.GetDecimal(2),
                                Currency = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            lstAccounts.ItemsSource = accounts;
        }

        private void BtnStatement_Click(object sender, RoutedEventArgs e)
        {
            if (lstAccounts.SelectedItem is Account acc)
                new StatementWindow(acc).ShowDialog();
            else MessageBox.Show("Выберите счёт");
        }

        private void BtnNewRequest_Click(object sender, RoutedEventArgs e)
        {
            new CreateRequestWindow(currentUser).ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }
    }

    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}