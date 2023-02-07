using System.ComponentModel.DataAnnotations;

namespace MAUIDnr1.Models;

public class PlayList : ICloneable
{
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = "";
    public DateTime DateCreated { get; set; }
    public List<Show> Shows { get; set; } = new List<Show>();

    public static Guid CreateGuid()
    {
        var obj = new object();
        var rnd = new Random(obj.GetHashCode());
        var bytes = new byte[16];
        rnd.NextBytes(bytes);
        return new Guid(bytes);
    }

    public object Clone()
    {
        return new PlayList()
        {
            Id = this.Id,
            Name = this.Name,
            DateCreated = this.DateCreated,
            Shows = this.Shows
        };
    }
}