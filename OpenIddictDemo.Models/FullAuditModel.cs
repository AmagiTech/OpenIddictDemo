using OpenIddictDemo.Models.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenIddictDemo.Models
{
    public abstract class FullAuditModel<T> : IIdentityModel<T>, IAuditedModel, IActivatableModel, ISoftDeletable
    {
        [Key]
        public T Id { get; set; }

        [StringLength(ModelConstants.MAX_USERID_LENGTH)]
        [Column(TypeName = "VARCHAR")]
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        [StringLength(ModelConstants.MAX_USERID_LENGTH)]
        [Column(TypeName = "VARCHAR")]
        public string? LastModifiedUserId { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
    }
}
