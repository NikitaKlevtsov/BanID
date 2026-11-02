using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace BanID
{
    public partial class StatementWindow : Window
    {
        private Account account;
        private string connectionString = "Data Source=DESKTOP-KIU2UL0\\SQLEXPRESS;Initial Catalog = уп задание 2;Integrated Security = True";

        public StatementWindow(Account acc)
        {
            InitializeComponent();
            account = acc;
            dpStart.SelectedDate = DateTime.Now.AddMonths(-1);
            dpEnd.SelectedDate = DateTime.Now;
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var transactions = new System.Collections.ObjectModel.ObservableCollection<Transaction>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT TransactionDate, Amount, Description, Type 
                               FROM Transactions 
                               WHERE AccountId = @accId AND TransactionDate BETWEEN @start AND @end
                               ORDER BY TransactionDate";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@accId", account.Id);
                    cmd.Parameters.AddWithValue("@start", dpStart.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@end", dpEnd.SelectedDate.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            transactions.Add(new Transaction
                            {
                                TransactionDate = reader.GetDateTime(0),
                                Amount = reader.GetDecimal(1),
                                Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Type = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            dgTransactions.ItemsSource = transactions;
            tbTotal.Text = $"Итого: {transactions.Sum(t => t.Amount):C}";
        }

        private void BtnSaveTxt_Click(object sender, RoutedEventArgs e)
        {
            if (dgTransactions.ItemsSource == null)
            {
                MessageBox.Show("Сначала сформируйте выписку");
                return;
            }

            var save = new SaveFileDialog { Filter = "Text files|*.txt", DefaultExt = "txt" };
            if (save.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Выписка по счёту {account.AccountNumber}");
                sb.AppendLine($"Период: {dpStart.SelectedDate:dd.MM.yyyy} - {dpEnd.SelectedDate:dd.MM.yyyy}");
                sb.AppendLine($"Баланс: {account.Balance:C}");
                sb.AppendLine();
                sb.AppendLine("Дата\t\tСумма\t\tТип\t\tОписание");
                sb.AppendLine("--------------------------------------------------------");

                var transactions = dgTransactions.ItemsSource as System.Collections.IEnumerable;
                foreach (var item in transactions)
                {
                    var t = (Transaction)item;
                    sb.AppendLine($"{t.TransactionDate:dd.MM.yyyy}\t{t.Amount:C}\t{t.Type}\t{t.Description}");
                }
                sb.AppendLine();
                sb.AppendLine($"Итого: {((System.Collections.IEnumerable)dgTransactions.ItemsSource).Cast<Transaction>().Sum(x => x.Amount):C}");

                File.WriteAllText(save.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Выписка сохранена");
            }
        }
    }

    public class Transaction
    {
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}