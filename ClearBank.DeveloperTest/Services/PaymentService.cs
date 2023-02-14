using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Types;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            var dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];

            Account account = null;

            // todo: configure using context injection instead of if/else statements
            if (dataStoreType == "Backup")
            {
                var accountDataStore = new BackupAccountDataStore();
                account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            }
            if (dataStoreType == "Test")
            {
                var accountDataStore = new TestDataStore();
                account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            }
            else
            {
                var accountDataStore = new AccountDataStore();
                account = accountDataStore.GetAccount(request.DebtorAccountNumber);
            }

            var result = new MakePaymentResult();

            switch (request.PaymentScheme)
            {
                case PaymentScheme.Bacs:
                {
                    if (account != null && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs))
                    {
                        result.Success = true;
                    }
                    break;
                }

                case PaymentScheme.FasterPayments:
                    if (account != null &&
                        account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments) &&
                        account.Balance >= request.Amount)
                    {
                        result.Success = true;
                    }
                    break;

                case PaymentScheme.Chaps:
                    if (account != null && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps) &&
                        account.Status == AccountStatus.Live)
                    {
                        result.Success = true;
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Invalid Payment Scheme Provided: {request.PaymentScheme}");

            }

            if (result.Success)
            {
                account.Balance -= request.Amount;

                // todo: again, configure using context injection instead of if/else statements
                if (dataStoreType == "Backup")
                {
                    var accountDataStore = new BackupAccountDataStore();
                    accountDataStore.UpdateAccount(account);
                }
                if (dataStoreType == "Test")
                {
                    var accountDataStore = new TestDataStore();
                    accountDataStore.UpdateAccount(account);
                }
                else
                {
                    var accountDataStore = new AccountDataStore();
                    accountDataStore.UpdateAccount(account);
                }
            }
            return result;
        }
    }
}