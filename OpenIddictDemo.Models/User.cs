using Microsoft.AspNetCore.Identity;
using OpenIddictDemo.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OpenIddictDemo.Models
{
    public class User : IdentityUser<Guid>, IIdentityModel<Guid>, IAuditedModel, IActivatableModel, ISoftDeletable
    {
        public User()
        {
            Id = Guid.NewGuid();
            SecurityStamp = Guid.NewGuid().ToString();
        }

        [StringLength(ModelConstants.MAX_NAME_LENGTH)]
        public string Name { get; set; }

        [StringLength(ModelConstants.MAX_NAME_LENGTH)]
        public string Surname { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public User(string userName) : this()
        {
            UserName = userName;
        }
    }
}
