using System.Text.Json.Nodes;

namespace Amazon.CloudFormation;

public static class TemplateUpdater
{
    public static async Task UpdateTemplateAsync(FileInfo templateFile, string resource, string propertyPath, string newValue)
    {
        if (templateFile == null) throw new ArgumentNullException(nameof(templateFile));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (propertyPath == null) throw new ArgumentNullException(nameof(propertyPath));
        if (newValue == null) throw new ArgumentNullException(nameof(newValue));
        if (templateFile == null) throw new ArgumentNullException(nameof(templateFile));
        if (resource == null) throw new ArgumentNullException(nameof(resource));
        if (propertyPath == null) throw new ArgumentNullException(nameof(propertyPath));
        if (newValue == null) throw new ArgumentNullException(nameof(newValue));
        if (string.IsNullOrEmpty(resource)) throw new ArgumentException("Value cannot be null or empty.", nameof(resource));
        if (string.IsNullOrEmpty(propertyPath)) throw new ArgumentException("Value cannot be null or empty.", nameof(propertyPath));

        if (!templateFile.Exists) throw new FileNotFoundException(templateFile.FullName);

        var json = await File.ReadAllTextAsync(templateFile.FullName);

        var templateNode = JsonNode.Parse(json);
        ArgumentNullException.ThrowIfNull(templateNode, nameof(templateNode));

        var resources = templateNode["Resources"];
        ArgumentNullException.ThrowIfNull(resources, nameof(resources));

        var blue = resources[resource];
        ArgumentNullException.ThrowIfNull(blue, nameof(blue));

        var properties = blue["Properties"];
        ArgumentNullException.ThrowIfNull(properties, nameof(properties));

        var effectedProperty = properties[propertyPath];
        ArgumentNullException.ThrowIfNull(effectedProperty, nameof(effectedProperty));

        var propertiesObject = properties.AsObject();
        propertiesObject.Remove(propertyPath);
        propertiesObject.Add(propertyPath,newValue);

        await File.WriteAllTextAsync(templateFile.FullName, templateNode.ToJsonString());
    }
}