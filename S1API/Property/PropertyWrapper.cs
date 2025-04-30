using Il2CppScheduleOne.Interaction;
using Il2CppScheduleOne.Misc;
using Il2CppScheduleOne.Delivery;
using System.Collections.Generic;
using UnityEngine;
using Il2CppScheduleOne.Tiles;
using System.Linq;

namespace S1API.Property
{
    /// <summary>
    /// Represents a wrapper class for handling properties derived from the
    /// Il2CppScheduleOne.Property.Property class. Provides an abstraction for
    /// interacting with property details and operations in Unity.
    /// </summary>
    public class PropertyWrapper : BaseProperty 
    {
        /// <summary>
        /// A readonly backing field encapsulating the core property instance
        /// used within the PropertyWrapper class. This field provides access
        /// to the underlying implementation of the property functionalities
        /// and is leveraged across multiple overriden members to delegate
        /// operations to the actual property instance.
        /// </summary>
        #if IL2CPPBEPINEX || IL2CPPMELON
        internal readonly Il2CppScheduleOne.Property.Property InnerProperty;
        #else
        internal readonly ScheduleOne.Property.Property InnerProperty;
        #endif
        /// <summary>
        /// A wrapper class that extends the functionality of <see cref="BaseProperty"/>
        /// and acts as a bridge to interact with an inner property implementation
        /// from the Il2CppScheduleOne.Property namespace.
        /// </summary>
#if IL2CPPBEPINEX || IL2CPPMELON
        public PropertyWrapper(Il2CppScheduleOne.Property.Property property)

        #else
        public PropertyWrapper(ScheduleOne.Property.Property property)
#endif
        {
            InnerProperty = property;
        }

        /// <summary>
        /// Gets the name of the property.
        /// Represents the underlying property name as defined by its implementation.
        /// </summary>
        public override string PropertyName => InnerProperty.PropertyName;

        /// <summary>
        /// Gets the unique code representing this property. This code serves as an identifier
        /// for distinguishing the property in the system and is typically defined in the internal
        /// implementation of the property.
        /// </summary>
        public override string PropertyCode => InnerProperty.PropertyCode;

        /// <summary>
        /// Gets the price of the property.
        /// </summary>
        /// <remarks>
        /// The price represents a floating-point value that denotes the monetary
        /// value or cost associated with the property. This property is read-only
        /// and retrieves the value from the underlying property implementation.
        /// </remarks>
        public override float Price => InnerProperty.Price;

        /// <summary>
        /// Gets a value indicating whether the property is currently owned.
        /// </summary>
        /// <remarks>
        /// This property reflects the ownership status of the property. Returns true if the property
        /// is owned and false otherwise. The ownership status is based on the internal state of the
        /// wrapped property implementation.
        /// </remarks>
        public override bool IsOwned => InnerProperty.IsOwned;

        /// <summary>
        /// Represents the maximum number of employees that can be allocated to the property.
        /// This property is both readable and writable, allowing for dynamic configuration
        /// of employee capacity based on the property's current requirements or constraints.
        /// </summary>
        public override int EmployeeCapacity
        {
            get => InnerProperty.EmployeeCapacity;
            set => InnerProperty.EmployeeCapacity = value;
        }

        /// <summary>
        /// Marks the property as owned within the PropertyWrapper implementation.
        /// Updates the ownership status by delegating the operation to the underlying
        /// Il2CppScheduleOne.Property.Property instance.
        /// This is typically used to signify that the property has been acquired or purchased.
        /// </summary>
        public override void SetOwned()
        {
            InnerProperty.SetOwned();
        }

        /// <summary>
        /// Determines whether a specified point lies within the boundary of the property.
        /// </summary>
        /// <param name="point">The point to check, specified as a Vector3 coordinate.</param>
        /// <returns>true if the point is within the property's boundary; otherwise, false.</returns>
        public override bool IsPointInside(Vector3 point)
        {
            return InnerProperty.DoBoundsContainPoint(point);
        }

        

        // NPCSpawnPoint handling - where NPC spawn as well as where they go to disappear when fired
        public Transform NPCSpawnPoint
        {
            get => InnerProperty.NPCSpawnPoint;
            set => InnerProperty.NPCSpawnPoint = value;
        }

        public void SetNPCSpawnLocation(Vector3 position) => NPCSpawnPoint.position = position;


        // Grids HAVE to be parented to the property they're for or they'll miss a GetComponentInParent<Property>() check later on the grid tile class
        public void ReparentGridObjectToProperty(Transform transform)
        {
            // find existing grid to see if there's already a spot to put them
            Transform existingGrid = InnerProperty?.GetComponentInChildren<Grid>()?.transform;
            Transform gridParent = existingGrid?.transform?.parent;

            // make one if not
            if (gridParent == null)
            {
                gridParent = new GameObject("Grids").transform;
                gridParent.SetParent(InnerProperty.transform, true);
            }

            transform.SetParent(gridParent, true);
        }


        // Realty handling
        public void RegisterRealtyPosterAndDialogue(Transform listingPoster, string listingTitle, bool addSalesConversation = true)
        {

            RegisterAndReparentPoster(listingPoster);

            if (addSalesConversation)
                InjectRealtyDialogueOption(listingTitle);
        }

        public void RegisterAndReparentPoster(Transform listingPoster)
        {
            // Register the listing poster with the property
            InnerProperty.ListingPoster = listingPoster;
            Transform raysWhiteBoard = GameObject.Find("/Map/Container/RE Office/Interior/Whiteboard")?.transform;
            InnerProperty.ListingPoster.SetParent(raysWhiteBoard, true);
        }

        public void InjectRealtyDialogueOption(string listingTitle)
        {
            // Inject the dialogue option for the property into the ray conversation using S1API.Dialogue
            // honestly I think adding a property to the dialogue like this should just be added straight to S1API.Dialogue

            /*
             * Helpful constants pulled from manormod:
            
            const string TargetDialogueContainerName = "EstateAgent_Sell";
            const string PropertyChoiceNodeGuid = "8e2ef594-96d9-43f2-8cfa-6efaea823a56";

            // Guid of the node that asks if you're sure you want to buy the property.
            const string ConfirmationNodeGuid = "b6f6a4ce-849c-4047-b705-7a20440de0e0"; // Target for Manor choice link

            // Node of "CONFIRM_BUY". If they get here he's saying "Outstanding", and they HAVE confirmed their purchase.
            const string PurchaseNodeGuid = "80b9c95c-384b-4314-be00-18f5b035ed10";

            */
        }

        // Employee Idle Points handling (must have enough to match employee capacity)
        public void AddEmployeeIdlePoint(Transform idlePoint) { }
        public void AddEmployeeIldePoints(List<Transform> idlePoints) { }

        public void RemoveEmployeeIdlePoint(Transform idlePoint) { }
        public void ClearEmployeeIdlePoints() { }

        // don't know if these below are considered leaking types or how we deal with that, could just always feed transform or something?


        // LoadingDocks handling (for deliveries)

        public void AddLoadingDock(LoadingDock loadingDock)
        {
            // il2cpp aint gonna like this I can tell you that right now lol
            var current_docks = InnerProperty.LoadingDocks;
            var current_docks_list = current_docks.ToList();
            current_docks_list.Add(loadingDock);
            InnerProperty.LoadingDocks = current_docks_list.ToArray();
        }

        public void AddLoadingDocks(List<LoadingDock> loadingDocks)
        {
            // il2cpp aint gonna like this I can tell you that right now lol
            var current_docks = InnerProperty.LoadingDocks;
            var current_docks_list = current_docks.ToList();
            current_docks_list.AddRange(loadingDocks);
            InnerProperty.LoadingDocks = current_docks_list.ToArray();
        }

        // I wont need any of the remove or clear functions for manormod but they seem useful for other mods
        public void RemoveLoadingDock(LoadingDock loadingDock)
        {
            var current_docks = InnerProperty.LoadingDocks;
            var current_docks_list = current_docks.ToList();
            current_docks_list.Remove(loadingDock);
            InnerProperty.LoadingDocks = current_docks_list.ToArray();
        }
        public void ClearLoadingDocks()
        {
            InnerProperty.LoadingDocks = new LoadingDock[0];
        }


        // Switches handling (lights)
        public void AddSwitch(ModularSwitch modularSwitch)
        {
            InnerProperty.Switches.Add(modularSwitch);
        }
        public void AddSwitches(List<ModularSwitch> modularSwitches)
        {
            for (int i = 0; i < modularSwitches.Count; i++)
                InnerProperty.Switches.Add(modularSwitches[i]);
        }

        public void RemoveSwitch(ModularSwitch modularSwitch)
        {
            InnerProperty.Switches.Remove(modularSwitch);
        }
        public void ClearSwitches()
        {
            InnerProperty.Switches.Clear();
        }

        // Toggleables handling (Blinds)
        public void AddToggleable(InteractableToggleable toggleable)
        {
            InnerProperty.Toggleables.Add(toggleable);
        }
        public void AddToggleables(List<InteractableToggleable> toggleables)
        {
            for (int i = 0; i < toggleables.Count; i++)
                InnerProperty.Toggleables.Add(toggleables[i]);
        }

        public void RemoveToggleable(InteractableToggleable toggleable)
        {
            InnerProperty.Toggleables.Remove(toggleable);
        }
        public void ClearToggleables()
        {
            InnerProperty.Toggleables.Clear();
        }
    }
}