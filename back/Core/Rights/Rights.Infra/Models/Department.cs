using Lucca.Core.Rights.Abstractions;
using Lucca.Core.Shared.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rights.Infra.Models
{
	internal class Department : IDepartment, ITreeable<Department>
	{
		public int Id { get; set; }

		public int SortOrder => int.Parse(Tag.Substring(Tag.Length - 2, 2));

		protected decimal Position { get; set; }

		private string _tag;
		protected string Tag
		{
			get
			{
				if (_tag != null)
				{
					return _tag;
				}

				var str = $"0{Position}";
				var positionToString = str.Substring(str.Length - 20, 20);

				var depthLevel = (int)Math.Ceiling((double)positionToString.TrimEnd('0').Length / 2);
				_tag = positionToString.Substring(0, depthLevel * 2);
				return _tag;
			}
		}

		public Department GetParent(IEnumerable<Department> allNodes)
		{
			var parentTag = Tag.Substring(0, Math.Max(0, Tag.Length - 2));
			return allNodes.FirstOrDefault(s => s.Tag == parentTag);
		}
	}
}