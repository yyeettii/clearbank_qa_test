using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data;

public class TestDataStore
{
    public Account GetAccount(string accountNumber)
    {
        return new Account
        {
            AccountNumber = accountNumber,
            Balance = 0,
            Status = AccountStatus.Live,
            AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs
        };
    }

    public void UpdateAccount(Account account)
    {
        throw new NotImplementedException("UpdateAccount not yet implemented");
    }
}