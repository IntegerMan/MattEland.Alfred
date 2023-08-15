using System.ComponentModel.DataAnnotations;

namespace MattEland.Alfred.Llama; 

public class AlfredLlamaOptions {
    [Required]
    public string Prompt { get; set; }
    [Required]
    public string ModelPath { get; set; }
    public string SessionPath { get; set; }
    public bool LoadSessionState { get; set; }
    public bool SaveSessionState { get; set; }
    [Required]
    public string UserName { get; set; } = "Batman";
    [Required]
    public string BotName { get; set; } = "ALFRED";
    public float Temperature { get; set; } = 0.5f;
    public int GpuLayerCount { get; set; } = 5;
}
