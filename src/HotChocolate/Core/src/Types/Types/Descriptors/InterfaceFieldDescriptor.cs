using System;
using System.Linq;
using System.Reflection;
using HotChocolate.Language;
using HotChocolate.Types.Descriptors.Definitions;
using HotChocolate.Types.Helpers;

namespace HotChocolate.Types.Descriptors;

public class InterfaceFieldDescriptor
    : OutputFieldDescriptorBase<InterfaceFieldDefinition>
    , IInterfaceFieldDescriptor
{
    private bool _argumentsInitialized;

    protected internal InterfaceFieldDescriptor(
        IDescriptorContext context,
        NameString fieldName)
        : base(context)
    {
        Definition.Name = fieldName.EnsureNotEmpty(nameof(fieldName));
    }

    protected internal InterfaceFieldDescriptor(
        IDescriptorContext context,
        InterfaceFieldDefinition definition)
        : base(context)
    {
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
    }

    protected internal InterfaceFieldDescriptor(
        IDescriptorContext context,
        MemberInfo member)
        : base(context)
    {
        Definition.Member = member
            ?? throw new ArgumentNullException(nameof(member));

        Definition.Name = context.Naming.GetMemberName(
            member,
            MemberKind.InterfaceField);
        Definition.Description = context.Naming.GetMemberDescription(
            member,
            MemberKind.InterfaceField);
        Definition.Type = context.TypeInspector.GetOutputReturnTypeRef(member);

        if (context.Naming.IsDeprecated(member, out var reason))
        {
            Deprecated(reason);
        }

        if (member is MethodInfo m)
        {
            Parameters = m.GetParameters().ToDictionary(t => new NameString(t.Name));
        }
    }

    protected internal override InterfaceFieldDefinition Definition { get; protected set; } = new();

    protected override void OnCreateDefinition(InterfaceFieldDefinition definition)
    {
        if (!Definition.AttributesAreApplied && Definition.Member is not null)
        {
            Context.TypeInspector.ApplyAttributes(
                Context,
                this,
                Definition.Member);
            Definition.AttributesAreApplied = true;
        }

        base.OnCreateDefinition(definition);

        CompleteArguments(definition);
    }

    private void CompleteArguments(InterfaceFieldDefinition definition)
    {
        if (!_argumentsInitialized && Parameters.Any())
        {
            FieldDescriptorUtilities.DiscoverArguments(
                Context,
                definition.Arguments,
                definition.Member);
            _argumentsInitialized = true;
        }
    }

    public new IInterfaceFieldDescriptor SyntaxNode(FieldDefinitionNode fieldDefinitionNode)
    {
        base.SyntaxNode(fieldDefinitionNode);
        return this;
    }

    public new IInterfaceFieldDescriptor Name(NameString name)
    {
        base.Name(name);
        return this;
    }

    public new IInterfaceFieldDescriptor Description(string description)
    {
        base.Description(description);
        return this;
    }

    [Obsolete("Use `Deprecated`.")]
    public IInterfaceFieldDescriptor DeprecationReason(string reason) =>
        Deprecated(reason);

    public new IInterfaceFieldDescriptor Deprecated(string reason)
    {
        base.Deprecated(reason);
        return this;
    }

    public new IInterfaceFieldDescriptor Deprecated()
    {
        base.Deprecated();
        return this;
    }

    public new IInterfaceFieldDescriptor Type<TOutputType>()
        where TOutputType : IOutputType
    {
        base.Type<TOutputType>();
        return this;
    }

    public new IInterfaceFieldDescriptor Type<TOutputType>(TOutputType outputType)
        where TOutputType : class, IOutputType
    {
        base.Type(outputType);
        return this;
    }

    public new IInterfaceFieldDescriptor Type(ITypeNode type)
    {
        base.Type(type);
        return this;
    }

    public new IInterfaceFieldDescriptor Type(Type type)
    {
        base.Type(type);
        return this;
    }

    public new IInterfaceFieldDescriptor Argument(
        NameString name,
        Action<IArgumentDescriptor> argument)
    {
        base.Argument(name, argument);
        return this;
    }

    public new IInterfaceFieldDescriptor Ignore(bool ignore = true)
    {
        base.Ignore(ignore);
        return this;
    }

    public new IInterfaceFieldDescriptor Directive<T>(T directive)
        where T : class
    {
        base.Directive(directive);
        return this;
    }

    public new IInterfaceFieldDescriptor Directive<T>()
        where T : class, new()
    {
        base.Directive<T>();
        return this;
    }

    public new IInterfaceFieldDescriptor Directive(
        NameString name,
        params ArgumentNode[] arguments)
    {
        base.Directive(name, arguments);
        return this;
    }

    public static InterfaceFieldDescriptor New(
        IDescriptorContext context,
        NameString fieldName) =>
        new InterfaceFieldDescriptor(context, fieldName);

    public static InterfaceFieldDescriptor New(
        IDescriptorContext context,
        MemberInfo member) =>
        new InterfaceFieldDescriptor(context, member);

    public static InterfaceFieldDescriptor From(
        IDescriptorContext context,
        InterfaceFieldDefinition definition) =>
        new InterfaceFieldDescriptor(context, definition);
}
