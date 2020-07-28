using System.Collections.Generic;
using DdbChallenge.Bll;
using DdbChallenge.Dal;
using DdbChallenge.Models;
using Xunit;
using Moq;

namespace tests.Bll
{
	public class CharacterManagerTest
	{
		[Fact]
		public void GetCallsGetCharacter()
		{
			long id = 1;
			Character character = new Character() { Id = id };
			var mockDal = new Mock<ICharacterData>();
			mockDal.Setup(dal => dal.GetCharacter(id)).Returns(character);
			CharacterManager characterManager = new CharacterManager(mockDal.Object);

			Character gotCharacter = characterManager.Get(id);

			mockDal.Verify(dal => dal.GetCharacter(id), Times.Once());
			Assert.Equal(character, gotCharacter);

		}

		[Fact]
		public void CreateSetsHp()
		{
			Character character = new Character() 
			{
				Level = 5,
				Classes = new List<CharClass>
				{
					new CharClass(){ HitDiceValue = 10, ClassLevel = 3 },
					new CharClass(){ HitDiceValue = 6, ClassLevel = 2 },
				},
				Stats = new Stats() { Constitution = 14 }
			};
			int calculatedHp = 36;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.CreateCharacter(character)).Returns(character);

			Character createdCharacter = characterManager.Create(character);

			Assert.Equal(calculatedHp, createdCharacter.MaxHp);
			Assert.Equal(calculatedHp, createdCharacter.CurrentHp);
			Assert.Equal(0, createdCharacter.TempHp);
		}

		[Fact]
		public void CreateWithItemIncreasesHp()
		{
			Character character = new Character() 
			{
				Level = 1,
				Classes = new List<CharClass>
				{
					new CharClass(){ HitDiceValue = 10, ClassLevel = 1 },
				},
				Stats = new Stats() { Constitution = 10 },
				Items = new List<Item>
				{
					new Item()
					{
						Modifier = new Modifier()
						{
							AffectedObject = "stats",
							AffectedValue = "constitution",
							Value = 2
						}
					},
					new Item()
					{
						Modifier = new Modifier()
						{
							AffectedObject = "stats",
							AffectedValue = "constitution",
							Value = 3
						}
					}
				}
			};
			int calculatedHp = 8;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.CreateCharacter(character)).Returns(character);

			Character createdCharacter = characterManager.Create(character);

			Assert.Equal(calculatedHp, createdCharacter.MaxHp);
			Assert.Equal(calculatedHp, createdCharacter.CurrentHp);
			Assert.Equal(0, createdCharacter.TempHp);
		}

		[Fact]
		public void CreateWithNegativeConReducesHp()
		{
			Character character = new Character() 
			{
				Level = 1,
				Classes = new List<CharClass>
				{
					new CharClass(){ HitDiceValue = 10, ClassLevel = 1 },
				},
				Stats = new Stats() { Constitution = 8 },
			};
			int calculatedHp = 5;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.CreateCharacter(character)).Returns(character);

			Character createdCharacter = characterManager.Create(character);

			Assert.Equal(calculatedHp, createdCharacter.MaxHp);
			Assert.Equal(calculatedHp, createdCharacter.CurrentHp);
			Assert.Equal(0, createdCharacter.TempHp);
		}

		[Fact]
		public void AttackReducesHp()
		{
			int maxHp = 12;
			Character character = new Character() 
			{
				MaxHp = maxHp,
				CurrentHp = 10,
				TempHp = 0
			};
			Attack attack = new Attack()
			{
				Damage = 3
			};
			int calculatedHp = 7;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.Attack(character, attack);

			Assert.Equal(maxHp, updatedCharacter.MaxHp);
			Assert.Equal(calculatedHp, updatedCharacter.CurrentHp);
			Assert.Equal(0, updatedCharacter.TempHp);
		}

		[Fact]
		public void AttackDoesNotReduceBelow0()
		{
			Character character = new Character() 
			{
				CurrentHp = 10
			};
			Attack attack = new Attack()
			{
				Damage = 12
			};
			int calculatedHp = 0;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.Attack(character, attack);

			Assert.Equal(calculatedHp, updatedCharacter.CurrentHp);
		}

		[Fact]
		public void AttackReducesTempHpFirst()
		{
			Character character = new Character() 
			{
				CurrentHp = 10,
				TempHp = 5
			};
			Attack attack = new Attack()
			{
				Damage = 12
			};
			int calculatedHp = 3;
			int calculatedTempHp = 0;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.Attack(character, attack);

			Assert.Equal(calculatedHp, updatedCharacter.CurrentHp);
			Assert.Equal(calculatedTempHp, updatedCharacter.TempHp);
		}


		[Fact]
		public void AttackWithResistanceDealsHalfDamage()
		{
			Character character = new Character() 
			{
				CurrentHp = 10,
				Defenses = new List<Resistance>
				{
					new Resistance()
					{
						Type = "slashing",
						Defense = "resistance"
					}
				}
			};
			Attack attack = new Attack()
			{
				Damage = 12,
				Type = "slashing"
			};
			int calculatedHp = 4;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.Attack(character, attack);

			Assert.Equal(calculatedHp, updatedCharacter.CurrentHp);
		}

		[Fact]
		public void AttackWithImmunityDealsNoDamage()
		{
			Character character = new Character() 
			{
				CurrentHp = 10,
				Defenses = new List<Resistance>
				{
					new Resistance()
					{
						Type = "fire",
						Defense = "immunity"
					}
				}
			};
			Attack attack = new Attack()
			{
				Damage = 12,
				Type = "fire"
			};
			int calculatedHp = 10;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.Attack(character, attack);

			Assert.Equal(calculatedHp, updatedCharacter.CurrentHp);
		}

		[Fact]
		public void HealIncreasesHp()
		{
			int maxHp = 12;
			Character character = new Character() 
			{
				MaxHp = maxHp,
				CurrentHp = 5,
				TempHp = 0
			};
			Heal heal = new Heal()
			{
				Hp = 3
			};
			int calculatedHp = 8;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.Heal(character, heal);

			Assert.Equal(maxHp, updatedCharacter.MaxHp);
			Assert.Equal(calculatedHp, updatedCharacter.CurrentHp);
			Assert.Equal(0, updatedCharacter.TempHp);
		}

		[Fact]
		public void HealDoesNotIncreaseAboveMax()
		{
			int maxHp = 12;
			Character character = new Character() 
			{
				MaxHp = maxHp,
				CurrentHp = 5,
			};
			Heal heal = new Heal()
			{
				Hp = 20 
			};
			int calculatedHp = maxHp;

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.Heal(character, heal);

			Assert.Equal(maxHp, updatedCharacter.MaxHp);
			Assert.Equal(calculatedHp, updatedCharacter.CurrentHp);
		}

		[Fact]
		public void AddTemporaryHpAddsTempHp()
		{
			int maxHp = 12;
			int currentHp = 5;
			int oldTempHp = 0;
			int newTempHp = 3;
			Character character = new Character() 
			{
				MaxHp = maxHp,
				CurrentHp = currentHp,
				TempHp = oldTempHp
			};
			TempHp tempHp = new TempHp()
			{
				Hp = newTempHp
			};

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.AddTemporaryHp(character, tempHp);

			Assert.Equal(maxHp, updatedCharacter.MaxHp);
			Assert.Equal(currentHp, updatedCharacter.CurrentHp);
			Assert.Equal(newTempHp, updatedCharacter.TempHp);
		}

		[Fact]
		public void AddTemporaryHpWithMoreReplacesExisting()
		{
			int oldTempHp = 2;
			int newTempHp = 3;
			Character character = new Character() 
			{
				TempHp = oldTempHp
			};
			TempHp tempHp = new TempHp()
			{
				Hp = newTempHp
			};

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.AddTemporaryHp(character, tempHp);

			Assert.Equal(newTempHp, updatedCharacter.TempHp);
		}

		[Fact]
		public void AddTemporaryHpWithLessDoesNotReplacesExisting()
		{
			int oldTempHp = 4;
			int newTempHp = 3;
			Character character = new Character() 
			{
				TempHp = oldTempHp
			};
			TempHp tempHp = new TempHp()
			{
				Hp = newTempHp
			};

			var mockDal = new Mock<ICharacterData>();
			CharacterManager characterManager = new CharacterManager(mockDal.Object);
			mockDal.Setup(dal => dal.UpdateCharacter(character)).Returns(character);

			Character updatedCharacter = characterManager.AddTemporaryHp(character, tempHp);

			Assert.Equal(oldTempHp, updatedCharacter.TempHp);
		}
	}
}