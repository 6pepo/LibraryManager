using System;

class Loan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime DueDate { get; set; }
    public bool IsReturned { get; set; } = false;
}

