using Microsoft.Extensions.Options;
using System.DirectoryServices;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Ldap
{
    public class LdapAuthService : ILdapAuthService
    {
        private readonly IConfiguration _config;

        public LdapAuthService(IConfiguration config)
        {
            _config = config;
        }
        public bool ValidateCredentials(string userName, string passWord)
        {
            try
            {
                string[] userParts = userName.Split('\\');
                string domain = userParts[0];
                string user = userParts.Length > 1 ? userParts[1] : userParts[0];

                string ldapPath = GetLdapPath(domain);
                using (DirectoryEntry entry = new DirectoryEntry(ldapPath, userName, passWord))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        var result = searcher.FindOne();
                        if (result != null)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // if we get an error, it means we have a login failure.  
                // Log specific exception  
                return false;
            }
        }
        private string GetLdapPath(string domain)
        {
            var ldapConfig = _config.GetSection("Ldap");
            if (ldapConfig.GetSection("Domains").GetSection(domain).Exists())
            {
                return ldapConfig.GetSection("Domains").GetSection(domain).Value;
            }

            // Si le domaine n'est pas spécifié, utilisez le chemin par défaut
            return ldapConfig.GetSection("DefaultDomain").Value;
        }
    }
}
