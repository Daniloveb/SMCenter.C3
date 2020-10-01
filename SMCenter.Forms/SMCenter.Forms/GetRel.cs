using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;

namespace SMCenter.Forms
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

         public static EnterpriseManagementObject GetObjectFromSingleRelationship(EnterpriseManagementGroup emg, ManagementPackRelationship classRelationship, Guid Id, bool IdObjectIsSource)
         {
             try
             {
                 EnterpriseManagementObject emo_Object = null;
                 if (IdObjectIsSource)
                 {
                     var items = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Id, classRelationship, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                     foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in items)
                     {
                         emo_Object = rel.TargetObject;
                     }
                 }
                 else
                 {
                     var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Id, classRelationship, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                     foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in items)
                     {
                         emo_Object = rel.SourceObject;
                     }
                 }
                 return emo_Object;
             }
             catch (Exception ex)
             {
                 System.Windows.MessageBox.Show("_Rel procedure error : " + ex.Message, "Service Manager");
                 return null;
             }
         }

         public static List<EnterpriseManagementObject> GetCollection(EnterpriseManagementGroup emg, ManagementPackRelationship classRelationship, ManagementPackClass FilterClass ,Guid Id, bool IdObjectIsSource)
         {
             try
             {
             List<EnterpriseManagementObject> _Collection = new List<EnterpriseManagementObject>();
             List<EnterpriseManagementObject> Collection = new List<EnterpriseManagementObject>();
             if (IdObjectIsSource)
             {
                 var items = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Id, classRelationship, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                 foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in items)
                 {
                     _Collection.Add(rel.TargetObject);
                 }
             }
             else
             {
                 var items = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Id, classRelationship, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                 foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel in items)
                 {
                     _Collection.Add(rel.SourceObject);
                 }
             }
                
             //Filter
             if (FilterClass != null)
             {
                 foreach (EnterpriseManagementObject E in _Collection)
                 {
                     if (E.IsInstanceOf(FilterClass)) Collection.Add(E);
                 }
             }
             else
             {
                 Collection = _Collection;
             }

             return Collection;
             }
             catch (Exception ex)
             {
                 System.Windows.MessageBox.Show("_Rel GetCollection procedure error : " + ex.Message, "Service Manager");
                 return null;
             }
         }
    }
}
