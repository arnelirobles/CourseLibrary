using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseLibrary.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _authorPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {nameof(AuthorDto.Id), new PropertyMappingValue(new List<string>() {nameof(Author.Id)}) },
                {nameof(AuthorDto.MainCategory), new PropertyMappingValue(new List<string>() {nameof(Author.MainCategory)}) },
                {nameof(AuthorDto.Age), new PropertyMappingValue(new List<string>() {nameof(Author.DateOfBirth)},true) },
                { nameof(AuthorDto.Name), new PropertyMappingValue(new List<string>() {nameof(Author.FirstName), nameof(Author.LastName) }) }
            };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<AuthorDto, Author>(_authorPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception("Cannot find exact property mapping instance for" + $" < {typeof(TSource)}, {typeof(TDestination)}");
        }
    }
}