namespace RefactoringChallenge.Model.Domain;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsVip { get; set; }
    public DateTime RegistrationDate { get; set; }
}
