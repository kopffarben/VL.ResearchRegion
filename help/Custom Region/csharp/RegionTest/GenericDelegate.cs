// For examples, see:
// https://thegraybook.vvvv.org/reference/extending/writing-nodes.html#examples

using VL.Core.PublicAPI;
using VL.Model;
using VL.AppServices.CompilerServices.CustomRegion;

namespace Main;



[ProcessNode(Name = "CSharpStatefullGenericDelegat", FragmentSelection = FragmentSelection.Explicit)]
public class GenericDelegate<T> : IRegion<T>, IDisposable
{
    private bool _init = true;
    private Func<T>? _patchInlayFactory;
    private Dictionary<InputDescription, object>  _inputValues = new();
    private Dictionary<OutputDescription, object> _outputValues = new();

    T patchInlay;

    #region IRegion<T> Members
    public void AcknowledgeInput(in InputDescription description, object outerValue)
    {
        _inputValues[description] = outerValue;
    }

    public void AcknowledgeOutput(in OutputDescription description, T patchInlay, object innerValue)
    {
        _outputValues[description] = innerValue;
    }

    public void RetrieveInput(in InputDescription description, T patchInlay, out object innerValue)
    {
        _inputValues.TryGetValue(description, out innerValue);
    }

    public void RetrieveOutput(in OutputDescription description, out object outerValue)
    {
        _outputValues.TryGetValue(description, out outerValue);
    }

    public void SetPatchInlayFactory( Func<T> patchInlayFactory)
    {
        this._patchInlayFactory = patchInlayFactory;
    }
    #endregion  

    [Fragment(Order = 0)]
    public GenericDelegate(Action<T> SetTInlay)
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