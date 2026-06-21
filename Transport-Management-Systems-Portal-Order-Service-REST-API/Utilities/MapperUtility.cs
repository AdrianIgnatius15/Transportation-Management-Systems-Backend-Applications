namespace Transport_Management_Systems_Portal_Order_Service_REST_API.Utilities
{
    public static class MapperUtility
    {
        public static TDestination Map<TSource, TDestination>(TSource source)
            where TDestination : new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            TDestination destination = new TDestination();
            var sourceProperties = typeof(TSource).GetProperties();
            var destinationProperties = typeof(TDestination).GetProperties();

            foreach (var sourceProp in sourceProperties)
            {
                var destProp = destinationProperties.FirstOrDefault(p => p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);
                if (destProp != null && destProp.CanWrite)
                {
                    destProp.SetValue(destination, sourceProp.GetValue(source));
                }
            }

            return destination;
        }
    }
}
