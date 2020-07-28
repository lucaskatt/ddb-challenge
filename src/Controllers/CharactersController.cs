using Microsoft.AspNetCore.Mvc;
using DdbChallenge.Bll;
using DdbChallenge.Models;

namespace DdbChallenge.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CharactersController : ControllerBase
	{
		private readonly ICharacterManager _characterManager;

		public CharactersController(ICharacterManager characterManager)
		{
			_characterManager = characterManager;
		}
		
		// GET api/characters/5
		[HttpGet("{id}")]
		public ActionResult<Character> GetCharacter(long id)
		{
			Character character = _characterManager.Get(id);
			if (character == null)
			{
				return NotFound();
			}
			return character;
		}

		// POST api/characters
		[HttpPost]
		public ActionResult PostCharacter(Character character)
		{
			character = _characterManager.Create(character);
			return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
		}

		// POST api/characters/5/attack
		[HttpPost("{id}/attack")]
		public ActionResult PostAttack(long id, Attack attack)
		{
			Character character = _characterManager.Get(id);
			if (character == null)
			{
				return NotFound();
			}
			character = _characterManager.Attack(character, attack);
			return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
		}

		// POST api/characters/5/heal
		[HttpPost("{id}/heal")]
		public ActionResult PostHeal(long id, Heal heal)
		{
			Character character = _characterManager.Get(id);
			if (character == null)
			{
				return NotFound();
			}
			character = _characterManager.Heal(character, heal);
			return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
		}

		// POST api/characters/5/temphp
		[HttpPost("{id}/temphp")]
		public ActionResult PostTempHp(long id, TempHp tempHp)
		{
			Character character = _characterManager.Get(id);
			if (character == null)
			{
				return NotFound();
			}
			character = _characterManager.AddTemporaryHp(character, tempHp);
			return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
		}
	}
}
