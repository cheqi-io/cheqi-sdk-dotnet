using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

public class ReturnLineItem
{
    [JsonPropertyName("lineItemId")]
    public string? LineItemId { get; set; }
    
    [JsonPropertyName("quantityReturned")]
    public decimal? QuantityReturned { get; set; }
    
    [JsonPropertyName("condition")]
    public ReturnCondition? Condition { get; set; }
    
    [JsonPropertyName("conditionNotes")]
    public string? ConditionNotes { get; set; }

    public ReturnLineItem()
    {
    }

    public ReturnLineItem(string lineItemId, decimal quantityReturned, 
        ReturnCondition condition, string conditionNotes)
    {
        LineItemId = lineItemId;
        QuantityReturned = quantityReturned;
        Condition = condition;
        ConditionNotes = conditionNotes;
    }
}
