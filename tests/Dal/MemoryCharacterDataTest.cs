using DdbChallenge.Dal;
using DdbChallenge.Models;
using Xunit;

namespace tests.Dal
{
	public class MemoryCharacterDataTest
	{
		[Fact]
		public void CreateCharacterReturnsCharacter()
		{
			MemoryCharacterData memoryCharacterData = new MemoryCharacterData();
			Character character = new Character(){ Name = "name" };

			Character createdCharacter = memoryCharacterData.CreateCharacter(character);

			Assert.Equal(character, createdCharacter);
		}

		[Fact]
		public void CreatedCharactersHaveDifferentIds()
		{
			MemoryCharacterData memoryCharacterData = new MemoryCharacterData();
			Character character1 = new Character(){ Name = "name1" };
			Character character2 = new Character(){ Name = "name2" };

			Character createdCharacter1 = memoryCharacterData.CreateCharacter(character1);
			Character createdCharacter2 = memoryCharacterData.CreateCharacter(character2);

			Assert.NotEqual(createdCharacter1.Id, createdCharacter2.Id);
		}

		[Fact]
		public void GetCharacterGetsCharacter()
		{
			MemoryCharacterData memoryCharacterData = new MemoryCharacterData();
			Character character = new Character(){ Name = "name" };
			Character createdCharacter = memoryCharacterData.CreateCharacter(character);

			Character gotCharacter = memoryCharacterData.GetCharacter(createdCharacter.Id);

			Assert.Equal(createdCharacter.Id, gotCharacter.Id);
		}

		[Fact]
		public void UpdateCharacterUpdatesCharacter()
		{
			MemoryCharacterData memoryCharacterData = new MemoryCharacterData();
			string oldName = "oldName";
			string newName = "newName";
			Character character = new Character(){ Name = oldName };
			Character createdCharacter = memoryCharacterData.CreateCharacter(character);
			createdCharacter.Name = newName;
			
			Character updatedCharacter = memoryCharacterData.UpdateCharacter(createdCharacter);

			Assert.Equal(createdCharacter.Id, updatedCharacter.Id);
			Assert.Equal(newName, updatedCharacter.Name);
		}
	}
}