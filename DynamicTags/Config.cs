namespace DynamicTags
{
	public class Config
	{
		/// <summary>
		/// Endpoint location for the external web API that the plugin will interact with.
		/// </summary>
		public string ApiEndpoint { get; set; } = "https://google.co.uk/";

		public bool TrackerEnabled { get; set; } = true;
		public bool TagsEnabled { get; set; } = true;
		public bool ReportingEnabled { get; set; } = true;
		public bool AutomaticNorthwoodReservedSlot { get; set; } = true;
	}
}
