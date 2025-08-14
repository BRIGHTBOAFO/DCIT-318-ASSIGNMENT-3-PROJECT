using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // a. Record type for Transaction (Immutable by default)
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // b. Transaction Processor Interface
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // c. Concrete processors with distinct implementations
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Bank Transfer] Processed {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Mobile Money] Processed {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[Crypto Wallet] Processed {transaction.Amount:C} for {transaction.Category} on {transaction.Date:d}");
        }
    }

    // d. Base Account class
    public class Account
    {
        public string AccountNumber { get; private set; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"Transaction Applied. New Balance: {Balance:C}");
        }
    }

    // e. Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
            }
            else
            {
                Balance -= transaction.Amount;
                Console.WriteLine($"Transaction Successful. Updated Balance: {Balance:C}");
            }
        }
    }

    // f. FinanceApp Simulation
    public class FinanceApp
    {
        private List<Transaction> _transactions = new List<Transaction>();

        public void Run()
        {
            // i. Create account
            var account = new SavingsAccount("ACC123456", 1000m);

            // ii. Create transactions
            var t1 = new Transaction(1, DateTime.Now, 200m, "Groceries");
            var t2 = new Transaction(2, DateTime.Now, 150m, "Utilities");
            var t3 = new Transaction(3, DateTime.Now, 500m, "Entertainment");

            // iii. Process transactions
            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);
            bankTransfer.Process(t2);
            cryptoWallet.Process(t3);

            // iv. Apply to account
            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3);

            // v. Store transactions
            _transactions.Add(t1);
            _transactions.Add(t2);
            _transactions.Add(t3);
        }

        // Main entry point
        public static void Main(string[] args)
        {
            FinanceApp app = new FinanceApp();
            app.Run();
        }
    }
}
