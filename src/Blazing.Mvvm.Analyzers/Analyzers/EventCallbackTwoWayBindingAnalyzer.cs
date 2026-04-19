using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Blazing.Mvvm.Analyzers.Analyzers;

/// <summary>
/// Detects EventCallback-based automatic two-way binding opportunities and obsolete manual patterns.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EventCallbackTwoWayBindingAnalyzer : DiagnosticAnalyzer
{
    private const string ScenarioPropertyName = "Scenario";
    private const string PropertyNameProperty = "PropertyName";
    private const string CallbackNameProperty = "CallbackName";
    private const string PropertyTypeProperty = "PropertyType";
    private const string HandlerNameProperty = "HandlerName";

    private const string ManualPatternScenario = "ManualPattern";
    private const string MissingCallbackScenario = "MissingCallback";
    private const string TypeMismatchScenario = "TypeMismatch";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.EventCallbackTwoWayBindingManualPattern,
            DiagnosticDescriptors.EventCallbackTwoWayBindingMissingCallback,
            DiagnosticDescriptors.EventCallbackTwoWayBindingTypeMismatch);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        var componentType = context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);
        if (componentType is null || componentType.TypeKind != TypeKind.Class || componentType.IsAbstract)
        {
            return;
        }

        var parameterAttribute = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.ParameterAttribute);
        var viewParameterAttribute = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.ViewParameterAttribute);
        var eventCallbackType = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.EventCallback);
        var mvvmComponentBaseType = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.MvvmComponentBaseMetadataName);
        var mvvmOwningComponentBaseType = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.MvvmOwningComponentBaseMetadataName);
        var mvvmLayoutComponentBaseType = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.MvvmLayoutComponentBaseMetadataName);
        var propertyChangedEventArgsType = context.SemanticModel.Compilation.GetTypeByMetadataName(AnalyzerConstants.TypeNames.PropertyChangedEventArgs);

        if (parameterAttribute is null ||
            viewParameterAttribute is null ||
            eventCallbackType is null ||
            mvvmComponentBaseType is null ||
            mvvmOwningComponentBaseType is null ||
            mvvmLayoutComponentBaseType is null ||
            propertyChangedEventArgsType is null)
        {
            return;
        }

        if (!TryGetViewModelType(componentType, mvvmComponentBaseType, mvvmOwningComponentBaseType, mvvmLayoutComponentBaseType, out var viewModelType))
        {
            return;
        }

        var componentProperties = GetComponentProperties(componentType, parameterAttribute, eventCallbackType);
        var viewModelProperties = GetViewParameterProperties(viewModelType, viewParameterAttribute);
        if (componentProperties.Parameters.Count == 0 || viewModelProperties.Count == 0)
        {
            return;
        }

        var bindings = new Dictionary<string, BindingInfo>(StringComparer.Ordinal);

        foreach (var componentParameter in componentProperties.Parameters.Values)
        {
            if (!viewModelProperties.TryGetValue(componentParameter.Name, out var viewModelProperty))
            {
                continue;
            }

            var callbackName = componentParameter.Name + AnalyzerConstants.Naming.ChangedSuffix;
            if (!componentProperties.Callbacks.TryGetValue(callbackName, out var callbackProperty))
            {
                context.ReportDiagnostic(CreateMissingCallbackDiagnostic(componentParameter, callbackName));
                continue;
            }

            if (!SymbolEqualityComparer.IncludeNullability.Equals(callbackProperty.ValueType, componentParameter.Type))
            {
                context.ReportDiagnostic(CreateTypeMismatchDiagnostic(componentParameter, callbackProperty));
                continue;
            }

            bindings[componentParameter.Name] = new BindingInfo(componentParameter, callbackProperty, viewModelProperty);
        }

        if (bindings.Count == 0)
        {
            return;
        }

        foreach (var methodDeclaration in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
        {
            if (methodDeclaration.Identifier.ValueText != AnalyzerConstants.MethodNames.OnInitialized)
            {
                continue;
            }

            if (TryMatchManualPattern(classDeclaration, context.SemanticModel, methodDeclaration, bindings, propertyChangedEventArgsType, out var match))
            {
                context.ReportDiagnostic(CreateManualPatternDiagnostic(match));
            }
        }
    }

    private static Diagnostic CreateMissingCallbackDiagnostic(ComponentParameterInfo componentParameter, string callbackName)
    {
        var properties = ImmutableDictionary<string, string?>.Empty
            .Add(ScenarioPropertyName, MissingCallbackScenario)
            .Add(PropertyNameProperty, componentParameter.Name)
            .Add(CallbackNameProperty, callbackName)
            .Add(PropertyTypeProperty, componentParameter.TypeName);

        return Diagnostic.Create(
            DiagnosticDescriptors.EventCallbackTwoWayBindingMissingCallback,
            componentParameter.Location,
            properties,
            componentParameter.Name,
            callbackName);
    }

    private static Diagnostic CreateTypeMismatchDiagnostic(ComponentParameterInfo componentParameter, CallbackParameterInfo callbackProperty)
    {
        var properties = ImmutableDictionary<string, string?>.Empty
            .Add(ScenarioPropertyName, TypeMismatchScenario)
            .Add(PropertyNameProperty, componentParameter.Name)
            .Add(CallbackNameProperty, callbackProperty.Name)
            .Add(PropertyTypeProperty, componentParameter.TypeName);

        return Diagnostic.Create(
            DiagnosticDescriptors.EventCallbackTwoWayBindingTypeMismatch,
            callbackProperty.Location,
            properties,
            callbackProperty.Name,
            componentParameter.TypeName,
            componentParameter.Name);
    }

    private static Diagnostic CreateManualPatternDiagnostic(ManualPatternMatch match)
    {
        var properties = ImmutableDictionary<string, string?>.Empty
            .Add(ScenarioPropertyName, ManualPatternScenario)
            .Add(PropertyNameProperty, match.PropertyName)
            .Add(CallbackNameProperty, match.CallbackName)
            .Add(HandlerNameProperty, match.HandlerName);

        return Diagnostic.Create(
            DiagnosticDescriptors.EventCallbackTwoWayBindingManualPattern,
            match.SubscriptionLocation,
            properties,
            match.PropertyName);
    }

    private static bool TryGetViewModelType(
        INamedTypeSymbol componentType,
        INamedTypeSymbol mvvmComponentBaseType,
        INamedTypeSymbol mvvmOwningComponentBaseType,
        INamedTypeSymbol mvvmLayoutComponentBaseType,
        out INamedTypeSymbol viewModelType)
    {
        var current = componentType;
        while (current is not null)
        {
            if (current.BaseType is { } baseType &&
                baseType.TypeArguments.Length == 1 &&
                (SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, mvvmComponentBaseType) ||
                 SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, mvvmOwningComponentBaseType) ||
                 SymbolEqualityComparer.Default.Equals(baseType.OriginalDefinition, mvvmLayoutComponentBaseType)) &&
                baseType.TypeArguments[0] is INamedTypeSymbol namedViewModelType)
            {
                viewModelType = namedViewModelType;
                return true;
            }

            current = current.BaseType;
        }

        viewModelType = null!;
        return false;
    }

    private static ComponentPropertyCollection GetComponentProperties(
        INamedTypeSymbol componentType,
        INamedTypeSymbol parameterAttribute,
        INamedTypeSymbol eventCallbackType)
    {
        var parameters = new Dictionary<string, ComponentParameterInfo>(StringComparer.Ordinal);
        var callbacks = new Dictionary<string, CallbackParameterInfo>(StringComparer.Ordinal);

        foreach (var property in EnumerateInstanceProperties(componentType))
        {
            if (!HasAttribute(property, parameterAttribute) || property.DeclaredAccessibility != Accessibility.Public || property.IsStatic)
            {
                continue;
            }

            if (TryGetEventCallbackValueType(property.Type, eventCallbackType, out var callbackValueType))
            {
                if (property.Name.EndsWith(AnalyzerConstants.Naming.ChangedSuffix, StringComparison.Ordinal))
                {
                    callbacks[property.Name] = new CallbackParameterInfo(property, callbackValueType);
                }

                continue;
            }

            parameters[property.Name] = new ComponentParameterInfo(property);
        }

        return new ComponentPropertyCollection(parameters, callbacks);
    }

    private static Dictionary<string, IPropertySymbol> GetViewParameterProperties(
        INamedTypeSymbol viewModelType,
        INamedTypeSymbol viewParameterAttribute)
    {
        var properties = new Dictionary<string, IPropertySymbol>(StringComparer.Ordinal);

        foreach (var property in EnumerateInstanceProperties(viewModelType))
        {
            if (property.DeclaredAccessibility != Accessibility.Public || property.IsStatic)
            {
                continue;
            }

            if (HasAttribute(property, viewParameterAttribute))
            {
                properties[property.Name] = property;
            }
        }

        return properties;
    }

    private static IEnumerable<IPropertySymbol> EnumerateInstanceProperties(INamedTypeSymbol type)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var current = type;
        while (current is not null)
        {
            foreach (var property in current.GetMembers().OfType<IPropertySymbol>())
            {
                if (seen.Add(property.Name))
                {
                    yield return property;
                }
            }

            current = current.BaseType;
        }
    }

    private static bool HasAttribute(IPropertySymbol property, INamedTypeSymbol attributeType)
        => property.GetAttributes().Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeType));

    private static bool TryGetEventCallbackValueType(ITypeSymbol type, INamedTypeSymbol eventCallbackType, out ITypeSymbol valueType)
    {
        if (type is INamedTypeSymbol namedType &&
            namedType.IsGenericType &&
            SymbolEqualityComparer.Default.Equals(namedType.OriginalDefinition, eventCallbackType))
        {
            valueType = namedType.TypeArguments[0];
            return true;
        }

        valueType = null!;
        return false;
    }

    private static bool TryMatchManualPattern(
        ClassDeclarationSyntax classDeclaration,
        SemanticModel semanticModel,
        MethodDeclarationSyntax onInitializedMethod,
        IReadOnlyDictionary<string, BindingInfo> bindings,
        INamedTypeSymbol propertyChangedEventArgsType,
        out ManualPatternMatch match)
    {
        match = null!;

        if (onInitializedMethod.ParameterList.Parameters.Count != 0 ||
            onInitializedMethod.Body is null ||
            onInitializedMethod.ExpressionBody is not null)
        {
            return false;
        }

        var statements = onInitializedMethod.Body.Statements;
        if (statements.Count is < 1 or > 2)
        {
            return false;
        }

        PropertyChangedSubscription? subscription = null;
        foreach (var statement in statements)
        {
            if (IsBaseLifecycleCall(statement, AnalyzerConstants.MethodNames.OnInitialized))
            {
                continue;
            }

            if (TryGetPropertyChangedSubscription(statement, semanticModel, out var candidateSubscription))
            {
                if (subscription is not null)
                {
                    return false;
                }

                subscription = candidateSubscription;
                continue;
            }

            return false;
        }

        if (subscription is null)
        {
            return false;
        }

        var handlerMethod = classDeclaration.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => method.Identifier.ValueText == subscription.HandlerSymbol.Name);
        if (handlerMethod is null ||
            !TryMatchHandlerMethod(handlerMethod, semanticModel, bindings, propertyChangedEventArgsType, out var propertyName, out var callbackName))
        {
            return false;
        }

        var disposeMethod = classDeclaration.Members.OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(method => IsCanonicalDisposeMethod(method, subscription.HandlerSymbol.Name));
        if (disposeMethod is null)
        {
            return false;
        }

        match = new ManualPatternMatch(
            propertyName,
            callbackName,
            subscription.HandlerSymbol.Name,
            subscription.Statement.GetLocation());
        return true;
    }

    private static bool TryMatchHandlerMethod(
        MethodDeclarationSyntax handlerMethod,
        SemanticModel semanticModel,
        IReadOnlyDictionary<string, BindingInfo> bindings,
        INamedTypeSymbol propertyChangedEventArgsType,
        out string propertyName,
        out string callbackName)
    {
        propertyName = string.Empty;
        callbackName = string.Empty;

        if (!handlerMethod.Modifiers.Any(SyntaxKind.AsyncKeyword) ||
            handlerMethod.Body is null ||
            handlerMethod.ExpressionBody is not null ||
            handlerMethod.ParameterList.Parameters.Count != 2)
        {
            return false;
        }

        var eventArgsParameter = semanticModel.GetDeclaredSymbol(handlerMethod.ParameterList.Parameters[1]) as IParameterSymbol;
        if (eventArgsParameter is null ||
            !SymbolEqualityComparer.Default.Equals(eventArgsParameter.Type, propertyChangedEventArgsType))
        {
            return false;
        }

        if (handlerMethod.Body.Statements.Count != 1 || handlerMethod.Body.Statements[0] is not IfStatementSyntax ifStatement || ifStatement.Else is not null)
        {
            return false;
        }

        if (!TryMatchCondition(ifStatement.Condition, semanticModel, bindings, eventArgsParameter, out propertyName))
        {
            return false;
        }

        if (!bindings.TryGetValue(propertyName, out var binding))
        {
            return false;
        }

        callbackName = binding.Callback.Name;
        return TryMatchCallbackInvocation(ifStatement.Statement, semanticModel, binding);
    }

    private static bool TryMatchCondition(
        ExpressionSyntax condition,
        SemanticModel semanticModel,
        IReadOnlyDictionary<string, BindingInfo> bindings,
        IParameterSymbol eventArgsParameter,
        out string propertyName)
    {
        propertyName = string.Empty;

        if (!TryGetConditionParts(condition, out var leftCondition, out var rightCondition))
        {
            return false;
        }

        if (TryMatchPropertyNameComparison(leftCondition, semanticModel, eventArgsParameter, out var candidatePropertyName) &&
            bindings.TryGetValue(candidatePropertyName, out var binding) &&
            IsPropertyInequality(rightCondition, semanticModel, binding))
        {
            propertyName = candidatePropertyName;
            return true;
        }

        if (TryMatchPropertyNameComparison(rightCondition, semanticModel, eventArgsParameter, out candidatePropertyName) &&
            bindings.TryGetValue(candidatePropertyName, out binding) &&
            IsPropertyInequality(leftCondition, semanticModel, binding))
        {
            propertyName = candidatePropertyName;
            return true;
        }

        return false;
    }

    private static bool TryGetConditionParts(ExpressionSyntax condition, out ExpressionSyntax leftCondition, out ExpressionSyntax rightCondition)
    {
        condition = StripParentheses(condition);
        if (condition is BinaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalAndExpression } andExpression)
        {
            leftCondition = StripParentheses(andExpression.Left);
            rightCondition = StripParentheses(andExpression.Right);
            return true;
        }

        leftCondition = null!;
        rightCondition = null!;
        return false;
    }

    private static bool TryMatchPropertyNameComparison(
        ExpressionSyntax expression,
        SemanticModel semanticModel,
        IParameterSymbol eventArgsParameter,
        out string propertyName)
    {
        propertyName = string.Empty;
        expression = StripParentheses(expression);
        if (expression is not BinaryExpressionSyntax { RawKind: (int)SyntaxKind.EqualsExpression } equalsExpression)
        {
            return false;
        }

        return (IsEventArgsPropertyNameAccess(equalsExpression.Left, semanticModel, eventArgsParameter) &&
                TryGetConstantPropertyName(equalsExpression.Right, semanticModel, out propertyName)) ||
               (IsEventArgsPropertyNameAccess(equalsExpression.Right, semanticModel, eventArgsParameter) &&
                TryGetConstantPropertyName(equalsExpression.Left, semanticModel, out propertyName));
    }

    private static bool IsEventArgsPropertyNameAccess(ExpressionSyntax expression, SemanticModel semanticModel, IParameterSymbol eventArgsParameter)
    {
        expression = StripParentheses(expression);
        if (expression is not MemberAccessExpressionSyntax memberAccess || memberAccess.Name.Identifier.ValueText != "PropertyName")
        {
            return false;
        }

        return semanticModel.GetSymbolInfo(memberAccess.Expression).Symbol is IParameterSymbol parameterSymbol &&
               SymbolEqualityComparer.Default.Equals(parameterSymbol, eventArgsParameter);
    }

    private static bool TryGetConstantPropertyName(ExpressionSyntax expression, SemanticModel semanticModel, out string propertyName)
    {
        var constantValue = semanticModel.GetConstantValue(StripParentheses(expression));
        if (constantValue.HasValue && constantValue.Value is string name && !string.IsNullOrWhiteSpace(name))
        {
            propertyName = name;
            return true;
        }

        propertyName = string.Empty;
        return false;
    }

    private static bool IsPropertyInequality(ExpressionSyntax expression, SemanticModel semanticModel, BindingInfo binding)
    {
        expression = StripParentheses(expression);
        if (expression is not BinaryExpressionSyntax { RawKind: (int)SyntaxKind.NotEqualsExpression } notEqualsExpression)
        {
            return false;
        }

        return (ReferencesViewModelProperty(notEqualsExpression.Left, semanticModel, binding.ViewModelProperty) &&
                ReferencesComponentProperty(notEqualsExpression.Right, semanticModel, binding.ComponentParameter.Symbol)) ||
               (ReferencesViewModelProperty(notEqualsExpression.Right, semanticModel, binding.ViewModelProperty) &&
                ReferencesComponentProperty(notEqualsExpression.Left, semanticModel, binding.ComponentParameter.Symbol));
    }

    private static bool TryMatchCallbackInvocation(StatementSyntax statement, SemanticModel semanticModel, BindingInfo binding)
    {
        statement = statement is BlockSyntax block && block.Statements.Count == 1 ? block.Statements[0] : statement;
        if (statement is not ExpressionStatementSyntax { Expression: AwaitExpressionSyntax { Expression: InvocationExpressionSyntax invocation } })
        {
            return false;
        }

        if (semanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol ||
            methodSymbol.Name != AnalyzerConstants.MethodNames.InvokeAsync ||
            invocation.ArgumentList.Arguments.Count != 1)
        {
            return false;
        }

        if (!ReferencesCallbackProperty(invocation.Expression, semanticModel, binding.Callback.Symbol))
        {
            return false;
        }

        return ReferencesViewModelProperty(invocation.ArgumentList.Arguments[0].Expression, semanticModel, binding.ViewModelProperty);
    }

    private static bool ReferencesCallbackProperty(ExpressionSyntax expression, SemanticModel semanticModel, IPropertySymbol callbackProperty)
    {
        expression = StripParentheses(expression);
        return expression is MemberAccessExpressionSyntax { Name.Identifier.ValueText: var methodName } memberAccess &&
               methodName == AnalyzerConstants.MethodNames.InvokeAsync &&
               semanticModel.GetSymbolInfo(memberAccess.Expression).Symbol is IPropertySymbol propertySymbol &&
               SymbolEqualityComparer.Default.Equals(propertySymbol, callbackProperty);
    }

    private static bool ReferencesViewModelProperty(ExpressionSyntax expression, SemanticModel semanticModel, IPropertySymbol viewModelProperty)
        => semanticModel.GetSymbolInfo(StripParentheses(expression)).Symbol is IPropertySymbol propertySymbol &&
           SymbolEqualityComparer.Default.Equals(propertySymbol, viewModelProperty);

    private static bool ReferencesComponentProperty(ExpressionSyntax expression, SemanticModel semanticModel, IPropertySymbol componentProperty)
        => semanticModel.GetSymbolInfo(StripParentheses(expression)).Symbol is IPropertySymbol propertySymbol &&
           SymbolEqualityComparer.Default.Equals(propertySymbol, componentProperty);

    private static bool IsCanonicalDisposeMethod(MethodDeclarationSyntax declaration, string handlerName)
    {
        if (declaration.Identifier.ValueText != AnalyzerConstants.MethodNames.Dispose ||
            declaration.ParameterList.Parameters.Count != 1 ||
            declaration.ParameterList.Parameters[0].Type is not PredefinedTypeSyntax { Keyword.RawKind: (int)SyntaxKind.BoolKeyword } ||
            declaration.Body is null ||
            declaration.ExpressionBody is not null)
        {
            return false;
        }

        var statements = declaration.Body.Statements;
        if (statements.Count is < 1 or > 2)
        {
            return false;
        }

        var hasDisposeGuard = false;
        foreach (var statement in statements)
        {
            if (IsBaseDisposeCall(statement))
            {
                continue;
            }

            if (statement is IfStatementSyntax ifStatement &&
                ifStatement.Condition is IdentifierNameSyntax { Identifier.ValueText: "disposing" } &&
                TryGetDisposeUnsubscription(ifStatement.Statement, handlerName))
            {
                if (hasDisposeGuard)
                {
                    return false;
                }

                hasDisposeGuard = true;
                continue;
            }

            return false;
        }

        return hasDisposeGuard;
    }

    private static bool TryGetDisposeUnsubscription(StatementSyntax statement, string handlerName)
    {
        statement = statement is BlockSyntax block && block.Statements.Count == 1 ? block.Statements[0] : statement;
        return statement is ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax { RawKind: (int)SyntaxKind.SubtractAssignmentExpression } assignment } &&
               IsViewModelPropertyChangedAccess(assignment.Left) &&
               StripParentheses(assignment.Right) is IdentifierNameSyntax { Identifier.ValueText: var candidateHandlerName } &&
               candidateHandlerName == handlerName;
    }

    private static bool TryGetPropertyChangedSubscription(StatementSyntax statement, SemanticModel semanticModel, out PropertyChangedSubscription subscription)
    {
        if (statement is ExpressionStatementSyntax expressionStatement &&
            expressionStatement.Expression is AssignmentExpressionSyntax { RawKind: (int)SyntaxKind.AddAssignmentExpression } assignment &&
            IsViewModelPropertyChangedAccess(assignment.Left) &&
            TryGetMethodSymbol(assignment.Right, semanticModel, out var handlerSymbol))
        {
            subscription = new PropertyChangedSubscription(expressionStatement, handlerSymbol);
            return true;
        }

        subscription = null!;
        return false;
    }

    private static bool IsViewModelPropertyChangedAccess(ExpressionSyntax expression)
    {
        expression = StripParentheses(expression);
        if (expression is not MemberAccessExpressionSyntax memberAccess || memberAccess.Name.Identifier.ValueText != "PropertyChanged")
        {
            return false;
        }

        var target = StripParentheses(memberAccess.Expression);
        return target switch
        {
            IdentifierNameSyntax { Identifier.ValueText: AnalyzerConstants.PropertyNames.ViewModel } => true,
            MemberAccessExpressionSyntax nestedMemberAccess
                when nestedMemberAccess.Name.Identifier.ValueText == AnalyzerConstants.PropertyNames.ViewModel &&
                     nestedMemberAccess.Expression is ThisExpressionSyntax => true,
            _ => false,
        };
    }

    private static bool TryGetMethodSymbol(ExpressionSyntax expression, SemanticModel semanticModel, out IMethodSymbol handlerSymbol)
    {
        var symbolInfo = semanticModel.GetSymbolInfo(StripParentheses(expression));
        handlerSymbol = symbolInfo.Symbol as IMethodSymbol ?? symbolInfo.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault()!;
        return handlerSymbol is not null;
    }

    private static bool IsBaseLifecycleCall(StatementSyntax statement, string methodName)
    {
        if (statement is not ExpressionStatementSyntax { Expression: InvocationExpressionSyntax invocation })
        {
            return false;
        }

        return invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
               memberAccess.Expression is BaseExpressionSyntax &&
               memberAccess.Name.Identifier.ValueText == methodName &&
               invocation.ArgumentList.Arguments.Count == 0;
    }

    private static bool IsBaseDisposeCall(StatementSyntax statement)
    {
        if (statement is not ExpressionStatementSyntax { Expression: InvocationExpressionSyntax invocation })
        {
            return false;
        }

        return invocation.Expression is MemberAccessExpressionSyntax memberAccess &&
               memberAccess.Expression is BaseExpressionSyntax &&
               memberAccess.Name.Identifier.ValueText == AnalyzerConstants.MethodNames.Dispose &&
               invocation.ArgumentList.Arguments.Count == 1 &&
               invocation.ArgumentList.Arguments[0].Expression is IdentifierNameSyntax { Identifier.ValueText: "disposing" };
    }

    private static ExpressionSyntax StripParentheses(ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            expression = parenthesizedExpression.Expression;
        }

        return expression;
    }

    private sealed class ComponentPropertyCollection(
        Dictionary<string, ComponentParameterInfo> parameters,
        Dictionary<string, CallbackParameterInfo> callbacks)
    {
        public Dictionary<string, ComponentParameterInfo> Parameters { get; } = parameters;
        public Dictionary<string, CallbackParameterInfo> Callbacks { get; } = callbacks;
    }

    private sealed class ComponentParameterInfo(IPropertySymbol symbol)
    {
        public IPropertySymbol Symbol { get; } = symbol;
        public string Name => Symbol.Name;
        public ITypeSymbol Type => Symbol.Type;
        public string TypeName => Symbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        public Location Location => Symbol.Locations.FirstOrDefault() ?? Location.None;
    }

    private sealed class CallbackParameterInfo(IPropertySymbol symbol, ITypeSymbol valueType)
    {
        public IPropertySymbol Symbol { get; } = symbol;
        public ITypeSymbol ValueType { get; } = valueType;
        public string Name => Symbol.Name;
        public Location Location => Symbol.Locations.FirstOrDefault() ?? Location.None;
    }

    private sealed class BindingInfo(ComponentParameterInfo componentParameter, CallbackParameterInfo callback, IPropertySymbol viewModelProperty)
    {
        public ComponentParameterInfo ComponentParameter { get; } = componentParameter;
        public CallbackParameterInfo Callback { get; } = callback;
        public IPropertySymbol ViewModelProperty { get; } = viewModelProperty;
    }

    private sealed class PropertyChangedSubscription(ExpressionStatementSyntax statement, IMethodSymbol handlerSymbol)
    {
        public ExpressionStatementSyntax Statement { get; } = statement;
        public IMethodSymbol HandlerSymbol { get; } = handlerSymbol;
    }

    private sealed class ManualPatternMatch(
        string propertyName,
        string callbackName,
        string handlerName,
        Location subscriptionLocation)
    {
        public string PropertyName { get; } = propertyName;
        public string CallbackName { get; } = callbackName;
        public string HandlerName { get; } = handlerName;
        public Location SubscriptionLocation { get; } = subscriptionLocation;
    }
}
