# QA Tech Test

## Notes

 - I had to use VSCode instead of Visual Studio
    - I also used a [Dev Container](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) for speed. The file can be ignored with no negative effect on the solution (or you can use it yourself if you have the same setup)
 - after `dotnet build`ing etc. you can, of course, run tests with `dotnet test`

## Tests

### `MakePaymentRequest_MissingPaymentScheme_Throws`

Rationale: if `PaymentScheme` is not defined in `MakePaymentRequest` then a default enum value is used ([see Enumeration best practices here](https://learn.microsoft.com/en-us/dotnet/api/system.enum?view=net-7.0#:~:text=If%20there%20is,other%20enumerated%20constants.)). As written, `PaymentScheme`'s default value references a real payment scheme. This could cause bugs. The switch in `MakePayment` also has no `default` clause, which doesn't safeguard against unexpected values. Again, this could cause bugs.

### `MakePaymentRequest_UpdateAccount_Throws`

I thought it was interesting that so much of `MakePayment` was dedicated to setting `MakePaymentResult.Success` to `false` when, as implemented, it would already be `false` (default value of `bool`). Seemed like a good candidate to tidy up some of the if/else statements as the inverse of their conditions should set `MakePayment.Success` to `true`.

I hesitate to add mocks to service tests. By making `TestDataStore.UpdateAccount()` throw an exception we verify that the code reaching that path works. If the `MakePayment` logic we refactored changed again tests like this would likely pick it up.

Writing this test was a bit odd. I could easily write something like `MakePaymentRequest_ValidRequest_ReturnsSuccessfulResult` that would verify you get a successful response. From a validation standpoint it didn't feel "right" to do that since `UpdateAccount` being unimplemented means `MakePaymentResult.Success` returning as `true` felt a bit wrong in my opinion. From a testability perspective, all implementations of a `IDataStore`-like interface should probably throw for unimplemented methods.

### `MakePaymentRequest_InvalidRequest_ReturnsUnsuccessfulResult`

Just a simple test to demonstrate a very broad "negative result". It would be tempting to iterate through the enums for the different "negative" scenarios, but their number is pretty limited, so I'd probably recommend not doing that as long as it's manageable. Tests should be simple to read and explicit about what they do/how they do it (as far as that is practical). I prefer asserting against typed objects and not properties because it can detect changes to the underlying object.

### Wrapping up

I may have been able to multiply my effort by installing a code analysis package. They're designed to find subtle but easy to miss errors that lead to unintended side effects (like what `MakePaymentRequest_MissingPaymentScheme_Throws` demonstrates).

The total time I spent on this was more like 2-3 hours. I spent long enough to get to the bottom of a few things that were bother me and "just enough" refactoring, but not enough that would lead to me pointlessly cutting code I could discuss in a conversation. I also didn't count doing this writeup as part of the elapsed time.

In general, it's hard to assess the value of testing like this. It would have been extremely helpful to know what other tests exist, what they exercise, and how they exercise it, as well as having a better understanding of the application architecture.