using System.Collections.Generic;
using DdbChallenge.Models;

namespace DdbChallenge.Dal
{
	public class MemoryCharacterData : ICharacterData
	{
		private Dictionary<long, Character> _characters = new Dictionary<long, Character>();
		private long _nextId = 0;

		public Character GetCharacter(long id)
		{
			Character character;
			_characters.TryGetValue(id, out character);
			return character;
		}

		public Character CreateCharacter(Character character)
		{
			// this is not thread safe; using an actual DB would solve this, or using a threadsafe cache like MemoryCache could work
			character.Id = _nextId++;
			_characters[character.Id] = character;
			return character;
		}

		public Character UpdateCharacter(Character character)
		{
			_characters[character.Id] = character;
			return character;
		}
	}
}