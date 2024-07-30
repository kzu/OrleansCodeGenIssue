using Models;

// Try to get codegen going for the referenced project
[assembly: GenerateCodeForDeclaringAssembly(typeof(Deposit))]

namespace OrleansGeneratorBug;

// Showcases that things work as expected for in-project types
[GenerateSerializer]
public partial record AccountState2(decimal Balance);

[GenerateSerializer]
public partial record Deposit2(decimal Amount);

public interface IAccountingGrain : IGrainWithStringKey
{
    // Exercises referenced project type
    Task<decimal> Deposit(Deposit message);
    // Exercises in-project type
    Task<decimal> Deposit2(Deposit2 message);
}

public class AccountingGrain : Grain, IAccountingGrain
{
    readonly IPersistentState<AccountState> state;

    public AccountingGrain([PersistentState("Accounting")] IPersistentState<AccountState> state)
        => this.state = state;

    public async Task<decimal> Deposit(Deposit message) => await DepositImpl(message.Amount);

    public async Task<decimal> Deposit2(Deposit2 message) => await DepositImpl(message.Amount);

    async Task<decimal> DepositImpl(decimal amount)
    {
        var balance = state.State.Balance + amount;
        state.State = state.State with { Balance = balance };
        await state.WriteStateAsync();
        return balance;
    }
}
