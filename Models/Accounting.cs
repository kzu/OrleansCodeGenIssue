using Orleans;

namespace Models;

[GenerateSerializer]
public partial record AccountState(decimal Balance);

[GenerateSerializer]
public partial record Deposit(decimal Amount);
