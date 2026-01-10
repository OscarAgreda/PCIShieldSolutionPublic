using MediatR;
namespace PCIShield.Infrastructure.Data.Sync
{
    public class CreateDoctorCommand : IRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}