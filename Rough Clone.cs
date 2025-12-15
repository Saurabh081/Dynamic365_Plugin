using System;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Services;

namespace GetFieldValue
{
    // clone the record using all fields in c# plugins V.U in dynamic365
    public class Class1 : IPlugin

    {
        public void Execute(IServiceProvider serviceProvider)
        {

            //Initializing Service Context.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    tracingService.Trace("Execution Started");

                    Entity target = (Entity)context.InputParameters["Target"];
                    Entity student = service.Retrieve(target.LogicalName, target.Id, new ColumnSet(true));

                    //======================================get a field value of different data type fields------------------------

                    //get a field value of a single line of text
                    var studentName = student.GetAttributeValue<string>("le_name");
                    tracingService.Trace("Student Name:-" + studentName);
                    //get a field value of a date and time field 
                    DateTime dateOFBirth = student.GetAttributeValue<DateTime>("le_dateofbirth");
                    tracingService.Trace("Date Of BIrth:-" + dateOFBirth);
                    // get a field value of a multiple option set field value
                    var semester = student.GetAttributeValue<OptionSetValue>("le_semester").Value;
                    tracingService.Trace("Semester:-" + semester);
                    // get a field text of a multiple option set field value 
                    var semestertext = student.FormattedValues["le_semester"];
                    tracingService.Trace("Semester:-" + semestertext);
                    // get a field value of a multiselect option set field value
                    OptionSetValueCollection subject = (OptionSetValueCollection)student["le_subjectdeatils"];
                    for (int i = 0; i < subject.Count; i++)
                    {
                        tracingService.Trace("Subject:-" + subject[i].Value);
                    }
                    // get a field text of a multiselect option set field value
                    var subjecttext = student.FormattedValues["le_subjectdeatils"];
                    tracingService.Trace("Semester:-" + subjecttext);
                    //get a field value of a phone no, data type field
                    var contactNo = student.GetAttributeValue<string>("le_contactno");
                    tracingService.Trace("ContactNo:-" + contactNo);
                    //get a field value of a phone no, data type field
                    var fatherNo = student.GetAttributeValue<string>("le_fathersphoneno");
                    tracingService.Trace("Father's Phone No.:-" + fatherNo);
                    //get a field value of a phone no, data type field
                    var motherNo = student.GetAttributeValue<string>("le_mothersphoneno");
                    tracingService.Trace("Mother's Phone No" + motherNo);
                    // get a field value of a look up data type field
                    EntityReference studentClass = (EntityReference)student.Attributes["le_class"];
                    tracingService.Trace("Class:-" + studentClass.Name + "\tClass id" + studentClass.Id);
                    // get a field value of a look up data type field
                    EntityReference teacher = (EntityReference)student.Attributes["le_teacher"];
                    tracingService.Trace("Teacher Name" + teacher.Name + "\tTeacher id" + teacher.Id);


                    //--------------------------------set a field value for different data types fields -------------------------

                    Entity NewStudent = new Entity("le_student");
                    NewStudent["le_name"] = studentName;
                    long d = dateOFBirth.Ticks;
                    NewStudent["le_dateofbirth"] = new DateTime(d);
                    NewStudent["le_semester"] = new OptionSetValue(semester);
                    NewStudent["le_subjectdeatils"] = new OptionSetValueCollection(subject);
                    NewStudent["le_contactno"] = contactNo;
                    NewStudent["le_fathersphoneno"] = fatherNo;
                    NewStudent["le_mothersphoneno"] = motherNo;
                    NewStudent["le_class"] = new EntityReference(studentClass.LogicalName, studentClass.Id);
                    NewStudent["le_teacher"] = new EntityReference(teacher.LogicalName, teacher.Id);

                    service.Create(NewStudent);

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
        }
    }
}