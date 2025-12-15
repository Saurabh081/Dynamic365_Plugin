using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace Associate_Disassociatestudentcount
{
    public class StudentCountUpdater : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Service Setup
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            try
            {
                tracingService.Trace("Plugin execution started");

                
                if (!context.InputParameters.Contains("Target") ||
                    !(context.InputParameters["Target"] is EntityReference))
                {
                    tracingService.Trace("Invalid target parameter");
                    return;
                }

                
                if (context.MessageName != "Associate" && context.MessageName != "Disassociate")
                    return;

                
                Relationship relationship = (Relationship)context.InputParameters["Relationship"];
                if (relationship.SchemaName != "crc4f_Course_crc4f_Student_crc4f_Student")
                {
                    tracingService.Trace($"Skipping unrelated relationship: {relationship.SchemaName}");
                    return;
                }

                
                EntityReference courseRef = (EntityReference)context.InputParameters["Target"];
                EntityReferenceCollection studentRefs = (EntityReferenceCollection)context.InputParameters["RelatedEntities"];

                
                Entity course = service.Retrieve(
                    courseRef.LogicalName,
                    courseRef.Id,
                    new Microsoft.Xrm.Sdk.Query.ColumnSet("crc4f_count"));

                int currentCount = course.GetAttributeValue<int>("crc4f_count");
                int newCount = currentCount;

                
                if (context.MessageName == "Associate")
                {
                    newCount += studentRefs.Count;
                    tracingService.Trace($"Adding {studentRefs.Count} students");
                }
                else 
                {
                    newCount = Math.Max(0, currentCount - studentRefs.Count);
                    tracingService.Trace($"Removing {studentRefs.Count} students");
                }

                
                if (newCount != currentCount)
                {
                    Entity updateCourse = new Entity(courseRef.LogicalName, courseRef.Id);
                    updateCourse["crc4f_count"] = newCount;
                    service.Update(updateCourse);
                    tracingService.Trace($"Updated {courseRef.Name} count: {currentCount} → {newCount}");
                }

                tracingService.Trace("Plugin execution completed");
            }
            catch (Exception ex)
            {
                tracingService.Trace($"ERROR: {ex.ToString()}");
                throw new InvalidPluginExecutionException($"Student count update failed: {ex.Message}");
            }
        }
    }
}