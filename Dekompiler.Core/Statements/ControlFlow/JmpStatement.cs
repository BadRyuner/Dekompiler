using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Cil;
using Dekompiler.Core.TypeSystem;
using Microsoft.AspNetCore.Components;

namespace Dekompiler.Core.Statements.ControlFlow;
public class JmpStatement : CompleteStatement
{
    private IMethodDescriptor _method = null!;

    public JmpStatement(MethodContext context) : base(context)
    {
    }

    public override CilCode[] Codes => [CilCode.Jmp];

    public override void Deserialize(CilInstruction instruction)
    {
        _method = (IMethodDescriptor)instruction.Operand!;
    }

    public override RenderFragment Render()
    {
        var render = Context.Renderer;
        var type = new TypeSignatureRenderer(Context.Renderer, _method!.DeclaringType!.ToTypeSignature(), _method,
               _method!.DeclaringType!.ToTypeDefOrRef()).RenderMember();
        var method = new MethodRenderer(render, _method).RenderMember();
        var arguments = Context.Method.Parameters;
        var argumentsSpans =
            Context.Renderer.Join(
                Context.Renderer.Concat(Context.Renderer.Constants.Comma, Context.Renderer.Constants.Space),
                arguments.Select(arg => render.Span(arg.Name, "parameter")).ToArray());
        return render.Concat(type, Context.Renderer.Constants.Dot, method, Context.Renderer.Constants.RightBracket, argumentsSpans, Context.Renderer.Constants.LeftBracket);
    }
}
