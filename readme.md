Solution contains:

1. Models: a class library project using only the `Microsoft.Orleans.Serialization.Abstractions` package 
   so record types used in grains messages can be annotated with `[GenerateSerializer]`. This is intended 
   as a contracts assembly, so we want to keep the Orleans dependencies to a minimum.
 
2. Hosting Orleans project: this contains the grain and full codegen.


The grain implements two strategies (methods) that showcase the issue (which is a codegen one):

1. `Deposit(Deposit message)`: the message type is a record in a referenced project, annotated with 
   with `[GenerateSerializer]`. The type/assembly is referenced and opted-in for referenced assembly 
   codegen via `[assembly: GenerateCodeForDeclaringAssembly(typeof(Deposit))]`

2. `Deposit2(Deposit2 message)`: the message type is a record in the same project as the grain, 
   also annotated with `[GenerateSerializer]`.

Other than the declaring project, there is no difference between the two.

Reproduce the bug:

1. Run the hosting project. 
2. Navigate to https://localhost:7125/account/1/deposit/100. The response should be the new balance. 
   Note how it's always an empty response.
3. Navigate to https://localhost:7125/account/1/deposit2/100. The response IS the new balance. 
   Every refresh appends more to the balance, which is the correct response.

After hitting 3., you can go back to 2. and see that what you get is the last balance updated by `Deposit2` 
(since there is only one state, to eliminate issues with state persistence). But you can never increment.

The generated codec for one in-project vs the referenced one differs as follows:

```csharp
public void Serialize<TBufferWriter>(ref global::Orleans.Serialization.Buffers.Writer<TBufferWriter> writer, global::OrleansGeneratorBug.Deposit2 instance)
    where TBufferWriter : global::System.Buffers.IBufferWriter<byte>
{
    global::Orleans.Serialization.Codecs.DecimalCodec.WriteField(ref writer, 0U, instance.Amount);
    writer.WriteEndBase();
}
```

The referenced type is not writing the Amount value at all:

```csharp
[global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
public void Serialize<TBufferWriter>(ref global::Orleans.Serialization.Buffers.Writer<TBufferWriter> writer, global::Models.Deposit instance)
    where TBufferWriter : global::System.Buffers.IBufferWriter<byte>
{
    writer.WriteEndBase();
}
```
