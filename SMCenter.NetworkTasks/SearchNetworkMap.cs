using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using System.Collections.ObjectModel;
using System.Windows;

namespace SMCenter.NetworkTasks
{
    class SearchNetworkMap
    {
        //private Guid NetworkAdapter_Guid;
        private ManagementPackRelationship relChildNetworkAdapterRefParentNetworkAdapter { get; set; }
        private ManagementPackRelationship relNetworkMapRefLocation { get; set; }
        private ManagementPackRelationship relLoctoLoc { get; set; }
        private ManagementPackRelationship relConfigItemRefLocation { get; set; }
        private ManagementPackRelationship relConfigItemRefRack { get; set; }
        private EnterpriseManagementGroup emg;

        //private EnterpriseManagementObject NetworkMap { get; set; }
        //private EnterpriseManagementObject LinkedLocation { get; set; }


        public SearchNetworkMap(EnterpriseManagementGroup _emg, ManagementPackRelationship _relConfigItemRefLocation, ManagementPackRelationship _relNetworkMapRefLocation, ManagementPackRelationship _relLoctoLoc, ManagementPackRelationship _relConfigItemRefRack)
        {
            try
        {
            emg = _emg;
            relNetworkMapRefLocation = _relNetworkMapRefLocation;
            relLoctoLoc = _relLoctoLoc;
            relConfigItemRefLocation = _relConfigItemRefLocation;
            relConfigItemRefRack = _relConfigItemRefRack;

        }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("SearchNetworkMap class constructor error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public bool SearchFromLocation(Guid Location_Id, out EnterpriseManagementObject NetworkMap, out EnterpriseManagementObject LinkedLocation)
        {
            try
            {
            NetworkMap = null;
            LinkedLocation = null;

            Guid NetworkMap_Id = new Guid();

            var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(Location_Id, relNetworkMapRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
            {
                NetworkMap_Id = relobj.SourceObject.Id;
                LinkedLocation = relobj.TargetObject;
            }
            //нет связи текущей локации с NetworkMap - ищем родительскую локацию.
            Guid CurrentLocation_Id = Location_Id;
            Guid ParentLocation_Id = new Guid();
            bool Ok = true;
            while (NetworkMap_Id == Guid.Empty && Ok == true)
            {
                //Ищем родительскую локацию.
                T_objects = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(CurrentLocation_Id, relLoctoLoc, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    ParentLocation_Id = relobj.SourceObject.Id;
                }
                //не нашли родительскую локацию - выходим.
                if (T_objects.Count==0)
                {
                    Ok = false;
                }
                //
                T_objects = emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(ParentLocation_Id, relNetworkMapRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    NetworkMap_Id = relobj.SourceObject.Id;
                    LinkedLocation = relobj.TargetObject;
                }
                //Если не нашли связанный NetworkMap - продолжаем цикл с родительской локацией
                if (NetworkMap_Id == Guid.Empty)
                {
                    CurrentLocation_Id = ParentLocation_Id;
                }
            }
            if (NetworkMap_Id == Guid.Empty) { return false; }
            else
            {
                NetworkMap = emg.EntityObjects.GetObject<EnterpriseManagementObject>(NetworkMap_Id, ObjectQueryOptions.Default);
                return true;
            }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("SearchNetworkMap class - SearchFromLocation procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                NetworkMap = null;
                LinkedLocation = null;
                return false;
            }
        }

        public bool SearchFromNode(Guid Node_Id, out EnterpriseManagementObject NetworkMap, out EnterpriseManagementObject LinkedLocation)
        {
            try
            {
            NetworkMap = null;
            LinkedLocation = null;
            Guid Location_Id = new Guid();
            ObservableCollection<Guid> GuidCol = new ObservableCollection<Guid>();
            Guid Rack_Id = new Guid();
            var T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Node_Id, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
            foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
            {
                Location_Id = relobj.TargetObject.Id;
            }
            //Если Node напрямую не связан с локацией - ищем связанный Rack
            if (Location_Id == Guid.Empty)
            {
                T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Node_Id, relConfigItemRefRack, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                {
                    Rack_Id = relobj.TargetObject.Id;
                }
                if (Rack_Id != Guid.Empty)
                {
                    T_objects = emg.EntityObjects.GetRelationshipObjectsWhereSource<EnterpriseManagementObject>(Rack_Id, relConfigItemRefLocation, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default);
                    foreach (EnterpriseManagementRelationshipObject<EnterpriseManagementObject> relobj in T_objects)
                    {
                        Location_Id = relobj.TargetObject.Id;
                    }
                }
            }
            if (Location_Id != Guid.Empty)
            {
                if (SearchFromLocation(Location_Id, out NetworkMap, out LinkedLocation))
                {
                    return true;
                }
                else
                { return false; }
            }
            else
            {
                return false;
            }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("SearchNetworkMap class - SearchFromNode procedure error : " + ex.Message, "Service Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                NetworkMap = null;
                LinkedLocation = null;
                return false;
            }

        }

    }
}
