using System.Configuration;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Types;
using NUnit.Framework;
using FluentAssertions;

namespace ClearBank.DeveloperTest.Tests;

[TestFixture]
public class PaymentServiceTests
{
    private PaymentService paymentService;

    [SetUp]
    public void init()
    {
        paymentService = new PaymentService();
    }

    [Test]
    public void MakePaymentRequest_MissingPaymentScheme_Throws()
    {
        // arrange
        // note: DAMP not DRY better for tests? strongly typed config may be preferred ("Backup" vs "aackup")
        ConfigurationManager.AppSettings["DataStoreType"] = "Test";
        var requestWithoutPaymentScheme = new MakePaymentRequest
        {
            CreditorAccountNumber = "1",
            DebtorAccountNumber = "2",
            Amount = 3,
            PaymentDate = new DateTime()
        };

        // act
        Action makePaymentWithoutPaymentScheme = () => paymentService.MakePayment(requestWithoutPaymentScheme);

        // assert
        makePaymentWithoutPaymentScheme.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void MakePaymentRequest_UpdateAccount_Throws()
    {
        // arrange
        ConfigurationManager.AppSettings["DataStoreType"] = "Test";
        var validRequest = new MakePaymentRequest
        {
            CreditorAccountNumber = "2",
            DebtorAccountNumber = "1",
            Amount = 3,
            PaymentDate = new DateTime(),
            PaymentScheme =
                PaymentScheme
                    .Bacs // as TestDataStore.GetAccount Account.AllowedPaymentSchemes == AllowedPaymentSchemes.Bacs
        };

        // act
        Action makePaymentWithoutPaymentScheme = () => paymentService.MakePayment(validRequest);

        // assert
        makePaymentWithoutPaymentScheme.Should().Throw<NotImplementedException>();
    }

    [Test]
    public void MakePaymentRequest_InvalidRequest_ReturnsUnsuccessfulResult()
    {
        // arrange
        ConfigurationManager.AppSettings["DataStoreType"] = "Test";
        var invalidRequest = new MakePaymentRequest
        {
            CreditorAccountNumber = "2",
            DebtorAccountNumber = "1",
            Amount = 3,
            PaymentDate = new DateTime(),
            PaymentScheme =
                PaymentScheme.Chaps // PaymentService "Test" Account.AllowedPaymentSchemes != PaymentScheme.Chaps
        };
        var expectedUnsucessfulResult = new MakePaymentResult
        {
            Success = false
        };

        // act
        var response = paymentService.MakePayment(invalidRequest);

        // assert
        response.Should().BeEquivalentTo(expectedUnsucessfulResult);
    }
}