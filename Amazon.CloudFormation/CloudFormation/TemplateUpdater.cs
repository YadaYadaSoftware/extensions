namespace Amazon.CloudFormation;

public static class TemplateUpdater
{
    public static Task UpdateTemplate(FileInfo templateFile, string resource, string propertyPath, string newValue)
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

        return Task.CompletedTask;
    }
}