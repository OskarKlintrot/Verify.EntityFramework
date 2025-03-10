﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;

class TrackerConverter :
    WriteOnlyJsonConverter<DbChangeTracker>
{
    static Func<DbChangeTracker, DbContext> func;

    static TrackerConverter()
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var internalContextField = typeof(DbChangeTracker).GetField("_internalContext", flags)!;
        var ownerField = internalContextField.FieldType.GetField("_owner", flags)!;

        func = tracker =>
        {
            var value = internalContextField.GetValue(tracker);
            return (DbContext) ownerField.GetValue(value);
        };
    }

    public override void Write(VerifyJsonWriter writer, DbChangeTracker tracker)
    {
        writer.WriteStartObject();
        var entries = tracker.Entries().ToList();
        HandleAdded(entries, writer);
        var data = func(tracker);
        HandleModified(entries, writer, data);
        HandleDeleted(entries, writer, data);

        writer.WriteEndObject();
    }

    static void HandleDeleted(List<DbEntityEntry> entries, VerifyJsonWriter writer, DbContext data)
    {
        var deleted = entries
            .Where(x => x.State == EntityState.Deleted)
            .ToList();
        if (!deleted.Any())
        {
            return;
        }

        writer.WritePropertyName("Deleted");
        writer.WriteStartObject();
        foreach (var entry in deleted)
        {
            writer.WritePropertyName(entry.Entity.GetType().Name);
            writer.WriteStartObject();
            WriteId(writer, entry, data);
            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    static void HandleAdded(List<DbEntityEntry> entries, VerifyJsonWriter writer)
    {
        var added = entries
            .Where(x => x.State == EntityState.Added)
            .ToList();
        if (!added.Any())
        {
            return;
        }

        writer.WritePropertyName("Added");
        writer.WriteStartObject();
        foreach (var entry in added)
        {
            writer.WritePropertyName(entry.Entity.GetType().Name);
            writer.WriteStartObject();

            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var value = entry.CurrentValues[propertyName];
                writer.WriteProperty(entry, value, propertyName);
            }

            writer.WriteEndObject();
        }

        writer.WriteEndObject();
    }

    static void HandleModified(List<DbEntityEntry> entries, VerifyJsonWriter writer, DbContext context)
    {
        var modified = entries
            .Where(x => x.State == EntityState.Modified)
            .ToList();
        if (!modified.Any())
        {
            return;
        }

        writer.WritePropertyName("Modified");
        writer.WriteStartObject();
        foreach (var entry in modified)
        {
            HandleModified(writer, entry, context);
        }

        writer.WriteEndObject();
    }

    static void HandleModified(VerifyJsonWriter writer, DbEntityEntry entry, DbContext context)
    {
        writer.WritePropertyName(entry.Entity.GetType().Name);
        writer.WriteStartObject();

        WriteId(writer, entry, context);
        foreach (var property in entry.ChangedProperties())
        {
            writer.WriteProperty(
                entry,
                new
                {
                    Original = property.Original,
                    Current = property.Current
                },
                property.Key);
        }

        writer.WriteEndObject();
    }

    static void WriteId(VerifyJsonWriter writer, DbEntityEntry entry, DbContext context)
    {
        var ids = context.FindPrimaryKeyValues(entry).ToList();
        if (!ids.Any())
        {
            return;
        }

        if (ids.Count == 1)
        {
            var (name, value) = ids.Single();
            writer.WriteProperty(entry, value, name);
        }
        else
        {
            writer.WriteProperty(entry, ids, "Ids");
        }
    }
}