// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2017 Tony Sneed.

using System;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Scaffolding.Handlebars.Tests.Fakes
{
    public class FakeAnnotationCodeGenerator : IAnnotationCodeGenerator
    {
        public string GenerateFluentApi(IModel model, IAnnotation annotation, string language)
        {
            throw new NotImplementedException();
        }

        public string GenerateFluentApi(IEntityType entityType, IAnnotation annotation, string language)
        {
            throw new NotImplementedException();
        }

        public string GenerateFluentApi(IKey key, IAnnotation annotation, string language)
        {
            throw new NotImplementedException();
        }

        public string GenerateFluentApi(IProperty property, IAnnotation annotation, string language)
        {
            throw new NotImplementedException();
        }

        public string GenerateFluentApi(IForeignKey foreignKey, IAnnotation annotation, string language)
        {
            throw new NotImplementedException();
        }

        public string GenerateFluentApi(IIndex index, IAnnotation annotation, string language)
        {
            throw new NotImplementedException();
        }

        public bool IsHandledByConvention(IModel model, IAnnotation annotation)
        {
            throw new NotImplementedException();
        }

        public bool IsHandledByConvention(IEntityType entityType, IAnnotation annotation)
        {
            throw new NotImplementedException();
        }

        public bool IsHandledByConvention(IKey key, IAnnotation annotation)
        {
            throw new NotImplementedException();
        }

        public bool IsHandledByConvention(IProperty property, IAnnotation annotation)
        {
            throw new NotImplementedException();
        }

        public bool IsHandledByConvention(IForeignKey foreignKey, IAnnotation annotation)
        {
            throw new NotImplementedException();
        }

        public bool IsHandledByConvention(IIndex index, IAnnotation annotation)
        {
            throw new NotImplementedException();
        }
    }
}
