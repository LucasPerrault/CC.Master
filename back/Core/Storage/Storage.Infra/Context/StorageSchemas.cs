namespace Storage.Infra.Context
{
	public static class StorageSchemas
	{
		public static readonly StorageSchema Shared = new StorageSchema("shared");
		public static readonly StorageSchema Core = new StorageSchema("core");
		public static readonly StorageSchema Instances = new StorageSchema("instances");
		public static readonly StorageSchema Billing = new StorageSchema("billing");
	}

	public class StorageSchema
	{
		public string Value { get; }

		internal StorageSchema(string schema)
		{
			Value = schema;
		}
	}
}
