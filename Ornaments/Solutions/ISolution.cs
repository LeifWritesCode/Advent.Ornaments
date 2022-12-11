namespace Ornaments.Solutions;

public interface ISolution
{
    bool TryParse(string input, out object parsed);

    Task<object> DoPartOneAsync(ISolutionContext solutionContext);

    Task<object> DoPartTwoAsync(ISolutionContext solutionContext);
}
