﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

using Microsoft.Azure.Commands.HealthcareApis.Common;
using Microsoft.Azure.Commands.HealthcareApis.Models;
using Microsoft.Azure.Commands.ResourceManager.Common.ArgumentCompleters;
using Microsoft.Azure.Management.HealthcareApis;
using Microsoft.Azure.Management.HealthcareApis.Models;
using Microsoft.Rest.Azure;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.HealthcareApis.Commands
{

    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "HealthcareApisService", DefaultParameterSetName = ListParameterSet), OutputType(typeof(PSHealthcareApisService))]
    public class GetAzureRmHealthcareApisService : HealthcareApisBaseCmdlet
    {
        protected const string ServiceNameParameterSet = "ServiceNameParameterSet";
        protected const string ResourceIdParameterSet = "ResourceIdParameterSet";
        protected const string ListParameterSet = "ListParameterSet";

        [Parameter(
           Mandatory = true,
           ParameterSetName = ServiceNameParameterSet,
           HelpMessage = "Resource Group Name.")]
        [Parameter(
           Mandatory = false,
           ParameterSetName = ListParameterSet,
           HelpMessage = "Resource Group Name.")]  
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(
          Mandatory = true,
          ParameterSetName = ServiceNameParameterSet,
          HelpMessage = "HealthcareApis Service Name.")]
        [Alias(HealthcareApisAccountNameAlias, FhirServiceNameAlias)]
        [ValidateNotNullOrEmpty]
        [ValidatePattern("^[a-z0-9][a-z0-9-]{1,21}[a-z0-9]$")]
        [ValidateLength(2, 64)]
        public string Name { get; set; }


        [Parameter(
           Mandatory = true,
           ParameterSetName = ResourceIdParameterSet,
           ValueFromPipelineByPropertyName = true,
           HelpMessage = "Resource Id Name.")]
        [ResourceIdCompleter("Microsoft.HealthcareApis/services")]
        [ValidateNotNullOrEmpty]
        public string ResourceId { get; set; }

        public override void ExecuteCmdlet()
        {
            base.ExecuteCmdlet();

            RunCmdLet(() =>
            {
                switch (ParameterSetName)
                {
                    case ServiceNameParameterSet:
                        {
                            var healthcareApisAccount = this.HealthcareApisClient.Services.Get(this.ResourceGroupName, this.Name);
                            WriteHealthcareApisAccount(healthcareApisAccount);
                            break;
                        }
                    case ResourceIdParameterSet:
                        {
                            string resourceGroupName;
                            string resourceName;

                            if (ValidateAndExtractName(this.ResourceId, out resourceGroupName, out resourceName))
                            {
                                var healthcareApisAccount = this.HealthcareApisClient.Services.Get(resourceGroupName, resourceName);
                                WriteHealthcareApisAccount(healthcareApisAccount);
                            }
                            break;
                        }
                    case ListParameterSet:
                        {
                            if (string.IsNullOrEmpty(this.ResourceGroupName))
                            {
                                IPage<ServicesDescription> healthcareApisServicesBySubscription = this.HealthcareApisClient.Services.List();
                                this.WriteObject(ToPSFhirServices(healthcareApisServicesBySubscription), enumerateCollection: true);
                                break;
                            }
                            else
                            {
                                IPage<ServicesDescription> healthcareApisServicesResourceGroup = this.HealthcareApisClient.Services.ListByResourceGroup(this.ResourceGroupName);
                                this.WriteObject(ToPSFhirServices(healthcareApisServicesResourceGroup), enumerateCollection: true);
                                break;
                            }
                        }
                }
            });
        }
    }
}
