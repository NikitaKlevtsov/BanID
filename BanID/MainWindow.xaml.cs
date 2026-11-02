using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using BanID.Helpers;

namespace BanID
{
    public partial class MainWindow : Window
    {
        private string currentCaptcha;
        private string connectionString = "Data Source=DESKTOP-KIU2UL0\\SQLEXPRESS;Initial Catalog = уп задание 2;Integrated Security = True";

        public MainWindow()
        {
            InitializeComponent();
            lblCaptcha.Text = currentCaptcha = CaptchaHelper.GenerateCaptcha();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtCaptcha.Text != currentCaptcha)
            {
                tbError.Text = "Неверная капча";
                lblCaptcha.Text = currentCaptcha = CaptchaHelper.GenerateCaptcha();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, PasswordHash, Salt, EncryptedPersonalData FROM Users WHERE Login = @login";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", txtLogin.Text);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string hash = reader.GetString(1);
                            string salt = reader.GetString(2);
                            string encryptedData = reader.GetString(3);
                            if (PasswordHelper.VerifyPassword(txtPassword.Password, hash, salt))
                            {
                                var user = new User { Id = id, Login = txtLogin.Text, EncryptedPersonalData = encryptedData };
                                CabinetWindow cabinet = new CabinetWindow(user);
                                cabinet.Show();
                                this.Close();
                                return;
                            }
                        }
                    }
                }
                tbError.Text = "Неверный логин или пароль";
                lblCaptcha.Text = currentCaptcha = CaptchaHelper.GenerateCaptcha();
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            new RegisterWindow().ShowDialog();
        }
    }

    // Простой класс для передачи пользователя
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string EncryptedPersonalData { get; set; }
    }
}