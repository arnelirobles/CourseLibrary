using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CourseLibrary.API.Defaults
{
    public static class IdentityDefaults
    {
        
        public static List<Client> GetClients()
        {
            var gt = new List<string>();
            gt.AddRange(GrantTypes.ResourceOwnerPassword.ToList());
            gt.Add("delegation");
            return new List<Client>
            {
                new Client
                {
                    ClientId = "APPLICABLENZ",
                    ClientName = "Admin Dashboard",
                    Description = "Dashboard application for the Courses API",
                    AllowedGrantTypes = gt,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api_full"
                    },
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AccessTokenLifetime = 3600,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 86400,
                    AbsoluteRefreshTokenLifetime = 604800
                }
            };
        }

        public static List<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                { 
                    Name = "api",
                    Description = "Main Course API",
                    ApiSecrets =
                    {
                        new Secret("4a5a9c3fad2a4a5db10d37dc966a2dd4".Sha256())
                    },
                    Scopes =
                    {
                        new Scope()
                        {
                            Name = "api_full",
                            DisplayName = "Full API Access",
                            Description = "Full access to the API (Get, Post, Put, Delete)"
                        },
                        new Scope()
                        {
                            Name = "api_read",
                            DisplayName = "Read Only API Access",
                            Description = "Read only access to the API  (Get)"
                        },
                        new Scope()
                        {
                            Name = "api_write",
                            DisplayName = "Write/Update Only API Access",
                            Description = "Write and update only access to the API  (Get, Post, Put)"
                        }
                    }
                }
            };
        }

        public static List<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }
    }
}
