using System.Collections.Generic;

namespace DdbChallenge.Models
{
	public class Character
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public List<CharClass> Classes { get; set; }
		public Stats Stats { get; set; }
		public List<Item> Items { get; set; }
		public List<Resistance> Defenses { get; set; }
		public int MaxHp { get; set; }
		public int CurrentHp { get; set; }
		public int TempHp { get; set; }
		//not implementing death for now; would need additional fields since 0 hp is just unconscious and need a way to track death saving throws, etc.
	}
}