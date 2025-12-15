using System;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloneRecordPlugin
{
    public class Clone_Record_Studen : IPlugin
    {
        // Task to clone the student record on update of name field using crm all deta types  using c# plugins 
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            try
            {
             
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity && context.PrimaryEntityName == "edu_student")
                {
                    Guid studentId = context.PrimaryEntityId;
                    Entity studentRecord = service.Retrieve("edu_student", studentId, new ColumnSet("edu_name", "edu_decimal", "edu_emailid", "edu_date", "edu_color", "edu_contact", "edu_currency", "edu_account", "edu_float"));

                    if (studentRecord != null)
                    {
                        string studentName = studentRecord.GetAttributeValue<string>("edu_name");
                        string studentEmailid = studentRecord.GetAttributeValue<string>("edu_emailid");
                        decimal studentDecimal = studentRecord.GetAttributeValue<decimal>("edu_decimal");
                        DateTime studentDate = studentRecord.GetAttributeValue<DateTime>("edu_date");
                        int studentContact = studentRecord.GetAttributeValue<int>("edu_contact");
                        EntityReference studentLookup = studentRecord.GetAttributeValue<EntityReference>("edu_account");
                        OptionSetValue colourOptionSet = studentRecord.GetAttributeValue<OptionSetValue>("edu_color");
                        OptionSetValueCollection currencymultiOptionSet = studentRecord.GetAttributeValue<OptionSetValueCollection>("edu_currency");
                        double studentFloat = studentRecord.GetAttributeValue<double>("edu_float");

                        Entity newStudentRecord = new Entity("edu_student");
                        newStudentRecord["edu_name"] = studentName;      // name string
                        newStudentRecord["edu_emailid"] = studentEmailid; // string
                        newStudentRecord["edu_decimal"] = studentDecimal; // decimal
                        newStudentRecord["edu_date"] = studentDate;       // Date & Time  
                        newStudentRecord["edu_contact"] = studentContact; // Integer
                        newStudentRecord["edu_account"] = studentLookup;  // lookup
                        newStudentRecord["edu_color"] = colourOptionSet;   // optionset
                        newStudentRecord["edu_currency"] = currencymultiOptionSet; // multioptionset
                        newStudentRecord["edu_float"] = studentFloat;       // float


                        service.Create(newStudentRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException($"An error occurred in Clone_Record_Studen plugin {ex.Message}", ex);
            }
        }
    }
}
