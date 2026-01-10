namespace PCIShield.BlazorMauiShared.Auth
{
    public class LoginUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public string MerchantId { get; set; }
        public string MerchantNumber { get; set; }
        public string ComplianceOfficerId { get; set; }
        public string ComplianceOfficerNumber { get; set; }
        public string Department { get; set; }
    }
}