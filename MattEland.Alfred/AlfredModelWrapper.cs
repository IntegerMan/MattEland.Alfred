using LLama;
using LLama.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.Alfred; 
public class AlfredModelWrapper : IDisposable {
    private readonly LLamaModel _model;

    public AlfredModelWrapper(IOptions<AlfredOptions> options, ILLamaLogger logger) {
        ModelParams modelParams = new(options.Value.ModelPath);
        _model = new LLamaModel(modelParams, "UTF-8", logger);
    }

    public LLamaModel Model => _model;

    public void Dispose() {
        _model.Dispose();
    }
}
