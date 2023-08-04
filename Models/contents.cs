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
	public partial class contents {

		[JsonProperty, Column(DbType = "int", IsPrimary = true, IsIdentity = true)]
		public int ID { get; set; }

		[JsonProperty, Column(DbType = "datetime")]
		public DateTime AddDate { get; set; }

		[JsonProperty, Column(IsNullable = false)]
		public string AddUser { get; set; }

		[JsonProperty, Column(StringLength = 2000, IsNullable = false)]
		public string Content { get; set; }

		[JsonProperty]
		public bool IsDelete { get; set; }

		[JsonProperty, Column(DbType = "int")]
		public int MenuID { get; set; }

		[JsonProperty, Column(DbType = "datetime")]
		public DateTime ModifyDate { get; set; }

		[JsonProperty, Column(IsNullable = false)]
		public string ModifyUser { get; set; }

		[JsonProperty, Column(IsNullable = false)]
		public string ModuleName { get; set; }

		[JsonProperty, Column(IsNullable = false)]
		public string Remark { get; set; }

		/// <summary>
		/// 0:文本 1:图片
		/// </summary>
		[JsonProperty, Column(DbType = "int")]
		public int Type { get; set; }

	}

}
