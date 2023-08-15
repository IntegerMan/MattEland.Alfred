using LLama;
using LLama.Common;
using Microsoft.Extensions.Options;

namespace MattEland.Alfred.Llama; 
public class AlfredModelWrapper : IDisposable {
    private readonly LLamaModel _model;

    public AlfredModelWrapper(IOptions<AlfredLlamaOptions> options, ILLamaLogger logger) {
        int seed = Random.Shared.Next();
        ModelParams modelParams = new(options.Value.ModelPath, gpuLayerCount: options.Value.GpuLayerCount, seed: seed);
        _model = new LLamaModel(modelParams, "UTF-8", logger);
    }

    public LLamaModel Model => _model;

    public void Dispose() {
        _model.Dispose();
    }
}
