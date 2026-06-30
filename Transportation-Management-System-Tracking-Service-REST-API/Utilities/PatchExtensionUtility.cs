using System.Reflection;

namespace Transportation_Management_System_Tracking_Service_REST_API.Utilities
{
    public static class PatchExtensions
    {
        public static void PatchFrom<TSource, TDestination>(
            this TDestination destination,
            TSource source
        )
        {
            if (source == null || destination == null)
                throw new ArgumentNullException("Source or Destination object cannot be null");

            PatchObjects(destination, source);
        }

        private static void PatchObjects(object destination, object source)
        {
            Type sourceType = source.GetType();
            Type destType = destination.GetType();

            foreach (
                PropertyInfo sourceProperty in sourceType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance
                )
            )
            {
                if (!sourceProperty.CanRead)
                    continue;

                PropertyInfo? destProperty = destType.GetProperty(
                    sourceProperty.Name,
                    BindingFlags.Public | BindingFlags.Instance
                );
                if (destProperty == null || !destProperty.CanWrite)
                    continue;

                object? sourceValue = sourceProperty.GetValue(source, null);
                if (sourceValue == null)
                    continue;

                if (
                    destProperty.PropertyType != typeof(string)
                    && destProperty.PropertyType.IsClass
                )
                {
                    object? destValue = destProperty.GetValue(destination, null);
                    if (destValue == null)
                    {
                        if (destProperty.PropertyType.GetConstructor(Type.EmptyTypes) == null)
                        {
                            continue;
                        }

                        destValue = Activator.CreateInstance(destProperty.PropertyType);
                        if (destValue == null)
                            continue;

                        destProperty.SetValue(destination, destValue, null);
                    }

                    PatchObjects(destValue, sourceValue);
                    continue;
                }

                if (destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    destProperty.SetValue(destination, sourceValue, null);
                }
            }
        }
    }
}
