using System;
using System.Collections.Generic;
using System.Web.Security;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace AssignSecurityRole
{
    internal class AssignRoleByName : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity && context.PrimaryEntityName == "crc4f_student")
                {
                    var userId = entity.Id;
                    tracingService.Trace($"User ID is: {userId}");


                    // retriving card role 
                    var query = new QueryExpression("role")
                    {
                        ColumnSet = new ColumnSet("roleid"),
                        Criteria =
                        {
                            Conditions =
                            {
                                new ConditionExpression("name", ConditionOperator.Equal, "Cards Role")
                            }
                        }
                    };
                    var roles = service.RetrieveMultiple(query);


                    Guid roleId = roles.Entities[0].Id;
                    tracingService.Trace($"Role ID is: {roleId}");

                    AssociateRequest request = new AssociateRequest
                    {
                        Target = new EntityReference("systemuser", userId),
                        RelatedEntities = new EntityReferenceCollection
                    {
                         new EntityReference("role", roleId)
                        },
                        Relationship = new Relationship("systemuserroles_association")
                    };

                    service.Execute(request);



                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in the IdentifyDuplicateRecord plugin.", ex);
            
            }
        }
    }
}
