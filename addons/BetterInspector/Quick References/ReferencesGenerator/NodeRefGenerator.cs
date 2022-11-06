using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class NodeRefGenerator : ISourceGenerator
{
    private struct Property
    {
        public string name;
        public string type;

        public Property(string name, string type)
        {
            this.name = name;
            this.type = type;
        }
    }

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var root = syntaxTree.GetRoot();

            // Get Fields with Require attribute
            var fieldsWithRequireAttribute = root
                    .DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .Where(field => field.AttributeLists
                            .Any(attributes => attributes.Attributes
                                    .Any(attribute => attribute.Name.ToString() is "NodeRef")));

            List<Property> properties = new();

            foreach (var field in fieldsWithRequireAttribute)
                foreach (var variable in field.Declaration.Variables)
                {
                    properties.Add(new Property(
                        variable.Identifier.ToString(),
                        field.Declaration.Type.ToString()));
                }

            if (properties.Count is 0) continue;

            StringBuilder sb = new();

            var @class = root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>().First();

            var usings = root.DescendantNodes()
                    .OfType<UsingDirectiveSyntax>();

            var @namespace = root.DescendantNodes()
                    .OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

            foreach (var use in usings)
                sb.AppendLine(use.ToString());
            sb.AppendLine();
            if (@namespace is not null)
            {
                sb.AppendLine($"namespace {@namespace?.Name.ToString()}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"partial class {@class.Identifier.ToString()} : {@class.BaseList?.Types.First().Type.ToString()}");
            sb.AppendLine("{");
            foreach (var prop in properties)
            {
                sb.AppendLine($"    [Export, TypedPath(typeof({prop.type}))]");
                sb.AppendLine($"    private NodePath _{prop.name};");
            }
            sb.AppendLine();
            sb.AppendLine("    public override void _Ready()");
            sb.AppendLine("    {");
            foreach (var prop in properties)
                sb.AppendLine($"        {prop.name} = GetNode<{prop.type}>(_{prop.name});");
            sb.AppendLine("        OnReady();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    partial void OnReady();");
            sb.AppendLine("}");
            if (@namespace is not null)
                sb.AppendLine("}");

            context.AddSource($"{@class.Identifier.ToString()}.g.cs", sb.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }
}
