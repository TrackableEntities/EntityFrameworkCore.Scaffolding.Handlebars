namespace Scaffolding.Handlebars.Tests
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
{{#if entity-type-errors}}
{{#each entity-type-errors}}
{{spaces 8}}{{{entity-type-error}}}
{{/each}}

{{/if}}

{{{> dbconstructor}}}

{{{> dbonconfiguring}}}

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
    /// {{comment}}
    /// </summary>
    {{/if}}
    {{#if class-annotation}}
    {{{class-annotation}}}
    {{/if}}
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
