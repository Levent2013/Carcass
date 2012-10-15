using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using Carcass.Common.Utility;

namespace Carcass.Common.Data.Extensions
{
    public static class MapperExtensions
    {
        /// <summary>
        /// Map creation method used to ignore all non-existing in <c>TDestination</c> properties
        /// Got here: http://stackoverflow.com/a/6474397
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="expression">Current expression</param>
        /// <returns>Updated expression</returns>
        public static IMappingExpression<TSource, TDestination>
                IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = Mapper.GetAllTypeMaps()
                .First(x => x.SourceType.Equals(sourceType) && x.DestinationType.Equals(destinationType));
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }

            return expression;
        }


        public static TDestination MapTo<TDestination>(this object source)
        {
            CreateMapping<TDestination>(source);
            return Mapper.Map<TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            CreateMapping<TSource, TDestination>();
            return Mapper.Map<TDestination>(source);
        }

        public static TDestination MapToDynamic<TSource, TDestination>(this TSource source)
        {
            CreateMapping<TSource, TDestination>();
            return Mapper.DynamicMap<TDestination>(source);
        }

        public static TDestination MapInto<TSource, TDestination>(this TSource source, TDestination destination)
        {
            Throw.IfNullArgument(destination, "destination");
            CreateMapping<TSource, TDestination>();
            return Mapper.Map<TSource, TDestination>(source, destination);
        }

        public static void MapIntoDynamic<TSource, TDestination>(this TSource source, TDestination destination)
        {
            Throw.IfNullArgument(destination, "destination");
            CreateMapping<TSource, TDestination>();
            Mapper.DynamicMap<TSource, TDestination>(source, destination);
        }

        private static void CreateMapping<TDestination>(object source)
        {
            // Check mapping exists - if not we can create it and check all properties map correctly
            if (Mapper.FindTypeMapFor(source.GetType(), typeof(TDestination)) == null)
            {
                Mapper.CreateMap(source.GetType(), typeof(TDestination));
                Mapper.AssertConfigurationIsValid();
            }
        } 

        private static void CreateMapping<TSource, TDestination>()
        {
            // Check mapping exists - if not we can create it and check all properties map correctly
            if (Mapper.FindTypeMapFor<TSource, TDestination>() == null)
            {
                Mapper.CreateMap<TSource, TDestination>();
                Mapper.AssertConfigurationIsValid();
            }
        } 

    }
}
