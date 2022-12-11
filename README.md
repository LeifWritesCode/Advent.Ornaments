# Ornaments SDK
An Advent of Code solutions runner and standard library.

## Quick usage

1. Add reference to Ornaments to your project.
2. In your main method, add...

```csharp
await AdventApp.CreateDefault().RunAsync(args);
```

3. Add a new class implementing `ISolution`.
4. Decorate (3) with a `SolutionAttribute` describing name, year, and date.
5. Build and run using `dotnet <yourdllname> solve -y <year> -d <day>`.

## Todo

- [ ] Implement core runner. (In progress)
- [ ] Implement data model.
- [ ] Implement input fetch support.
- [ ] Implement submission support.
- [ ] Implement existing standard library classses.
- [ ] Implement wait support.

