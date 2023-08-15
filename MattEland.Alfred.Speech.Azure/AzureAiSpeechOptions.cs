using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MattEland.Alfred.Speech.Azure;

public class AzureAiSpeechOptions
{
    [Required]
    public string SubscriptionKey { get; set; }

    public string VoiceName { get; set; } = "en-GB-AlfieNeural";

    [Required]
    public string Region { get; set; }
}