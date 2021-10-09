using CommonData.Entities;
using System.ComponentModel.DataAnnotations;

namespace CommonData.DTOs
{
    public class AcceptedEmailDomainDTO
    {
        public int OrganisationId { get; set; }
        public string DomainName { get; set; }

        public static explicit operator AcceptedEmailDomain(AcceptedEmailDomainDTO dto) => new AcceptedEmailDomain
        {
            OrganisationId = dto.OrganisationId,
            DomainName = dto.DomainName
        };
        public static explicit operator AcceptedEmailDomainDTO(AcceptedEmailDomain domain) => new AcceptedEmailDomainDTO
        {
            OrganisationId = domain.OrganisationId,
            DomainName = domain.DomainName
        };
    }
}
