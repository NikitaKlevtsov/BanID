using System;
using System.Data.SqlClient;
using System.Windows;
using BanID.Helpers;
using BanID.Models;

namespace BanID
{
    public partial class RegisterWindow : Window
    {
        private string connectionString = "Data Source=DESKTOP-KIU2UL0\\SQLEXPRESS;Initial Catalog = уп задание 2;Integrated Security = True";
        public RegisterWindow() => InitializeComponent();

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtPasswordConfirm.Password)
            {
                tbError.Text = "Пароли не совпадают";
                return;
            }
            var personal = new PersonalData
            {
                FullName = txtFullName.Text,
                Passport = txtPassport.Text,
                Address = txtAddress.Text,
                Phone = txtPhone.Text
            };
            string encrypted = EncryptionHelper.Encrypt(personal.ToDataString());
            var (hash, salt) = PasswordHelper.HashPassword(txtPassword.Password);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Users (Login, PasswordHash, Salt, EncryptedPersonalData) VALUES (@login, @hash, @salt, @enc)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", txtLogin.Text);
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Parameters.AddWithValue("@enc", encrypted);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Регистрация успешна!");
                        this.Close();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2627) // уникальность
                            tbError.Text = "Логин уже существует";
                        else
                            tbError.Text = "Ошибка базы данных";
                    }
                }
            }
        }
    }
}