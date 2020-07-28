using DdbChallenge.Models;

namespace DdbChallenge.Dal
{
	public interface ICharacterData
	{
		Character GetCharacter(long Id);

		Character CreateCharacter(Character character);

		Character UpdateCharacter(Character character);
	}
}