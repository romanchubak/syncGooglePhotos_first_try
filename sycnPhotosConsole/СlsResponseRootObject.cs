using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace sycnPhotosConsole {
	//one class in one .cs file, this is very bad
	public class СlsResponseRootObject {
		public List<MediaItem> mediaItems { get; set; }
		public string nextPageToken { get; set; }
	}

	public class MediaItem {
		public string id { get; set; }
		public string productUrl { get; set; }
		public string baseUrl { get; set; }
		public string mimeType { get; set; }
		public MediaMetadata mediaMetadata { get; set; }
		public string filename { get; set; }
	}

	public class MediaMetadata {
		public DateTime creationTime { get; set; }
		public string width { get; set; }
		public string height { get; set; }
		public Photo photo { get; set; }
	}

	public class Photo {
		public string cameraMake { get; set; }
		public string cameraModel { get; set; }
		public double focalLength { get; set; }
		public double apertureFNumber { get; set; }
		public int isoEquivalent { get; set; }
	}

	public class GoogleFilterDate {
		[JsonProperty(PropertyName = "year")]
		public int Year { get; set; }
		[JsonProperty(PropertyName = "month")]
		public int Month { get; set; }
		[JsonProperty(PropertyName = "day")]
		public int Day { get; set; }
	}

	[JsonObject(Title = "startDate")]
	public class StartDate {
		[JsonProperty(PropertyName = "year")]
		public int Year { get; set; }
		[JsonProperty(PropertyName = "month")]
		public int Month { get; set; }
		[JsonProperty(PropertyName = "day")]
		public int Day { get; set; }
	}
	
	[JsonObject(Title = "endDate")]
	public class EndDate {
		[JsonProperty(PropertyName = "year")]
		public int Year { get; set; }
		[JsonProperty(PropertyName = "month")]
		public int Month { get; set; }
		[JsonProperty(PropertyName = "day")]
		public int Day { get; set; }
	}
	
	
	[JsonObject(Title = "dateFilters")]
	public class DateFilters {
		[JsonProperty(PropertyName = "dates")]
		public List<GoogleFilterDate> Dates { get; set; }
	}
	
	[JsonObject(Title = "filters")]
	public class Filters {
		[JsonProperty(PropertyName = "dateFilter")]
		public DateFilters DateFilters { get; set; }
	}
}