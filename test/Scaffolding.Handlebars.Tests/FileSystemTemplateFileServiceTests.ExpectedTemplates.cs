﻿namespace Scaffolding.Handlebars.Tests
{
    public partial class FileSystemTemplateFileServiceTests
    {
        private static class ExpectedTemplates
        {
            public const string ContextTemplate =
@"{{> dbimports}}

namespace {{namespace}}
{
    public partial class {{class}} : DbContext
    {
{{{> dbsets}}}

{{{> dbconstructor}}}

{{#unless suppress-on-configuring}}
{{{> dbonconfiguring}}}

{{/unless}}
{{{on-model-creating}}}
    }
}
";

            public const string EntityTemplate =
@"{{> imports}}

namespace {{namespace}}
{
    {{#if comment}}
    /// <summary>
{{comment}}
    /// </summary>
    {{/if}}
    {{#each class-annotations}}
    {{{class-annotation}}}
    {{/each}}
    public partial class {{class}}
    {
        {{{> constructor}}}
        {{{> properties}}}
    }
}
";
        }
    }
}
