using System;
using System.Collections.Generic;
using System.Linq;

namespace Artemis.Storage.Entities.General;

public class QueuedActionEntity
{
    public Guid Id { get; set; }
    public string Type { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public List<QueuedActionParameterEntity> Parameters { get; set; } = new();

    public string GetParameter(string key)
    {
        return Parameters.FirstOrDefault(p => p.Key == key)?.Value;
    }
}

public class QueuedActionParameterEntity
{
    public QueuedActionParameterEntity(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; set; }
    public string Value { get; set; }
}