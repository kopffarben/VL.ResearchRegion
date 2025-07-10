// For examples, see:
// https://thegraybook.vvvv.org/reference/extending/writing-nodes.html#examples

using VL.Core.PublicAPI;
using VL.Model;
using VL.AppServices.CompilerServices.CustomRegion;

namespace Main;

public interface TestReginInterface
{
    void Invoke(int InputOne, int InputTwo, out int Result);
}

[ProcessNode(Name = "CSharpStatefullDelegat", FragmentSelection = FragmentSelection.Explicit)]
public class Delegate : IRegion<TestReginInterface>, IDisposable
{
    private bool _init = true;
    private Func<TestReginInterface>? _patchInlayFactory;
    private Dictionary<InputDescription, object>  _inputValues = new();
    private Dictionary<OutputDescription, object> _outputValues = new();

    TestReginInterface patchInlay;

    #region IRegion<TestReginInterface> Members
    public void AcknowledgeInput(in InputDescription description, object outerValue)
    {
        _inputValues[description] = outerValue;
    }

    public void AcknowledgeOutput(in OutputDescription description, TestReginInterface patchInlay, object innerValue)
    {
        _outputValues[description] = innerValue;
    }

    public void RetrieveInput(in InputDescription description, TestReginInterface patchInlay, out object innerValue)
    {
        _inputValues.TryGetValue(description, out innerValue);
    }

    public void RetrieveOutput(in OutputDescription description, out object outerValue)
    {
        _outputValues.TryGetValue(description, out outerValue);
    }

    public void SetPatchInlayFactory( Func<TestReginInterface> patchInlayFactory)
    {
        this._patchInlayFactory = patchInlayFactory;
    }
    #endregion  

    [Fragment(Order = 0)]
    public Delegate()
    {

    }

    [Fragment(Order = 1)]
    public void Update()
    {
        if (_init)
        {
             _init = false;
            if (_patchInlayFactory != null)
            {
                var patchInlay = _patchInlayFactory();
            }
        }        
    }
    

    public void Dispose()
    {
        if (patchInlay is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}