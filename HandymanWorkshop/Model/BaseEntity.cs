using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HandymanWorkshop.Model
{
    public abstract class BaseEntity
    {
        [JsonIgnore]
        public int Id { get; set; }

        public Guid Uuid { get; set; } = Guid.NewGuid();

        public bool IsDeleted { get; set; } = false;

        public DateTime? DeletedAt { get; set; }

    }
}
