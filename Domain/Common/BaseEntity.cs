namespace DataAnalysis.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
    public int? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public int? DeletedBy { get; set; }

    protected BaseEntity()
    {
        CreatedDate = DateTime.UtcNow;
        IsDeleted = false;
    }
}