using DdbChallenge.Models;

namespace DdbChallenge.Bll
{
	public interface ICharacterManager
	{
		Character Get(long id);

		Character Create(Character character);

		Character Attack(Character character, Attack attack);

		Character Heal(Character character, Heal heal);

		Character AddTemporaryHp(Character character, TempHp tempHp);
	}
}