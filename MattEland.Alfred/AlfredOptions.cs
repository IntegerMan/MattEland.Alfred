using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.Alfred; 

public class AlfredOptions {
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
}
