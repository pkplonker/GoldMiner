using System.Collections.Generic;

namespace Save
{
	public interface ISaveLoadIO
	{
		public Dictionary<string, object> Load();

		public void Save(Dictionary<string, object> data);
		public void Clear();
	}
}