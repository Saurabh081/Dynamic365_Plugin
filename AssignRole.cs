using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace AssignSecurityRole
{
    public class AssignRole : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                //if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity && context.PrimaryEntityName == "crc4f_student" && context.MessageName == "Create")
                {
                    {
                        // Get Guid of current user
                        Guid userId = context.InitiatingUserId;
                        tracingService.Trace("Displaying user ID: {0}", userId);


                        // guid of Cards Role
                        Guid roleId = new Guid("f9e46d0f-d53e-4d5c-b06b-24948b4606ed"); // cards role
                        tracingService.Trace("Card Role ID : {0}", roleId);
                        //Guid approvalsRoleId = new Guid("b9b08637-acf6-e711-a95a-000d3a11f5ee"); // approvla user
                        //tracingService.Trace("Approvals User ID: {0}", approvalsRoleId);


                        // Assign the role to the user
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

            }

            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in the IdentifyDuplicateRecord plugin.", ex);
            }
        }
    }
}

