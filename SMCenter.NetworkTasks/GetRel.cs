using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

namespace SMCenter.NetworkTasks
{
    // Класс для возврата Relationship при единичной связи
    // использован для лучшей читаемости кода.
     static class _Rel
    {
         public static EnterpriseManagementRelationshipObject<EnterpriseManagementObject> GetSingleRelationship(EnterpriseManagementGroup emg, ManagementPackRelationship classRelationship, Guid Id, bool IdObjectIsSource)
         {
             try
             {
                 EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relObject = null;
                 if (IdObjectIsSource)
                 {
                     var items = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Id, classRelationship, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                     foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in items)
                     {
                         relObject = rel;
                     }
                 }
                 else
                 {
                     var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Id, classRelationship, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                     foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in items)
                     {
                         relObject = rel;
                     }
                 }
                 return relObject;
             }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("_Rel procedure error : " + ex.Message, "Service Manager");
                return null;
            }
         }
         
    }
}
