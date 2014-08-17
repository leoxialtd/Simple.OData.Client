﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;

namespace Simple.OData.Client
{
    public sealed partial class EdmEntitySet
    {
        public static EdmEntitySet FromModel(IEdmEntitySet entitySet)
        {
            return new EdmEntitySet
            {
                Name = entitySet.Name,
                EntityType = GetEntityTypeName(entitySet.Type),
            };
        }

        private static string GetEntityTypeName(IEdmType type)
        {
            var typeName = type.FullTypeName();
            const string collectionPrefix = "Collection(";
            if (typeName.StartsWith(collectionPrefix))
                return typeName.Substring(collectionPrefix.Length, typeName.Length - collectionPrefix.Length - 1);
            else
                return typeName;
        }
    }

    public sealed partial class EdmEntityType
    {
        public static EdmEntityType FromModel(IEdmEntityType type)
        {
            return new EdmEntityType
            {
                Namespace = type.Namespace,
                Name = type.Name,
                BaseType = FromModel(type.BaseType),
                Abstract = type.IsAbstract,
                OpenType = type.IsOpen,
                Key = EdmKey.FromModel(type.DeclaredKey),
                Properties = type.DeclaredProperties.Where(x => x.PropertyKind == EdmPropertyKind.Structural)
                    .Select(x => EdmProperty.FromModel(x as IEdmStructuralProperty)).ToArray(),
                NavigationProperties = type.NavigationProperties().Select(EdmNavigationProperty.FromModel).ToArray(),
            };
        }

        public static EdmEntityType FromModel(IEdmStructuredType type)
        {
            return type == null ? null : FromModel(type as IEdmEntityType);
        }
    }

    public sealed partial class EdmComplexType
    {
        public static EdmComplexType FromModel(IEdmComplexType type)
        {
            return new EdmComplexType
            {
                Namespace = type.Namespace,
                Name = type.Name,
                Properties = type.StructuralProperties().Select(EdmProperty.FromModel).ToArray(),
            };
        }
    }

    public sealed partial class EdmEnumType
    {
        public static EdmEnumType FromModel(IEdmEnumType type)
        {
            return new EdmEnumType
            {
                Namespace = type.Namespace,
                Name = type.Name,
            };
        }
    }

    public sealed partial class EdmProperty
    {
        public static EdmProperty FromModel(IEdmStructuralProperty property)
        {
            return new EdmProperty()
            {
                Name = property.Name,
                Type = EdmPropertyType.FromModel(property.Type),
                Nullable = property.Type.IsNullable,
                ConcurrencyMode = property.ConcurrencyMode.ToString(),
            };
        }
    }

    public sealed partial class EdmNavigationProperty
    {
        public static EdmNavigationProperty FromModel(IEdmNavigationProperty property)
        {
            return new EdmNavigationProperty()
            {
                // TODO
                Name = property.Name,
                PartnerName = (property.Partner.DeclaringType as IEdmEntityType).Name,
                FromRole = property.Name, // TODO
                ToRole = property.Partner.Name, // TODO
                Relationship = "", // TODO
                Multiplicity = GetMultiplicityString(property.Partner.TargetMultiplicity()),
            };
        }

        private static string GetMultiplicityString(EdmMultiplicity multiplicity)
        {
            switch (multiplicity)
            {
                case EdmMultiplicity.ZeroOrOne:
                    return "0..1";
                case EdmMultiplicity.One:
                    return "1";
                case EdmMultiplicity.Many:
                    return "*";
                default:
                    throw new ArgumentException("Invalid multiplicity " + multiplicity);
            }
        }
    }

    public sealed partial class EdmKey
    {
        public static EdmKey FromModel(IEnumerable<IEdmStructuralProperty> properties)
        {
            return properties == null ? null : new EdmKey()
            {
                Properties = properties.Select(x => x.Name).ToArray()
            };
        }
    }

    public sealed partial class EdmParameter
    {
        public static EdmParameter FromModel(IEdmOperationParameter parameter)
        {
            return new EdmParameter
            {
                Name = parameter.Name,
                Type = EdmPropertyType.FromModel(parameter.Type),
                Mode = "", // TODO
            };
        }
    }
}
