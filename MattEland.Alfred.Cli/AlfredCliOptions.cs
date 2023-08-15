using System.ComponentModel.DataAnnotations;

namespace MattEland.Alfred.Cli;

public class AlfredCliOptions {
    [Required]
    public string ModelPath { get; set; }
}

