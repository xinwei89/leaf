﻿// Copyright (c) 2019, UW Medicine Research IT, University of Washington
// Developed by Nic Dobbins and Cliff Spital, CRIO Sean Mooney
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Model.Options
{
    public class AuthenticationOptions
    {
        public const string Saml2 = @"SAML2";
        public const string Unsecured = @"UNSECURED";
        public const string ActiveDirectory = @"ACTIVEDIRECTORY";

        public static readonly IEnumerable<string> LocalMechanisms = new string[] { Unsecured, ActiveDirectory };
        public static readonly IEnumerable<string> FederatedMechanisms = new string[] { Saml2 };
        public static readonly IEnumerable<string> ValidMechanisms = LocalMechanisms.Concat(FederatedMechanisms);

        public Uri LogoutURI { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public int InactiveTimeoutMinutes { get; set; }
        public AuthenticationMechanism Mechanism { get; set; }

        static bool ValidMechanism(string mech) => ValidMechanisms.Contains(mech);
        public AuthenticationOptions WithMechanism(string value)
        {
            var tmp = value.ToUpper();
            if (!ValidMechanism(tmp))
            {
                throw new LeafConfigurationException($"{value} is not a supported authentication mechanism");
            }

            switch (tmp)
            {
                case Saml2:
                    Mechanism = AuthenticationMechanism.Saml2;
                    break;
                case ActiveDirectory:
                    Mechanism = AuthenticationMechanism.ActiveDirectory;
                    break;
                default:
                    Mechanism = AuthenticationMechanism.Unsecured;
                    break;
            }

            return this;
        }

        public AuthenticationOptions WithLogoutURI(string value)
        {
            LogoutURI = new Uri(value);
            return this;
        }

        public bool IsSaml2 => Mechanism == AuthenticationMechanism.Saml2;
        public bool IsActiveDirectory => Mechanism == AuthenticationMechanism.ActiveDirectory;
        public bool IsUnsecured => Mechanism == AuthenticationMechanism.Unsecured;
    }

    public class SAML2AuthenticationOptions : IBindTarget
    {
        public const AuthenticationMechanism Mechanism = AuthenticationMechanism.Saml2;
        public SAML2AuthenticationHeaderMappingOptions Headers { get; set; }

        public bool DefaultEqual()
        {
            return Headers == null;
        }
    }

    public class ActiveDirectoryAuthenticationOptions : IBindTarget
    {
        public const AuthenticationMechanism Mechanism = AuthenticationMechanism.ActiveDirectory;
        public DomainConnectionOptions DomainConnection { get; set; }

        public bool DefaultEqual()
        {
            return DomainConnection == null;
        }
    }

    public class SAML2AuthenticationHeaderMappingOptions
    {
        public string ScopedIdentity { get; set; }
    }

    public enum AuthenticationMechanism
    {
        Unsecured = 0,
        ActiveDirectory = 1,
        Saml2 = 2
    }
}
