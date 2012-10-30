using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using MvcExtensions;
using Carcass.Common.Resources;
using Carcass.Models;

namespace Carcass.Models.Configurations
{
    public class UserProfileConfiguration : ModelMetadataConfiguration<UserProfile>
    {
        public UserProfileConfiguration()
        {
            Configure(model => model.Id).AsHidden();
            Configure(model => model.Email)
                .Required()
                .DisplayName(() => AccountResources.Email)
                .MaximumLength(255)
                .Order(1)
                .AsEmail();

            Configure(model => model.FirstName)
                .DisplayName(() => AccountResources.FirstName)
                .MaximumLength(255)
                .Order(2);

            Configure(model => model.LastName)
                .DisplayName(() => AccountResources.LastName)
                .MaximumLength(255)
                .Order(3);

            Configure(model => model.TimeZoneOffset)
              .DisplayName(() => AccountResources.TimeZone)
              .RenderAction("TimeZones", "Account")
              .Order(4);

            // .NullDisplayText("---Please Select---")
        }
    }
}