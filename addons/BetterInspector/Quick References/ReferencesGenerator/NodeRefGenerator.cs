using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator]
public class NodeRefGenerator : ISourceGenerator
{
    private record Property
    {
        public readonly string name;
        public readonly string type;
        public readonly string foldout;

        public Property(string name, string type, string foldout)
        {
            this.name = name;
            this.type = type;
            this.foldout = foldout;
        }
    }

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var syntaxTree in context.Compilation.SyntaxTrees)
        {
            var root = syntaxTree.GetRoot();

            // Get Fields with attributes
            var fieldsAttribute = root
                    .DescendantNodes()
                    .OfType<FieldDeclarationSyntax>()
                    .Where(field => field.AttributeLists.Count > 0);

            List<Property> properties = new();

            foreach (var field in fieldsAttribute)
            {
                AttributeSyntax? attribute = null;

                foreach (AttributeListSyntax attributeList in field.AttributeLists)
                {
                    if (attributeList.Attributes.Any(anyAttribute =>
                    {
                        if (anyAttribute.Name.ToString() is "NodeRef")
                        {
                            attribute = anyAttribute;
                            return true;
                        }
                        return false;
                    }))
                        break;
                }

                if (attribute == null)
                    continue;

                string type = field.Declaration.Type.ToString();
                string foldout = "\"References\"";

                // Get specified foldout name
                if (attribute.ArgumentList != null)
                    foreach (var argument in attribute.ArgumentList.Arguments)
                    {
                        if (argument.NameEquals?.Name.ToString() == "foldout")
                        {
                            foldout = argument.Expression.ToString();
                            break;
                        }
                    }

                foreach (var variable in field.Declaration.Variables)
                {
                    properties.Add(new Property(
                        variable.Identifier.ToString(),
                        type,
                        foldout));
                }
            }

            if (properties.Count is 0)
                continue;

            var @class = root.DescendantNodes()
                    .OfType<ClassDeclarationSyntax>().First();

            var usings = root.DescendantNodes()
                    .OfType<UsingDirectiveSyntax>();

            var @namespace = root.DescendantNodes()
                    .OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();

            StringBuilder sb = new();

            foreach (var use in usings)
                sb.AppendLine(use.ToString());
            sb.AppendLine();
            if (@namespace is not null)
            {
                sb.AppendLine($"namespace {@namespace?.Name.ToString()}");
                sb.AppendLine("{");
            }
            
            sb.AppendLine($"partial class {@class.Identifier} : {@class.BaseList?.Types.First()?.Type.ToString()}");
            sb.AppendLine("{");
            foreach (var prop in properties)
            {
                sb.AppendLine($"    [Export, TypedPath(typeof({prop.type})), InFoldout({prop.foldout}, position = FoldoutPosition.Bottom)]");
                sb.AppendLine($"    private NodePath _{prop.name};");
            }
            sb.AppendLine();
            sb.AppendLine("    public override void _Ready()");
            sb.AppendLine("    {");
            foreach (var prop in properties)
                sb.AppendLine($"        {prop.name} = GetNodeOrNull<{prop.type}>(_{prop.name});");
            sb.AppendLine("        OnReady();");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    partial void OnReady();");
            sb.AppendLine("}");
            if (@namespace is not null)
                sb.AppendLine("}");

            context.AddSource($"{@class.Identifier}.g.cs", sb.ToString());
        }
    }

    public void Initialize(GeneratorInitializationContext context) { }
}
