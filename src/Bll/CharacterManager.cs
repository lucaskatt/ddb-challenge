using System;
using System.Linq;
using DdbChallenge.Dal;
using DdbChallenge.Models;

namespace DdbChallenge.Bll
{
	public class CharacterManager : ICharacterManager
	{
		private readonly ICharacterData _characterData;

		public CharacterManager(ICharacterData characterData)
		{
			_characterData = characterData;
		}

		public Character Get(long id)
		{
			return _characterData.GetCharacter(id);
		}

		public Character Create(Character character)
		{
			//could add validation in here for all of the fields if this were a real project
			//force stats, resistances, classes, etc. to be enums
			//make sure attacks, heals, temphp are all positive

			int maxHp = 0;

			//calculate hp from hit dice
			foreach (CharClass charClass in character.Classes)
			{
				//using average hit dice result to determine hp, and not starting lvl 1 with max roll
				//hit dice are all even numbers so don't need to worry about integer division
				maxHp += (charClass.HitDiceValue / 2 + 1) * charClass.ClassLevel;
			}

			//calculate hp from constitution
			int conScore = character.Stats.Constitution;
			if (character.Items != null)
			{
				foreach (Item item in character.Items)
				{
					if (item.Modifier.AffectedObject == "stats" && item.Modifier.AffectedValue == "constitution")
					{
						conScore += item.Modifier.Value;
					}
				}
			}
			//c# integer division rounds towards 0, not negative infinity; using right shift to get the behavior we want for negative values
			maxHp += ((conScore - 10) >> 1) * character.Level; 
			
			//initialize hp
			character.MaxHp = maxHp;
			character.CurrentHp = maxHp;
			character.TempHp = 0;

			return _characterData.CreateCharacter(character);
		}

		public Character Attack(Character character, Attack attack)
		{
			int damage = attack.Damage;

			//keeping this simple by ignoring the case where the character has multiple defenses for the same type
			//that would be prevented by validation during character creation anyways
			if (character.Defenses != null)
			{
				Resistance resistance = character.Defenses.FirstOrDefault(defense => defense.Type == attack.Type);

				if (resistance == null)
				{
					//don't modify damage in this case
				}
				else if (resistance.Defense == "immunity")
				{
					damage = 0;
				}
				else if (resistance.Defense == "resistance")
				{
					damage /= 2;
				}
			}

			int healthDamage = Math.Max(damage - character.TempHp, 0); //get damage remaining
			character.TempHp = Math.Max(character.TempHp - damage, 0);
			character.CurrentHp = Math.Max(character.CurrentHp - healthDamage, 0);

			return _characterData.UpdateCharacter(character);
		}

		public Character Heal(Character character, Heal heal)
		{
			character.CurrentHp = Math.Min(character.CurrentHp + heal.Hp, character.MaxHp);
			return _characterData.UpdateCharacter(character);
		}

		public Character AddTemporaryHp(Character character, TempHp tempHp)
		{
			character.TempHp = Math.Max(tempHp.Hp, character.TempHp);
			return _characterData.UpdateCharacter(character);
		}
	}
}