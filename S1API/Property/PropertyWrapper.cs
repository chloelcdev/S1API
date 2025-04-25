using UnityEngine;

namespace S1API.Property
{
    public class PropertyWrapper : BaseProperty // <-- NO ABSTRACT HERE
    {
        internal readonly Il2CppScheduleOne.Property.Property InnerProperty;

        public PropertyWrapper(Il2CppScheduleOne.Property.Property property)
        {
            InnerProperty = property;
        }

        public override string PropertyName => InnerProperty.PropertyName;
        public override string PropertyCode => InnerProperty.PropertyCode;
        public override float Price => InnerProperty.Price;
        public override bool IsOwned => InnerProperty.IsOwned;

        public override int EmployeeCapacity
        {
            get => InnerProperty.EmployeeCapacity;
            set => InnerProperty.EmployeeCapacity = value;
        }

        public override void SetOwned()
        {
            InnerProperty.SetOwned();
        }

        public override bool IsPointInside(Vector3 point)
        {
            return InnerProperty.DoBoundsContainPoint(point);
        }
    }
}