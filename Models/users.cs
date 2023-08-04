using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace CMS.Models {

	[JsonObject(MemberSerialization.OptIn)]
	public partial class users {

		[JsonProperty, Column(DbType = "int", IsPrimary = true)]
		public int ID { get; set; }

		[JsonProperty, Column(DbType = "datetime")]
		public DateTime AddDate { get; set; }

		[JsonProperty]
		public bool IsDelete { get; set; }

		[JsonProperty, Column(IsNullable = false)]
		public string Name { get; set; }

		[JsonProperty, Column(IsNullable = false)]
		public string Password { get; set; }

		[JsonProperty, Column(DbType = "int")]
		public int Sex { get; set; }

		[JsonProperty, Column(IsNullable = false)]
		public string UserName { get; set; }

	}

}
