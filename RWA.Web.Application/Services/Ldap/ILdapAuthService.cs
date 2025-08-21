namespace RWA.Web.Application.Services.Ldap
{
    public interface ILdapAuthService
    {
        bool ValidateCredentials(string userName, string passWord);
    }
}
