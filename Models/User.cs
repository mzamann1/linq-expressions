
namespace LinqExpressions.Models;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }

    public override string ToString()
    {
        return $"Name => {Name} , Age => {Age.ToString()}";
    }
}