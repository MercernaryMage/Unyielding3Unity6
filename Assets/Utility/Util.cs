using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Windows;
using System.Text.RegularExpressions;
using UnityEngine.Profiling;


public static class Util
{
	public delegate bool CharacterEvaluation(Character c);

	public static Tuple<List<Tile>, Tile>? FindSmallestRoute(Dictionary<Character, Tuple<List<Tile>, Tile>> data, CharacterEvaluation evaluationFunction)
	{
		Tuple<List<Tile>, Tile> shortestRoute = null;
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in data)
		{
			if (pair.Key.alive == false)
			{
				continue;
			}
			if (evaluationFunction != null)
			{
				if (!evaluationFunction(pair.Key))
				{
					continue;
				}
			}
			if (shortestRoute == null)
			{
				shortestRoute = pair.Value;
			}
			else
			{
				if (shortestRoute.Item1.Count > pair.Value.Item1.Count)
				{
					shortestRoute = pair.Value;
				}
			}
		}
		return shortestRoute;
	}

	public static void ShortenPathToMaxRange(Tuple<List<Tile>, Tile> data, int range)
	{
		while (data.Item1.Count > range)
		{
			data.Item1.RemoveAt(data.Item1.Count - 1);
		}
	}

	public static void RemoveOutOfRangeRoutes(Dictionary<Character, Tuple<List<Tile>, Tile>> data, int range)
	{
		List<Character> characterRoutesToRemove = new List<Character>();
		foreach (KeyValuePair<Character, Tuple<List<Tile>, Tile>> pair in data)
		{
			if (pair.Value.Item1.Count > range)
			{
				characterRoutesToRemove.Add(pair.Key);
			}
		}
		foreach (Character c in characterRoutesToRemove)
		{
			data.Remove(c);
		}
	}

	public static void ShortenPathToDesiredRange(Tuple<List<Tile>, Tile> routeData, Character routingCharacter, Character hero, int desiredRange)
	{
		List<Tile> heroTiles = TileGrid.Instance.FindCharacter(hero);

		for (int i = 0; i < routeData.Item1.Count; ++i)
		{
			Tile routeTile = routeData.Item1[i];
			List<Tile> wouldOccupy = TileGrid.Instance.WhatTilesWouldCharacterTake(routingCharacter, routeTile);

			int minDist = int.MaxValue;
			if (wouldOccupy != null)
			{
				foreach (Tile myTile in wouldOccupy)
				{
					foreach (Tile heroTile in heroTiles)
					{
						int dist = TileGrid.Distance(myTile, heroTile);
						if (dist < minDist) minDist = dist;
					}
				}
			}

			if (minDist <= desiredRange)
			{
				while (routeData.Item1.Count > i + 1)
				{
					routeData.Item1.RemoveAt(routeData.Item1.Count - 1);
				}
				return;
			}
		}
	}

	public static Character FindFarthestHeroWithinRange(int range, Character character)
	{
		List<Tile> myTiles = TileGrid.Instance.FindCharacter(character);
		Character farthestHero = null;
		int farthestDistance = -1;

		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive) continue;
			if (hero.gameObject.GetComponent<Downed>() != null) continue;

			List<Tile> heroTiles = TileGrid.Instance.FindCharacter(hero);
			int minDistToHero = int.MaxValue;
			foreach (Tile myTile in myTiles)
			{
				foreach (Tile heroTile in heroTiles)
				{
					int dist = TileGrid.Distance(myTile, heroTile);
					if (dist < minDistToHero) minDistToHero = dist;
				}
			}

			if (minDistToHero <= range && minDistToHero > farthestDistance)
			{
				farthestDistance = minDistToHero;
				farthestHero = hero;
			}
		}

		return farthestHero;
	}

	public static void PullToAttacker(Character target, Character owningCharacter)
	{
		List<Tile> attackerTiles = TileGrid.Instance.FindCharacter(owningCharacter);
		List<Tile> targetOriginalTiles = TileGrid.Instance.FindCharacter(target);
		List<Tile> possibleTiles = new List<Tile>(TileGrid.Instance.tiles);

		possibleTiles = possibleTiles.OrderBy(p =>
		{
			List<Tile> wouldOccupy = TileGrid.Instance.WhatTilesWouldCharacterTake(target, p);
			if (wouldOccupy == null)
			{
				return int.MaxValue;
			}

			int minDist = int.MaxValue;
			foreach (Tile targetTile in wouldOccupy)
			{
				foreach (Tile at in attackerTiles)
				{
					int dist = TileGrid.Distance(targetTile, at);
					if (dist < minDist) minDist = dist;
				}
			}
			return minDist;
		})
		.ThenBy(p =>
		{
			int minDist = int.MaxValue;
			foreach (Tile origTile in targetOriginalTiles)
			{
				int dist = TileGrid.Distance(p, origTile);
				if (dist < minDist) minDist = dist;
			}
			return minDist;
		})
		.ToList();

		foreach (Tile t in possibleTiles)
		{
			if (TileGrid.Instance.WouldCharacterFitAtTile(target, t))
			{
				TileGrid.Instance.MoveCharacterToTile(target, t);
				return;
			}
		}
	}

	public static void BringCardToTopOfDeck(Character character, string className)
	{
		Card found = null;

		foreach (Card card in character.cards)
		{
			if (card.cardScriptableObject.className == className)
			{
				found = card;
				break;
			}
		}

		if (found == null)
		{
			foreach (Card card in character.cardDiscard)
			{
				if (card.cardScriptableObject.className == className)
				{
					found = card;
					character.cardDiscard.Remove(found);
					break;
				}
			}
		}

		if (found == null)
		{
			return;
		}

		character.cards.Remove(found);
		character.cards.Insert(0, found);
	}

	private static System.Random rng = new System.Random();

	public static void Shuffle<T>(IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = UnityEngine.Random.Range(0, n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static ActionController.AttackProfile GetAttackProfile(ActionPattern pattern)
	{
		List<(int number, int face, int flat, ActionPattern.DamageType type)> damages = Util.ParseDiceString(pattern.damageString);
		ActionController.AttackProfile attackProfile = 
			new ActionController.AttackProfile(damages[0].number, damages[0].face, damages[0].flat);
		attackProfile.damageType = damages[0].type;
		attackProfile.ranged = pattern.ranged;
		attackProfile.accuracy = pattern.accuracy;
		attackProfile.breach = pattern.keywords.Contains("Breach") 
							   || attackProfile.damageType == ActionPattern.DamageType.Burning;
		return attackProfile;
	}

	public static string GetDamageTypeNameForDisplay(ActionPattern.DamageType type)
	{
		if (type == ActionPattern.DamageType.Physical)
		{
			return "P";
		}
		else if (type == ActionPattern.DamageType.Magic)
		{
			return "M";
		}
		else if (type == ActionPattern.DamageType.Burning)
		{
			return "B";
		}
		else
		{
			return "";
		}
	}

	public static int ExtractValueFromString(string s)
	{
		string digitString = new string(s.Where(char.IsDigit).ToArray());
		if (int.TryParse(digitString, out int value))
		{
			return value;
		}
		return 0;
	}

	public static List<(int x, int y, int z, ActionPattern.DamageType type)> ParseDiceString(string input)
	{
		//1d6
		//1d6+1
		//1d6B
		//1d6+1B
		//1
		//1B
		var match = Regex.Match(input, @"\b(\d+)(?:d(\d+)(?:\+(\d+))?)?([MPB])?\b");

		if (!match.Success)
		{
			throw new ArgumentException("Input is not in the expected format: 'word xdy+z'");
		}

		int x, y, z;
		if (match.Groups[2].Success)
		{
			x = int.Parse(match.Groups[1].Value);
			y = int.Parse(match.Groups[2].Value);
			z = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 0;
		}
		else
		{
			x = 0;
			y = 0;
			z = int.Parse(match.Groups[1].Value);
		}
		ActionPattern.DamageType type = match.Groups[4].Value switch
		{
			"P" => ActionPattern.DamageType.Physical,
			"M" => ActionPattern.DamageType.Magic,
			"B" => ActionPattern.DamageType.Burning,
			_ => ActionPattern.DamageType.None,
		};

		return new List<(int x, int y, int z, ActionPattern.DamageType type)>() { (x, y, z, type) };
	}

	public static int RollDice(int number, int face)
	{
		int result = 0;
		for (int i = 0; i < number; ++i)
		{
			result += UnityEngine.Random.Range(1, face + 1);
		}
		return result;
	}

	public static Quaternion GetFacing(Direction d)
	{
		if (d == Direction.North)
		{
			return Quaternion.Euler(0, 0, 0);
		}
		else if (d == Direction.East)
		{
			return Quaternion.Euler(0, 90, 0);
		}
		else if (d == Direction.South)
		{
			return Quaternion.Euler(0, 180, 0);
		}
		else
		{
			return Quaternion.Euler(0, 270, 0);
		}
	}

	public static List<Character> GetHeroesInRange(Character owningCharacter, int range)
	{
		List<Character> result = new List<Character>();

		foreach (Character hero in BattleController.Instance.heroes)
		{
			if (!hero.alive)
			{
				continue;
			}
			if (hero.gameObject.GetComponent<Downed>() != null)
			{
				continue;
			}

			if (TileGrid.Instance.GetDistanceBetweenCharacters(owningCharacter, hero) <= range)
			{
				result.Add(hero);
			}
		}

		return result;
	}

	public static void FilterToLOS(TemplateLibrary.TilesAndDirection tilesAndDirection, Character target)
	{
		for (int i = tilesAndDirection.tiles.Count - 1; i >= 0; --i)
		{
			if (!TileGrid.Instance.DoesTileHaveLOSToCharacter(tilesAndDirection.tiles[i], target))
			{
				tilesAndDirection.tiles.RemoveAt(i);
			}
		}
	}

	public class JSONSerializer
	{
		static JsonSerializerSettings settings = new JsonSerializerSettings()
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			Converters = new List<JsonConverter>()
		{
			new Vector3Converter(),
		},
		};
		public static string Serialize(object obj)
		{
			return JsonConvert.SerializeObject(obj, settings);
		}

		public static T Deserialize<T>(string str)
		{
			return JsonConvert.DeserializeObject<T>(str, settings);
		}

		class Vector3Converter : JsonConverter<Vector3>
		{
			public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				if (reader.TokenType != JsonToken.StartObject)
					throw new Exception($"Invalid json. Expected StartObject");

				Vector3 output = default;

				while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
				{
					switch ((string)reader.Value)
					{
						case "x":
							output.x = (float)reader.ReadAsDouble().Value;
							break;
						case "y":
							output.y = (float)reader.ReadAsDouble().Value;
							break;
						case "z":
							output.z = (float)reader.ReadAsDouble().Value;
							break;
						default:
							reader.Skip();
							break;
					}
				}

				if (reader.TokenType != JsonToken.EndObject)
					throw new Exception($"Invalid json. Expected end object");

				return output;
			}

			public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
			{
				writer.WriteStartObject();
				writer.WritePropertyName("x");
				writer.WriteValue(value.x);
				writer.WritePropertyName("y");
				writer.WriteValue(value.y);
				writer.WritePropertyName("z");
				writer.WriteValue(value.z);
				writer.WriteEndObject();
			}
		}
	}

	static System.Random rand;

	static public float Range(float min, float max)
	{
		if (rand == null)
		{
			rand = new System.Random();
		}
		float t = (float)rand.NextDouble();
		
		return min + (max - min) * t;
	}

	static public int Range(int minInclusive, int maxExclusive)
	{
		if (rand == null)
		{
			rand = new System.Random();
		}
		return rand.Next(minInclusive, maxExclusive);
	}
}

public static class StringExtensions
{
	public static bool ContainsIgnoreCase(this string source, string value)
	{
		return source.ToLower().Contains(value.ToLower());
	}

	public static bool ContainsIgnoreCase(this IEnumerable<string> source, string value)
	{
		return source.Any(s => s.ToLower() == value.ToLower());
	}
}
